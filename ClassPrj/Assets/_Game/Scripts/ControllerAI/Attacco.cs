using UnityEngine;
using System.Collections;
using System;

public class Attacco : IStato {

    public string Nome {get; set;}
    
    private FSM MioCervello;



    public void Inizializza(FSM oggetto)
    {
        MioCervello = oggetto;
      
    }
    
    public void PreparoEsecuzione()
    {
        if (MioCervello.attaccoDaVicino)
        {
           
            MioCervello.Animatore.SetBool("Pugno", true);
         
            //lo stopdistance è impostato a 0.8 nel pattugliamento
            // invece nell'inseguimento e nell'attacco lo abbiamo messo uguale alla distanza di attacco. 
            //lo abbiamo messo in entrambi(inseguimento e attacco e non solo nell'inseguimento) perchè se
            //per caso passa da pattugliamento ad attacco senza passare dall'inseguimento
            //deve sapere che la stopping distance è cambiata comunque se no legge ancora 0.8(del pattugliamento e non è corretto).

        }
        else
        {
       
            MioCervello.Animatore.SetBool("PrendiArco", true);
            MioCervello.Animatore.SetBool("TiraFreccie", true);
           

        }
        MioCervello.Agente.stoppingDistance = MioCervello.distanzaAttacco;

    }

    public void Esecuzione()
    {
        MioCervello.Agente.SetDestination(MioCervello.ObiettivoNemico.position);
   
    }

    public void EsecuzioneTerminata()
    {
        if (MioCervello.attaccoDaVicino)        
            MioCervello.Animatore.SetBool("Pugno", false); 
         
        else
        {
            MioCervello.Animatore.SetBool("PrendiArco", false);
            MioCervello.Animatore.SetBool("TiraFreccie", false);
        }      
    }  

}
