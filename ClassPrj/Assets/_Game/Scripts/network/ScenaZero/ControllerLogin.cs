using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
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
    public InputField casellaNome;
    public Text erroreText;
    public string localhost = "127.0.0.1";
    public int port = 9933;
    public string remoteHost = "40.68.126.217";
    public scegliHost tipoHost = scegliHost.remotehost;
    public string zona = "BasicExamples";
    private string host;
    private SmartFox sfs;

    public void BottoneLogin()
    {
        ManagerScenaZero.AttivaDisattivaCanvasGroupLogin(false);
        erroreText.text = "";

        ConfigData cfg = new ConfigData();
        cfg.Host = host;
        cfg.Port = port;
        cfg.Zone = zona;

        sfs = new SmartFox();

        sfs.ThreadSafeMode = true;
        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);

        sfs.Connect(cfg);
    }

    private void OnConnection(BaseEvent evt)
    {
        bool connessioneAvvenuta = (bool)evt.Params["success"];
        if (connessioneAvvenuta)
        {
            SmartFoxConnection.Connection = sfs;
            sfs.Send(new LoginRequest(casellaNome.text));
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

    private void OnLogin(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];
        Statici.userLocaleId = user.Id;
        sfs.Send(new JoinRoomRequest("The Lobby"));
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
        StartCoroutine(GestoreCanvasAltreScene.ScenaInCarica("Scena Iniziale", "Titolo gioco o qualsiasi altra cosa vogliamo scriverci.", ManagerScenaZero.ImmagineCaricamento, ManagerScenaZero.ScrittaCaricamento));
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
    }

    // Update is called once per frame
    private void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
    }
}