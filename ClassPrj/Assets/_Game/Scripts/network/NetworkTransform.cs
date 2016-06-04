﻿using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using System;

public class NetworkTransform 
{ 
    public Vector3 position;
    public float rotation = 0f;
    public float forward = 0f;
    public byte attacchi=0; // byte contiene 3 informazioni : attacco1,attacco2 e salto
    public float jump=0f;
    public float jumpLeg=0f;
    public float turn = 0f;

    public double timeStamp=0;

    public static NetworkTransform CreaOggettoNetworktransform(Vector3 posizione, float rotazione,float forw, byte attacchi,double time,float jump,float jumpLeg,float turn)
    {
        NetworkTransform trans = new NetworkTransform();  

        trans.position = posizione;
        trans.rotation = rotazione;
        trans.forward = forw;
        trans.attacchi = attacchi;
        trans.timeStamp = time;
        trans.jump = jump;
        trans.jumpLeg = jumpLeg;
        trans.turn = turn;
        
        return trans;
    }
}
