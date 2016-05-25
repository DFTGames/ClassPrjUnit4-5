using Sfs2X;
using Sfs2X.Core;
using UnityEngine;

public class ScenaInizialeNetwork : MonoBehaviour
{
    private static ScenaInizialeNetwork me;

    private SmartFox sfs;

    public static void VaiAlleStanze()
    {
        me.sfs.RemoveAllEventListeners();        
        ManagerIniziale.CaricaScena("ScenaStanze", "The Lobby");
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        sfs.RemoveAllEventListeners();        
        ManagerIniziale.CaricaScena("ScenaZero", "Connection lost");
    }

    // Use this for initialization
    private void Start()
    {
        me = this;
        if (!Statici.multigiocatoreOn)
            return;

        Application.runInBackground = true;
        if (!SmartFoxConnection.NonNulla)
        {
            ManagerIniziale.CaricaScena("ScenaZero", "You're not connected to the server.");
            return;
        }

        sfs = SmartFoxConnection.Connection;
        sfs.ThreadSafeMode = true;
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
    }

    // Update is called once per frame
    private void Update()
    {
        if (sfs != null)
            sfs.ProcessEvents();
    }
}