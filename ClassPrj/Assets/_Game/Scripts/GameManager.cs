using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

 
    public Text nomeText;
    public Text valoreVitaText;
    public Text valoreAttaccoText;
    public Text valoreTipoText;
    public Text valoreDifesaText;
    public Slider sliderVita;
    

    private static GameManager me;
    private Serializzabile<AmicizieSerializzabili> datiDiplomazia;
    private Serializzabile<ValoriPersonaggioS> datiPersonaggio;
    private bool fatto = false;
    private float vitaAttuale;
    private float vitaMassima=0f;

    public static string tagEssere = null;
    private string tagDellAltro = null;
    float ritardo = 0f;
    private RaycastHit hit;
    private Collider precedente = null;
    private Collider attuale = null;

    public float VitaAttuale
    {
        get
        {
            return vitaAttuale;
        }

        set
        {
            vitaAttuale = Mathf.Clamp(value, 0, vitaMassima);
          
        }
    }

    void Start()
    {
        me = this;

       

        //carico diplomazia
        datiDiplomazia = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);


        //carico dati personaggio
        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);

        vitaMassima = datiPersonaggio.Dati.VitaMassima;
        //visualizzo dati personaggio:
        nomeText.text = Statici.nomePersonaggio;
        valoreVitaText.text = datiPersonaggio.Dati.Vita.ToString();
        valoreTipoText.text = datiPersonaggio.Dati.classe.ToString();
        valoreAttaccoText.text = datiPersonaggio.Dati.Attacco.ToString();
        valoreDifesaText.text = datiPersonaggio.Dati.difesa.ToString();
        sliderVita.minValue = 0f;
        sliderVita.maxValue= vitaMassima;
        sliderVita.value = datiPersonaggio.Dati.Vita;
        VitaAttuale = datiPersonaggio.Dati.Vita;
     
        }

  
    // Update is called once per frame
    void Update()
    {
       if (datiPersonaggio.Dati.Vita != vitaAttuale)
        {
           
            valoreVitaText.text = VitaAttuale.ToString();
            sliderVita.value = VitaAttuale;                    
            datiPersonaggio.Dati.Vita = VitaAttuale;
            datiPersonaggio.Salva();
        }
        
        if (tagEssere != null)
        {
            ritardo +=Time.deltaTime;
            if (ritardo > 3f)
            {
                ritardo = 0f;
                tagEssere = null;
            }            
        }

           if (Input.GetMouseButtonDown(0))
           {
             if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
           {

              
                   Debug.Log(hit.collider.tag);
                   attuale = hit.collider;
                   tagDellAltro = hit.collider.tag;
                   if (precedente != attuale)
                   {
                       if (precedente != null && precedente.transform.FindChild("quadDiSelezione") && precedente.transform.FindChild("quadDiSelezione").gameObject.activeInHierarchy)
                           precedente.transform.FindChild("quadDiSelezione").gameObject.SetActive(false);
                       precedente = attuale;
                   }
                   if (attuale.transform.FindChild("quadDiSelezione"))
                       attuale.transform.FindChild("quadDiSelezione").gameObject.SetActive(true);

               

           } 
           }


    }



    public void BottoneDichiaroGuerra()
    {
        for (int i = 0; i < datiDiplomazia.Dati.tipoEssere.Length; i++)
        {
            if (datiDiplomazia.Dati.tipoEssere[i].Equals("Player"))
            {
                for (int j = 0; j < datiDiplomazia.Dati.tipoEssere.Length; j++)
                {
                    if (datiDiplomazia.Dati.tipoEssere[j].Equals(tagDellAltro))
                    {
                        if (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] != Amicizie.Nemico)
                        {
                            datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] =Amicizie.Nemico;
                            datiDiplomazia.Dati.matriceAmicizie[j].elementoAmicizia[i] = Amicizie.Nemico;

                            datiDiplomazia.Salva();
                            
                            tagEssere = tagDellAltro;
                         
                            break;
                        }
                    }
                  
                }
            }
        }
    }
    public void BottoneMiAlleo()
    {
        for (int i = 0; i < datiDiplomazia.Dati.tipoEssere.Length; i++)
        {
            if (datiDiplomazia.Dati.tipoEssere[i].Equals("Player"))
            {
                for (int j = 0; j < datiDiplomazia.Dati.tipoEssere.Length; j++)
                {
                    if (datiDiplomazia.Dati.tipoEssere[j].Equals(tagDellAltro))
                    {
                        if (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] != Amicizie.Alleato)
                        {
                            datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] = Amicizie.Alleato;
                            datiDiplomazia.Dati.matriceAmicizie[j].elementoAmicizia[i] = Amicizie.Alleato;

                            datiDiplomazia.Salva();

                            tagEssere = tagDellAltro;

                            
                            break;
                        }
                    }

                }
            }
        }
    }

    public void BottoneRiceviDanno()
    {
        VitaAttuale -= 1f;
        
    }

    public void BottonePozioneVita()
    {
        VitaAttuale += 1f;
    }
}