using UnityEngine;
using System.Collections;
using System;

public class Attacco : IStato {

    public string Nome {get; set;}
    
    private FSM MioCervello;
    private Transform mTrasform;
    private float distanzaTRaGiocatoreGoblin;


    public void Inizializza(FSM oggetto)
    {
        mTrasform = MioCervello.gameObject.GetComponent<Transform>();
        
    }
    
    public void PreparoEsecuzione()
    {
        
    }

    public void Esecuzione()
    {
        if (MioCervello.ObiettivoNemico != null && MioCervello.inZonaAttacco)
        {          
           MioCervello.Animatore.SetFloat("Velocita", 0f);
             MioCervello.Animatore.SetBool("Pugno", true);         
            distanzaTRaGiocatoreGoblin = distanzaTRaGiocatoreGoblin = Vector3.Distance(MioCervello.Agente.transform.position, MioCervello.ObiettivoNemico.position);           
            if (distanzaTRaGiocatoreGoblin <= MioCervello.distanzaAttaccoGoblinSpada)
            {
                MioCervello.inZonaAttacco = true;
            }
            else MioCervello.inZonaAttacco = false;
        }
    }

    public void EsecuzioneTerminata()
    {
        MioCervello.Animatore.SetBool("Pugno", false);        
    }  

}
