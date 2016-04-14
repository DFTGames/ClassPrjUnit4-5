using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using System;
using Sfs2X.Entities;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SFSLobby : MonoBehaviour
{

    public Text chatTxt;
    public InputField msgField;
    private SmartFox sfs;

    private string ZoneName = "BasicExamples";
    private string RoomName = "The Lobby";

    void Start()
    {
        Application.runInBackground = true;

        if (SFSConnection.eStatoInizializzato)
            sfs = SFSConnection.Connection;
        else
        {
            SceneManager.LoadScene("Login");
            return;
        }

        sfs.ThreadSafeMode = true;

        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionsLost);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnter);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExit);
        sfs.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPubblicMessage);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);

        sfs.Send(new JoinRoomRequest(RoomName));
    }
    void Reset()
    {
        sfs.RemoveAllEventListeners();
    }
    public void inviaMessaggio()
    {
        if (msgField.text != "")
            sfs.Send(new PublicMessageRequest(msgField.text));
        msgField.text = "";
    }

    public void exitBtn()
    {
        sfs.Disconnect();
    }

    private void messaggiSistema(string messaggio)
    {
        chatTxt.text += "<color=#808080ff>" + messaggio + "</color>\n";
    }

    private void messaggioPubblico(User user, string messaggio)
    {
        chatTxt.text += "<b>" + (user == sfs.MySelf ? "You :" : user.Name + ":") + "</b> " + messaggio + "\n";
    }

    private void OnPubblicMessage(BaseEvent evt)
    {
        User sender = (User)evt.Params["sender"];
        string messaggio = (string)evt.Params["message"];

        messaggioPubblico(sender, messaggio);

    }
    private void OnConnectionsLost(BaseEvent evt)
    {
        Reset();

        SceneManager.LoadScene("Login");
    }

    private void OnRoomJoin(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
        Debug.Log("Entrato nella stanza :" + evt.Params["room"]);

        ISFSObject isfsoOut = new SFSObject();  //manda al java

        isfsoOut.PutInt("NumA", 5);
        isfsoOut.PutInt("NumB", 10); sfs.Send(new ExtensionRequest("SommaNumeri", isfsoOut));
        sfs.Send(new ExtensionRequest("Prod", isfsoOut));

        messaggiSistema("\nSei entrato nella stanza : " + room.Name);

    }

    private void OnRoomJoinError(BaseEvent evt)
    {
        Debug.Log("ErrorCode (" + evt.Params["ErrorCode"] + ")" + "Msg :" + evt.Params["errorMessage"]);
    }

    private void OnUserEnter(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];
        Room room = (Room)evt.Params["room"];
        messaggiSistema(user.Name + "è entrato nella stanza " + room.Name);
    }

    private void OnUserExit(BaseEvent evt)
    {
        User user = (User)evt.Params["user"];
        Room room = (Room)evt.Params["room"];
        messaggiSistema(user.Name + "è uscito dalla stanza " + room.Name);
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        string cmd = (string)evt.Params["cmd"];
        ISFSObject isfso = (SFSObject)evt.Params["params"];
        if (cmd == "SommaNumeri")
        {
            Debug.Log(cmd + ": " + isfso.GetInt("NumC"));
        }
        else if (cmd == "Prod")
        {
            Debug.Log(cmd + "ninfea: il prodotto e' : " + isfso.GetInt("NumD"));
        }
    }

    void OnApplicationQuit()
    {
        if (sfs.IsConnected)
            sfs.Disconnect();
        Debug.Log(sfs.IsConnected);
    }
    void Update()
    {
        if(sfs != null)
        sfs.ProcessEvents();
    }

}
