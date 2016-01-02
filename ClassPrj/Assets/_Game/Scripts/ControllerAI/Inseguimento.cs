using UnityEngine;
using System.Collections;
using System;

public class Inseguimento : IStato
{
    public string Nome { get; set; }

    private FSM Cervello;
    private NavMeshAgent agente;
    private Animator animatore;

    public void Inizializza(FSM oggetto)
    {
        
        Cervello = oggetto;  
        agente = Cervello.GetComponent<NavMeshAgent>();
        animatore = Cervello.gameObject.GetComponent<Animator>();
    }

    public void PreparoEsecuzione()
    {

    }

    public void Esecuzione()
    {

       
       
          
        if (Cervello.ObiettivoNemico != null)
        {
            agente.destination = Cervello.ObiettivoNemico.position; //Se serve aggiugnere .Position
            agente.speed = 3.5f;
            animatore.SetFloat("Velocita", 1f);
        }
    }

    public void EsecuzioneTerminata()
    {

    }
}

