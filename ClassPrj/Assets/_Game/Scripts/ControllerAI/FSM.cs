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
    public float alphaGrad = 140f;
    public float ampiezza = 3f;
    public float dimensioneHandleDistArmi = 5f;
    public float dimensioneHandleVista = 5f;
    public float distanzaAttaccoGoblinArco = 7f;
    public float distanzaAttaccoGoblinPugno = 2f;
    public float quantoCiVedoSenzaOcchiali = 10f;
    public float velocitaOscillazioneVista = 2f;
    public bool visualizzaHandleAttacco = true;
    public bool visualizzaHandleVista = true;
    private NavMeshAgent agente;
    private Animator animatore;
    private IStato Attacco;
    private SphereCollider colliderSferaVista;
    private DatiPersonaggio datiPersonaggio;
    private float distanzaAttacco;
    private float distanzaTraPlayerGoblin;
    private int indexPercorso = -1;
    private IStato inseguimento;
    private Transform miaTransform;
    private IStato morte;
    private bool obiettivoInVista;
    private Transform obiettivoNemico;
    private IStato pattugliamento;
    private IStato statoCorrente;
    private IStato statoPrecedente;
    private SwitchVivoMortoAI switchVivoMorto;
    private TipoArma tipoArma;
    private bool ucciso = false;

    public NavMeshAgent Agente
    {
        get
        {
            return agente;
        }
    }

    public Animator Animatore
    {
        get
        {
            return animatore;
        }
    }

    public SphereCollider ColliderSferaVista
    {
        get
        {
            return colliderSferaVista;
        }
    }

    public DatiPersonaggio DatiPersonaggio
    {
        get
        {
            return datiPersonaggio;
        }

        set
        {
            datiPersonaggio = value;
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

    public bool inZonaAttacco { get; set; }

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

    public SwitchVivoMortoAI SwitchVivoMorto
    {
        get
        {
            return switchVivoMorto;
        }

        set
        {
            switchVivoMorto = value;
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

    public bool Ucciso
    {
        get
        {
            return ucciso;
        }

        set
        {
            ucciso = value;
        }
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

    public void Resuscita(float quanto)
    {
        DatiPersonaggio.Vita += quanto;
        Ucciso = false;
        SwitchVivoMorto.DisattivaRagdoll();
    }

    public void RiceviDanno(float quanto)
    {
        DatiPersonaggio.Vita -= quanto;
    }

    private void FixedUpdate()
    {
        if (!Ucciso)
            colliderSferaVista.radius = quantoCiVedoSenzaOcchiali + ampiezza * Mathf.Sin(Time.time * velocitaOscillazioneVista);
        else//se non lo metto a 0 non cade quando è morto.
            colliderSferaVista.radius = 0;
    }

    private void Start()
    {
        //per ora in caso multiplayer disattiviamo tutti i personaggi AI: poi si vede
        if (Statici.multigiocatoreOn)
            gameObject.SetActive(false);

        miaTransform = GetComponent<Transform>();
        agente = GetComponent<NavMeshAgent>();
        animatore = GetComponent<Animator>();
        DatiPersonaggio = GetComponent<DatiPersonaggio>();
        colliderSferaVista = GetComponentInChildren<SphereCollider>();
        colliderSferaVista = gameObject.transform.FindChild("SferaVista").GetComponent<SphereCollider>();
        SwitchVivoMorto = GetComponent<SwitchVivoMortoAI>();
        obiettivoNemico = null;
        obiettivoInVista = false;
        inZonaAttacco = false;
        colliderSferaVista.radius = quantoCiVedoSenzaOcchiali;
        pattugliamento = new Pattugliamento();
        inseguimento = new Inseguimento();
        Attacco = new Attacco();
        morte = new Morte();
        Attacco.Inizializza(this);
        pattugliamento.Inizializza(this);
        inseguimento.Inizializza(this);
        morte.Inizializza(this);
        statoCorrente = pattugliamento;        
        Statici.RegistraDatiPersonaggio(DatiPersonaggio);
        Statici.RecuperaDizionariDiplomazia(DatiPersonaggio.IdMiaClasse);
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
        if (DatiPersonaggio.Vita > 0)
        {
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
        else if (!Ucciso)
        {
            statoCorrente = morte;
            statoCorrente.Esecuzione();
        }
    }
}