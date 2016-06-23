using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StanzeManager : MonoBehaviour
{
    public GameObject bottonePartita;
    public Button bottoneSgruppa;
    public Button buttonReady;
    public CanvasGroup canvasGroup;
    public CanvasGroup canvasGroupChat;
    public Text contenutoChat;
    public Transform ContenutoListaPartite;
    public Text erroreText;
    public GameObject immagineCaricamento;
    public InputField inputChat;
    public InputField inputNomePartita;
    public Text messaggio;
    public Transform[] posizioniPlayerIniziali;
    public Text scrittaCaricamento;
    public Slider sliderUserMax;
    public Transform startPoint;
    public Text userMaxText;
    private const string EXTENSION_CLASS = "sferenetsfsextension.SfereNetSfsExtension";
    private const string EXTENSION_ID = "sfere";
    private static StanzeManager me;
    private CanvasGroup contenutoPartiteCanvasGroup;
    private Dictionary<int, GameObject> dizionarioStanzeDiGioco = new Dictionary<int, GameObject>();
    private List<int> listaPronti = null;
    private bool messaggioSuSchermoOwnerOn = false;
    private short numeroMassimoUtentiInStanza = 4;
    private short numeroMaxDecisoDaOwner = 1;
    //private SmartFox sfs;
    private float tempo = 0f;
    private int utentiInStanza = 0;

    public void AggiornaNumeroMassimoUser()
    {
        numeroMaxDecisoDaOwner = (short)sliderUserMax.value;
        userMaxText.text = numeroMaxDecisoDaOwner.ToString();
    }

    public void BottoneGioca()
    {
        if (Statici.nomePersonaggio != string.Empty && Statici.datiPersonaggio.Dati.nomePersonaggio != string.Empty)
        {
            if (inputNomePartita.text.Replace(" ", "") != string.Empty)
            {
                ResettaErrore();
                BloccaSbloccaCanvas(false);
                RoomSettings settings = new RoomSettings(inputNomePartita.text.Replace(" ", "").Trim());
                settings.GroupId = "games";
                settings.IsGame = true;
                settings.MaxUsers = numeroMaxDecisoDaOwner;
                settings.MaxSpectators = 0;
                SFSRoomVariable roomVariableStart = new SFSRoomVariable("gameStarted", false);
                roomVariableStart.IsPersistent = true;
                settings.Variables.Add(roomVariableStart);
                settings.Extension = new RoomExtension(EXTENSION_ID, EXTENSION_CLASS);
                Statici.sfs.Send(new CreateRoomRequest(settings, true, Statici.sfs.LastJoinedRoom));
            }
            else
                erroreText.text = "Assegnare un nome alla partita";
        }
        else
            erroreText.text = "Inserire un nome e scegliere una classe";
    }

    /// <summary>
    /// Se il player locale vuole uscire da una stanza di gioco
    /// chiede di unirsi alla lobby e abilita la ui delle partite.
    /// </summary>
    public void BottoneSgruppa()
    {
        listaPronti.Clear();
        buttonReady.gameObject.SetActive(false);
        Statici.sfs.Send(new JoinRoomRequest("The Lobby"));
        BloccaSbloccaCanvas(true);
    }

    public void ButtonReady()
    {
        buttonReady.interactable = false;
        Statici.sfs.Send(new ExtensionRequest("ready", new SFSObject(), Statici.sfs.LastJoinedRoom));
    }

    public void ClickBottonePartita(int roomId)
    {
        if (Statici.nomePersonaggio != string.Empty && Statici.datiPersonaggio.Dati.nomeModello != string.Empty)
        {
            BloccaSbloccaCanvas(false);
            Statici.sfs.Send(new JoinRoomRequest(roomId));
        }
        else
            erroreText.text = "Inserire un nome e scegliere una classe";
    }

    public void Disconnect()
    {
        Statici.sfs.Disconnect();
    }

    public void InviaMessaggioChat()
    {
        string messaggioChat = inputChat.text;
        if (messaggioChat.Trim() == string.Empty)
            return;
        inputChat.text = string.Empty;
        SFSObject objOut = new SFSObject();
        objOut.PutUtfString("nome", Statici.nomePersonaggio);
        Statici.sfs.Send(new PublicMessageRequest(messaggioChat, objOut));
        canvasGroupChat.interactable = false;
    }

    /// <summary>
    /// La stanza di gioco viene rimossa quando è vuota.
    /// Tutti riceveranno questo evento e se avevano ancora visualizzato il bottone della partita
    /// relativo a quella stanza, lo distruggono perchè la stanza non esiste più.
    /// </summary>
    /// <param name="evt"></param>
    public void OnRoomRemoved(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
        if (dizionarioStanzeDiGioco.ContainsKey(room.Id))
        {
            Destroy(dizionarioStanzeDiGioco[room.Id]);
            dizionarioStanzeDiGioco.Remove(room.Id);
            PopolaListaPartite();
        }
    }

    public void ResettaErrore()
    {
        erroreText.text = "";
    }

    private void Awake()
    {
        Application.runInBackground = true;
        me = this;
        canvasGroupChat.interactable = false;
        canvasGroup.interactable = false;
    }

    private void BloccaSbloccaCanvas(bool abilita)
    {
        canvasGroup.interactable = abilita;
        contenutoPartiteCanvasGroup.interactable = abilita;
        bottoneSgruppa.interactable = !abilita;
    }

    private void CaricaScena()
    {
        ResettaEventListner();

        Statici.playerRemotiGestore.Clear();
        StartCoroutine(GestoreCanvasAltreScene.ScenaInCarica("Isola", "Isola", immagineCaricamento, scrittaCaricamento));
    }

    /// <summary>
    /// invia i dati al server in modo che possa avvisare l'utente o gli utenti interessati.
    /// Se l'intero passato al metodo è -1, verranno avvisati tutti i client presenti nella stanza,
    /// questo deve accadere quando l'utente locale entra nella stanza di gioco per avvisare quelli già
    /// presenti delle sue caratteristiche.
    /// Se l'id dello user è diverso da -1, il server avviserà solo quell'utente,
    /// questo accade quando un client si accorge che un nuovo utente è entrato nella sua stanza.
    /// In questo caso lo spawnme non viene inviato a tutti perchè quelli già presenti in stanza erano stati già avvisati.
    /// </summary>
    /// <param name="userId"></param>
    private void InvioDatiPlayerLocale(int userId)
    {
        //passare tutti i dati e non solo questi
        SFSObject objOut = new SFSObject();
        objOut.PutUtfString("model", Statici.datiPersonaggio.Dati.nomeModello);
        objOut.PutUtfString("nome", Statici.datiPersonaggio.Dati.nomePersonaggio);
        objOut.PutUtfString("classe", Statici.datiPersonaggio.Dati.classe);
        objOut.PutBool("gioc", true);
        objOut.PutInt("liv", Statici.datiPersonaggio.Dati.Livello);
        objOut.PutFloat("mana", Statici.datiPersonaggio.Dati.Mana);
        objOut.PutFloat("manaM", Statici.datiPersonaggio.Dati.ManaMassimo);
        objOut.PutFloat("exp", Statici.datiPersonaggio.Dati.Xp);
        objOut.PutFloat("expM", Statici.datiPersonaggio.Dati.XPMassimo);
        objOut.PutFloat("vita", Statici.datiPersonaggio.Dati.Vita);
        objOut.PutFloat("vitaM", Statici.datiPersonaggio.Dati.VitaMassima);
        objOut.PutFloat("att", Statici.datiPersonaggio.Dati.Attacco);
        objOut.PutFloat("dif", Statici.datiPersonaggio.Dati.difesa);
        objOut.PutUtfString("scena", SceneManager.GetActiveScene().name);
        objOut.PutInt("usIn", userId);
        Statici.sfs.Send(new ExtensionRequest("spawnMe", objOut, Statici.sfs.LastJoinedRoom));
    }

    private void OnApplicationQuit()
    {
        buttonReady.gameObject.SetActive(false);
        Statici.sfs.Send(new LeaveRoomRequest());
        Disconnect();
    }

    private void OnConnectionLost(BaseEvent evt)
    {   //stampare errore, aspettare tot secondi e poi caricare scena.
        Statici.nomePersonaggio = string.Empty;
        ResettaErrore();
        BloccaSbloccaCanvas(true);
        Statici.sfs.RemoveAllEventListeners();
        StartCoroutine(GestoreCanvasAltreScene.ScenaInCarica("ScenaZero", "Connection Lost", immagineCaricamento, scrittaCaricamento));
    }

    /// <summary>
    /// Se il client riceve il comando:
    /// 1)spawnme=se il comando lo riceve l'utente che lo ha mandato,
    /// recupera il numeroSpawn che serve sia per recuperare la posizione iniziale di spawn
    /// che per aggiornare la casella dei punteggi relativa a se stesso.
    /// Istanzia il suo personaggio locale e gli assegna il nome il numero spawn e l'id.
    /// Se invece lo ricevono gli altri player, istanzieranno l'avatar corrispondente ecc..
    /// 2)ownOut=questo comando viene inviato dal server quando l'owner della stanza esce
    /// dalla stanza di gioco. Quelli che ne facevano parte ricevono il comando e devono
    /// abbandonare la stanza e tornare alla lobby.
    /// Quando la stanza di gioco sarà vuota, verrà rimossa in automatico e tutti riceveranno
    /// l'evento Room Removed.
    /// </summary>
    /// <param name="evt"></param>
    private void OnExtensionResponse(BaseEvent evt)
    {
        ISFSObject sfsObjIn = (SFSObject)evt.Params["params"];
        string cmd = (string)evt.Params["cmd"];
        switch (cmd)
        {
            case ("spawnMe"):

                int utente = sfsObjIn.GetInt("ut");
                int numeroSpawn = sfsObjIn.GetInt("nSpawn");
                if (Statici.sfs.MySelf.Id == utente)
                {
                    Statici.numeroPostoSpawn = numeroSpawn;
                    Vector3 posizioneIniziale = GameObject.Find("Postazione" + Statici.numeroPostoSpawn.ToString()).transform.position;
                    Statici.playerLocaleGO.transform.position = posizioneIniziale;
                    Statici.playerLocaleGO.GetComponentInChildren<TextMesh>().text = Statici.nomePersonaggio;

                    Statici.playerLocaleGO.GetComponent<NetworkPlayer>().User = utente;

                    Statici.datiPersonaggioLocale = Statici.playerLocaleGO.GetComponent<DatiPersonaggio>();
                    Statici.datiPersonaggioLocale.IndicePunteggio = numeroSpawn;
                    Statici.datiPersonaggioLocale.Utente = Statici.userLocaleId;
                    buttonReady.gameObject.SetActive(true);
                    buttonReady.interactable = true;
                    return;
                }
                if (!Statici.playerRemotiGestore.ContainsKey(utente))
                {
                    string modello = sfsObjIn.GetUtfString("model");
                    string nome = sfsObjIn.GetUtfString("nome");
                    Vector3 posizioneIniziale = GameObject.Find("Postazione" + numeroSpawn.ToString()).transform.position;
                    float rotazioneIniziale = GameObject.Find("Postazione" + numeroSpawn.ToString()).transform.rotation.y;
                    SpawnRemotePlayer(utente, modello, nome, numeroSpawn, posizioneIniziale, Quaternion.Euler(0, rotazioneIniziale, 0));
                }
                break;

            case ("ownOut"):
                Statici.messaggio = "La partita è stata eliminata perchè l'owner è uscito.";
                Statici.ownerUscito = true;
                BottoneSgruppa();
                break;

            case ("ready"):
                Statici.nomeModello = Statici.datiPersonaggio.Dati.nomeModello;
                Statici.posizioneInizialeMulti = "start";
                CaricaScena();
                break;

            case ("uR"):
                int utentePronto = sfsObjIn.GetInt("uReady");
                if (utentePronto == Statici.sfs.MySelf.Id)
                    Statici.playerLocaleGO.GetComponentInChildren<TextMesh>().color = Color.green;
                else
                    Statici.playerRemotiGestore[utentePronto].textmesh.color = Color.green;

                break;

            case ("lPrt"):
                int[] utentiPronti = sfsObjIn.GetIntArray("listUP");
                listaPronti = new List<int>(utentiPronti);

                break;

            default:
                break;
        }
    }

    private void OnPublicMessage(BaseEvent evt)
    {
        User sender = (User)evt.Params["sender"];
        SFSObject objIn = (SFSObject)evt.Params["data"];
        string msg = (string)evt.Params["message"];
        string mittente = (sender.IsItMe) ? "Tu" : objIn.GetUtfString("nome");
        string coloreSender = (mittente == "Tu") ? "<color=#FF3333>" : "<color=#6600FF>";
        contenutoChat.text += coloreSender + "<b>" + mittente + " : " + "</b>" + "</color>" + " <color=#0000FF>" + msg + "</color>" + "\n";
        canvasGroupChat.interactable = true;
    }

    private void OnRoomAdded(BaseEvent evt)
    {
        PopolaListaPartite();
    }

    private void OnRoomCreationError(BaseEvent evt)
    {
        BloccaSbloccaCanvas(true);
        erroreText.text = "impossibile creare la stanza perchè: " + (string)evt.Params["errorMessage"];
    }

    /// <summary>
    /// Appena il player locale entra in una stanza di gioco,
    /// invia al server il nome modello e il nome scelto e la scena in cui sta(InviaDatiPlayerLocale)
    /// Il -1 indica che i dati devono essere inviati a tutti gli utenti della stanza.
    /// Se invece non si mette -1 ma un altro numero maggiore di zero, stiamo indicando un utente specifico
    /// a cui bisogna inviare le info.
    /// Se invece il giocatore locale entra nella lobby e non in una stanza di gioco,
    /// Se è la prima volta che entra resetterà solo alcune variabili, invece se è
    /// entrato nella lobby dopo essere stato dentro una stanza di gioco, dovrà prima
    /// distruggere il suo player locale e se nella stanza di gioco c'erano altri player
    /// dovrà distruggere gli avatar e pulire la lista degli avatar, visto che non è
    /// più in quella stanza e quindi non deve vederli.
    /// </summary>
    /// <param name="evt"></param>
    private void OnRoomJoin(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
        if (room.IsGame)

            InvioDatiPlayerLocale(-1);
        else
        {
            if (Statici.playerLocaleGO != null)
            {
                Statici.playerLocaleGO.transform.position = startPoint.position;
                Statici.playerLocaleGO.GetComponentInChildren<TextMesh>().color = Color.white;
                Statici.numeroPostoSpawn = -1;
                Statici.datiPersonaggioLocale = null;
            }
            if (Statici.playerRemotiGestore.Count != 0)
            {
                foreach (KeyValuePair<int, InfoPlayer> playerRemoto in Statici.playerRemotiGestore)
                {
                    Destroy(playerRemoto.Value.gameObject);
                }

                Statici.playerRemotiGestore.Clear();
            }
        }
    }

    private void OnRoomJoinError(BaseEvent evt)
    {
        BloccaSbloccaCanvas(true);
        string ragioneErrore = (string)evt.Params["errorMessage"];
        erroreText.text = "Impossibile entrare nella stanza per questo motivo :" + ragioneErrore;
    }

    private void OnUserCountChange(BaseEvent evento)
    {
        PopolaListaPartite();
    }

    /// <summary>
    /// se un utente remoto entra nella stanza di gioco in cui mi trovo,
    /// Invio i miei dati solo a quello che è entrato in modo tale che mi possa vedere.
    /// </summary>
    /// <param name="evt"></param>
    private void OnUserEnterRoom(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
        User user = (User)evt.Params["user"];
        if (room.IsGame && user != Statici.sfs.MySelf)
            InvioDatiPlayerLocale(user.Id);
    }

    /// <summary>
    /// Se un giocatore remoto esce dalla partita,
    /// distruggo il suo avatar e lo elimino dalla lista di avatar della stanza di gioco attuale.
    /// </summary>
    /// <param name="evt"></param>
    private void OnUserExitRoom(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];
        Room room = (Room)evt.Params["room"];
        if (!room.IsGame || user == Statici.sfs.MySelf)
            return;
        if (Statici.playerRemotiGestore.ContainsKey(user.Id))
        {
            Destroy(Statici.playerRemotiGestore[user.Id].gameObject);
            Statici.playerRemotiGestore.Remove(user.Id);
            if (listaPronti.Contains(user.Id))
                listaPronti.Remove(user.Id);
        }
    }

    private void PopolaListaPartite()
    {//sostituire i dizionari fare un getcomponentin children nel contenitore delle partite così mi recupero l'id dal gamelistitem
        List<Room> rooms = Statici.sfs.RoomManager.GetRoomList();

        for (int i = 0; i < rooms.Count; i++)
        {
            GameObject nuovoGOPartita = null;
            int roomId = rooms[i].Id;

            if (!rooms[i].IsGame || rooms[i].IsHidden || rooms[i].IsPasswordProtected)
                continue;
            else if (dizionarioStanzeDiGioco.ContainsKey(rooms[i].Id) && rooms[i].UserCount < rooms[i].MaxUsers)

                nuovoGOPartita = dizionarioStanzeDiGioco[roomId];
            else if (rooms[i].UserCount < rooms[i].MaxUsers && !rooms[i].GetVariable("gameStarted").GetBoolValue())
            {
                nuovoGOPartita = Instantiate(bottonePartita) as GameObject;
                dizionarioStanzeDiGioco.Add(roomId, nuovoGOPartita);
            }
            else if (dizionarioStanzeDiGioco.ContainsKey(rooms[i].Id) && rooms[i].UserCount == rooms[i].MaxUsers)
            {
                Destroy(dizionarioStanzeDiGioco[rooms[i].Id]);
                dizionarioStanzeDiGioco.Remove(rooms[i].Id);
            }
            if (nuovoGOPartita != null)
            {
                GameListItem roomItem = nuovoGOPartita.GetComponent<GameListItem>();
                roomItem.nomeStanza.text = rooms[i].Name + rooms[i].UserCount + "/" + rooms[i].MaxUsers;
                roomItem.roomId = roomId;
                roomItem.button.onClick.AddListener(() => ClickBottonePartita(roomId));
                nuovoGOPartita.transform.SetParent(ContenutoListaPartite, false);
            }
        }
    }

    private void ResettaEventListner()
    {
        Statici.sfs.RemoveAllEventListeners();
    }

    private void SpawnRemotePlayer(int user, string modello, string nomeP, int numeroSpawn, Vector3 posizione, Quaternion rotazione)
    {
        GameObject remotePlayer = Instantiate(Resources.Load(modello), posizione, rotazione) as GameObject;

        Statici.playerRemotiGestore.Add(user, remotePlayer);

        if (listaPronti.Contains(user))
            Statici.playerRemotiGestore[user].textmesh.color = Color.green;
        Statici.playerRemotiGestore[user].textmesh.text = nomeP;

        Statici.playerRemotiGestore[user].datiPersonaggio.Utente = user;
        Statici.playerRemotiGestore[user].datiPersonaggio.IndicePunteggio = numeroSpawn;
    }

    // Use this for initialization
    private void Start()
    {
        Statici.inGioco = false;
        canvasGroupChat.interactable = true;
        canvasGroup.interactable = true;
        Statici.datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);
        Statici.finePartita = false;
        sliderUserMax.minValue = 2;
        sliderUserMax.maxValue = numeroMassimoUtentiInStanza;
        contenutoPartiteCanvasGroup = ContenutoListaPartite.GetComponent<CanvasGroup>();
        BloccaSbloccaCanvas(true);
        buttonReady.gameObject.SetActive(false);
        ResettaErrore();
        if (!SmartFoxConnection.NonNulla)
        {
            StartCoroutine(GestoreCanvasAltreScene.ScenaInCarica("ScenaZero", "You're not connected to the server.", immagineCaricamento, scrittaCaricamento));
            return;
        }
