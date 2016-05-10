using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Util;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ControllerLogin : MonoBehaviour {

    public string host = "127.0.0.1";
    public int port = 9933;
    public string zona = "BasicExamples";
    public Text erroreText;
    public InputField casellaNome;
   

    private SmartFox sfs;
   


    // Use this for initialization
    void Start () {
        erroreText.text = string.Empty;
        
	}
	
	// Update is called once per frame
	void Update () {
        if (sfs != null)
            sfs.ProcessEvents();
	}

    public void BottoneLogin()
    {
        Application.runInBackground = true;
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

    private void OnRoomJoinError(BaseEvent evt)
    {
        ManagerScenaZero.AttivaDisattivaCanvasGroupLogin(true);
        erroreText.text = "Impossibile unirsi alla Lobby :" + (string)evt.Params["errorMessage"];
    }

    private void OnRoomJoin(BaseEvent evt)
    {
        
        ResettaListnerAbilitaUI(false);
        SceneManager.LoadScene("Scena Iniziale");
      
    }

   
    private void OnLoginError(BaseEvent evt)
    {
        sfs.Disconnect();
        ResettaListnerAbilitaUI(true);
        erroreText.text = "Login fallito :" + (string)evt.Params["errorMessage"];
    }

    private void OnLogin(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];
        Statici.userLocaleId = user.Id;
        sfs.Send(new JoinRoomRequest("The Lobby"));
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        string ragione = (string)evt.Params["reason"];
        if (ragione != ClientDisconnectionReason.MANUAL)
            erroreText.text = "Connessione persa :" + ragione;
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

    private void ResettaListnerAbilitaUI(bool abilita)
    {
        sfs.RemoveAllEventListeners();
        ManagerScenaZero.AttivaDisattivaCanvasGroupLogin(abilita);
        erroreText.text = "";
    }
}
