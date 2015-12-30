using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text nomeText;
    public Text valoreVitaText;
    public Text valoreAttaccoText;
    public Text valoreTipoText;
    public Text valoreDifesaText;
    public Slider sliderVita;
    public static string tagEssere = null;
    public static string tagDiColuiCheVuoleCambiareAmicizia = "Player";
    public static Transform signoloEssereT = null;
    public static List<string> nemici = null;
    public static int contatoreDaCambiare = 0;
    public static Dictionary<string, List<string>> dizionarioDiNemici = new Dictionary<string, List<string>>();
    public static Dictionary<string, List<string>> dizionarioDiAmici = new Dictionary<string, List<string>>();
    public static Dictionary<string, List<string>> dizionarioDiIndifferenti = new Dictionary<string, List<string>>();

    private static GameManager me;
    public Serializzabile<AmicizieSerializzabili> datiDiplomazia;
    private Serializzabile<ValoriPersonaggioS> datiPersonaggio;
    private bool fatto = false;
    private float vitaAttuale;
    private float vitaMassima = 0f;

    private string tagDellAltro = null;

    private RaycastHit hit;
    private Collider precedente = null;
    private Collider attuale = null;
    private float ritardo = 0f;
    private string nomeScenaDaCaricare = string.Empty;
    private int numeroScena = 0;

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

  
  
    private void Start()
    {
        me = this;

        //carico diplomazia
        datiDiplomazia = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);

        //carico dati personaggio
        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);

        
        if(GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)")!=null)
             GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)").transform.position = GameObject.Find(datiPersonaggio.Dati.posizioneCheckPoint).transform.position;
        else
            Instantiate(Resources.Load(datiPersonaggio.Dati.nomeModello), GameObject.Find(datiPersonaggio.Dati.posizioneCheckPoint).transform.position, Quaternion.identity);

       
        vitaMassima = datiPersonaggio.Dati.VitaMassima;
        //visualizzo dati personaggio:
        nomeText.text = Statici.nomePersonaggio;
        valoreVitaText.text = datiPersonaggio.Dati.Vita.ToString();
        valoreTipoText.text = datiPersonaggio.Dati.classe.ToString();
        valoreAttaccoText.text = datiPersonaggio.Dati.Attacco.ToString();
        valoreDifesaText.text = datiPersonaggio.Dati.difesa.ToString();
        sliderVita.minValue = 0f;
        sliderVita.maxValue = vitaMassima;
        sliderVita.value = datiPersonaggio.Dati.Vita;
        VitaAttuale = datiPersonaggio.Dati.Vita;

        RecuperaDizionariDiplomazia();
    }

    private void RecuperaDizionariDiplomazia()
    {
        dizionarioDiNemici.Clear();
        dizionarioDiIndifferenti.Clear();
        dizionarioDiAmici.Clear();
        List<string> tmpNemici = null;
        List<string> tmpAmici = null;
        List<string> tmpIndifferenti = null;
        for (int i = 0; i < datiDiplomazia.Dati.tipoEssere.Length; i++)
        {
            tmpNemici = new List<string>();
            tmpIndifferenti = new List<string>();
            tmpAmici = new List<string>();
            for (int j = 0; j < datiDiplomazia.Dati.tipoEssere.Length; j++)
            {
                switch (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j])
                {
                    case Amicizie.Neutro:
                        if (!tmpIndifferenti.Contains(datiDiplomazia.Dati.tipoEssere[j]))
                        {
                            tmpIndifferenti.Add(datiDiplomazia.Dati.tipoEssere[j]);
                        }
                        break;

                    case Amicizie.Alleato:
                        if (!tmpAmici.Contains(datiDiplomazia.Dati.tipoEssere[j]))
                        {
                            tmpAmici.Add(datiDiplomazia.Dati.tipoEssere[j]);
                        }
                        break;

                    case Amicizie.Nemico:
                        if (!tmpNemici.Contains(datiDiplomazia.Dati.tipoEssere[j]))
                        {
                            tmpNemici.Add(datiDiplomazia.Dati.tipoEssere[j]);
                        }
                        break;

                    default:
                        break;
                }
            }
            if (!dizionarioDiNemici.ContainsKey(datiDiplomazia.Dati.tipoEssere[i]))
                dizionarioDiNemici.Add(datiDiplomazia.Dati.tipoEssere[i], tmpNemici);
            if (!dizionarioDiAmici.ContainsKey(datiDiplomazia.Dati.tipoEssere[i]))
                dizionarioDiAmici.Add(datiDiplomazia.Dati.tipoEssere[i], tmpAmici);
            if (!dizionarioDiIndifferenti.ContainsKey(datiDiplomazia.Dati.tipoEssere[i]))
                dizionarioDiIndifferenti.Add(datiDiplomazia.Dati.tipoEssere[i], tmpIndifferenti);
        }
    }

    // Update is called once per frame
    private void Update()
    {

        
        if (Application.loadedLevelName == nomeScenaDaCaricare && !nomeScenaDaCaricare.Equals(string.Empty) && GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)")!=null)//+
        {
            GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)").transform.position = GameObject.Find(datiPersonaggio.Dati.posizioneCheckPoint).transform.position;
            
            nomeScenaDaCaricare = string.Empty;
        }

        if (datiPersonaggio.Dati.Vita != vitaAttuale)
        {
            valoreVitaText.text = VitaAttuale.ToString();
            sliderVita.value = VitaAttuale;
            datiPersonaggio.Dati.Vita = VitaAttuale;
            datiPersonaggio.Salva();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && !EventSystem.current.IsPointerOverGameObject())
            {
               
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

    public void DichiaroGuerra()
    {
        for (int i = 0; i < datiDiplomazia.Dati.tipoEssere.Length; i++)
        {
            if (datiDiplomazia.Dati.tipoEssere[i].Equals(tagDiColuiCheVuoleCambiareAmicizia))
            {
                for (int j = 0; j < datiDiplomazia.Dati.tipoEssere.Length; j++)
                {
                    if (datiDiplomazia.Dati.tipoEssere[j].Equals(tagDellAltro))
                    {
                        if (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] != Amicizie.Nemico)
                        {
                            datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] = Amicizie.Nemico;
                            datiDiplomazia.Dati.matriceAmicizie[j].elementoAmicizia[i] = Amicizie.Nemico;

                            datiDiplomazia.Salva();

                            break;
                        }
                    }
                }
            }
        }
        RecuperaDizionariDiplomazia();
    }

    public void MiAlleo()
    {
        for (int i = 0; i < datiDiplomazia.Dati.tipoEssere.Length; i++)
        {
            if (datiDiplomazia.Dati.tipoEssere[i].Equals(tagDiColuiCheVuoleCambiareAmicizia))
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

                            break;
                        }
                    }
                }
            }
        }
        RecuperaDizionariDiplomazia();
    }

    public void BottoneRiceviDanno()
    {
        VitaAttuale -= 1f;
    }

    public void BottonePozioneVita()
    {
        VitaAttuale += 1f;
    }

    public static void MemorizzaCheckPoint(string nomeCheckPoint)
    {
        me.datiPersonaggio.Dati.posizioneCheckPoint = nomeCheckPoint;
        me.datiPersonaggio.Salva();
    }
    public static void MemorizzaProssimaScena(string nomeScena, string nomeCheck)
    {
        
        me.datiPersonaggio.Dati.posizioneCheckPoint = nomeCheck;
        me.datiPersonaggio.Dati.nomeScena = nomeScena;

      /*  Scene tmpProvanumeroscena = SceneManager.GetSceneByName(nomeScena);
        me.numeroScena = tmpProvanumeroscena.buildIndex;//perchè restituisce -1?mistero della fede...
        me.datiPersonaggio.Dati.nomeScena = nomeScena;*/

        me.datiPersonaggio.Salva();
       
        SceneManager.LoadScene(nomeScena);

        me.nomeScenaDaCaricare = nomeScena;
       
    }

   /* void OnLevelWasLoaded(int level)
    {
        Debug.Log(numeroScena);
        if(level==numeroScena)
        {
            
            GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)").transform.position = GameObject.Find(datiPersonaggio.Dati.posizioneCheckPoint).transform.position;
        }

    }*/


    }