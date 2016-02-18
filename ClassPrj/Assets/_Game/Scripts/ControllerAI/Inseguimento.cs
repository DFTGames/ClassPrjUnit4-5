using UnityEngine;
using System.Collections;
using System;

public class Inseguimento : IStato
{
    public string Nome { get; set; }

    private FSM Cervello;
    private float distanzaTRaGiocatoreGoblin;

    public void Inizializza(FSM oggetto)
    {
        Cervello = oggetto;
    }

    public void PreparoEsecuzione()
    {
        Cervello.Agente.speed = 3.5f;
    }

    public void Esecuzione()
    {
        if (!Cervello.inZonaAttacco)
        {
            Cervello.Agente.destination = Cervello.ObiettivoNemico.position; 
            Cervello.Animatore.SetFloat("Velocita", Cervello.Agente.velocity.normalized.magnitude);
            distanzaTRaGiocatoreGoblin = Vector3.Distance(Cervello.Agente.transform.position, Cervello.ObiettivoNemico.position);
            if (distanzaTRaGiocatoreGoblin <= Cervello.distanzaAttaccoGoblinSpada)
            {
                Cervello.inZonaAttacco = true;
            }
            else Cervello.inZonaAttacco = false;
        }
    }

    public void EsecuzioneTerminata()
    {

    }
}