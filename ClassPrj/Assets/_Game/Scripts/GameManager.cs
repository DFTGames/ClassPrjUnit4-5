using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{   
    public static string classeDiColuiCheVuoleCambiareAmicizia = string.Empty;   
    public static List<string> nemici = null;        
    public static Dictionary<classiPersonaggi, List<classiPersonaggi>> dizionarioDiNemici = new Dictionary<classiPersonaggi, List<classiPersonaggi>>();
    public static Dictionary<classiPersonaggi, List<classiPersonaggi>> dizionarioDiAmici = new Dictionary<classiPersonaggi, List<classiPersonaggi>>();
    public static Dictionary<classiPersonaggi, List<classiPersonaggi>> dizionarioDiIndifferenti = new Dictionary<classiPersonaggi, List<classiPersonaggi>>();
    public GameData databseInizialeAmicizie;
    public caratteristichePersonaggioV2 databaseInizialeProprieta;
    public static Transform PersonaggioPrincipaleT;
    public static PadreGestore padreGestore;
    public static Dictionary<string, int> dizionarioPercorsi = new Dictionary<string, int>();
    public static DatiPersonaggio personaggio;

    private Dictionary<int, DatiPersonaggio> registroDatiPersonaggi = new Dictionary<int, DatiPersonaggio>();
    private static GameManager me;
    private Serializzabile<AmicizieSerializzabili> datiDiplomazia;
    public static Serializzabile<ValoriPersonaggioS> datiPersonaggio;    
    private string classeEssereSelezionato = string.Empty;
    private RaycastHit hit;
    private Collider precedente = null;
    private Collider attuale = null;
    private Vista vistaGoblin; 
       
    void Awake()
    {
        me = this;
    }
  
    private void Start()
    {
        GameObject tmpPdr = GameObject.Find("PadrePercorso");
        if (tmpPdr != null)
            padreGestore = tmpPdr.GetComponent<PadreGestore>();
        else
            Debug.LogError("Ma ci fai o ci sei ????..sei un cazzone....manca il padrePercorso");
        Statici.assegnaAssetDatabase(ref databseInizialeAmicizie, ref databaseInizialeProprieta);
        FileSerializzatiDelPersonaggio();
        if (Statici.sonoPassatoDallaScenaIniziale)
        {            
            GameObject tmpObjP = Instantiate(Resources.Load(datiPersonaggio.Dati.nomeModello), GameObject.Find(datiPersonaggio.Dati.posizioneCheckPoint).transform.position, Quaternion.identity) as GameObject;
            PersonaggioPrincipaleT = tmpObjP.transform;
            RecuperaDizionariDiplomazia();
        }
        else
        {            
            //Questo else serve nel caso in cui facciamo play da una scena che non sia quella iniziale
            //verrà così caricato un personaggio per fare le prove(il magoBlu).
            //il personaggio verrà caricato sempre nella scena in cui si è fatto play.                   
            if (datiPersonaggio.Dati.nomePersonaggio == null)
            {
                datiPersonaggio.Dati.Vita = 10;
                datiPersonaggio.Dati.Attacco = 20;
                datiPersonaggio.Dati.difesa = 5;
                datiPersonaggio.Dati.Xp = 19;
                datiPersonaggio.Dati.Livello = 1;
                datiPersonaggio.Dati.nomeModello = "MagoBluM";
                datiPersonaggio.Dati.nomePersonaggio = "PersonaggioDiProva";
                datiPersonaggio.Dati.classe =classiPersonaggi.magoAcqua.ToString();
                datiPersonaggio.Dati.VitaMassima = 10;
                datiPersonaggio.Dati.ManaMassimo = 20;
                datiPersonaggio.Dati.XPMassimo = 100;
                datiPersonaggio.Dati.posizioneCheckPoint = "start";             
                datiPersonaggio.Dati.nomeScena = SceneManager.GetActiveScene().name;
                datiPersonaggio.Salva();
            }            
            if (datiDiplomazia.Dati.tipoEssere[0] == null)
            {
                for (int i = 0; i < databseInizialeAmicizie.classiEssere.Length; i++)
                {
                    datiDiplomazia.Dati.tipoEssere[i] = databseInizialeAmicizie.classiEssere[i];
                }
                for (int i = 0; i < databseInizialeAmicizie.classiEssere.Length; i++)
                {
                    datiDiplomazia.Dati.matriceAmicizie[i] = databseInizialeAmicizie.matriceAmicizie[i];

                    for (int j = 0; j < databseInizialeAmicizie.classiEssere.Length; j++)
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
            Statici.CopiaIlDB();
            Statici.sonoPassatoDallaScenaIniziale = true;           
        }       
    }

    public static void RegistraDatiPersonaggio(DatiPersonaggio datiPersonaggio)
    {  
        me.registroDatiPersonaggi.Add(datiPersonaggio.GetInstanceID(), datiPersonaggio);
        RecuperaDati(datiPersonaggio);  
    }

    public static void RecuperaDati(DatiPersonaggio datiStatistici)
    {
        int tmpID = datiStatistici.GetInstanceID();       
        int indice = me.databaseInizialeProprieta.classePersonaggio.IndexOf(me.registroDatiPersonaggi[tmpID].miaClasse.ToString());
        me.registroDatiPersonaggi[tmpID].Giocabile = me.databaseInizialeProprieta.matriceProprieta[indice].giocabile;
        if (!me.registroDatiPersonaggi[tmpID].Giocabile)
        { //se è un personaggio AI recupero i dati dallo scriptble object
            me.registroDatiPersonaggi[tmpID].VitaMassima = me.databaseInizialeProprieta.matriceProprieta[indice].Vita;
            me.registroDatiPersonaggi[tmpID].Vita = me.databaseInizialeProprieta.matriceProprieta[indice].Vita;
            me.registroDatiPersonaggi[tmpID].ManaMassimo = me.databaseInizialeProprieta.matriceProprieta[indice].Mana;
            me.registroDatiPersonaggi[tmpID].Mana = me.databaseInizialeProprieta.matriceProprieta[indice].Mana;      
            me.registroDatiPersonaggi[tmpID].Livello = me.databaseInizialeProprieta.matriceProprieta[indice].Livello;
            me.registroDatiPersonaggi[tmpID].XpMassimo = me.databaseInizialeProprieta.matriceProprieta[indice].Xp;
            me.registroDatiPersonaggi[tmpID].Xp = me.databaseInizialeProprieta.matriceProprieta[indice].Xp;          
            me.registroDatiPersonaggi[tmpID].Attacco = me.databaseInizialeProprieta.matriceProprieta[indice].Attacco;
            me.registroDatiPersonaggi[tmpID].Difesa = me.databaseInizialeProprieta.matriceProprieta[indice].difesa;            
        }
        else
        {  //se è giocabile recupero i dati dal file serializzato
            me.registroDatiPersonaggi[tmpID].VitaMassima = datiPersonaggio.Dati.VitaMassima;
            me.registroDatiPersonaggi[tmpID].Vita = datiPersonaggio.Dati.Vita;
            me.registroDatiPersonaggi[tmpID].ManaMassimo = datiPersonaggio.Dati.ManaMassimo;
            me.registroDatiPersonaggi[tmpID].Mana = datiPersonaggio.Dati.Mana;           
            me.registroDatiPersonaggi[tmpID].Livello = datiPersonaggio.Dati.Livello;       
            me.registroDatiPersonaggi[tmpID].XpMassimo = datiPersonaggio.Dati.XPMassimo;
            me.registroDatiPersonaggi[tmpID].Xp = datiPersonaggio.Dati.Xp;
            me.registroDatiPersonaggi[tmpID].Attacco = datiPersonaggio.Dati.Attacco;
            me.registroDatiPersonaggi[tmpID].Difesa = datiPersonaggio.Dati.difesa;
            me.registroDatiPersonaggi[tmpID].Nome = datiPersonaggio.Dati.nomePersonaggio;
            GestoreCanvasAltreScene.AggiornaDati(datiStatistici);
            personaggio = datiStatistici;
            classeDiColuiCheVuoleCambiareAmicizia = datiStatistici.miaClasse.ToString();
        }  
    }
    private void FileSerializzatiDelPersonaggio()
    {   //N.B.:non invertire le due righe.
        //carico dati personaggio
        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);
        //carico diplomazia
        datiDiplomazia = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);
    }

    private void RecuperaDizionariDiplomazia()
    {
        dizionarioDiNemici.Clear();
        dizionarioDiIndifferenti.Clear();
        dizionarioDiAmici.Clear();
        List<classiPersonaggi> tmpNemici = null;
        List<classiPersonaggi> tmpAmici = null;
        List<classiPersonaggi> tmpIndifferenti = null;
        for (int i = 0; i < datiDiplomazia.Dati.tipoEssere.Length; i++)
        {
            tmpNemici = new List<classiPersonaggi>();
            tmpIndifferenti = new List<classiPersonaggi>();
            tmpAmici = new List<classiPersonaggi>();
            for (int j = 0; j < datiDiplomazia.Dati.tipoEssere.Length; j++)
            {
                switch (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j])
                {
                    case Amicizie.Neutro:
                        if (!tmpIndifferenti.Contains((classiPersonaggi)j))                        
                            tmpIndifferenti.Add((classiPersonaggi)j);                        
                        break;
                    case Amicizie.Alleato:
                        if (!tmpAmici.Contains((classiPersonaggi)j))                        
                            tmpAmici.Add((classiPersonaggi)j);                        
                        break;
                    case Amicizie.Nemico:                      
                        if (!tmpNemici.Contains((classiPersonaggi)j))                        
                            tmpNemici.Add((classiPersonaggi)j);                        
                        break;
                    default:
                        break;
                }
            }
            if (!dizionarioDiNemici.ContainsKey((classiPersonaggi)i))
                dizionarioDiNemici.Add((classiPersonaggi)i, tmpNemici);
            if (!dizionarioDiAmici.ContainsKey((classiPersonaggi)i))
                dizionarioDiAmici.Add((classiPersonaggi)i, tmpAmici);
            if (!dizionarioDiIndifferenti.ContainsKey((classiPersonaggi)i))
                dizionarioDiIndifferenti.Add((classiPersonaggi)i, tmpIndifferenti);
        }     
        if(vistaGoblin!=null)   
           vistaGoblin.AmiciziaCambiata = true;
    }
        

    // Update is called once per frame
    private void Update()
    {       
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && !EventSystem.current.IsPointerOverGameObject())
            {
                attuale = hit.collider;
                DatiPersonaggio tmpDati = hit.collider.GetComponent<DatiPersonaggio>();
                if (tmpDati != null)
                {
                    classeEssereSelezionato = hit.collider.GetComponent<DatiPersonaggio>().miaClasse.ToString();
                    vistaGoblin = hit.collider.GetComponent<Vista>();
                }

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
            if (me.datiDiplomazia.Dati.tipoEssere[i].Equals(classeDiColuiCheVuoleCambiareAmicizia))
            {
                for (int j = 0; j < me.datiDiplomazia.Dati.tipoEssere.Length; j++)
                {
                    if (me.datiDiplomazia.Dati.tipoEssere[j].Equals(me.classeEssereSelezionato))
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
            if (me.datiDiplomazia.Dati.tipoEssere[i].Equals(classeDiColuiCheVuoleCambiareAmicizia))
            {
                for (int j = 0; j < me.datiDiplomazia.Dati.tipoEssere.Length; j++)
                {
                    if (me.datiDiplomazia.Dati.tipoEssere[j].Equals(me.classeEssereSelezionato))
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
  
    public static void MemorizzaCheckPoint(string nomeCheckPoint)
    {
        datiPersonaggio.Dati.posizioneCheckPoint = nomeCheckPoint;
        datiPersonaggio.Salva();
    }

    public static void MemorizzaProssimaScena(string nomeScena, string nomeCheck)
    {
        datiPersonaggio.Dati.posizioneCheckPoint = nomeCheck;
        datiPersonaggio.Dati.nomeScena = nomeScena;
        datiPersonaggio.Salva();
        me.StartCoroutine(GestoreCanvasAltreScene.ScenaInCarica(nomeScena));
    }
}