using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using System;

public class NetworkTransform : MonoBehaviour {

    public   Vector3 position;
    public   Vector3 rotation;
    public   double timeStamp=0;
    
    public static NetworkTransform CreaOggettoNetworktransform(Vector3 posizione, Vector3 rotazione)
    {
        NetworkTransform trans = new NetworkTransform();
     
        trans.position = posizione;
        trans.rotation = rotazione;
        trans.timeStamp = Time.time;
       
        return trans;
    }
}
