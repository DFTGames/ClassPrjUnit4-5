using UnityEngine;
using System.Collections;

//interfaccia degli Stati:
public interface IStato
{
    string Nome
    {
        get; set;
    }

    void Inizializza(FSM oggetto);

    void PreparoEsecuzione();

    void Esecuzione();

    void EsecuzioneTerminata();

}