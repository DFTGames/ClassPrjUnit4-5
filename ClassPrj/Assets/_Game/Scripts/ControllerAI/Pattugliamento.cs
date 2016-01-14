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
            if (Agente.remainingDistance <= Agente.stoppingDistance)
            {    
                indiceDestinazioni = indiceDestinazioni < Percorso.transform.childCount ? ++indiceDestinazioni : 0;
                ProssimaDirezione = Percorso[indiceDestinazioni];
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
        datiDiplomazia = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);
        MioCervello = oggetto;

        // ..MI TROVA L'INDICE DEL PERCORSO DEL CERVELLO(PERSONAGGIO) A CUI E' ATTACCATO
        for (int i = 0; i < datiDiplomazia.Dati.indexPercorsi.Length; i++)
        {
            if (MioCervello.gameObject.tag == datiDiplomazia.Dati.tipoEssere[i])
            {                
                MioCervello.IndexPercorso = datiDiplomazia.Dati.indexPercorsi[i];
            }
        }

       
        //-Vedere se meglio con impostazione nostra di indiceDestionazioni oppure fare in modo che a ogni chiamata al Gestore PErcorso si incrementa da solo
        if (MioCervello == null) return;
        if (MioCervello.IndexPercorso < 0) return;
        Agente = MioCervello.gameObject.GetComponent<NavMeshAgent>();
        Animatore = MioCervello.gameObject.GetComponent<Animator>();

        GameObject tm = GameObject.Find("PadrePercorso");
        if (tm == null) return;
        PadreGestore tmpObj = tm.GetComponent<PadreGestore>();
        if (tmpObj == null) return;  //Paranoia luc-Code
       
        Percorso =tmpObj[MioCervello.IndexPercorso];
        indiceDestinazioni = -1;
    }

    public void PreparoEsecuzione()
    {

    }
}

