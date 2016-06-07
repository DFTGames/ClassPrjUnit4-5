using UnityEngine;
using UnityEngine.UI;

public class ManagerScenaZero : MonoBehaviour
{
    public Animator animatoreLogin;
    public Animator animatoreScelta;
    public CanvasGroup canvasGroupLogin;
    public CanvasGroup canvasGroupScelta;
    public GameObject immagineCaricamento;
    public Text scrittaCaricamento;
    private static ManagerScenaZero me;

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

    public static Text ScrittaCaricamento
    {
        get
        {
            return me.scrittaCaricamento;
        }

        set
        {
            me.scrittaCaricamento = value;
        }
    }

    public static void AttivaDisattivaCanvasGroupLogin(bool abilita)
    {
        me.canvasGroupLogin.interactable = abilita;
    }

    public void BottoneMultiplayer()
    {
        animatoreScelta.SetTrigger("cambiaStato");
        animatoreLogin.SetTrigger("cambiaStato");
        Statici.multigiocatoreOn = true;
        AttivaDisattivaCanvasGroupScelta(false);
        AttivaDisattivaCanvasGroupLogin(true);
    }

    public void BottoneSinglePlayer()
    {
        AttivaDisattivaCanvasGroupLogin(false);
        AttivaDisattivaCanvasGroupScelta(false);
        Statici.multigiocatoreOn = false;
        StartCoroutine(GestoreCanvasAltreScene.ScenaInCarica("Scena Iniziale",test(),ImmagineCaricamento, ScrittaCaricamento));

    }
    public string test()
    {
        string[] testo = { "..Benvenuto nel magico Mondo del SinglePlayer...dove Tristezza e solitudine regneranno ", "....Premio Games Award 2016 come miglior Single player..ci sarai solo tu e se hai fortuna vedrai GOBLIN   ", ".......Malinconia    portami via.." };
        return testo[Random.Range(0, testo.Length)];

    }
    private void AttivaDisattivaCanvasGroupScelta(bool abilita)
    {
        canvasGroupScelta.interactable = abilita;
    }

    // Use this for initialization
    private void Start()
    {
        Statici.inGioco = false;
        me = this;
        AttivaDisattivaCanvasGroupScelta(true);
        AttivaDisattivaCanvasGroupLogin(false);
        ImmagineCaricamento = immagineCaricamento;
        ScrittaCaricamento = scrittaCaricamento;
    }
}