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
        animatoreScelta.SetTrigger("cambiaStato");
        animatoreLogin.SetTrigger("cambiaStato");
        Statici.multigiocatoreOn = false;
        AttivaDisattivaCanvasGroupScelta(false);
        AttivaDisattivaCanvasGroupLogin(true);
       

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