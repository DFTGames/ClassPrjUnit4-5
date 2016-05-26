using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestoreCanvasAltreScene : MonoBehaviour
{
    public static string nomeProssimaScena = string.Empty;
    public GameObject abilitaPannelloProveNetwork;
    public Image cursoreImage;
    public GameObject immagineCaricamento;
    public Text nomeScenaText;
    public Text nomeText;
    public GameObject pannelloTest;
    public Slider sliderVita;
    public Text valoreAttaccoText;
    public Text valoreDifesaText;
    public Text valoreTipoText;
    public Text valoreVitaText;
    private static GestoreCanvasAltreScene me;
    private bool fatto = false;
    private DatiPersonaggio personaggio;
    private string tagDellAltro = null;

    public static GameObject ImmagineCaricamento
    {
        get
        {
            return me.immagineCaricamento;
        }

        set
        {
            me.immagineCaricamento = value;
        }
    }

    public static Text NomeScenaText
    {
        get
        {
            return me.nomeScenaText;
        }

        set
        {
            me.nomeScenaText = value;
        }
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //  Debug.Log("son qua " + pannelloTest.activeInHierarchy);

            if (pannelloTest.activeInHierarchy)
                pannelloTest.SetActive(false);
            else

                pannelloTest.SetActive(true);
            if (Statici.multigiocatoreOn) abilitaPannelloProveNetwork.SetActive(true);
            else abilitaPannelloProveNetwork.SetActive(false);
        }
    }

    internal static IEnumerator ScenaInCarica(string nomeScena, string testoDaVisualizzare, GameObject immagineGOInCarica, Text scritta)
    {
        AsyncOperation asynCaricamentoScena = SceneManager.LoadSceneAsync(nomeScena);
        if (!asynCaricamentoScena.isDone)
        {
            immagineGOInCarica.SetActive(true);
            scritta.text = "Loading... " + testoDaVisualizzare;
        }
        else if (immagineGOInCarica.activeInHierarchy)
            immagineGOInCarica.SetActive(false);
        yield return null;
    }

    private void Awake()
    {
        me = this;
    }

    private void Start()
    {
        ImmagineCaricamento = immagineCaricamento;
        NomeScenaText = nomeScenaText;
    }
}