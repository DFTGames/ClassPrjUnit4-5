using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

 enum TipoMaga
{
    magaRossa,
    magaVerde,
        
}

public class ManagerIniziale : MonoBehaviour {

    public Animator AnimatoreMenu;
    public Animator AltrieMenu1;
    public Animator AltrieMenu2;

    public Text CasellaTipo;
    public Text ValoreVita;
    public Text ValoreAttacco;
    public Text ValoreDifesa;
    public Text TestoNome;

    public GameData dataBaseIniziale;

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

    }

    public void AvviaGioco()
    {
        string[] Cartelle = System.IO.Directory.GetDirectories(Application.persistentDataPath);
        for (int i = 0; i < Cartelle.Length; i++)
        {
            if (Cartelle[i] == TestoNome.text || TestoNome.text == null)
            {
                Debug.Log("esisteGia");
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
        
        Debug.Log(indiceButton);
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

	}

