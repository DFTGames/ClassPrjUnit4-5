using UnityEngine;

public enum TipoArma
{
    Pugno,
    Arco
}

/// <summary>
/// cervello
/// </summary>
[RequireComponent(typeof(NavMeshAgent)),
    RequireComponent(typeof(Animator))]
public class FSM : MonoBehaviour
{
    public float quantoCiVedoSenzaOcchiali = 10f;
    public float alphaGrad = 140f;
    public bool visualizzaHandleVista = true;
    public float ampiezza = 3f;
    public float velocitaOscillazioneVista = 2f;
    public bool inZonaAttacco { get; set; }
    public bool visualizzaHandleAttacco = true;
    public float distanzaAttaccoGoblinArco = 7f;
    public float distanzaAttaccoGoblinPugno = 2f;
    public float dimensioneHandleVista = 5f;
    public float dimensioneHandleDistArmi = 5f;

    private float distanzaAttacco;
    private float distanzaTraPlayerGoblin;
    private bool obiettivoInVista;
    private TipoArma tipoArma;
    private int indexPercorso = -1;
    private SphereCollider colliderSferaVista;
    private Transform obiettivoNemico;
    private Transform miaTransform;
    private NavMeshAgent agente;
    private Animator animatore;
    private IStato statoCorrente;
    private IStato statoPrecedente;
    private IStato pattugliamento;
    private IStato inseguimento;
    private IStato Attacco;
    private DatiPersonaggio datiPersonaggio;

    public Transform ObiettivoNemico
    {
        get
        {
            return obiettivoNemico;
        }

        set
        {
            obiettivoNemico = value;
        }
    }

    public Transform MiaTransform
    {
        get
        {
            return miaTransform;
        }
        set
        {
            miaTransform = value;
        }
    }

    public Animator Animatore
    {
        get
        {
            return animatore;
        }
    }

    public NavMeshAgent Agente
    {
        get
        {
            return agente;
        }
    }

    public SphereCollider ColliderSferaVista
    {
        get
        {
            return colliderSferaVista;
        }
    }

    public int IndexPercorso
    {
        get
        {
            return indexPercorso;
        }

        set
        {
            if (value < 0) indexPercorso = -1;
            else
                indexPercorso = value;
        }
    }

    public TipoArma TipoArma
    {
        get
        {
            return tipoArma;
        }

        set
        {
            tipoArma = value;
            DistanzaAttacco = (value == TipoArma.Pugno) ? distanzaAttaccoGoblinPugno : distanzaAttaccoGoblinArco;
        }
    }

    public bool ObiettivoInVista
    {
        get
        {
            return obiettivoInVista;
        }

        set
        {
            obiettivoInVista = value;
        }
    }

    public float DistanzaAttacco
    {
        get
        {
            return distanzaAttacco;
        }

        set
        {
            distanzaAttacco = value;
        }
    }

    public float DistanzaTraPlayerGoblin
    {
        get
        {
            return distanzaTraPlayerGoblin;
        }

        set
        {
            distanzaTraPlayerGoblin = value;
        }
    }

    private void Start()
    {
        miaTransform = GetComponent<Transform>();
        agente = GetComponent<NavMeshAgent>();
        animatore = GetComponent<Animator>();
        obiettivoNemico = null;
        obiettivoInVista = false;
        inZonaAttacco = false;
        colliderSferaVista = GetComponentInChildren<SphereCollider>();
        colliderSferaVista.radius = quantoCiVedoSenzaOcchiali;
        pattugliamento = new Pattugliamento();
        inseguimento = new Inseguimento();
        Attacco = new Attacco();
        Attacco.Inizializza(this);
        pattugliamento.Inizializza(this);
        inseguimento.Inizializza(this);
        statoCorrente = pattugliamento;
        datiPersonaggio = GetComponent<DatiPersonaggio>();
        GameManager.RegistraDatiPersonaggio(datiPersonaggio);
    }

    private void Update()
    {
        if (statoCorrente != statoPrecedente)
        {
            if (statoPrecedente != null)
                statoPrecedente.EsecuzioneTerminata();
            statoCorrente.PreparoEsecuzione();
            statoPrecedente = statoCorrente;
        }

        if (ObiettivoNemico != null)
        {
            if (inZonaAttacco && ObiettivoInVista)
                statoCorrente = Attacco;
            else
                statoCorrente = inseguimento;
        }
        else
            statoCorrente = pattugliamento;

        statoCorrente.Esecuzione();

    }

    private void FixedUpdate()
    {
        colliderSferaVista.radius = quantoCiVedoSenzaOcchiali + ampiezza * Mathf.Sin(Time.time * velocitaOscillazioneVista);
    }

    //richiamare questo metodo come evento dell'animazione attacco nel frame finale
    public void FaiDanno()
    {
        //controllare se il personaggio è girato verso il nemico e quindi se è nel suo arco di attacco
        //farsi dare la percentuale di resistere all'attacco dal nemico
        //calcolare un numero random da 1 a 100, per esempio supponendo che la percentuale del nemico di resistere sia del 20%,
        //se il  numero random è un numero inferiore a 20 l'attacco non è andato a buon fine se invece è un numero da 21 a 100 è andato a buonfine.
        //se l'attacco è andato a buonfine:
        //recuperare attaccobase del personaggio da DatiPersonaggio, e recuperare tutti i dati del nemico relativi alla sua difesa
        //calcolare il danno da effettuare in base a tutti i valori sopra citati(secondo una qualche equazione che li lega)
        //mandare un messaggio al metodo del nemico RiceviDanno passando come parametro la quantità di danno inflitta
    }

    public void RiceviDanno(float quanto)
    {
        datiPersonaggio.Vita -= quanto;
    }

}