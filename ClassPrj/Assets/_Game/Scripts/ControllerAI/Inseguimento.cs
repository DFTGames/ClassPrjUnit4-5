using UnityEngine;
using System.Collections;
using System;

public class Inseguimento : IStato
{
    public string Nome { get; set; }

    private FSM Cervello;
    private NavMeshAgent agente;
    private GameObject PersonaggioDaInseguire;

    public void Inizializza(FSM oggetto)
    {
        Cervello = oggetto;
        agente = Cervello.GetComponent<NavMeshAgent>();
        PersonaggioDaInseguire = GameObject.FindGameObjectWithTag("Player");
    }

    public void PreparoEsecuzione()
    {

    }

    public void Esecuzione()
    {
        agente.destination = PersonaggioDaInseguire.transform.position;
    }

    public void EsecuzioneTerminata()
    {

    }
}
