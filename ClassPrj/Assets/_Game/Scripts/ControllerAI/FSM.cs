using UnityEngine;

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
    public bool obiettivoInVista { get; set; }
    public bool inZonaAttacco { get; set; }
    public bool visualizzaHandleAttacco = true;
    public float distanzaAttaccoGoblinArco = 7f;
    public float distanzaAttaccoGoblinPugno = 2f;
    public float dimensioneHandleVista = 5f;
    public float dimensioneHandleDistArmi = 5f;
    public float distanzaAttacco;

  
    public bool attaccoDaVicino = false;

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

    public Transform ObiettivoNemico
    {
        get
        {
            return obiettivoNemico;
        }

        set
        {
            obiettivoNemico = value;
            obiettivoInVista = (value == null )? false : true;
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

    private void Start()
    {
        miaTransform=GetComponent<Transform>();
        agente=GetComponent<NavMeshAgent>();
        animatore=GetComponent<Animator>();
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

        if (obiettivoInVista)
        {
            if (!inZonaAttacco)
                statoCorrente = inseguimento;
            else
                statoCorrente = Attacco;
          
        }
        else
        {
            
            statoCorrente = pattugliamento;
            inZonaAttacco = false;
        }
        statoCorrente.Esecuzione();
        Debug.Log(attaccoDaVicino);
    }

    private void FixedUpdate()
    {
        colliderSferaVista.radius = quantoCiVedoSenzaOcchiali + ampiezza * Mathf.Sin(Time.time * velocitaOscillazioneVista);
    }
}
