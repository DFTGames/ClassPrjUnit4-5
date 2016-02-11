using UnityEngine;
using System.Collections;



public class Pattugliamento : IStato
{

    public string Nome { get; set; }

    private int indiceDestinazioni = 0;
    private FSM MioCervello;
    private NavMeshAgent Agente;
    private Transform ProssimaDirezione;

    private GestorePercorso Percorso=null;
    private Animator Animatore;

    private Serializzabile<AmicizieSerializzabili> datiDiplomazia;
 

    public void Esecuzione()
    {
        
        if (Percorso != null && Percorso.transform.childCount>0)
        {
            Agente.stoppingDistance = 0.8f;
            if (Agente.remainingDistance <= Agente.stoppingDistance)
            {    
                indiceDestinazioni = indiceDestinazioni < Percorso.transform.childCount ? ++indiceDestinazioni : 0;
                Transform tmpPercorso = Percorso[indiceDestinazioni]; //Se nessun Nodo e' stato impostato come percorso
                if (tmpPercorso == null) return;
                ProssimaDirezione = tmpPercorso;
                Agente.SetDestination(ProssimaDirezione.position);                
            }
            Animatore.SetFloat("Velocita", 0.5f);
        }
    }

    public void EsecuzioneTerminata()
    {

    }

    public void Inizializza(FSM oggetto)
    {

        MioCervello = oggetto;

        if (GameManager.dizionarioPercorsi == null) return;

        if (GameManager.dizionarioPercorsi.ContainsKey(MioCervello.gameObject.tag))
            MioCervello.IndexPercorso = GameManager.dizionarioPercorsi[MioCervello.gameObject.tag];

        //-Vedere se meglio con impostazione nostra di indiceDestionazioni oppure fare in modo che a ogni chiamata al Gestore PErcorso si incrementa da solo
        if (MioCervello == null) return;
        if (MioCervello.IndexPercorso < 0) return;
        Agente = MioCervello.gameObject.GetComponent<NavMeshAgent>();
        Animatore = MioCervello.gameObject.GetComponent<Animator>();

        GameObject tm = GameObject.Find("PadrePercorso");
        if (tm == null) return;
        PadreGestore tmpObj = tm.GetComponent<PadreGestore>();
        if (tmpObj == null) return;  //Paranoia luc-Code

        Percorso = tmpObj[MioCervello.IndexPercorso];
        indiceDestinazioni = -1;
    }

    public void PreparoEsecuzione()
    {

    }
}