//        sfs = SmartFoxConnection.Connection;
        Statici.sfs.ThreadSafeMode = true;
        Statici.sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        Statici.sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        Statici.sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        Statici.sfs.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdded);
        Statici.sfs.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError);
        Statici.sfs.AddEventListener(SFSEvent.ROOM_REMOVE, OnRoomRemoved);
        Statici.sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        Statici.sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        Statici.sfs.AddEventListener(SFSEvent.USER_COUNT_CHANGE, OnUserCountChange);
        Statici.sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
        Statici.sfs.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);

        Statici.playerLocaleGO = Instantiate(Resources.Load(Statici.datiPersonaggio.Dati.nomeModello), startPoint.position, Quaternion.identity) as GameObject;
        NetworkPlayer netPlayer = Statici.playerLocaleGO.AddComponent<NetworkPlayer>();
        netPlayer.playerLocale = true;
        Statici.playerLocaleGO.GetComponent<ControllerMaga>().Net = netPlayer;

        Statici.playerLocaleGO.GetComponentInChildren<TextMesh>().text = Statici.nomePersonaggio;
        PopolaListaPartite();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Statici.sfs != null)
            Statici.sfs.ProcessEvents();

        //Se l'owner è uscito e la partita non è finita avviso che la stanza è stata rimossa
        if (Statici.ownerUscito && !Statici.finePartita)
        {
            messaggio.text = Statici.messaggio;
            messaggioSuSchermoOwnerOn = true;
            Statici.messaggio = string.Empty;
            Statici.ownerUscito = false;
        }
        if (messaggioSuSchermoOwnerOn)
        {
            tempo += Time.deltaTime;
            if (tempo > 5f)
            {
                messaggio.text = "";
                tempo = 0f;
                messaggioSuSchermoOwnerOn = false;
            }
        }
    }
}