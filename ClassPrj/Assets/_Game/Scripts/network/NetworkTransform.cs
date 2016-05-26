using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using System;

public class NetworkTransform : MonoBehaviour {

    public   Vector3 position;
    public   float  rotation;
    public float forward;
    public bool attacco1;
    public bool attacco2;

    public   double timeStamp=0;

    public static NetworkTransform CreaOggettoNetworktransform(Vector3 posizione, float rotazione,float forw, bool att1,bool att2)
    {
        NetworkTransform trans = new NetworkTransform();
     
        trans.position = posizione;
        trans.rotation = rotazione;
        trans.forward = forw;
        trans.attacco1 = att1;
        trans.attacco2 = att2;
        trans.timeStamp = Time.time;
        
        return trans;
    }
}
