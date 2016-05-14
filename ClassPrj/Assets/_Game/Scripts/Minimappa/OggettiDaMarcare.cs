using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OggettiDaMarcare : MonoBehaviour
{

    public bool nascondiSeFuoriMappa = true;

    private Image imageMarcatore;
    private Transform playerT;
    private Minimappa minimappa;
    private Vector2 nuovaPosizioneMarcatore;
    private bool bloccaOggetto;
    private GameObject marcatore;
    private Sprite spriteOggetto;
    private classiPersonaggi miaClasse;
    private classiPersonaggi classePlayer;
    private bool sonoUnEssereVivente = false;
    private float misuraSprite = 20f;
    private bool controllaAmicizia = true;
    private bool valoreInizializzato = false;
    private bool giocabile = false;
    private DatiPersonaggio datiPersonaggio;
    private bool spriteDiplomaziaMulti = false;

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

    // Use this for initialization
    void Start()
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
                miaClasse = datiPersonaggio.miaClasse;
                giocabile = datiPersonaggio.Giocabile;
                if (!Statici.multigiocatoreOn)
                {
                    classePlayer = Statici.PersonaggioPrincipaleT.GetComponent<DatiPersonaggio>().miaClasse;
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

    // Update is called once per frame
    void Update()
    {
        if (!Statici.inGioco)
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
    public bool Visibile()
    {
        return imageMarcatore.gameObject.activeSelf;
    }
    public void NascondiMarcatore()
    {
        imageMarcatore.gameObject.SetActive(false);
    }
    public void VisualizzaMarcatore()
    {
        imageMarcatore.gameObject.SetActive(true);
    }


}
