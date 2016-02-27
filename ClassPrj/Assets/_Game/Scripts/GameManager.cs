using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum NomeClasse
{
    GameManager,
    ManagerIniziale
}

public class GameManager : MonoBehaviour
{
    public static string tagEssere = null;
    public static string tagDiColuiCheVuoleCambiareAmicizia = "Player";
    public static Transform signoloEssereT = null;
    public static List<string> nemici = null;
    public static int contatoreDaCambiare = 0;

    public static Dictionary<string, int> dizionarioPercorsi = new Dictionary<string, int>();

    //fare dizionari indici...stringa e numerico dizionario
    public static Dictionary<string, List<string>> dizionarioDiNemici = new Dictionary<string, List<string>>();

    public static Dictionary<string, List<string>> dizionarioDiAmici = new Dictionary<string, List<string>>();
    public static Dictionary<string, List<string>> dizionarioDiIndifferenti = new Dictionary<string, List<string>>();
    public GameData databseInizialeAmicizie;
    public caratteristichePersonaggioV2 databaseInizialeProprieta;
    public static Transform PersonaggioPrincipaleT;

    private static GameManager me;
    private Serializzabile<AmicizieSerializzabili> datiDiplomazia;
    private Serializzabile<ValoriPersonaggioS> datiPersonaggio;
    private bool fatto = false;
    public static PadreGestore padreGestore;

    private string tagDellAltro = null;

    private RaycastHit hit;
    private Collider precedente = null;
    private Collider attuale = null;

    private string nomeScenaDaCaricare = string.Empty;
    private int numeroScena = 0;
    private float vitaAttuale;
    private float vitaMassima = 0f;
    private float attacco = 0f;
    private float difesa = 0f;
    private string classe = string.Empty;
    private string nome = string.Empty;

    

    public static float VitaAttuale
    {
        get
        {
            return me.vitaAttuale;
        }

        set
        {
            me.vitaAttuale = Mathf.Clamp(value, 0, me.vitaMassima);
        }
    }

    public static float VitaMassima
    {
        get
        {
            return me.vitaMassima;
        }

        set
        {
            me.vitaMassima = value;
        }
    }

    public static float Attacco
    {
        get
        {
            return me.attacco;
        }

        set
        {
            me.attacco = value;
        }
    }

    public static float Difesa
    {
        get
        {
            return me.difesa;
        }

        set
        {
            me.difesa = value;
        }
    }

    public static string Classe
    {
        get
        {
            return me.classe;
        }

        set
        {
            me.classe = value;
        }
    }

    public static string Nome
    {
        get
        {
            return me.nome;
        }

        set
        {
            me.nome = value;
        }
    }

    private void Start()
    {
        me = this;
        GameObject tmpPdr = GameObject.Find("PadrePercorso");
        if (tmpPdr != null)
            padreGestore = tmpPdr.GetComponent<PadreGestore>();
        else
            Debug.LogError("Ma ci fai o ci sei ????..sei un cazzone....manca il padrePercorso");

        if (Statici.sonoPassatoDallaScenaIniziale)//+
        {
            //carico diplomazia
            datiDiplomazia = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);

            //carico dati personaggio
            datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);

            VitaMassima = datiPersonaggio.Dati.VitaMassima;
            VitaAttuale = datiPersonaggio.Dati.Vita;
            Attacco = datiPersonaggio.Dati.Attacco;
            Difesa = datiPersonaggio.Dati.difesa;
            Nome = datiPersonaggio.Dati.nomePersonaggio;
            Classe = datiPersonaggio.Dati.classe;
            GestoreCanvasAltreScene.AggiornaDati();

