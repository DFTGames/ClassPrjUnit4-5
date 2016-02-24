using UnityEngine;

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
      
        if (Cervello.ObiettivoInVista)
            Cervello.Agente.stoppingDistance = Cervello.DistanzaAttacco;
        else
            Cervello.Agente.stoppingDistance = 2*Cervello.Agente.radius + 0.15f;
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