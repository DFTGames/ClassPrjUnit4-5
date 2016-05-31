using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using System;

public class NetworkTransform : MonoBehaviour {

    public   Vector3 position;
    public   float  rotation;
    public float forward;
    public byte attacchi;

    public double timeStamp=0;

    public static NetworkTransform CreaOggettoNetworktransform(Vector3 posizione, float rotazione,float forw, byte attacchi,double time)
    {
        NetworkTransform trans = new NetworkTransform();
     
        trans.position = posizione;
        trans.rotation = rotazione;
        trans.forward = forw;
        trans.attacchi = attacchi;
        trans.timeStamp = time;
        
        return trans;
    }
}
