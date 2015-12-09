using UnityEngine;

/// <summary>
/// cervello
/// </summary>
[RequireComponent(typeof(NavMeshAgent)),
    RequireComponent(typeof(Animator))]
public class FSM : MonoBehaviour
{
    public int classeGoblin = 1;
    public float quantoCiVedoSenzaOcchiali = 10f;
    public float alphaGrad = 140f;
    public bool visualizzaHandleVista = true;
    public float ampiezza = 3f;
    public float velocitaOscillazioneVista = 2f;
    public bool obiettivoInVista { get; set; }
    public bool inZonaAttacco { get; set; }
    public bool visualizzaHandleAttacco = true;
    public float distanzaAttaccoGoblinArco = 7f;
    public float distanzaAttaccoGoblinSpada = 2f;
    public float dimensioneHandleVista = 5f;
    public float dimensioneHandleDistArmi = 5f;

    private SphereCollider colliderSferaVista;
    private Transform obiettivoNemico;
    private IStato statoCorrente;
    private IStato statoPrecedente;
    private IStato pattugliamento;
    private IStato inseguimento;
    // private IStato attacco;

    public Transform ObiettivoNemico
    {
        get
        {
            return obiettivoNemico;
        }

        set
        {
            obiettivoNemico = value;
            obiettivoInVista = value == null ? false : true;
        }
    }

    public SphereCollider ColliderSferaVista
    {
        get
        {
            return colliderSferaVista;
        }
    }

    private void Start()
    {
        obiettivoNemico = null;
        obiettivoInVista = false;
        inZonaAttacco = false;
        colliderSferaVista = GetComponentInChildren<SphereCollider>();
        colliderSferaVista.radius = quantoCiVedoSenzaOcchiali;
         pattugliamento = new Pattugliamento();
        inseguimento = new Inseguimento();
        //Levare commento sotto quando qualcuno implementa lo stato di attacco:
        //  attacco = new Attacco();
         //attacco.Inizializza(this);
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
           /*  else
                 statoCorrente = attacco;*/ //levare il commento quando qualcuno implementa lo stato di attacco
        }
            else
                statoCorrente = pattugliamento;

        statoCorrente.Esecuzione();
    }

    private void FixedUpdate()
    {
        colliderSferaVista.radius = quantoCiVedoSenzaOcchiali + ampiezza * Mathf.Sin(Time.time * velocitaOscillazioneVista);
    }
}