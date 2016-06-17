using System;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Util;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Sfs2X.Entities.Variables;
using System.Collections;

public enum scegliHost
{
    localhost,
    remotehost
}

public class ControllerLogin : MonoBehaviour
{
    const string CMD_REGISTRA = "regUt";
    const string CMD_LOGIN = "logUt";
    const string STR_SUCCESSO = "successo";
    const string STR_MESSAGGIO_ERRORE = "messaggioErrore";
    const string STR_ID_UTENTE = "idUtente";
    const string CMD_RICHIESTA_PERSONAGGI = "persg";
    const string CMD_RICHIESTA_PERSONAGGI_UTENTE = "persU";
    const string CMD_RICHIESTA_DIPLOMAZIA = "dip";
    const string CMD_RICHIESTA_DIPLOMAZIA_PERSONAGGIO = "dipP";
    const string CMD_RICHIESTA_TS_UTENTI = "tsUtenti";
    const string CMD_NESSUN_PERSONAGGIO_TROVATO = "noPers";
    const string TIMESTAMP = "tStamp";
    const string CMD_TIMESTAMP = "timeS";
    const int NUMERO_TAB_LEGATI_AD_UTENTE = 2;//tabelle PersonaggiUtenti e DiplomaziaPersonaggio che sono legate al timestamp di Utenti

    public InputField casellaNome;
    public Text erroreText;
    public InputField password;
    public InputField email;
    

    public string localhost = "127.0.0.1";
    public int TcpPort = 9933;
    public int UdpPort = 9933;
    public string remoteHost = "40.118.64.248"; //RICORDARSI..NON METTERE GLI SPAZI..
    public scegliHost tipoHost = scegliHost.remotehost;
    public string zona = "ZonaAccessoGioco";

    private string host;
    private SmartFox sfs;
    private bool IsRegistrazione;
    private static ControllerLogin me;

