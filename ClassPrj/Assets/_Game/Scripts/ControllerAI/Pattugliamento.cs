using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pattugliamento : IStato
{

    public string Nome { get; set; }

    private int indiceDestinazioni = 0;
    private FSM MioCervello;

    private GestorePercorso Percorso = null;

    private List<int> elencoPercorsiDisponibili = new List<int>();

    public void Esecuzione()
    {

        if (Percorso != null && Percorso.transform.childCount > 0)
        {

            if (MioCervello.Agente.remainingDistance <= MioCervello.Agente.stoppingDistance)
            {
                indiceDestinazioni = indiceDestinazioni < Percorso.transform.childCount ? ++indiceDestinazioni : 0;
                Transform tmpPercorso = Percorso[indiceDestinazioni]; //Se nessun Nodo e' stato impostato come percorso
                if (tmpPercorso == null)
                {
                    Debug.LogError("!! SEI Proprio UN PIRLA!! e pure cazzone..Non ci sono nodi per questo percorso");
                    return;

                }
                MioCervello.MiaTransform = tmpPercorso;
                MioCervello.Agente.SetDestination(MioCervello.MiaTransform.position);
            }
            MioCervello.Animatore.SetFloat("Velocita", 0.5f);
        }
        else
        {
            Debug.LogError("Testa di minchia! ti sei dimenticato Percorso o Il percorso non ha figli");
        }
    }

    public void EsecuzioneTerminata()
    {

    }

    public void Inizializza(FSM oggetto)
    {

        MioCervello = oggetto;

        if (!MioCervello.DatiPersonaggio.Giocabile)
            elencoPercorsiDisponibili = GameManager.databaseInizialePercorsi.trovaPercorsiDaPersonaggio(MioCervello.DatiPersonaggio.miaClasse);

        if (elencoPercorsiDisponibili.Count > 0)
            MioCervello.IndexPercorso = elencoPercorsiDisponibili[Random.Range(0, elencoPercorsiDisponibili.Count)];  //per ora gli assegno percorso casuale...da sistemare e completare

        foreach (int elem in elencoPercorsiDisponibili)
            Debug.Log("classe" + MioCervello.DatiPersonaggio.miaClasse + " idx " + elem);
        Debug.Log("percorso scelto " + MioCervello.IndexPercorso);

        /*
        if (GameManager.dizionarioPercorsi.ContainsKey(MioCervello.gameObject.tag))
            MioCervello.IndexPercorso = GameManager.dizionarioPercorsi[MioCervello.gameObject.tag];
            */
        //-Vedere se meglio con impostazione nostra di indiceDestionazioni oppure fare in modo che a ogni chiamata al Gestore PErcorso si incrementa da solo

        if (MioCervello.IndexPercorso < 0) //
        {
            Debug.LogError("Ue' picio.......Non hai stato assegnato nessun percorso..quindi se ne sta fermo in attesa di Input");
            return;
        }

        Percorso = Statici.padreGestore[MioCervello.IndexPercorso];
        indiceDestinazioni = -1;  //per convenzione ho assegnato il valore -1 per default (percorso no assegnato)
    }

    public void PreparoEsecuzione()
    {
        MioCervello.Agente.speed = 1.75f;
        // Da completare!!
        MioCervello.Agente.stoppingDistance = 0.8f;

    }
}

