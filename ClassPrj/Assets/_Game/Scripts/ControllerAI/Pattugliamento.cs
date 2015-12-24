using UnityEngine;
using System.Collections;



public class Pattugliamento : IStato
{

    public string Nome { get; set; }

    private int indiceDestinazioni = 0;
    private bool CambiaDirezione = false;
    private FSM MioCervello;
    private NavMeshAgent Agente;
    private Transform ProssimaDirezione;
    //  private Transform[] Destinazioni;
    private GestorePercorso Percorso=null;
    private Animator Animatore;

    //******
    private Serializzabile<AmicizieSerializzabili> amicizie;
    //*******

    void Start()
    {
        //*****    AGGIUNTO...MI TROVA L'INDICE DEL PERCORSO DEL CERVELLO(PERSONAGGIO) A CUI E' ATTACCATO
        //VALUTARE SE E' CORRETTO CERCARLO DA QUA ..O ALTROVE..
        //.Andrebbe fatto dopo che il cervello e' stato istanziato...(OnEnable viene prima dello start..quindi se lo metto dentro a questo metodo
        //e il cervello viene creato dopo non penso vada bene...bohhh

        amicizie = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);        
        for (int i = 0; i < amicizie.Dati.indexPercorsi.Length; i++)

            if (MioCervello.gameObject.tag == amicizie.Dati.tagEssere[i]) MioCervello.IndexPercorso = amicizie.Dati.indexPercorsi[i];

        //***
    }

    public void Esecuzione()
    {
        if (Percorso != null)
        {
            if (Agente.remainingDistance <= Agente.stoppingDistance)

            {
                indiceDestinazioni = indiceDestinazioni < Percorso.transform.childCount ? ++indiceDestinazioni : 0;
                ProssimaDirezione = Percorso[indiceDestinazioni];
                Agente.SetDestination(ProssimaDirezione.position);
                //  Agente.speed = 0.5f;
                CambiaDirezione = false;

            }
            Animatore.SetFloat("Velocita", 0.5f);
        }
    }

    public void EsecuzioneTerminata()
    {

    }

    public void Inizializza(FSM oggetto)
    {

        //*****************************
        //-VEDERE SE E' CORRETTO ...Se occorre recuperare il GestorePErcorso in questo modo..oppure era meglio assegnare la referenza al gestorePercorso nel database dei percorsi
        //-Vedere se meglio con impostazione nostra di indiceDestionazioni oppure fare in modo che a ogni chiamata al Gestore PErcorso si incrementa da solo
        if (MioCervello.IndexPercorso < 0) return;
        Agente = MioCervello.gameObject.GetComponent<NavMeshAgent>();
        Animatore = MioCervello.gameObject.GetComponent<Animator>();

        GameObject tmpObj = GameObject.Find("PadrePercorso");

        if (tmpObj == null) return;
        for (int i = 0; i < tmpObj.transform.childCount; i++)        
        if (tmpObj.transform.GetChild(i).GetComponent<GestorePercorso>().IndexPercorso == MioCervello.IndexPercorso)
        {
            Percorso = tmpObj.transform.GetChild(i).GetComponent<GestorePercorso>();
                indiceDestinazioni = -1;
                break;
        }
        //********************************
        /*
        if (MioCervello.IndexPercorso == 1)
        {
            Percorso = GameObject.Find("Percorso_1").GetComponent
                <GestorePercorso>();
            indiceDestinazioni = -1;

        }
        else if (MioCervello.IndexPercorso == 2)
        {
            Percorso = GameObject.Find("Percorso_2").GetComponent
                <GestorePercorso>();
            indiceDestinazioni = -1;

        }
        else if (MioCervello.IndexPercorso == 3)
        {
            Percorso = GameObject.Find("Percorso_3").GetComponent
                <GestorePercorso>();
            indiceDestinazioni = -1;

        }
        
        else Debug.Log("IMPOSTAZIONE NON DETERMINATA");
        */

    }

    public void PreparoEsecuzione()
    {

    }
}

