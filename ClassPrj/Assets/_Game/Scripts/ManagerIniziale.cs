using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;



public class ManagerIniziale : MonoBehaviour
{
    public Animator AnimatoreMenu;
    public Animator AltrieMenu1;
    public Animator AltrieMenu2;

    public Button BottoneCaricaOff;
    public Button EliminaPartita;

    public Text CasellaTipo;
    public Text ValoreVita;
    public Text ValoreAttacco;
    public Text ValoreDifesa;
    public Text TestoNome;
    public Text Errore;
    public Text ErroreCaricamento;

    public Text VitaAttuale;
    public Text AttaccoAtuale;
    public Text DifesaAttuale;

    public GameData databseInizialeAmicizie;
    public caratteristichePersonaggioV2 databaseInizialeProprieta;
    public Dropdown ElencoCartelle;

    private int indiceButton = 0;
    private Serializzabile<ValoriPersonaggioS> datiPersonaggio;
   
    private Serializzabile<AmicizieSerializzabili> datiDiplomazia;
    private static ManagerIniziale Me;

    public int IndiceButton
    {
        get
        {
            return indiceButton;
        }

        set
        {
            int valoreMinimo = 0;
            //int valoreMassimo = Enum.GetValues(typeof(TipoMaga)).Length - 1;
            int valoreMassimo = databaseInizialeProprieta.classePersonaggio.Count - 1;
            indiceButton = Mathf.Clamp(value, valoreMinimo, valoreMassimo);
            if (value > valoreMassimo)
                indiceButton = valoreMinimo;
            if (value < valoreMinimo)
                indiceButton = valoreMassimo;
        }
    }