            if (GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)") != null)
            {
                PersonaggioPrincipaleT = GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)").transform;
                PersonaggioPrincipaleT.position = GameObject.Find(datiPersonaggio.Dati.posizioneCheckPoint).transform.position;
   
            }
            else
            {
                
                GameObject tmpObjP = Instantiate(Resources.Load(datiPersonaggio.Dati.nomeModello), GameObject.Find(datiPersonaggio.Dati.posizioneCheckPoint).transform.position, Quaternion.identity) as GameObject;
                PersonaggioPrincipaleT = tmpObjP.transform;
            }
            RecuperaDizionariDiplomazia();
        }
        else
        {
            //Questo else serve nel caso in cui facciamo play da una scena che non sia quella iniziale
            //verrà così caricato un personaggio per fare le prove.
            //il personaggio verrà caricato sempre nella scena in cui si è fatto play.
            //N.B. se si vuole provare il salvataggio dell'ultima scena bisogna però fare la trafila a partire dalla prima scena
            //perchè questo else caricherà il personaggio solo nella scena in cui viene fatto play.(per maggiori info chiedere a Ninfea)
            Statici.Metodo_Charlie(ref databseInizialeAmicizie, ref databaseInizialeProprieta);
            Statici.nomePersonaggio = "PersonaggioDiProva";

            datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);

            if (datiPersonaggio.Dati.nomePersonaggio == null)
            {
                datiPersonaggio.Dati.Vita = 10;
                datiPersonaggio.Dati.Attacco = 20;
                datiPersonaggio.Dati.difesa = 5;
                datiPersonaggio.Dati.Xp = 19;
                datiPersonaggio.Dati.Livello = 1;

                datiPersonaggio.Dati.nomeModello = "MagoBluM";

                datiPersonaggio.Dati.nomePersonaggio = "PersonaggioDiProva";
                datiPersonaggio.Dati.classe = "Mago";

                datiPersonaggio.Dati.VitaMassima = 10;
                datiPersonaggio.Dati.ManaMassimo = 20;
                datiPersonaggio.Dati.XPMassimo = 100;
                datiPersonaggio.Dati.posizioneCheckPoint = "start";
                datiPersonaggio.Dati.nomeScena = Application.loadedLevelName;
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
            Statici.SerializzaPercorsi(ref databseInizialeAmicizie, ref datiDiplomazia, ref dizionarioPercorsi);
            GameObject tmpObjP = Instantiate(Resources.Load(datiPersonaggio.Dati.nomeModello), GameObject.Find(datiPersonaggio.Dati.posizioneCheckPoint).transform.position, Quaternion.identity) as GameObject;
            PersonaggioPrincipaleT = tmpObjP.transform;
            RecuperaDizionariDiplomazia();
            VitaMassima = datiPersonaggio.Dati.VitaMassima;
            VitaAttuale = datiPersonaggio.Dati.Vita;
            Attacco = datiPersonaggio.Dati.Attacco;
            Difesa = datiPersonaggio.Dati.difesa;
            Nome = datiPersonaggio.Dati.nomePersonaggio;
            Classe = datiPersonaggio.Dati.classe;
            GestoreCanvasAltreScene.AggiornaDati();
            Statici.CopiaIlDB();
            Statici.sonoPassatoDallaScenaIniziale = true;
        }
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
        if (Application.loadedLevelName == nomeScenaDaCaricare && !nomeScenaDaCaricare.Equals(string.Empty) && GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)") != null)//+
        {
            GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)").transform.position = GameObject.Find(datiPersonaggio.Dati.posizioneCheckPoint).transform.position;

            nomeScenaDaCaricare = string.Empty;
        }

        if (datiPersonaggio.Dati.Vita != VitaAttuale)
        {
            me.datiPersonaggio.Dati.Vita = VitaAttuale;
            me.datiPersonaggio.Salva();
            GestoreCanvasAltreScene.AggiornaVita();
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

    public static void DichiaroGuerra()
    {
        for (int i = 0; i < me.datiDiplomazia.Dati.tipoEssere.Length; i++)
        {
            if (me.datiDiplomazia.Dati.tipoEssere[i].Equals(tagDiColuiCheVuoleCambiareAmicizia))
            {
                for (int j = 0; j < me.datiDiplomazia.Dati.tipoEssere.Length; j++)
                {
                    if (me.datiDiplomazia.Dati.tipoEssere[j].Equals(me.tagDellAltro))
                    {
                        if (me.datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] != Amicizie.Nemico)
                        {
                            me.datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] = Amicizie.Nemico;
                            me.datiDiplomazia.Dati.matriceAmicizie[j].elementoAmicizia[i] = Amicizie.Nemico;

                            me.datiDiplomazia.Salva();

                            break;
                        }
                    }
                }
            }
        }
        me.RecuperaDizionariDiplomazia();
    }

    public static void MiAlleo()
    {
        for (int i = 0; i < me.datiDiplomazia.Dati.tipoEssere.Length; i++)
        {
            if (me.datiDiplomazia.Dati.tipoEssere[i].Equals(tagDiColuiCheVuoleCambiareAmicizia))
            {
                for (int j = 0; j < me.datiDiplomazia.Dati.tipoEssere.Length; j++)
                {
                    if (me.datiDiplomazia.Dati.tipoEssere[j].Equals(me.tagDellAltro))
                    {
                        if (me.datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] != Amicizie.Alleato)
                        {
                            me.datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] = Amicizie.Alleato;
                            me.datiDiplomazia.Dati.matriceAmicizie[j].elementoAmicizia[i] = Amicizie.Alleato;

                            me.datiDiplomazia.Salva();

                            break;
                        }
                    }
                }
            }
        }
        me.RecuperaDizionariDiplomazia();
    }

    public static void RiceviDanno()
    {
        VitaAttuale -= 1f;
    }

    public static void PozioneVita()
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

        me.datiPersonaggio.Salva();

        SceneManager.LoadScene(nomeScena);

        me.nomeScenaDaCaricare = nomeScena;
    }
}