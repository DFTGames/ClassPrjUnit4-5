using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class InfoPlayer : IDisposable
{
    public GameObject gameObject;
    public NetworkPlayer networkPlayer;
    public ControllerMaga controllerPlayer;
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
public class GestoreInfoClassi : Dictionary<int, InfoPlayer>, IDisposable
{
    public bool Add(int user, GameObject gameObj)
    {
        if (this.ContainsKey(user)) return false;
        InfoPlayer info = new InfoPlayer();
        info.gameObject = gameObj;     
        info.networkPlayer = AddNetworkPlayer(info);
       // info.controllerPlayer = info.gameObject.GetComponent<ControllerMaga>();
        info.datiPersonaggio = info.gameObject.GetComponent<DatiPersonaggio>(); ;
        info.switchVivoMorto = info.gameObject.GetComponent<SwitchVivoMorto>();
        info.textmesh =  info.gameObject.GetComponentInChildren<TextMesh>();


        this.Add(user, info);
        return true;
    }

    private NetworkPlayer AddNetworkPlayer(InfoPlayer info)
    {
        NetworkPlayer nt = info.gameObject.GetComponent<NetworkPlayer>();
        if (nt == null)       
            nt = info.gameObject.AddComponent<NetworkPlayer>();

        NetworkTransformInterpolation tmpInt = info.gameObject.GetComponent<NetworkTransformInterpolation>();
        if (tmpInt== null)
            tmpInt= info.gameObject.AddComponent<NetworkTransformInterpolation>();
        tmpInt.Contr = info.gameObject.GetComponent<ControllerMaga>();
        tmpInt.Anim = info.gameObject.GetComponent<Animator>();
        nt.Inter = tmpInt;
        tmpInt.StartReceiving();
        return nt;
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