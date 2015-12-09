using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;

 enum TipoMaga
{
    magaRossa,
    magaVerde,
        
}

public class ManagerIniziale : MonoBehaviour {

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

    public GameData dataBaseIniziale;
    public Dropdown ElencoCartelle;
  

    private int indiceButton = 0;
    private Serializzabile<ValoriPersonaggioS> DatiPersonaggio;
    //per il momento
    private float VitaMagaRossa = 5f;
    private float AttaccoMagaRossa = 5f;
    private float DifesaMagaRossa = 2f;

    private float VitaMagaVerde = 10f;
    private float AttaccoMagaVerde = 3f;
    private float DifesaMagaVerde = 2f;
    private Serializzabile<AmicizieSerializzabili> datiDiplomazia;
    private static ManagerIniziale  Me;

    public int IndiceButton
    {
        get
        {
            return indiceButton;
        }

        set
        {
            int valoreMinimo = 0;
            int valoreMassimo = Enum.GetValues(typeof(TipoMaga)).Length - 1;
            indiceButton = Mathf.Clamp(value, valoreMinimo, valoreMassimo);
            if (value > valoreMassimo)
                indiceButton = valoreMinimo;
            if (value < valoreMinimo)
                indiceButton = valoreMassimo;
        }
    }

   


    // Use this for initialization
    void Start () {
        Me = this;
        CasellaTipo.text = TipoMaga.magaRossa.ToString();
       
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

    }

    public void  CaricaScena()
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
        DatiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);
        Debug.Log(DatiPersonaggio.Dati);
        if (DatiPersonaggio.Dati.Nome == null )
        {
            
           if (IndiceButton == 0)
            {
                DatiPersonaggio.Dati.Vita = VitaMagaRossa;
                DatiPersonaggio.Dati.Attacco = AttaccoMagaRossa;
                DatiPersonaggio.Dati.Difesa = DifesaMagaRossa;
              
                 
            }
            else
            {
                DatiPersonaggio.Dati.Vita = VitaMagaVerde;
                DatiPersonaggio.Dati.Attacco = AttaccoMagaVerde;
                DatiPersonaggio.Dati.Difesa = DifesaMagaVerde;
                

            }
            DatiPersonaggio.Dati.Nome = TestoNome.text;
            DatiPersonaggio.Dati.TipoMaga = CasellaTipo.text;
            DatiPersonaggio.Salva();   

        }
        
        datiDiplomazia = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);
        if (datiDiplomazia.Dati.tipoEssere[0] == null)
        {
            for (int i = 0; i < dataBaseIniziale.tagEssere.Length; i++)
            {
                datiDiplomazia.Dati.tipoEssere[i] = dataBaseIniziale.tagEssere[i];
            }
            for (int i = 0; i < dataBaseIniziale.tagEssere.Length; i++)
            {
                datiDiplomazia.Dati.matriceAmicizie[i] = dataBaseIniziale.matriceAmicizie[i];

                for (int j = 0; j < dataBaseIniziale.tagEssere.Length; j++)
                {
                    datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] = dataBaseIniziale.matriceAmicizie[i].elementoAmicizia[j];
                }
            }

            datiDiplomazia.Salva();
        }
        Application.LoadLevel(1);
    }

    public void Anulla1 ()
    {
        AltrieMenu1.SetBool("Torna", false);
        AnimatoreMenu.SetBool("Via", false);
    }
    public void Anulla2()
    {
        AltrieMenu2.SetBool("Torna",false);
        AnimatoreMenu.SetBool("Via", false);
    }

    public void Precedente ()
    {
        IndiceButton--;
    }

    public void Sucessivo()
    {

        IndiceButton++;
       
    }


    // Update is called once per frame
    void Update () {
        
       
        CasellaTipo.text = Enum.GetValues
            (typeof(TipoMaga)).GetValue(IndiceButton).ToString();

        if (IndiceButton == 0)
        {
            // per ora
            ValoreVita.text = VitaMagaRossa.ToString();
            ValoreAttacco.text = AttaccoMagaRossa.ToString();
            ValoreDifesa.text = DifesaMagaRossa.ToString();          
        }
        else
        {
            ValoreVita.text = VitaMagaVerde.ToString();
            ValoreAttacco.text = AttaccoMagaVerde.ToString();
            ValoreDifesa.text = DifesaMagaVerde.ToString();
        }
           }      
    public void RecuperaElencoCartelle()
    {
        ErroreCaricamento.text = string.Empty;
        Dropdown.OptionData Tmp = null;
        ElencoCartelle.options.Clear();
        DirectoryInfo Di = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] Drs = Di.GetDirectories();
        for ( int i = 0; i < Drs.Length; i++)
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
            if (ElencoCartelle.value >= 0)
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

        DatiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);

        VitaAttuale.text = DatiPersonaggio.Dati.Vita.ToString();
        AttaccoAtuale.text = DatiPersonaggio.Dati.Attacco.ToString();
        DifesaAttuale.text = DatiPersonaggio.Dati.Difesa.ToString();
    }

    public void CancellaPartita ()
    {
        if (Directory.Exists(Path.Combine
            (Application.persistentDataPath, Statici.nomePersonaggio)))

            Directory.Delete(Path.Combine
            (Application.persistentDataPath, Statici.nomePersonaggio),true);

    }
	}

