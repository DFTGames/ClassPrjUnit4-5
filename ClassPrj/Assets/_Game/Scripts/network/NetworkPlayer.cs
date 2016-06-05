﻿using UnityEngine;
using System.Collections;

public enum azioniPlayer : byte
{
    attacco1=1,
    attacco2=2,
    salto=4
}

public class NetworkPlayer : MonoBehaviour
{
    
    private int user = -2; //inizializzato alla cazzum...(e' fuori dal intervallo che verra usato)
    public bool playerLocale { get; set; }
    // private float tempoInvioTransorm = 0.1f;
    private float timeCorrente;
    private float timeAfterStop;
    private bool movimentoDirty;
    private float rotazione;
    //private float Protazione;
    private Animator anim;
    private ControllerMaga controller;
    private NetworkTransformInterpolation inter;

    private byte attacchi;

    public int User
    {
        set
        {
            user = value;
        }
    }

    public NetworkTransformInterpolation Inter
    {
        set
        {
            inter = value;
        }
    }

    void Start()
    {

        controller = GetComponent<ControllerMaga>();
        if (controller != null) anim = controller.GetComponent<Animator>();
        timeCorrente = Time.time;
    }



    void Update()
    { //QUA DENTRO C'E LA LOGICA PER I PLAYER LOCALI

          if (!playerLocale || (playerLocale && !Statici.partenza)) return; //Statici.partenza controllar

        // DA SISTEMARE
     //   if (Statici.IsPointAndClick) rotazione = transform.localEulerAngles.y;                     // rotazione = transform.rotation.eulerAngles.y;  //unico comportamento diverso se siamo punta & clicca o tastiera
     //   else rotazione = transform.localEulerAngles.y;//controller.Rotazione;



        attacchi = 0;
        if (controller.Attacco1) attacchi = (byte)azioniPlayer.attacco1;
        if (controller.Attacco2) attacchi =(byte)azioniPlayer.attacco2;
        if (!controller.ATerra) attacchi = (byte)azioniPlayer.salto;

        if (attacchi > 0) movimentoDirty = true;


        if (Statici.multigiocatoreOn && Statici.inGioco)
        {
            // Debug.Log("****rotazione "+ rotazione);
            // if (controller.Forward > 0)   come era prima
              if (controller.Forward != 0 || controller.RotBool || !controller.ATerra)              
            {
              //  Debug.Log("Sto saltando " + controller.Jump);
              //  Debug.Log("jumleg " + controller.JumpLeg);
             //   Debug.Log("controller.Forward  " + controller.Forward);
             //   Debug.Log("controller.RotBool  " + controller.RotBool);
                movimentoDirty = true;
                timeAfterStop = Time.time;
                rotazione = transform.localEulerAngles.y; //memorizzo il ultimo movimento effettuato ...
             //   Debug.Log(" ruotando " + rotazione);
            }
        }

        if (!movimentoDirty) return;

        if ((timeCorrente + Statici.tempoInvioTransform) < Time.time)
        {
          //  Debug.Log("sto inviando ");
            NetworkTransform net = NetworkTransform.CreaOggettoNetworktransform(transform.position, transform.localEulerAngles.y, controller.Forward, attacchi, 0,controller.Jump,controller.JumpLeg,controller.Rotazione,false);  //nel invio messo time a zero in quanto non lo uso (esempuio del shooter che lo manda sul server ma non viene usato)(idem per il ispointClick in invio non lo uso)

            ManagerNetwork.InviaTransformAnimazioniLocali(net);           
            attacchi = 0;  //azzero l'attacco  (potevo usare operatore binario per azzerarlo : attacchi^= attacchi (Xor);
            timeCorrente = Time.time;
            if ((timeAfterStop + 0.5f) < Time.time) movimentoDirty = false;  //mi aspetta tot tempo per fare in modo che mi prenda con sicurezza anche l'arresto..considerando che siamo in UDP..e non tutti i pacchetti arrivano(vedere lezione 11 modulo 7)
        }


    }

    //QUESTO METODO E' PER I PLAYER REMOTI
    public void ricevitransform(NetworkTransform net, int netUser)
    {
        if (playerLocale || user != netUser || net==null) return;

        inter.ReceivedTransform(net);

        //lo lascio qua invece di metterlo nel NetworkTransformInterpolation perche deve essere eseguito non nel update(vedi originale)

        if ((net.attacchi & (byte)azioniPlayer.attacco1) == (byte)azioniPlayer.attacco1) anim.SetTrigger("attacco1");    //ho usato operatori binari 

        else if ((net.attacchi & (byte)azioniPlayer.attacco2) == (byte)azioniPlayer.attacco2) anim.SetTrigger("attacco2");  //ho usato operatori binari 
       

    }

}
