using UnityEngine;
using UnityEngine.UI;

public class OggettiDaMarcare : MonoBehaviour
{
    public bool nascondiSeFuoriMappa = true;

    private bool bloccaOggetto;
    private int classePlayer;
    private bool controllaAmicizia = true;
    private DatiPersonaggio datiPersonaggio;
    private bool giocabile = false;
    private Image imageMarcatore;
    private GameObject marcatore;
    private int miaClasse;
    private Minimappa minimappa;
    private float misuraSprite = 20f;
    private Vector2 nuovaPosizioneMarcatore;
    private Transform playerT;
    private bool sonoUnEssereVivente = false;
    private bool spriteDiplomaziaMulti = false;
    private Sprite spriteOggetto;
    private bool valoreInizializzato = false;

    public bool BloccaOggetto
    {
        get
        {
            return bloccaOggetto;
        }

        set
        {
            bloccaOggetto = value;
        }
    }

    public bool ControllaAmicizia
    {
        get
        {
            return controllaAmicizia;
        }

        set
        {
            controllaAmicizia = value;
        }
    }

    public GameObject Marcatore
    {
        get
        {
            return marcatore;
        }

        set
        {
            marcatore = value;
        }
    }

    public Vector2 NuovaPosizioneMarcatore
    {
        get
        {
            return nuovaPosizioneMarcatore;
        }
        set
        {
            if (!BloccaOggetto)
                nuovaPosizioneMarcatore = value;
            else
                nuovaPosizioneMarcatore = Vector2.ClampMagnitude(value, minimappa.Massimo.magnitude);
        }
    }

    public void NascondiMarcatore()
    {
        imageMarcatore.gameObject.SetActive(false);
    }

    public bool Visibile()
    {
        return imageMarcatore.gameObject.activeSelf;
    }

    public void VisualizzaMarcatore()
    {
        imageMarcatore.gameObject.SetActive(true);
    }

    private void CambiaSpritePerDiplomazia()
    {
        if (Statici.dizionarioDiNemici[classePlayer].Contains(miaClasse))
        {
            spriteOggetto = minimappa.spriteNemico;
            misuraSprite = minimappa.misuraSpriteNemico;
        }
        else
        {
            spriteOggetto = minimappa.spriteAmico;
            misuraSprite = minimappa.misuraSpriteAmico;
        }
        imageMarcatore.sprite = spriteOggetto;
    }

    // Use this for initialization
    private void Start()
    {
        if (!Statici.inGioco)
            return;
        minimappa = GameObject.Find("Minimappa").GetComponent<Minimappa>();
        switch (gameObject.layer)
        {
            case (14):
                spriteOggetto = minimappa.spritePortale;
                misuraSprite = minimappa.misuraSpritePortale;
                sonoUnEssereVivente = false;
                break;

            case (11):
                datiPersonaggio = gameObject.GetComponent<DatiPersonaggio>();
                miaClasse = datiPersonaggio.IdMiaClasse;
                giocabile = datiPersonaggio.Giocabile;
                if (!Statici.multigiocatoreOn)
                {
                    classePlayer = Statici.PersonaggioPrincipaleT.GetComponent<DatiPersonaggio>().IdMiaClasse;
                    spriteOggetto = minimappa.spriteAmico;
                    misuraSprite = minimappa.misuraSpriteAmico;
                }
                else
                {
                    //per ora messi tutti nemici perchè se sono multiplayer le amicizie vanno decise sul server mentre i non giocabili non li visualizzo.
                    if (giocabile)
                    {
                        spriteOggetto = minimappa.spriteNemico;
                        misuraSprite = minimappa.misuraSpriteNemico;
                    }
                    else
                        return;//se è multiplayer e non giocabile disattivo il personaggio AI per cui non deve vedersi il suo marcatore.
                }
                sonoUnEssereVivente = true;

                break;

            default:
                Debug.LogError("l'oggetto " + gameObject.name + " non appartiene ad un layer preso in considerazione dalla minimappa.");
                break;
        }

        Marcatore = new GameObject("Marcatore");
        imageMarcatore = Marcatore.AddComponent<Image>();
        if (Statici.multigiocatoreOn && giocabile)
        {
            DatiMarcatoreMulti datiMarcatoreMultiutente = Marcatore.AddComponent<DatiMarcatoreMulti>();
            datiMarcatoreMultiutente.idUtente = datiPersonaggio.Utente;
            minimappa.listaUserIdMarcati.Add(datiMarcatoreMultiutente);
        }

        Marcatore.transform.SetParent(minimappa.transform);
        imageMarcatore.sprite = spriteOggetto;
        imageMarcatore.rectTransform.localPosition = Vector3.zero;
        imageMarcatore.rectTransform.localScale = Vector3.one;
        imageMarcatore.rectTransform.sizeDelta = new Vector2(misuraSprite, misuraSprite);
        playerT = minimappa.PlayerT;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Statici.inGioco || minimappa.PlayerT == null)
            return;

        NuovaPosizioneMarcatore = minimappa.CalcolaPosizioneMarcatore(transform.position); //imposto la posizione
        imageMarcatore.rectTransform.localPosition = NuovaPosizioneMarcatore;//assegno la posizione.
        minimappa.GestisciVisibilitaMarcatore(this); //Gestisco La visibilità o l'invisibilità del marcatore
        if (!Statici.multigiocatoreOn && sonoUnEssereVivente && ControllaAmicizia)
        {
            CambiaSpritePerDiplomazia();
            //ControllaAmicizia = false;
        }
    }
}