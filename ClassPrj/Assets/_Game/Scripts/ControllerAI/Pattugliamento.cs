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

    //IMPOSTAZIONE PROVVISORIA IN ATTESA DELLA MATRICE PERCORSO

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

        MioCervello = oggetto;
        Agente = MioCervello.gameObject.GetComponent<NavMeshAgent>();
        Animatore = MioCervello.gameObject.GetComponent<Animator>();
        //IMPOSTAZIONE PROVVISORIA IN ATTESA DELLA MATRICE PERCORSO
        if (MioCervello.classeGoblin == 1)
        {
            Percorso = GameObject.Find("Percorso_1").GetComponent
                <GestorePercorso>();
            indiceDestinazioni = -1;

        }
        else if (MioCervello.classeGoblin == 2)
        {
            Percorso = GameObject.Find("Percorso_2").GetComponent
                <GestorePercorso>();
            indiceDestinazioni = -1;

        }
        else if (MioCervello.classeGoblin == 3)
        {
            Percorso = GameObject.Find("Percorso_3").GetComponent
                <GestorePercorso>();
            indiceDestinazioni = -1;

        }
        else Debug.Log("IMPOSTAZIONE NON DETERMINATA");

    }

    public void PreparoEsecuzione()
    {

    }
}

