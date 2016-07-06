﻿using UnityEngine;
using UnityEngine.UI;

public class GestoreCanvasNetwork : MonoBehaviour
{
    public Animator animatoreChat;
    public GameObject canvaMultiGO;
    public CanvasGroup canvasGroupChat;
    public GameObject chat;
    public Text chiAttaccaText;
    public Text contenutoChat;
    public InputField inputChat;
    public GameObject pannelloMorto;
    public Text vitaNemicoText;
    public Text vitaNomeText;
    private static GestoreCanvasNetwork me;
    private ManagerNetwork managerNet;
    private ManagerNetworkSinglePlayer managerNetSingle;

    //  private static GestoreCanvas me;
    private bool miStannoAttaccando = false;

    private float tempo = 0f;

    public void ApriChiudiChat()
    {
        animatoreChat.SetTrigger("chatOnOff");
    }

    public void AttivaDisattivaInputChat(bool attiva)
    {
        canvasGroupChat.interactable = attiva;
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

    public void PannelloMorteOff()
    {
        if (Statici.multigiocatoreOn)
            managerNet.Resuscita();
        else
            managerNetSingle.Resuscita();
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

    public void ScriviMessaggioChat(string mittente, string messaggio)
    {
        contenutoChat.text += "<color=#FF3333>" + "<b>" + mittente + " : " + "</b>" + "</color>" + " <color=#0000FF>" + messaggio + "</color>" + "\n";
        AttivaDisattivaInputChat(true);
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

    public void VisualizzaDatiUserSelezionato(string nome, double vita)
    {
        ResettaScrittaNemicoAttaccato(true);
        vitaNemicoText.text = "Nome :" + nome + ": Vita: " + vita;
    }

    public void VisualizzaNascondiCanvasGroup(bool attiva)
    {
        canvaMultiGO.SetActive(attiva);
    }
   


    // Use this for initialization
    private void Start()
    {
        me = this;
        if (!Statici.multigiocatoreOn)
            chat.SetActive(false);


        pannelloMorto.SetActive(false);
        ResettaScrittaChiAttacca(false);
        ResettaScrittaNemicoAttaccato(false);
        AttivaDisattivaInputChat(true);
        if(Statici.multigiocatoreOn)
          managerNet = GameObject.Find("ManagerNetwork").GetComponent<ManagerNetwork>();
        else
          managerNetSingle= GameObject.Find("ManagerNetwork").GetComponent<ManagerNetworkSinglePlayer>();
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
}