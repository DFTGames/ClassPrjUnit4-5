using UnityEngine;
using System.Collections;
using System;

public class Inseguimento : IStato
{
    public string Nome { get; set; }

    private FSM Cervello;
    private NavMeshAgent agente;
    private GameObject PersonaggioDaInseguire;
    private Animator animatore;

    public void Inizializza(FSM oggetto)
    {
        Cervello = oggetto;
        agente = Cervello.GetComponent<NavMeshAgent>();
        PersonaggioDaInseguire = GameObject.FindGameObjectWithTag("Player");
        animatore = Cervello.gameObject.GetComponent<Animator>();
        agente.speed = 1f;
    }

    public void PreparoEsecuzione()
    {

    }

    public void Esecuzione()
    {
        agente.destination = PersonaggioDaInseguire.transform.position;
        animatore.SetFloat("Velocita", agente.speed);
    }

    public void EsecuzioneTerminata()
    {

    }
}
