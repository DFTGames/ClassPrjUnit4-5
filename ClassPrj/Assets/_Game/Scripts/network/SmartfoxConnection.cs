using Sfs2X;
using UnityEngine;

/**
 * Singleton class with static fields to hold a reference to SmartFoxServer connection.
 * It is useful to access the SmartFox class from anywhere in the game.
 */

public class SmartFoxConnection : MonoBehaviour
{
    private static SmartFoxConnection me;
    private static SmartFox sfs;

    public static SmartFox Connection
    {
        get
        {
            if (me == null)
            {
                me = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
            }
            return sfs;
        }
        set
        {
            if (me == null)
            {
                me = new GameObject("SmartFoxConnection").AddComponent(typeof(SmartFoxConnection)) as SmartFoxConnection;
            }
            sfs = value;
        }
    }

    public static bool NonNulla
    {
        get
        {
            return (sfs != null);
        }
    }

    // Handle disconnection automagically
    // ** Important for Windows users - can cause crashes otherwise
    private void OnApplicationQuit()
    {
        //mi disconnetto da smartfox
        if (sfs.IsConnected)
        {
            sfs.Disconnect();
        }

        //chiudo la connessione con il DB locale
        if (Statici.conn.State == System.Data.ConnectionState.Open)
            Statici.conn.Close();
    }
}