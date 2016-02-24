using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestoreCanvasAltreScene : MonoBehaviour
{
    public Text nomeText;
    public Text valoreVitaText;
    public Text valoreAttaccoText;
    public Text valoreTipoText;
    public Text valoreDifesaText;
    public Slider sliderVita;
    public GameObject immagineCaricamento;
    public Text nomeScenaText;
    public static string nomeProssimaScena=string.Empty;
    public Image cursoreImage;
    public GameObject pannelloTest;

    private static GestoreCanvasAltreScene me;
    private Serializzabile<AmicizieSerializzabili> datiDiplomazia;
    private Serializzabile<ValoriPersonaggioS> datiPersonaggio;
    private bool fatto = false;
  

    private string tagDellAltro = null;



    public void Gerra()
    {
        GameManager.DichiaroGuerra();
    }

    public void Pace()
    {
        GameManager.MiAlleo();
    }

    public void BottoneAumentaVita()
    {
        GameManager.PozioneVita();
    }

    public void BottoneRiceviDanno()
    {
        GameManager.RiceviDanno();
    }

    private void Awake()
    {
        me = this;
    }

  
    public static void AggiornaDati()
    {
        me.nomeText.text = GameManager.Nome;
        me.valoreVitaText.text = GameManager.VitaAttuale.ToString();
        me.valoreTipoText.text = GameManager.Classe.ToString();
        me.valoreAttaccoText.text = GameManager.Attacco.ToString();
        me.valoreDifesaText.text = GameManager.Difesa.ToString();
        me.sliderVita.minValue = 0f;
        me.sliderVita.maxValue = GameManager.VitaMassima;
        me.sliderVita.value = GameManager.VitaAttuale;
    }

    public static void AggiornaVita()
    {
        me.valoreVitaText.text = GameManager.VitaAttuale.ToString();
        me.sliderVita.value = GameManager.VitaAttuale;
     
    }

    public void Update()
    {
        //il metodo isLoadingLevel è deprecato ma non trovo un corrispondente che non lo è, che devo scrivere al posto suo?)
        if (Application.isLoadingLevel && !immagineCaricamento.activeInHierarchy)
        {
            immagineCaricamento.SetActive(true);
            nomeScenaText.text = "Loading... "+nomeProssimaScena;
        }
        else if (immagineCaricamento.activeInHierarchy)
            immagineCaricamento.SetActive(false);

        //verificare con pino se va bene scrivere così o nell'altro modo usando lo script Cursore:
        //  cursoreImage.transform.position = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (pannelloTest.activeInHierarchy)
                pannelloTest.SetActive(false);
            else
                pannelloTest.SetActive(true);
        }
    }
}