    // Use this for initialization
    private void Start()
    {
        Me = this;
      //  CasellaTipo.text = TipoMaga.magaRossa.ToString();
        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);
       
    }

    public void NuovaPartita()
    {
        AltrieMenu1.SetBool("Torna", true);
        AnimatoreMenu.SetBool("Via", true);
    }

    public void CaricaPartita()
    {
        AltrieMenu2.SetBool("Torna", true);
        AnimatoreMenu.SetBool("Via", true);
        RecuperaElencoCartelle();
        RecuperaDatiGiocatore();
    }

    public void CaricaScena()
    {
        Application.LoadLevel(1);
    }

    public void AvviaGioco()
    {
        Errore.text = String.Empty;
        if (TestoNome.text.Trim() == string.Empty)
        {
            Errore.text = "Inserire un nome";
            return;
        }

        DirectoryInfo dI = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] dirs = dI.GetDirectories();
        for (int i = 0; i < dirs.Length; i++)
        {
            if (dirs[i].Name.ToLower() == TestoNome.text.ToLower())
            {
                Errore.text = "Esiste Gia Un Personaggio con questo nome";
                return;
            }
        }
        Statici.nomePersonaggio = TestoNome.text;

        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);

        if (datiPersonaggio.Dati.nomePersonaggio == null)
        {
            datiPersonaggio.Dati.Vita = databaseInizialeProprieta.matriceProprieta[IndiceButton].Vita;
            datiPersonaggio.Dati.Attacco = databaseInizialeProprieta.matriceProprieta[IndiceButton].Attacco;
            datiPersonaggio.Dati.difesa = databaseInizialeProprieta.matriceProprieta[IndiceButton].difesa;
            datiPersonaggio.Dati.Xp = databaseInizialeProprieta.matriceProprieta[IndiceButton].Xp;
            datiPersonaggio.Dati.Livello = databaseInizialeProprieta.matriceProprieta[IndiceButton].Livello;
            datiPersonaggio.Dati.nomeM = databaseInizialeProprieta.matriceProprieta[IndiceButton].nomeM;
            datiPersonaggio.Dati.nomeF = databaseInizialeProprieta.matriceProprieta[IndiceButton].nomeF;
            datiPersonaggio.Dati.nomePersonaggio = TestoNome.text;
            datiPersonaggio.Dati.classe = CasellaTipo.text;

            datiPersonaggio.Dati.VitaMassima = databaseInizialeProprieta.matriceProprieta[IndiceButton].Vita;
            datiPersonaggio.Dati.ManaMassimo = databaseInizialeProprieta.matriceProprieta[IndiceButton].Mana;
            datiPersonaggio.Dati.XPMassimo = databaseInizialeProprieta.matriceProprieta[IndiceButton].Xp;

            datiPersonaggio.Salva();
        }

        datiDiplomazia = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);
        if (datiDiplomazia.Dati.tipoEssere[0] == null)
        {
            for (int i = 0; i < databseInizialeAmicizie.tagEssere.Length; i++)
            {
                datiDiplomazia.Dati.tipoEssere[i] = databseInizialeAmicizie.tagEssere[i];
            }
            for (int i = 0; i < databseInizialeAmicizie.tagEssere.Length; i++)
            {
                datiDiplomazia.Dati.matriceAmicizie[i] = databseInizialeAmicizie.matriceAmicizie[i];

                for (int j = 0; j < databseInizialeAmicizie.tagEssere.Length; j++)
                {
                    datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] = databseInizialeAmicizie.matriceAmicizie[i].elementoAmicizia[j];
                }
            }

            datiDiplomazia.Salva();
        }
        Application.LoadLevel(1);
    }

    public void Anulla1()
    {
        AltrieMenu1.SetBool("Torna", false);
        AnimatoreMenu.SetBool("Via", false);
    }

    public void Anulla2()
    {
        AltrieMenu2.SetBool("Torna", false);
        AnimatoreMenu.SetBool("Via", false);
    }

    public void Precedente()
    {
        IndiceButton--;
    }

    public void Sucessivo()
    {
        IndiceButton++;
    }

    // Update is called once per frame
    private void Update()
    {
        /*CasellaTipo.text = Enum.GetValues
            (typeof(TipoMaga)).GetValue(IndiceButton).ToString();*/
        CasellaTipo.text = databaseInizialeProprieta.classePersonaggio[IndiceButton].ToString();

        ValoreVita.text = databaseInizialeProprieta.matriceProprieta[IndiceButton].Vita.ToString();
        ValoreAttacco.text = databaseInizialeProprieta.matriceProprieta[IndiceButton].Attacco.ToString();
        ValoreDifesa.text = databaseInizialeProprieta.matriceProprieta[IndiceButton].difesa.ToString();
    }

    public void RecuperaElencoCartelle()
    {
        ErroreCaricamento.text = string.Empty;
        Dropdown.OptionData Tmp = null;
        ElencoCartelle.options.Clear();
        DirectoryInfo Di = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] Drs = Di.GetDirectories();
        for (int i = 0; i < Drs.Length; i++)
        {
            Tmp = new Dropdown.OptionData();
            Tmp.text = Drs[i].Name;
            ElencoCartelle.options.Add(Tmp);
        }
        if (Drs.Length > 0)
        {
            BottoneCaricaOff.interactable = true;
            EliminaPartita.interactable = true;
            ElencoCartelle.value = Drs.Length;
            ElencoCartelle.value = 0;
            if (ElencoCartelle.value > 0)
                Statici.nomePersonaggio = ElencoCartelle.options[ElencoCartelle.value].text;
            else
                Statici.nomePersonaggio = string.Empty;
        }
        else
        {
            BottoneCaricaOff.interactable = false;
            EliminaPartita.interactable = false;
            ErroreCaricamento.text = "Non ci sono partite salvate";
        }
    }

    public void RecuperaDatiGiocatore()
    {
        Statici.nomePersonaggio =
        ElencoCartelle.options[ElencoCartelle.value].text;
        Debug.Log(ElencoCartelle.value);
        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);

        VitaAttuale.text = datiPersonaggio.Dati.Vita.ToString();
        AttaccoAtuale.text = datiPersonaggio.Dati.Attacco.ToString();
        DifesaAttuale.text = datiPersonaggio.Dati.difesa.ToString();
    }

    public void CancellaPartita()
    {
        if (Directory.Exists(Path.Combine
            (Application.persistentDataPath, Statici.nomePersonaggio)))

            Directory.Delete(Path.Combine
            (Application.persistentDataPath, Statici.nomePersonaggio), true);
    }
}