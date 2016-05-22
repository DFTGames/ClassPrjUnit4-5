using UnityEngine;
using UnityEngine.UI;

public class GestoreCanvasNetwork : MonoBehaviour
{
    public GameObject canvaMultiGO;
    public Text chiAttaccaText;
    public GameObject pannelloMorto;
    public Text vitaNemicoText;
    public Text vitaNomeText;
    public CanvasGroup canvasGroup;
    public InputField inputChat;
    public Text contenutoChat;
    public Animator animatoreChat; 

    //  private static GestoreCanvas me;
    private bool miStannoAttaccando = false;
    private float tempo = 0f;
    private ManagerNetwork managerNet;

    public void PannelloMorteOff()
    {
        managerNet.Resuscita();
        pannelloMorto.SetActive(false);
    }

    public void PannelloMorteOn()
    {
        pannelloMorto.SetActive(true);
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

    public void VisualizzaDatiPlayerLocale(string nome, string vita)
    {
        vitaNomeText.text = "Io: Nome:" + nome + " " + "Vita:" + vita + "Id:" + Statici.userLocaleId.ToString();
    }

    public void VisualizzaDatiUserSelezionato(string nome, float vita)
    {
        ResettaScrittaNemicoAttaccato(true);
        vitaNemicoText.text ="Nome :" + nome + ": Vita: " + vita;
    }

    // Use this for initialization
    private void Start()
    {
        if (!Statici.multigiocatoreOn)
        {
            canvaMultiGO.SetActive(false);
            return;
        }
        // me = this;
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

    public void ApriChiudiChat()
    {
        animatoreChat.SetTrigger("chatOnOff");
    }

      public void AttivaDisattivaInputChat(bool attiva)
      {
          canvasGroup.interactable = attiva;
      }

      public void InviaMessaggioChat()
      {
          if (inputChat.text.Trim() != string.Empty)
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