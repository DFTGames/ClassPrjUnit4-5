using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using System;

public class NetworkTransform : MonoBehaviour {


    public Vector3 position;
    public float rotation = 0f;
    public float forward = 0f;
    public byte attacchi=0;
    public float jump=0f;
    public float jumpLeg=0f;

    public double timeStamp=0;

    public static NetworkTransform CreaOggettoNetworktransform(Vector3 posizione, float rotazione,float forw, byte attacchi,double time,float jump,float jumpLeg)
    {
        NetworkTransform trans = new NetworkTransform();

        trans.position = posizione;
        trans.rotation = rotazione;
        trans.forward = forw;
        trans.attacchi = attacchi;
        trans.timeStamp = time;
        trans.jump = jump;
        trans.jumpLeg = jumpLeg;
        
        return trans;
    }
}
