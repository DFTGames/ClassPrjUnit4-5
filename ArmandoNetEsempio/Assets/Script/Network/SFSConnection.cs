using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using System;
using Sfs2X.Entities;
using UnityEngine.UI;

public class SFSConnection : MonoBehaviour
{
    private static SFSConnection me;
    private static SmartFox sfs;

    public static SmartFox Connection
    {
        get
        {
            if (me == null)
                me = new GameObject("SFSConnection").AddComponent(typeof(SFSConnection)) as SFSConnection;
            return sfs;
        }
        set
        {
            if (me == null)
                me = new GameObject("SFSConnection").AddComponent(typeof(SFSConnection)) as SFSConnection;
            sfs = value;
        }
    }

    public static bool eStatoInizializzato
    {
        get
        {
            return (sfs != null);
        }
    }

    void OnApplicationQuit()
    {
        if (sfs.IsConnected)
            sfs.Disconnect();
    }
}
