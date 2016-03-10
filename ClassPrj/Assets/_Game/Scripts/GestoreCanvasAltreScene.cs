using System.Collections;
using System.Collections.Generic;
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
    private DatiPersonaggio personaggio;

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
        float quanto = 1f;
        personaggio.gameObject.GetComponent<ControllerMaga>().PozioneVita(quanto);
    }

    public void BottoneRiceviDanno()
    {
        float quanto = 1f;
       personaggio.gameObject.GetComponent<ControllerMaga>().RiceviDanno(quanto);
    }

    private void Awake()
    {
        me = this;
    }
  
    public static void AggiornaDati(DatiPersonaggio datiPersonaggio)
    {     
        me.nomeText.text = datiPersonaggio.Nome;
        me.valoreVitaText.text = datiPersonaggio.Vita.ToString();
        me.valoreTipoText.text = datiPersonaggio.miaClasse.ToString();
        me.valoreAttaccoText.text = datiPersonaggio.Attacco.ToString();
        me.valoreDifesaText.text = datiPersonaggio.Difesa.ToString();
        me.sliderVita.minValue = 0f;
        me.sliderVita.maxValue = datiPersonaggio.VitaMassima;
        me.sliderVita.value = datiPersonaggio.Vita;
        me.personaggio = datiPersonaggio;
    }

    public static void AggiornaVita()
    {
        me.valoreVitaText.text = me.personaggio.Vita.ToString();
        me.sliderVita.value = me.personaggio.Vita;
     
    }

    public void Update()
    {      
        if (Input.GetKeyDown(KeyCode.F12))
        {
            if (pannelloTest.activeInHierarchy)
                pannelloTest.SetActive(false);
            else
                pannelloTest.SetActive(true);
        }
    }

    internal static IEnumerator ScenaInCarica(string nomeScena)
    {
        AsyncOperation asynCaricamentoScena = SceneManager.LoadSceneAsync(nomeScena);
        if (!asynCaricamentoScena.isDone && !me.immagineCaricamento.activeInHierarchy)
        {
            me.immagineCaricamento.SetActive(true);
            me.nomeScenaText.text = "Loading... " + nomeProssimaScena;
        }
        else if (me.immagineCaricamento.activeInHierarchy)
            me.immagineCaricamento.SetActive(false);
        yield return null;
    }
}