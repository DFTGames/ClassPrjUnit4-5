using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class InfoPlayer:IDisposable
{
    public GameObject gameObject;
    public NetworkPlayer networkPlayer;
    public DatiPersonaggio datiPersonaggio;
    public SwitchVivoMorto switchVivoMorto;
    public TextMesh textmesh;

    public void Dispose()
    {
        gameObject = null;
        networkPlayer = null;
        datiPersonaggio = null;
        switchVivoMorto = null;
        textmesh = null;

    }
}


public class GestoreInfoClassi : Dictionary<int,InfoPlayer>,IDisposable {
 
    public bool Add(int user,GameObject gameO,NetworkPlayer networkP,DatiPersonaggio datiP,SwitchVivoMorto vivoMorto,TextMesh text)
    {
        if (this.ContainsKey(user)) return false;

        InfoPlayer info = new InfoPlayer();
        info.gameObject = gameO;
        info.networkPlayer = networkP;
        info.datiPersonaggio = datiP;
        info.switchVivoMorto = vivoMorto;
        info.textmesh = text;

        this.Add(user, info);
        return true;
    }


    public void Dispose()
    {
       // foreach(var elem in dizionario)
       foreach (KeyValuePair<int, InfoPlayer> gg in this)
        {
          (gg.Value as InfoPlayer).Dispose();
        }
        this.Clear();
    }
}
