using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Requests;
using Sfs2X.Util;
using UnityEngine.SceneManagement;
using System;

public class ScenaInizialeNetwork : MonoBehaviour {

    private static ScenaInizialeNetwork me; 

    private SmartFox sfs;

    // Use this for initialization
    void Start () {
        me = this;
        if (!Statici.multigiocatoreOn)
            return;

        Application.runInBackground = true;
        if (!SmartFoxConnection.NonNulla)
        {
            SceneManager.LoadScene("ScenaZero");
            return;
        }

        sfs = SmartFoxConnection.Connection;
        sfs.ThreadSafeMode = true;
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
     
    }

    public static void VaiAlleStanze()
    {
        me.sfs.RemoveAllEventListeners();
        SceneManager.LoadScene("ScenaStanze");
        
    }
   

    private void OnConnectionLost(BaseEvent evt)
    {
        sfs.RemoveAllEventListeners();
        SceneManager.LoadScene("ScenaZero");
    }

    // Update is called once per frame
    void Update () {
        if (sfs != null)
            sfs.ProcessEvents();
    }
}
