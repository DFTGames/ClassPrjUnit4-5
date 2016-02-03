using UnityEngine;
using System.Collections;
using System;

public class Inseguimento : IStato
{
    public string Nome { get; set; }

    private FSM Cervello;
    private NavMeshAgent agente;
    private Animator animatore;
    private float distanzaTRaGiocatoreGoblin;

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

       
       
          
        if (Cervello.ObiettivoNemico != null && !Cervello.inZonaAttacco)
        {
            agente.destination = Cervello.ObiettivoNemico.position; //Se serve aggiugnere .Position
            agente.stoppingDistance = 2f;
            agente.speed = 3.5f;
            animatore.SetFloat("Velocita", 1f);
            distanzaTRaGiocatoreGoblin = Vector3.Distance(agente.transform.position, Cervello.ObiettivoNemico.position);
            Debug.Log("distanzaAttacco" + distanzaTRaGiocatoreGoblin);
            if (distanzaTRaGiocatoreGoblin <= 2f)
            {
                Cervello.inZonaAttacco = true;
            }
            else  Cervello.inZonaAttacco = false;
           

            
        }
        
    }

    public void EsecuzioneTerminata()
    {

    }
}

