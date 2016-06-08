using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Util;
using UnityEngine;
using UnityEngine.UI;

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
        }
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

    //Chiamato dal bottone LOGIN sul canvas Registrazione/login
    public void BottoneLogin()
    {
        IsRegistrazione = false;
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
            SmartFoxConnection.Connection = sfs;
            //   sfs.InitUDP();
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
    }

    private void OnUdpInit(BaseEvent evt)
    {
        // Remove SFS2X listeners


        if ((bool)evt.Params["success"])
        {
            // Set invert mouse Y option
            //   OptionsManager.InvertMouseY = invertMouseToggle.isOn;
            Debug.Log(" Udp funkia ");
            // Load lobby scene
            //    Application.LoadLevel("Lobby");
        }
        else
        {
            // Disconnect
            sfs.Disconnect();
            Debug.Log(" Udp non funkia ");
            // Show error message
            //  errorText.text = "UDP initialization failed: " + (string)evt.Params["errorMessage"];
        }
    }

    private void OnLogin(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];
        Statici.userLocaleId = user.Id;
        //  sfs.InitUDP(host,UdpPort);
        sfs.InitUDP();
        //sfs.Send(new JoinRoomRequest("The Lobby"));       //Viene fatto sul server tramite Postlogin
    }

    private void OnLoginError(BaseEvent evt)
    {
        sfs.Disconnect();
        ResettaListnerAbilitaUI(true);
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
        int g = Random.Range(0, (testo.Length));   //usato testo.Lenght anziche Lenght-1 perche negli interi occorre incrementarlo di uno (vedi lezione in cui ne ha parlato)
        return testo[g];
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
        erroreText.text = string.Empty;
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

    // Update is called once per frame
    private void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
    }
}