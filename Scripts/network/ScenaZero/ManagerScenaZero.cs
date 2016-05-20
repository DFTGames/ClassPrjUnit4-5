using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ManagerScenaZero : MonoBehaviour {

    private static ManagerScenaZero me;

    public Animator animatoreScelta;
    public Animator animatoreLogin;
    public CanvasGroup canvasGroupScelta;
    public CanvasGroup canvasGroupLogin;

	// Use this for initialization
	void Start () {
        Statici.inGioco = false;
        me = this;
        AttivaDisattivaCanvasGroupScelta(true);
        AttivaDisattivaCanvasGroupLogin(false);
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
        SceneManager.LoadScene("Scena Iniziale");        
    }

    private void AttivaDisattivaCanvasGroupScelta(bool abilita)
    {
        canvasGroupScelta.interactable = abilita;       
    }

    public static void AttivaDisattivaCanvasGroupLogin(bool abilita)
    {
        me.canvasGroupLogin.interactable = abilita;

    }
}
