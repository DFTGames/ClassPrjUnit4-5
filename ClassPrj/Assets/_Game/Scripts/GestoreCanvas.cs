using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestoreCanvas : MonoBehaviour
{
    public Text chiAttaccaText;
    public GameObject pannelloMorto;
    public Text vitaNemicoText;
    public Text vitaNomeText;
    public CanvasGroup canvasGroup;
    public InputField inputChat;   
    public Text contenutoChat;
    

   // private static GestoreCanvas me;
    private bool miStannoAttaccando = false;
    private string nomeDaVisualizzare;
    private GameObject playerLocaleGoCanvas;
    private float tempo = 0f;
    private int userDaVisualizzareNemico;
    private string vitaDaVisualizzare;
    private string vitaDaVisualizzareNemico;
    private ManagerNetwork managerNet;

    public string NomeDaVisualizzare
    {
        get
        {
            return nomeDaVisualizzare;
        }

        set
        {
            nomeDaVisualizzare = value;
        }
    }

    public  GameObject PlayerLocaleGoCanvas
    {
        get
        {
            return playerLocaleGoCanvas;
        }

        set
        {
            playerLocaleGoCanvas = value;
        }
    }

    public  int UserDaVisualizzareNemico
    {
        get
        {
            return userDaVisualizzareNemico;
        }

        set
        {
            userDaVisualizzareNemico = value;
        }
    }

    public string VitaDaVisualizzare
    {
        get
        {
            return vitaDaVisualizzare;
        }

        set
        {
            vitaDaVisualizzare = value;
            vitaNomeText.text = "Io: Nome:" + NomeDaVisualizzare + " " + "Vita:" + value + "Id:" + Statici.userLocaleId.ToString();
        }
    }

    public string VitaDaVisualizzareNemico
    {
        get
        {
            return vitaDaVisualizzareNemico;
        }

        set
        {
            
            vitaDaVisualizzareNemico = value;
            vitaNemicoText.text = "Nemico: vita: " + value + "Id: " + UserDaVisualizzareNemico.ToString();
        }
    }

    public void ResettaScrittaChiAttacca(bool attiva)
    {
        chiAttaccaText.gameObject.SetActive(attiva);
    }

    public void ResettaScrittaNemicoAttaccato(bool attiva)
    {
        vitaNemicoText.gameObject.SetActive(attiva);
    }

    public void VisualizzaChiTiAttacca(string nomeAttaccante)
    {
        tempo = 0f;
        miStannoAttaccando = true;
        ResettaScrittaChiAttacca(true);
        chiAttaccaText.text = nomeAttaccante + " ti sta attaccando!";
    }

    public void PannelloMorteOff()
    {        
        managerNet.Resuscita();
        pannelloMorto.SetActive(false);
    }

    public void PannelloMorteOn()
    {      
        pannelloMorto.SetActive(true);
    }

    // Use this for initialization
    private void Start()
    {
      //  me = this;
        pannelloMorto.SetActive(false);
        ResettaScrittaChiAttacca(false);
        ResettaScrittaNemicoAttaccato(false);
        AttivaDisattivaInputChat(true);
        managerNet = GameObject.Find("ManagerNetwork").GetComponent<ManagerNetwork>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (miStannoAttaccando)
        {
            tempo += Time.deltaTime;
            if (tempo >= 3f)
            {
                ResettaScrittaChiAttacca(false);
                tempo = 0f;
                miStannoAttaccando = false;
            }
        }

    }

    public void AttivaDisattivaInputChat(bool attiva)
    {
        canvasGroup.interactable = attiva;
    }

    public void InviaMessaggioChat()
    {
        if (inputChat.text != string.Empty)
        {
            AttivaDisattivaInputChat(false);
            
            managerNet.InviaChat(inputChat.text);
            inputChat.text = string.Empty;
        }
    }

    public  void ScriviMessaggioChat(string mittente, string messaggio)
    {
        contenutoChat.text += "<color=#FF3333>" + "<b>" + mittente + " : "+ "</b>"+ "</color>"  +" <color=#0000FF>"+messaggio+"</color>"+ "\n";
        AttivaDisattivaInputChat(true);
       
    }
}