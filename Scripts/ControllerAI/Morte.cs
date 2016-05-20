using UnityEngine;

public class Morte : IStato
{
    public string Nome { get; set; }    
    private FSM Cervello;

    public void Inizializza(FSM oggetto)
    {
        Cervello = oggetto;        
    }

    public void PreparoEsecuzione()
    {
        Cervello.Ucciso = true;
        Cervello.SwitchVivoMorto.AttivaRagdoll();       
    }

    public void Esecuzione()
    {
      
    }

    public void EsecuzioneTerminata()
    {
        
    }
}