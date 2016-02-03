using UnityEngine;
using System.Collections;
using System;

public class Attacco : IStato {

    public string Nome {get; set;}

    private FSM MioCervello;
    private NavMeshAgent Agente;
    private Animator Animatore;
    private Transform mTrasform;
   
    private float distanzaTRaGiocatoreGoblin;


    public void Inizializza(FSM oggetto)
    {
        MioCervello = oggetto;
        Agente = MioCervello.GetComponent<NavMeshAgent>();
        Animatore = MioCervello.gameObject.GetComponent<Animator>();
        mTrasform = MioCervello.gameObject.GetComponent<Transform>();

    }
    
    public void PreparoEsecuzione()
    {
        
    }

    public void Esecuzione()
    {
        if (MioCervello.ObiettivoNemico != null && MioCervello.inZonaAttacco)
        {


            //Agente.speed = 0f;
            Animatore.SetFloat("Velocita", 0f);
             Animatore.SetBool("Pugno", true);
          //  Agente.destination = MioCervello.ObiettivoNemico.position;

            distanzaTRaGiocatoreGoblin = distanzaTRaGiocatoreGoblin = Vector3.Distance(Agente.transform.position, MioCervello.ObiettivoNemico.position);
            Debug.Log("distanzaAttacco" + distanzaTRaGiocatoreGoblin);
            if (distanzaTRaGiocatoreGoblin <= 2f)
            {
                MioCervello.inZonaAttacco = true;
            }
            else MioCervello.inZonaAttacco = false;

        }
    }

    public void EsecuzioneTerminata()
    {
        Animatore.SetBool("Pugno", false);
        Agente.speed = 0.5f;
    }

    

   

}
