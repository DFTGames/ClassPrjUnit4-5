using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using UnityEngine.SceneManagement;
using System;

public class SFSLogin : MonoBehaviour
{

    public string ServerHost = "127.0.0.1";
    public int ServerPort = 9933;
    public string UserName = "";
    public InputField nameLogin;
    public Text erroreTxt;

    private string ZoneName = "BasicExamples";
    private string RoomName = "The Lobby";
    private SmartFox sfs;

    // Use this for initialization
    void Start()
    {

        Application.runInBackground = true;
    }

    public void loginBtn()
    {
        if (nameLogin.text != null)
            UserName = nameLogin.text;

        sfs = new SmartFox();
        sfs.ThreadSafeMode = true;

        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnections);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionsLost);
        sfs.AddEventListener(SFSEvent.LOGIN, OnLoggin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLogginError);

        sfs.Connect(ServerHost, ServerPort);

        nameLogin.text = "";
    }

    private void OnConnectionsLost(BaseEvent evt)
    {
        erroreTxt.text = "Connection was lost, Reason: " + (string)evt.Params["reason"];
    }

    void Reset()
    {
        sfs.RemoveAllEventListeners();
    }

    void OnConnections(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            Debug.Log("Connessione avvenuta!");
            SFSConnection.Connection = sfs;
            sfs.Send(new LoginRequest(UserName, "", ZoneName));
        }
        else
        {
            Debug.Log("Connessione Fallita");
            Reset();
        }
    }

    private void OnLoggin(BaseEvent evt)
    {
        Reset();

        Debug.Log("Log In : " + evt.Params["user"]);

        SceneManager.LoadScene("LobbyChat");
    }

    private void OnLogginError(BaseEvent evt)
    {
        Reset();

        Debug.Log("Login Error msg:" + evt.Params["errorMessage"] + "Error Code: "
        + evt.Params["errorCode"]);

        erroreTxt.text = "Login Error msg:" + evt.Params["errorMessage"] + "Error Code: "
        + evt.Params["errorCode"];
    }
    // Update is called once per frame
    void Update()
    {
        if(sfs != null)
        sfs.ProcessEvents();
    }
}