    /// <summary>
    /// Inizializza SFS se non ancora fatto ed aggiunge gli eventListner necessari
    /// </summary>
    private void InizializzaSFS()
    {
        if (sfs == null)
        {
            sfs = new SmartFox();

            sfs.ThreadSafeMode = true;
            sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
            sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
            sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
            sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
            sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
            sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
            sfs.AddEventListener(SFSEvent.UDP_INIT, OnUdpInit);
            sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
            sfs.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVariableUpdate);

        }
    }

    private void OnUserVariableUpdate(BaseEvent evt)
    {
        Statici.idDB = sfs.MySelf.GetVariable("dbid").GetIntValue();       
        string timeStampUtente = sfs.MySelf.GetVariable("tsU").GetStringValue();      
        Statici.ControllaTSUtenti(Statici.idDB, timeStampUtente, IsRegistrazione);
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        SFSObject objIn = (SFSObject)evt.Params["params"];
        string cmd = (string)evt.Params["cmd"];
        SFSArray arrayPers = null;
        string ts = string.Empty;
        switch (cmd)
        {
            case (CMD_TIMESTAMP):
               SFSArray arrayTime =(SFSArray) objIn.GetSFSArray("timeList");
                Statici.numeroTabelleAggTimeStamp = arrayTime.Size()+1;//num record Tab sincronizzazioneDB+Utenti               

                for (int i = 0; i < arrayTime.Size(); i++)
                {
                    SFSObject objArr =(SFSObject) arrayTime.GetSFSObject(i);
                    string nomeTabella = objArr.GetUtfString("nomeTabella");
                    ts = objArr.GetUtfString("aggiornatoAl");
                    Statici.ControllaTimeStampTabelleBase(nomeTabella, ts);
                }           
                break;
            case (CMD_RICHIESTA_PERSONAGGI):
                Debug.Log("sto elaborando Personaggi");
                arrayPers = (SFSArray)objIn.GetSFSArray("persL");               
                if (objIn.ContainsKey(TIMESTAMP))//controllo paranoia
                {
                    ts = objIn.GetUtfString(TIMESTAMP);
                    if (Statici.AggiornaPersonaggi(arrayPers))                   
                        Statici.AggiornaTimeStampTabelleBase("Personaggi", ts);
                }
                else
                    Debug.LogError("non è stato ricevuto il timeStamp");
                break;
            case (CMD_RICHIESTA_PERSONAGGI_UTENTE):
                Debug.Log("sto elaborando PersonaggiUtenti");
                arrayPers = (SFSArray)objIn.GetSFSArray("persL");             
               
                if (Statici.AggiornaPersonaggiUtente(arrayPers))
                {
                    if (Statici.contatoreTabelleDiUtentiOk == NUMERO_TAB_LEGATI_AD_UTENTE) {
                        Debug.Log("posso richiedere il nuovo ts Utenti");
                        RichiediTSUtentiNew();
                    }
                }
                    
             
                break;
            case (CMD_RICHIESTA_DIPLOMAZIA):
                Debug.Log("sto elaborando Diplomazia");
                arrayPers = (SFSArray)objIn.GetSFSArray("dipL");
                if (objIn.ContainsKey(TIMESTAMP))//controllo paranoia
                {
                    ts = objIn.GetUtfString(TIMESTAMP);
                    if (Statici.AggiornaDiplomazia(arrayPers))
                        Statici.AggiornaTimeStampTabelleBase("Diplomazia", ts);

                }
                else
                    Debug.LogError("non è stato ricevuto il timeStamp");

                break;
            case (CMD_RICHIESTA_DIPLOMAZIA_PERSONAGGIO):
                Debug.Log("sto elaborando DiplomaziaPersonaggio");
                arrayPers = (SFSArray)objIn.GetSFSArray("dipL");
                if (Statici.AggiornaDiplomaziaPersonaggio(arrayPers))
                {
                    if (Statici.contatoreTabelleDiUtentiOk == NUMERO_TAB_LEGATI_AD_UTENTE)//se PersonaggiUtente e DiplomaziaPersonaggio sono stati aggiornati posso chiedere il nuovo timeStamp
                    {
                        Debug.Log("posso richiedere il nuovo ts Utenti");
                        RichiediTSUtentiNew();
                    }
                }          

                break;
            case (CMD_RICHIESTA_TS_UTENTI):
                Debug.Log("sto elaborando Utenti");               
                string timeStampRemotoNew= objIn.GetUtfString("ts");
                Statici.AggiornaTsUtenti(timeStampRemotoNew);
              
                break;
            case (CMD_NESSUN_PERSONAGGIO_TROVATO):
                Statici.contatoreTabelleDiUtentiOk++;
                if (Statici.contatoreTabelleDiUtentiOk == NUMERO_TAB_LEGATI_AD_UTENTE)
                {
                    Debug.Log("posso richiedere il nuovo ts Utenti");
                    RichiediTSUtentiNew();
                }
                break;
        }
    }

    public static void RichiediTSUtentiNew()
    {
        me.sfs.Send(new ExtensionRequest(CMD_RICHIESTA_TS_UTENTI, new SFSObject()));
    }
    //Chiamato dal bottone Registrazione sul canvas Registrazione/Login
    public void BottoneRegistrazione()
    {
        IsRegistrazione = true;
        if (string.IsNullOrEmpty(casellaNome.text.Trim())
            || string.IsNullOrEmpty(password.text.Trim())
            || string.IsNullOrEmpty(email.text.Trim()))
        {
            erroreText.text = "Compilare correttamente nome, password ed email";
            return;
        }

        ManagerScenaZero.AttivaDisattivaCanvasGroupLogin(false);
        erroreText.text = "";

        ConfigData cfg = new ConfigData();
        cfg.Host = host;
        cfg.Port = TcpPort;
        cfg.Zone = zona;

        InizializzaSFS();

        sfs.Connect(cfg);
    }

    public static void RichiestaDatiTabellaRemota(string nomeTabella)
    {
        if (nomeTabella == "Personaggi")
            me.sfs.Send(new ExtensionRequest(CMD_RICHIESTA_PERSONAGGI, new SFSObject()));
        else if (nomeTabella == "PersonaggiUtente")   
            me.sfs.Send(new ExtensionRequest(CMD_RICHIESTA_PERSONAGGI_UTENTE, new SFSObject()));
      
        else if (nomeTabella == "Diplomazia")
            me.sfs.Send(new ExtensionRequest(CMD_RICHIESTA_DIPLOMAZIA, new SFSObject()));
        else if (nomeTabella == "DiplomaziaPersonaggio")      
            me.sfs.Send(new ExtensionRequest(CMD_RICHIESTA_DIPLOMAZIA_PERSONAGGIO, new SFSObject()));
        
        //aggiungere altri else if se necessario
    }

    //Chiamato dal bottone LOGIN sul canvas Registrazione/login
    public void BottoneLogin()
    {
        IsRegistrazione = false;
        if (string.IsNullOrEmpty(casellaNome.text.Trim())
          || string.IsNullOrEmpty(password.text.Trim()))          
        {
            erroreText.text = "Compilare correttamente nome, password ed email";
            return;
        }

        ManagerScenaZero.AttivaDisattivaCanvasGroupLogin(false);
        erroreText.text = "";

        ConfigData cfg = new ConfigData();
        cfg.Host = host; //tcp 
        cfg.Port = TcpPort;  //tcp 
        cfg.UdpHost = host;  //udp
        cfg.UdpPort = UdpPort;  //udp
        cfg.Zone = zona;

        InizializzaSFS();

        sfs.Connect(cfg);
    }

    private void OnConnection(BaseEvent evt)
    {
        bool connessioneAvvenuta = (bool)evt.Params["success"];
        if (connessioneAvvenuta)
        {
            Statici.CopiaIlDB();
            SmartFoxConnection.Connection = sfs;           
            CryptoManager crMan = new CryptoManager();
            string pwdCriptata = crMan.CriptaPassword(password.text.Trim(), false);

            Debug.Log("On Connection: " + (IsRegistrazione ? "Registra" : "Login"));
            Debug.Log("utente: " + casellaNome.text);
            Debug.Log("pwd: " + pwdCriptata);
            Debug.Log("email: " + email.text);

            if (IsRegistrazione)
            {
                ISFSObject sfso = new SFSObject();
                sfso.PutUtfString("pwd", pwdCriptata);
                sfso.PutUtfString("email", email.text.Trim());
                sfso.PutBool("isReg", true);
                //eventualmente nome e cognome quando avremo quei campi a video, conviene magari riorganizzare un attimo la GUI
                Debug.Log("Siamo in registrazione utente");
                sfs.Send(new LoginRequest(casellaNome.text, "", zona, sfso));
            }
            else
            {
                ISFSObject sfso = new SFSObject();
                sfso.PutUtfString("pwd", pwdCriptata);
                sfso.PutBool("isReg", false);
                Debug.Log("Siamo in Login utente");
                sfs.Send(new LoginRequest(casellaNome.text, "", zona, sfso));
            }
        }
        else
        {
            ResettaListnerAbilitaUI(true);
            erroreText.text = "connessione fallita";
        }
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        string ragione = (string)evt.Params["reason"];
        if (ragione != ClientDisconnectionReason.MANUAL)
            erroreText.text = "Connessione persa :" + ragione;
        ResettaVariabiliPerDB();
        ManagerScenaZero.AttivaDisattivaCanvasGroupLogin(true);

    }

    private void OnUdpInit(BaseEvent evt)
    {
       


        if ((bool)evt.Params["success"])
        {
         
            Debug.Log(" Udp funkia ");
           
        }
        else
        {
          
            sfs.Disconnect();
            Debug.Log(" Udp non funkia ");
            
        }
    }

    private void OnLogin(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];
        Statici.userLocaleId = user.Id;       
        sfs.InitUDP();
      
    }

    private void OnLoginError(BaseEvent evt)
    {
        sfs.Disconnect();     
        ManagerScenaZero.AttivaDisattivaCanvasGroupLogin(true);
        erroreText.text = "Login fallito :" + (string)evt.Params["errorMessage"];
        
    }

    private void OnRoomJoin(BaseEvent evt)
    {
        ResettaListnerAbilitaUI(false);
        StartCoroutine(GestoreCanvasAltreScene.ScenaInCarica("Scena Iniziale", GiveMeText(), ManagerScenaZero.ImmagineCaricamento, ManagerScenaZero.ScrittaCaricamento));
    }

    string GiveMeText()
    {
        string[] testo = { "..una leggera brezza ti spettina le ciglia..ma tu hai l'ombrellino ", "PinoStudent......Dove finisce la realta..e inizia L'incubo...", "Loggati come ti pare..basta che non scrivi Nomi alla cazzum", "....perche non giochi con i Bigodini ai capelli??", "....Benvenuto nella terra dei Cachi  ", "..MmHmmmhhh..MmmmmMmHmm......non pensare male...mi sto mangiando una Papaya", "......Lasciate Ogni speranza o Voi Che Entrate", "..Mentre aspetti ..mangiati un Mandarino", "....Hai mai Pensato di fare un corso Accelerato Di java???", "...Se non riuscite ad attaccare...sappiate che e' colpa di Piero", "...Il Miglior gestore di Percorsi mai Implementato in un gioco ", "....il prodotto potrebbe contenere traccie di Automi..", "....Occhio che ninfea ha disseminato Trappole Lungo Il percorso", "...Pino..eddai,fai una partita pure tu", "...invece di giocare vai a raddrizzare i Radicchi nell'orto del vicino", "..mentre aspetti...Stringi forte i denti con la lingua in mezzo", "...Hai vinto un biglietto per Pinolandia...", "....(insert coin per continuare)", "...Mai mangiato la pizza ai frutti di Bosco? ", "..Se trovi il principe con i capelli fucsia....ritira Coupon per una ceretta Gratis da Piero (Rif Piero p.c.)" };
        int g = UnityEngine.Random.Range(0, (testo.Length));   //usato testo.Lenght anziche Lenght-1 perche negli interi occorre incrementarlo di uno (vedi lezione in cui ne ha parlato)
        return testo[g];
    }

    internal static void SollevaErroreAggiornamentoDB(string errore)
    {
        me.erroreText.text = errore;
    }

    private void OnRoomJoinError(BaseEvent evt)
    {
        ManagerScenaZero.AttivaDisattivaCanvasGroupLogin(true);
        erroreText.text = "Impossibile unirsi alla Lobby :" + (string)evt.Params["errorMessage"];
    }

    private void ResettaListnerAbilitaUI(bool abilita)
    {
        sfs.RemoveAllEventListeners();
        ManagerScenaZero.AttivaDisattivaCanvasGroupLogin(abilita);
        erroreText.text = "";
    }


    // Use this for initialization
    private void Start()
    {
     
        Application.runInBackground = true;
        ResettaVariabiliPerDB();
        erroreText.text = string.Empty;
        me = this;       
        switch (tipoHost)
        {
            case scegliHost.localhost:
                host = localhost;
                break;

            case scegliHost.remotehost:
                host = remoteHost;
                break;

            default:
                break;
        }
        host = host.Trim();   //cosi' toglie gli spazi ( se ci lascio gli spazi non va)
    }

    private void ResettaVariabiliPerDB()
    {
          Statici.contatoreTimeStampOk = 0;//contatore dei timeStamp aggiornati
          Statici.contatoreTabelleDiUtentiOk = 0;//contatore delle tabelle aggiornate dopo il controllo del timestamp della tabella Utenti  
          Statici.numeroTabelleAggTimeStamp = 0;//numero delle tabelle locali di cui si deve controllare il loro timeStamp:righe della tabella sincronizzazioneDB+tabella Utenti
                                                //chiudo la connessione con il DB locale
        if (Statici.conn != null && Statici.conn.State == System.Data.ConnectionState.Open)
        {
            Statici.conn.Close();
            Statici.conn = null;
        }
          Statici.idDB = 0;
   }

    internal static void EntraNellaLobby()
    {
        me.sfs.Send(new JoinRoomRequest("The Lobby"));
    }

    // Update is called once per frame
    private void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
        
    }
}