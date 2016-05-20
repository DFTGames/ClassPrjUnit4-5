using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerNetwork : MonoBehaviour
{
    public Text punteggi;
    public Text[] punteggiGiocatori;
    public Button sgruppaButton;

    private static ManagerNetwork me;
    private ControllerMaga controllerPlayer;

    private GameManager gameManager;
    private GestoreCanvasNetwork gestoreCanvasNetwork;
    private Minimappa minimappa;
    private string nomeScenaSuccessiva = string.Empty;
    private SmartFox sfs;
    private float tempo = 5f;

    public static void AggiornaPunteggi(int indicePunteggio, string nome, int numeroUccisioni)
    {
        me.punteggiGiocatori[indicePunteggio].text = nome + ": " + numeroUccisioni.ToString();
    }

    public static void CambiaScenaPortale(string nomeScena)
    {
        me.sfs.Send(new ExtensionRequest("del", new SFSObject(), me.sfs.LastJoinedRoom));
        me.nomeScenaSuccessiva = nomeScena;
    }

    public static void TimeSyncRequest()
    {   if (!Statici.inGioco) return;
            me.sfs.Send(new ExtensionRequest("getTime", new SFSObject(), me.sfs.LastJoinedRoom));
    }


    /// <summary>
    /// Se il player locale vuole uscire da una stanza di gioco
    /// chiede di unirsi alla lobby e abilita la ui delle partite.
    /// </summary>
    public void BottoneSgruppa()
    {
        sfs.Send(new JoinRoomRequest("The Lobby"));
    }

    public void Disconnect()
    {
        sfs.Disconnect();
    }

    public void ModificaRaggioRemoto(float distanzaRaggio)
    {
        ISFSObject sfsObjOut = new SFSObject();
        sfsObjOut.PutFloat("dr", distanzaRaggio);
        sfs.Send(new ExtensionRequest("raggio", sfsObjOut, sfs.LastJoinedRoom));
    }

    public void NemicoColpito(int utenteColpito)
    {
        ISFSObject sfsObjOut = new SFSObject();
        sfsObjOut.PutInt("uco", utenteColpito);

        sfs.Send(new ExtensionRequest("danno", sfsObjOut, me.sfs.LastJoinedRoom));
    }

    public void OnObjectMessage(BaseEvent evt)
    {
        ISFSObject dataObj = (SFSObject)evt.Params["message"];
        SFSUser sender = (SFSUser)evt.Params["sender"];

        if (dataObj.ContainsKey("cmd"))
        {
            switch (dataObj.GetUtfString("cmd"))
            {
                case "rm":
                    Debug.Log("Rimuovo il player remoto " + sender.Id);
                    RemoveRemotePlayer(sender);
                    break;
            }
        }
    }

    internal void InviaChat(string text)
    {        
        SFSObject objOut = new SFSObject();
        objOut.PutUtfString("nome", Statici.nomePersonaggio);
        sfs.Send(new PublicMessageRequest(text,objOut));
    }

    internal void Resuscita()
    {
        SFSObject objOut = new SFSObject();
        objOut.PutFloat("vitaM", Statici.datiPersonaggioLocale.VitaMassima);
        sfs.Send(new ExtensionRequest("res", objOut, sfs.LastJoinedRoom));
    }

    private IEnumerator FinePartita()
    {
        yield return new WaitForSeconds(5f);
        BottoneSgruppa();
    }

    /*
    private void InviaTransformLocali()
    {
        ISFSObject objOut = new SFSObject();
        objOut.PutFloat("x", Statici.playerLocaleGO.transform.position.x);
        objOut.PutFloat("y", Statici.playerLocaleGO.transform.position.y);
        objOut.PutFloat("z", Statici.playerLocaleGO.transform.position.z);
        objOut.PutFloat("rot", Statici.playerLocaleGO.transform.rotation.eulerAngles.y);
        sfs.Send(new ExtensionRequest("regT", objOut, sfs.LastJoinedRoom));
        controllerPlayer.MovementDirty = false;
    }
    */
    public static void  InviaTransformLocali(NetworkTransform ne)  //MODIFICA BY LUCA
    {
        ISFSObject objOut = new SFSObject();
        objOut.PutFloat("x", ne.position.x);
        objOut.PutFloat("y", ne.position.y);
        objOut.PutFloat("z", ne.position.z);
        objOut.PutFloat("rx", ne.rotation.x);
        objOut.PutFloat("ry", ne.rotation.y);
        objOut.PutFloat("rz", ne.rotation.z);
        me.sfs.Send(new ExtensionRequest("regT", objOut, me.sfs.LastJoinedRoom));
      //  me.controllerPlayer.MovementDirty = false;
    }
   
    public static void InviaAnimazioneControllerTast(float forward, float turn,bool onGround , float jump , float jumpLeg,bool attacco1,bool attacco2)
    {
        SFSObject objOut = new SFSObject();
        objOut.PutFloat("f", forward);
        objOut.PutFloat("t", turn);
        objOut.PutBool("o", onGround);
        objOut.PutFloat("j", jump);
        objOut.PutFloat("jL", jumpLeg);
        objOut.PutBool("a1", attacco1);
        objOut.PutBool("a2", attacco2);
        me.sfs.Send(new ExtensionRequest("SanT", objOut,me. sfs.LastJoinedRoom));

    }
    public static void InviaAnimazioneControllerClick(float forward, bool attacco1, bool attacco2)
    {
        SFSObject objOut = new SFSObject();
        objOut.PutFloat("f", forward);
        objOut.PutBool("a1", attacco1);
        objOut.PutBool("a2", attacco2);
        me.sfs.Send(new ExtensionRequest("SanC", objOut, me.sfs.LastJoinedRoom));

    }

    private void InvioDatiPlayerLocale(int userId)
    {
        SFSObject objOut = new SFSObject();
        // objOut.PutUtfString("model", Statici.classePersonaggio);
        //objOut.PutUtfString("nome", Statici.nomePersonaggio);
        objOut.PutFloat("x", Statici.playerLocaleGO.transform.position.x);
        objOut.PutFloat("y", Statici.playerLocaleGO.transform.position.y);
        objOut.PutFloat("z", Statici.playerLocaleGO.transform.position.z);
        objOut.PutFloat("rx", Statici.playerLocaleGO.transform.rotation.eulerAngles.x);
        objOut.PutFloat("ry", Statici.playerLocaleGO.transform.rotation.eulerAngles.y);
        objOut.PutFloat("rz", Statici.playerLocaleGO.transform.rotation.eulerAngles.z);
        objOut.PutInt("usIn", userId);
        objOut.PutUtfString("scena", SceneManager.GetActiveScene().name);
        // objOut.PutFloat("vita", Statici.datiPersonaggioLocale.Vita);
        sfs.Send(new ExtensionRequest("respawn", objOut, sfs.LastJoinedRoom));
    }

    private void OnApplicationQuit()
    {
        sfs.Send(new LeaveRoomRequest());
        Disconnect();
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        sfs.RemoveAllEventListeners();
        SceneManager.LoadScene("ScenaConnessione");
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        ISFSObject sfsObjIn = (SFSObject)evt.Params["params"];
        string cmd = (string)evt.Params["cmd"];
        switch (cmd)
        {
            case ("respawn"):

                int utente = sfsObjIn.GetInt("ut");
                string nomeScenaAttualeAvatar = sfsObjIn.GetUtfString("scena");
                string classe = sfsObjIn.GetUtfString("classe");
                // int numeroPostoSpawn = sfsObjIn.GetInt("nSpawn");
                // int numeroUccisioni = sfsObjIn.GetInt("nUcc");
                if (nomeScenaAttualeAvatar != SceneManager.GetActiveScene().name)
                    return;
                string modello = sfsObjIn.GetUtfString("model");
                string nome = sfsObjIn.GetUtfString("nome");
                float vitaIniziale = sfsObjIn.GetFloat("vita");
                float vitaMax = sfsObjIn.GetFloat("vitaM");
                float mana = sfsObjIn.GetFloat("mana");
                float manaMax = sfsObjIn.GetFloat("manaM");
                float xp = sfsObjIn.GetFloat("xp");
                float xpMax = sfsObjIn.GetFloat("xpM");
                float attacco = sfsObjIn.GetFloat("att");
                float difesa = sfsObjIn.GetFloat("dif");
                int livello = sfsObjIn.GetInt("liv");
                bool giocabile = sfsObjIn.GetBool("gioc");

                Vector3 posizioneIniziale = new Vector3(0, 1, 0);
                posizioneIniziale.x = sfsObjIn.GetFloat("x");
                posizioneIniziale.y = sfsObjIn.GetFloat("y");
                posizioneIniziale.z = sfsObjIn.GetFloat("z");

                Vector3 rotazioneIniziale = new Vector3(0, 0, 0);
                rotazioneIniziale.x= sfsObjIn.GetFloat("rx");
                rotazioneIniziale.y= sfsObjIn.GetFloat("ry");
                rotazioneIniziale.z= sfsObjIn.GetFloat("rz");

                if (sfs.MySelf.Id == utente)
                {
                    Statici.datiPersonaggioLocale.VitaMassima = vitaMax;
                    Statici.datiPersonaggioLocale.Vita = vitaIniziale;
                    Statici.datiPersonaggioLocale.ManaMassimo = manaMax;
                    Statici.datiPersonaggioLocale.Mana = mana;
                    Statici.datiPersonaggioLocale.XpMassimo = xpMax;
                    Statici.datiPersonaggioLocale.Xp = xp;
                    Statici.datiPersonaggioLocale.Attacco = attacco;
                    Statici.datiPersonaggioLocale.Difesa = difesa;
                    Statici.datiPersonaggioLocale.Livello = livello;
                    Statici.datiPersonaggioLocale.Giocabile = giocabile;
                    gestoreCanvasNetwork.VisualizzaDatiPlayerLocale(Statici.nomePersonaggio, Statici.datiPersonaggioLocale.Vita.ToString());
                    GestoreCanvasAltreScene.AggiornaDati(Statici.datiPersonaggioLocale);

                    return;
                }
                if (!Statici.PlayersRemoti.ContainsKey(utente))
                {
                    GameObject remotePlayer = Instantiate(Resources.Load(modello),posizioneIniziale, Quaternion.Euler(rotazioneIniziale.x, rotazioneIniziale.y, rotazioneIniziale.z)) as GameObject; 
                    remotePlayer.GetComponent<ControllerMaga>().enabled = false;  //???
                    Statici.aggiungiComponenteAnimazione(remotePlayer,false);  //AGGIUNTO PER LE ANIMAZIONI
                   
                    DatiPersonaggio datiPersonaggioRemoto = remotePlayer.GetComponent<DatiPersonaggio>();
                    datiPersonaggioRemoto.SonoUtenteLocale = false;
                    datiPersonaggioRemoto.VitaMassima = vitaMax;
                    datiPersonaggioRemoto.Vita = vitaIniziale;
                    datiPersonaggioRemoto.ManaMassimo = manaMax;
                    datiPersonaggioRemoto.Mana = mana;
                    datiPersonaggioRemoto.XpMassimo = xpMax;
                    datiPersonaggioRemoto.Xp = xp;
                    datiPersonaggioRemoto.Attacco = attacco;
                    datiPersonaggioRemoto.Difesa = difesa;
                    datiPersonaggioRemoto.Livello = livello;
                    datiPersonaggioRemoto.Giocabile = giocabile;
                    datiPersonaggioRemoto.Nome = nome;              
                    remotePlayer.GetComponentInChildren<TextMesh>().text = nome;
                    datiPersonaggioRemoto.Utente = utente;    
                    Statici.AggiungiDizionarioNetwork(utente, remotePlayer,false);
                    Statici.partenza = true;
                    InvioDatiPlayerLocale(utente);//invio i miei dati solo all'utente specificato
                }

                break;

            case ("datiUcc"):
                int numeroPostoSpawn = sfsObjIn.GetInt("nSpawn");
                int numeroUccisioni = sfsObjIn.GetInt("nUcc");
                string nomeUtente = sfsObjIn.GetUtfString("nome");
                AggiornaPunteggi(numeroPostoSpawn, nomeUtente, numeroUccisioni);

                break;

            case ("regT"):   //MODIFCA BY LUCA
                int user = sfsObjIn.GetInt("u");
                if (Statici.datiPersonaggioLocale.Utente == user)
                    return;

                Vector3 pos = new Vector3(0, 1, 0);   // ?? 
                pos.x = sfsObjIn.GetFloat("x");
                pos.y = sfsObjIn.GetFloat("y");
                pos.z = sfsObjIn.GetFloat("z");
                Vector3 rot = new Vector3(0, 0, 0);
                rot.x = sfsObjIn.GetFloat("rx");
                rot.y = sfsObjIn.GetFloat("ry");
                rot.z = sfsObjIn.GetFloat("rz");

                NetworkTransform net = NetworkTransform.CreaOggettoNetworktransform(pos, rot);
               
                if (Statici.PlayerTransform.ContainsKey(user))
                Statici.PlayerTransform[user].ricevitransform(net, user);

                break;
            case ("anT"):
                /*
                int user1 = sfsObjIn.GetInt("id");

                if (Statici.datiPersonaggioLocale.Utente != user1 && Statici.PlayersRemoti.ContainsKey(user1))                                 
                        Statici.PlayersRemoti[user1].GetComponent<AnimSyncronRiceiver>().eseguiAnimazioniRemoteT(sfsObjIn);                
                    return;
                    */
                break;
            case ("anC"):
                int user2 = sfsObjIn.GetInt("id");       
                if (Statici.datiPersonaggioLocale.Utente != user2 && Statici.PlayersRemoti.ContainsKey(user2))  
                    Statici.PlayersRemoti[user2].GetComponent<AnimSyncronRiceiver>().eseguiAnimazioniRemoteC(sfsObjIn);
                return;
                break;
            case ("danno"):
                int utenteColpitoId = sfsObjIn.GetInt("uci");
                float vita = sfsObjIn.GetFloat("vita");
                int utenteCheHaInflittoDannoId = sfsObjIn.GetInt("userI");
                if (Statici.datiPersonaggioLocale.Utente == utenteColpitoId)
                {
                    Statici.datiPersonaggioLocale.Vita = vita;
                    gestoreCanvasNetwork.VisualizzaDatiPlayerLocale(Statici.nomePersonaggio, Statici.datiPersonaggioLocale.Vita.ToString());
                    gestoreCanvasNetwork.VisualizzaChiTiAttacca(Statici.PlayersRemoti[utenteCheHaInflittoDannoId].GetComponentInChildren<TextMesh>().text);
                    GestoreCanvasAltreScene.AggiornaDati(Statici.datiPersonaggioLocale);
                    if (Statici.datiPersonaggioLocale.Vita<=0f)
                    {
                        Statici.playerLocaleGO.GetComponent<SwitchVivoMorto>().AttivaRagdoll();
                        gestoreCanvasNetwork.PannelloMorteOn();
                    }
                }
                else
                {
                    if (Statici.PlayersRemoti.ContainsKey(utenteColpitoId))
                    {
                        DatiPersonaggio datiPersonaggioColpito = Statici.PlayersRemoti[utenteColpitoId].GetComponent<DatiPersonaggio>();
                        datiPersonaggioColpito.Vita = vita;
                        if (datiPersonaggioColpito.Vita<=0f)
                            Statici.PlayersRemoti[utenteColpitoId].GetComponent<SwitchVivoMorto>().AttivaRagdoll();
                    }
                }

                break;
 
            case ("res"):
                int uId = sfsObjIn.GetInt("u");
                float vitaResurrezione = sfsObjIn.GetFloat("vita");
                if (Statici.datiPersonaggioLocale.Utente != uId)
                {
                    DatiPersonaggio datiPersonaggioRemoto = Statici.PlayersRemoti[uId].GetComponent<DatiPersonaggio>();
                    datiPersonaggioRemoto.Vita = vitaResurrezione;
                    Statici.PlayersRemoti[uId].GetComponent<SwitchVivoMorto>().DisattivaRagdoll();
                }
                else
                {
                    Statici.datiPersonaggioLocale.Vita = vitaResurrezione;
                    gestoreCanvasNetwork.VisualizzaDatiPlayerLocale(Statici.nomePersonaggio, Statici.datiPersonaggioLocale.Vita.ToString());
                    Statici.playerLocaleGO.GetComponent<SwitchVivoMorto>().DisattivaRagdoll();
                    GestoreCanvasAltreScene.AggiornaDati(Statici.datiPersonaggioLocale);
                }
                break;

            case ("del"):
                int userIdDaDeletare = sfsObjIn.GetInt("ud");
                if (Statici.datiPersonaggioLocale.Utente == userIdDaDeletare)
                {
                    Statici.partenza = false;
                    Statici.RimuoviDizionarioAllNetwork();
                     Destroy(Statici.playerLocaleGO);
                    Statici.playerLocaleGO = null;
                    sfs.RemoveAllEventListeners();
                    SceneManager.LoadScene(nomeScenaSuccessiva);

                    return;
                }
                if (Statici.PlayersRemoti.ContainsKey(userIdDaDeletare))
                {
                    minimappa.DistruggiMarcatore(userIdDaDeletare);
                    Destroy(Statici.PlayersRemoti[userIdDaDeletare]);
                    Statici.RimuoviDizionarioUtenteNetwork(userIdDaDeletare);
                }
                break;

            case ("ownOut"):
                Statici.messaggio = "La partita è stata eliminata perchè l'owner è uscito.";
                Statici.ownerUscito = true;
                BottoneSgruppa();
                break;

            case ("fine"):
                Statici.finePartita = true;
                sgruppaButton.interactable = false;
                int numeroUccisioniWinner = sfsObjIn.GetInt("nWin");
                int idWinner = sfsObjIn.GetInt("win");
                string nomeWinner = sfsObjIn.GetUtfString("nomeW");
                punteggi.gameObject.SetActive(true);
                if (idWinner != -1)
                {
                    if (Statici.datiPersonaggioLocale.Utente == idWinner)
                        punteggi.text = "Hai vinto!!! Punteggio: " + numeroUccisioniWinner;
                    else
                        punteggi.text = nomeWinner + " ha vinto!!! Con un punteggio di: " + numeroUccisioniWinner;
                }
                else
                    punteggi.text = "Nessun vincitore";
                StartCoroutine(FinePartita());
                break;
            case ("time"): 
                    HandleServerTime(sfsObjIn);
                break;

                    default:
                break;
        }
    }

    private void HandleServerTime(ISFSObject dt)
    {
        long time = dt.GetLong("t");
        TimeManager.Instance.Synchronize(Convert.ToDouble(time));
    }

    private void OnPublicMessage(BaseEvent evt)
    {
          User sender = (User)evt.Params["sender"];
          string msg = (string)evt.Params["message"];
          SFSObject onjIn = (SFSObject)evt.Params["data"];
          string mittente = (sender.IsItMe) ? "Tu" : onjIn.GetUtfString("nome");
          gestoreCanvasNetwork.ScriviMessaggioChat(mittente, msg);        
    }

    private void OnRoomJoin(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];

        if (!room.IsGame)
        {
            if (Statici.playerLocaleGO != null)
            {
         
                Destroy(Statici.playerLocaleGO);
                Statici.playerLocaleGO = null;
            }
            if (Statici.PlayersRemoti.Count != 0)
            {
             
                foreach (KeyValuePair<int, GameObject> playerRemoto in Statici.PlayersRemoti)
                {
                    Destroy(playerRemoto.Value);                 
                }
                Statici.RimuoviDizionarioAllNetwork();

            }
            sgruppaButton.interactable = false;
            Statici.numeroPostoSpawn = -1;
            sfs.RemoveAllEventListeners();
            SceneManager.LoadScene("ScenaStanze");
        }
    }

    private void OnUserEnterRoom(BaseEvent evt)
    {
        /*   Room room = (Room)evt.Params["room"];
           User user = (User)evt.Params["user"];
           if (room.IsGame && user != sfs.MySelf)
               //InvioDatiPlayerLocale(user.Id);
               InvioDatiPlayerLocale(-1);*/
    }

    private void OnUserExitRoom(BaseEvent evt)
    {
        SFSUser user = (SFSUser)evt.Params["user"];
        RemoveRemotePlayer(user);
    }

    private void RemoveLocalPlayer()
    {
        SFSObject obj = new SFSObject();
        obj.PutUtfString("cmd", "rm");
        sfs.Send(new ObjectMessageRequest(obj, sfs.LastJoinedRoom));
    }

    private void RemoveRemotePlayer(SFSUser user)
    {
        if (user == sfs.MySelf) return;

        if (Statici.PlayersRemoti.ContainsKey(user.Id))
        {
            minimappa.DistruggiMarcatore(user.Id);
            Destroy(Statici.PlayersRemoti[user.Id]);
            Statici.RimuoviDizionarioUtenteNetwork(user.Id);

        }
    }

    private void SpawnaPlayerLocale()
    {
        Statici.playerLocaleGO = Instantiate(Resources.Load(Statici.nomeModello), GameObject.Find(Statici.posizioneInizialeMulti + Statici.numeroPostoSpawn.ToString()).transform.position, Quaternion.identity) as GameObject;      
        Statici.aggiungiComponenteAnimazione(Statici.playerLocaleGO,true);  //AGGIUNTO PER LE ANIMAZIONI
        Statici.playerLocaleGO.GetComponentInChildren<TextMesh>().text = Statici.nomePersonaggio;
        Statici.datiPersonaggioLocale = Statici.playerLocaleGO.GetComponent<DatiPersonaggio>();
        Statici.datiPersonaggioLocale.Nome = Statici.nomePersonaggio;
        Statici.datiPersonaggioLocale.Utente = Statici.userLocaleId;
        Statici.datiPersonaggioLocale.SonoUtenteLocale = true;
        controllerPlayer = Statici.playerLocaleGO.GetComponent<ControllerMaga>();

        Statici.AggiungiDizionarioNetwork(Statici.userLocaleId, Statici.playerLocaleGO,true);

        InvioDatiPlayerLocale(-1);//avviso tutti quelli nella mia scena
    }

    // Use this for initialization
    private void Start()
    {
        if (!Statici.multigiocatoreOn)
            return;
        TimeManager.Instance.Init();
        Application.runInBackground = true;
        punteggi.text = "";
        punteggi.gameObject.SetActive(false);

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gestoreCanvasNetwork = GameObject.Find("ManagerCanvasMultiplayer").GetComponent<GestoreCanvasNetwork>();
        me = this;
        minimappa = GameObject.Find("Minimappa").GetComponent<Minimappa>();
        if (!SmartFoxConnection.NonNulla)
        {
            SceneManager.LoadScene("ScenaZero");
            return;
        }
        sfs = SmartFoxConnection.Connection;
        sfs.ThreadSafeMode = true;
        sfs.AddEventListener(SFSEvent.OBJECT_MESSAGE, OnObjectMessage);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
        sfs.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);

        SpawnaPlayerLocale();
    }

    // Update is called once per frame
    private void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
        // ELIMINATO BY LUCA
    //    if (Statici.playerLocaleGO != null && controllerPlayer != null && controllerPlayer.MovementDirty && Statici.partenza)
     //       InviaTransformLocali();
        //FINE ELIMINAZIONE
    }
}