using UnityEngine;
using System.Collections;
using System;

public class Attacco : IStato {

    public string Nome {get; set;}

    private FSM MioCervello;
    private NavMeshAgent Agente;
    private Animator Animatore;
    private Transform mTrasform;
    private RaycastHit hit;


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
        if (MioCervello.ObiettivoNemico != null)
        {
            Vector3 direzione = mTrasform.TransformDirection(Vector3.forward);
           if (Physics.Raycast(mTrasform.position, direzione, 1f))
            {
              Agente.Stop();
             Animatore.SetBool("Pugno", true);
             
            }

           
        }
    }

    public void EsecuzioneTerminata()
    { }

    

   

}
