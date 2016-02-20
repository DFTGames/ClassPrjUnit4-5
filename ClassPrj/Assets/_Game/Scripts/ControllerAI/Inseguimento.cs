using UnityEngine;
using System.Collections;
using System;

public class Inseguimento : IStato
{
    public string Nome { get; set; }

    private FSM Cervello;


    public void Inizializza(FSM oggetto)
    {
        Cervello = oggetto;
    }

    public void PreparoEsecuzione()
    {
        Cervello.Agente.speed = 3.5f;     
        Cervello.Agente.stoppingDistance = Cervello.distanzaAttacco;
       


    }

    public void Esecuzione()
    {

        Cervello.Agente.SetDestination(Cervello.ObiettivoNemico.position);
        Cervello.Animatore.SetFloat("Velocita", Cervello.Agente.velocity.normalized.magnitude);
       
    }

    public void EsecuzioneTerminata()
    {

    }
}