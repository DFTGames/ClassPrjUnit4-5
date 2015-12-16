using UnityEngine;

[ExecuteInEditMode]
public class VisualizzaAmicizie : MonoBehaviour
{
  
    private Serializzabile<AmicizieSerializzabili> datiDiplomazia;
    public string[] nemici;
    public string[] amici;
    public string[] indifferenti;

    private int numeroNemici;
    private int numeroAmici;
    private int numeroIndifferenti;


  
    // Use this for initialization
    void Start()
    {


      
        Visualizza();
    }

    void Update()
    {
        if (GameManager.tagEssere==gameObject.tag)
        {            
            Visualizza();
           
        }
    }

    private void Visualizza()
    {
        datiDiplomazia = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);
        numeroNemici = 0;
        numeroAmici = 0;
        numeroIndifferenti = 0;
       
        for (int i = 0; i < datiDiplomazia.Dati.tipoEssere.Length; i++)
        {
            if (datiDiplomazia.Dati.tipoEssere[i].Equals(gameObject.tag))
            {
                for (int j = 0; j < datiDiplomazia.Dati.tipoEssere.Length; j++)
                {
                    if (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] == Amicizie.Nemico)
                        numeroNemici++;
                    else if (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] == Amicizie.Alleato)
                        numeroAmici++;
                    else if (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] == Amicizie.Neutro)
                        numeroIndifferenti++;
                }
            }
        }

        nemici = new string[numeroNemici];
        amici = new string[numeroAmici];
        indifferenti = new string[numeroIndifferenti];
        int a = 0;
        int b = 0;
        int c = 0;

        for (int i = 0; i < datiDiplomazia.Dati.tipoEssere.Length; i++)
        {
            if (datiDiplomazia.Dati.tipoEssere[i].Equals(gameObject.tag))
            {
                for (int j = 0; j < datiDiplomazia.Dati.tipoEssere.Length; j++)
                {
                    if (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] == Amicizie.Nemico)
                    {
                        nemici[a] = datiDiplomazia.Dati.tipoEssere[j];
                        a++;
                    }
                    else if (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] == Amicizie.Alleato)
                    {
                        amici[b] = datiDiplomazia.Dati.tipoEssere[j];
                        b++;
                    }
                    else if (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] == Amicizie.Neutro)
                    {
                        indifferenti[c] = datiDiplomazia.Dati.tipoEssere[j];
                        c++;
                    }
                }
            }
        }

        
    }
    
}
  
       
