using UnityEngine;

/// <summary>
/// cervello
/// </summary>
[RequireComponent(typeof(NavMeshAgent)),
    RequireComponent(typeof(Animator)),
    RequireComponent(typeof(SphereCollider))]
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

    private IStato statoCorrente;
    private IStato statoPrecedente;
    private IStato pattugliamento;
    private IStato inseguimento;
   // private IStato attacco;

    private void ControlloVista(bool visto)
    {
        obiettivoInVista = visto;
    }

    private void ControlloInZonaAttacco(bool possoAttaccare)
    {
        inZonaAttacco = possoAttaccare;
    }

    private void Start()
    {
        obiettivoInVista = false;
        inZonaAttacco = false;
        colliderSferaVista = GetComponent<SphereCollider>();
        colliderSferaVista.radius = quantoCiVedoSenzaOcchiali;
        pattugliamento = new Pattugliamento();
        inseguimento = new Inseguimento();
        //Levare commento sotto quando qualcuno implementa lo stato di attacco:
        //  attacco = new Attacco(); 
        //  attacco.Inizializza(this); 
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

        statoCorrente.Esecuzione();
        if (obiettivoInVista)
        {
            if (!inZonaAttacco)
                statoCorrente = inseguimento;
           /* else
                statoCorrente = attacco;*/  //levare il commento quando qualcuno implementa lo stato di attacco
        }
        else
            statoCorrente = pattugliamento;
    }

    private void FixedUpdate()
    {
        colliderSferaVista.radius = quantoCiVedoSenzaOcchiali + ampiezza * Mathf.Sin(Time.time * velocitaOscillazioneVista);
    }
}