using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

internal enum Sesso
{
    maschio,
    femmina
}

public class ManagerIniziale : MonoBehaviour
{ 
    public Animator animatoreMainMenu;
    public Animator animatoreMenuCreazione;
    public Animator animatoreMenuCarica;
    public Button bottoneCaricaDaMainManu;
    public Button bottoneEliminaPartita;
    public Text casellaTipo;
    public Text valoreVita;
    public Text valoreAttacco;
    public Text valoreDifesa;
    public Text testoNome;
    public Text erroreCreazioneText;
    public Text erroreCaricamento;
    public Text vitaCaricamento;
    public Text attaccoCaricamento;
    public Text difesaCaricamento;
    public GameData databseInizialeAmicizie;
    public caratteristichePersonaggioV2 databaseInizialeProprieta;
    public Dropdown elencoCartelleDropDown;
    public Dropdown elencoSessiDropDown;
    public Transform posizioneInizialeCamera;
    public Image pannelloImmagineSfondo;
    public Text nomeScenaText;
    public Transform posizioneCameraCarica;
    [Range(0f, 20f)]
    public float altezzaCamera = 1f;
    [Range(-20f, 20f)]
    public float ZOffSet;
    public float DampTime;

    private int indiceClasseSuccessivaPrecedente = 0;
    private Serializzabile<ValoriPersonaggioS> datiPersonaggio;
    private Serializzabile<AmicizieSerializzabili> datiDiplomazia;  
    private string scena = string.Empty;
    private GameObject tmpGODaEliminareSeSelezioniAltroGO = null;
    private GameObject tmpGOPrecedente = null;
    private Dictionary<string, Transform> dizionarioPosizioniPrecedenti = new Dictionary<string, Transform>();
    private bool nuovaPartita = false;
    private bool caricaPartita = false;
    private bool personaggiInCarica = false;
    private bool personaggioProvaEsiste = false; 
    private Transform targetT = null;
    private Vector3 velocita = Vector3.zero;
    private Vector3 posizioneCamera;
    private Transform cameraT;
    private Dictionary<string, GameObject> dizionarioCollegamentoNomiConModelli = new Dictionary<string, GameObject>();   

    public int IndiceClasseSuccessivaPrecedente
    {
        get
        {
            return indiceClasseSuccessivaPrecedente;
        }
        set
        {
            int valoreMinimo = 0;
            int valoreMassimo = databaseInizialeProprieta.classePersonaggio.Count - 1;
            indiceClasseSuccessivaPrecedente = Mathf.Clamp(value, valoreMinimo, valoreMassimo);
            if (value > valoreMassimo)
                indiceClasseSuccessivaPrecedente = valoreMinimo;
            if (value < valoreMinimo)
                indiceClasseSuccessivaPrecedente = valoreMassimo;
        }
    }

    // Use this for initialization
    private void Start()
    {     
        nomeScenaText.gameObject.SetActive(false);      
        Statici.Metodo_Charlie(ref databseInizialeAmicizie, ref databaseInizialeProprieta);        
        cameraT = Camera.main.transform;
        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);
        //istanzio tutti i personaggi
        for (int i = 0; i < databaseInizialeProprieta.matriceProprieta.Count; i++)
        {
            string tmpNomeModelloM = databaseInizialeProprieta.matriceProprieta[i].nomeM;
            string tmpNomeModelloF = databaseInizialeProprieta.matriceProprieta[i].nomeF;
            dizionarioCollegamentoNomiConModelli.Add(tmpNomeModelloM,Instantiate(Resources.Load(tmpNomeModelloM), GameObject.Find("postazione" + i).transform.FindChild("posizioneM").position, new Quaternion(0f, 180f, 0f, 0f)) as GameObject);            
            dizionarioCollegamentoNomiConModelli.Add(tmpNomeModelloF,Instantiate(Resources.Load(tmpNomeModelloF), GameObject.Find("postazione" + i).transform.FindChild("posizioneF").position, Quaternion.identity) as GameObject);          
            dizionarioPosizioniPrecedenti.Add(dizionarioCollegamentoNomiConModelli[tmpNomeModelloM].name, GameObject.Find("postazione" + i).transform.FindChild("posizioneM"));
            dizionarioPosizioniPrecedenti.Add(dizionarioCollegamentoNomiConModelli[tmpNomeModelloF].name, GameObject.Find("postazione" + i).transform.FindChild("posizioneF"));          
        }
        Statici.CopiaIlDB();
    }

    public void NuovaPartita()
    {
        animatoreMenuCreazione.SetBool("Torna", true);
        animatoreMainMenu.SetBool("Via", true);
        nuovaPartita = true;
        RecuperaSesso();
    }

    public void CaricaPartita()
    {
        caricaPartita = true;
        animatoreMenuCarica.SetBool("Torna", true);
        animatoreMainMenu.SetBool("Via", true);
        RecuperaElencoCartelle();
        RecuperaDatiGiocatore();
    }

    public void CaricaScenaDaCaricamento()
    {
        Statici.sonoPassatoDallaScenaIniziale = true;    
        Statici.SerializzaPercorsi(ref databseInizialeAmicizie, ref datiDiplomazia, ref GameManager.dizionarioPercorsi);
        /*
        l'if else qui sotto, serve per verificare se la scena in cui vogliamo far spuntare il personaggio
        esiste ancora o no nel build settings. Perchè può capitare di cancellar euna scena o rinominalrla.
        Per evitare di far andar ein errore il gioco, se la scena non esiste più il personaggio viene caricato
        nell'isola altrimenti nell'ultima scena visitata.
        */
        if (!Application.CanStreamedLevelBeLoaded(datiPersonaggio.Dati.nomeScena))
        {
            datiPersonaggio.Dati.nomeScena = "Isola";
            datiPersonaggio.Dati.posizioneCheckPoint = "start";
            datiPersonaggio.Salva();
            SceneManager.LoadScene("Isola");
        }
        else
            SceneManager.LoadScene(datiPersonaggio.Dati.nomeScena);
    }

    public void CaricaPartitaDaCreazione()
    {
        erroreCreazioneText.text = String.Empty;
        if (testoNome.text.Trim() == string.Empty)
        {
            erroreCreazioneText.text = "Inserire un nome";
            return;
        }
        Statici.sonoPassatoDallaScenaIniziale = true;
        DirectoryInfo dI = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] dirs = dI.GetDirectories();
        for (int i = 0; i < dirs.Length; i++)
        {
            if (dirs[i].Name.ToLower() == testoNome.text.ToLower())
            {
                erroreCreazioneText.text = "Esiste Gia Un Personaggio con questo nome";
                return;
            }
        }       
        Statici.nomePersonaggio = testoNome.text;
        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);
        if (datiPersonaggio.Dati.nomePersonaggio == null)
        {
            datiPersonaggio.Dati.Vita = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Vita;
            datiPersonaggio.Dati.Attacco = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Attacco;
            datiPersonaggio.Dati.difesa = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].difesa;
            datiPersonaggio.Dati.Xp = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Xp;
            datiPersonaggio.Dati.Livello = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Livello;
            if (elencoSessiDropDown.value == 0)
                datiPersonaggio.Dati.nomeModello = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeM;
            else
                datiPersonaggio.Dati.nomeModello = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeF;
            datiPersonaggio.Dati.nomePersonaggio = testoNome.text;
            datiPersonaggio.Dati.classe = casellaTipo.text;
            datiPersonaggio.Dati.VitaMassima = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Vita;
            datiPersonaggio.Dati.ManaMassimo = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Mana;
            datiPersonaggio.Dati.XPMassimo = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Xp;
            datiPersonaggio.Dati.posizioneCheckPoint = "start";
            datiPersonaggio.Dati.nomeScena = "Isola";
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
        Statici.SerializzaPercorsi(ref databseInizialeAmicizie, ref datiDiplomazia, ref GameManager.dizionarioPercorsi);
        personaggiInCarica = true;
        SceneManager.LoadScene(datiPersonaggio.Dati.nomeScena);
    }

    public void AnnullaDaCreazione()
    {
        animatoreMenuCreazione.SetBool("Torna", false);
        animatoreMainMenu.SetBool("Via", false);
        nuovaPartita = false;
    }

    public void AnnullaDaCaricamento()
    {
        if (tmpGOPrecedente != null)
        {
            tmpGOPrecedente.transform.position = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].position;
            tmpGOPrecedente.transform.rotation = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].rotation;
        }
        animatoreMenuCarica.SetBool("Torna", false);
        animatoreMainMenu.SetBool("Via", false);
        caricaPartita = false;
    }

    public void Precedente()
    {
        IndiceClasseSuccessivaPrecedente--;
        RecuperaSesso();
    }

    public void Sucessivo()
    {
        IndiceClasseSuccessivaPrecedente++;
        RecuperaSesso();
    }

    // Update is called once per frame
    private void Update()
    {
        casellaTipo.text = databaseInizialeProprieta.classePersonaggio[IndiceClasseSuccessivaPrecedente].ToString();
        valoreVita.text = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Vita.ToString();
        valoreAttacco.text = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Attacco.ToString();
        valoreDifesa.text = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].difesa.ToString();
        //se non ho selezionato ne carica ne nuova partita oppure se sono in carica ma non ho personaggi già creati,
        //inquadro la posizione iniziale.
        if ((!nuovaPartita && !caricaPartita) || (caricaPartita && !personaggiInCarica))
        {
            Image tmpImage = pannelloImmagineSfondo;
            if (tmpImage.color.a < 1f)
                tmpImage.color += new Color(0f, 0f, 0f, 0.2f) * Time.deltaTime * 3f;
            targetT = posizioneInizialeCamera;
            posizioneCamera = new Vector3(targetT.position.x, targetT.position.y + altezzaCamera, targetT.position.z + ZOffSet);
            CambiaPosizioneTelecamera();
        }//inquadro il modello selezionato in carica partita:
        else if (caricaPartita && !nuovaPartita)
        {
            Image tmpImage = pannelloImmagineSfondo;
            if (tmpImage.color.a > 0f)
                tmpImage.color -= new Color(0f, 0f, 0f, 0.1f) * Time.deltaTime * 1.5f;         
            if(dizionarioCollegamentoNomiConModelli.ContainsKey(datiPersonaggio.Dati.nomeModello))
            {
                targetT = dizionarioCollegamentoNomiConModelli[datiPersonaggio.Dati.nomeModello].transform;
                posizioneCamera = new Vector3(targetT.position.x, targetT.position.y + altezzaCamera, targetT.position.z + ZOffSet);
                CambiaPosizioneTelecamera();
            }
        }//se scelgo il maschio in nuova partita lo inquadro:
        else if (elencoSessiDropDown.value == 0 && nuovaPartita && !caricaPartita)
        {
            Image tmpImage = pannelloImmagineSfondo;
            if (tmpImage.color.a > 0f)
                tmpImage.color -= new Color(0f, 0f, 0f, 0.1f) * Time.deltaTime * 1.5f;          
            targetT = dizionarioCollegamentoNomiConModelli[databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeM].transform;
            posizioneCamera = new Vector3(targetT.position.x, targetT.position.y + altezzaCamera, targetT.position.z - ZOffSet);
            CambiaPosizioneTelecamera();
        }//se scelgo la femmina in nuova partita, la inquadro:
        else if (elencoSessiDropDown.value == 1 && nuovaPartita && !caricaPartita)
        {
            Image tmpImage = pannelloImmagineSfondo;
            if (tmpImage.color.a > 0f)
                tmpImage.color -= new Color(0f, 0f, 0f, 0.1f) * Time.deltaTime * 1.5f;
            targetT = dizionarioCollegamentoNomiConModelli[databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeF].transform;
            posizioneCamera = new Vector3(targetT.position.x, targetT.position.y + altezzaCamera, targetT.position.z + ZOffSet);
            CambiaPosizioneTelecamera();
        }
        //mentre sto caricando una nuova scena faccio spuntare un immagine di sfondo e il nome della scena facendo scomparire eventuali pannelli attivi:
        if (Application.isLoadingLevel && pannelloImmagineSfondo.color.a != 1f)
        {
            animatoreMenuCarica.gameObject.SetActive(false);
            animatoreMenuCreazione.gameObject.SetActive(false);
            animatoreMainMenu.gameObject.SetActive(false);
            nomeScenaText.gameObject.SetActive(true);
            nomeScenaText.text = "Loading... " + datiPersonaggio.Dati.nomeScena;
            pannelloImmagineSfondo.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
    }

    private void CambiaPosizioneTelecamera()
    {
        cameraT.position = Vector3.SmoothDamp(cameraT.position, posizioneCamera, ref velocita, DampTime);
        if (ZOffSet != 0f)        
            cameraT.LookAt(targetT.position);        
    }

    public void RecuperaElencoCartelle()
    {
        erroreCaricamento.text = string.Empty;
        Dropdown.OptionData Tmp = null;
        elencoCartelleDropDown.options.Clear();
        DirectoryInfo Di = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] Drs = Di.GetDirectories();
        for (int i = 0; i < Drs.Length; i++)
        {
            Tmp = new Dropdown.OptionData();
            Tmp.text = Drs[i].Name;
            if (Drs[i].Name != "PersonaggioDiProva")
                elencoCartelleDropDown.options.Add(Tmp);
            else
                personaggioProvaEsiste = true;
        }
        int numeroCartelleMinimo = 0;
        if (!personaggioProvaEsiste)
            numeroCartelleMinimo = 0;
        else
            numeroCartelleMinimo = 1;
        if (Drs.Length > numeroCartelleMinimo)
        {
            personaggiInCarica = true;
            bottoneCaricaDaMainManu.interactable = true;
            bottoneEliminaPartita.interactable = true;
            elencoCartelleDropDown.value = -1;    //visualizzo sempre il primo elemento della lista
            Statici.nomePersonaggio = elencoCartelleDropDown.options[elencoCartelleDropDown.value].text;
        }
        else
        {
            personaggiInCarica = false;
            Statici.nomePersonaggio = string.Empty;
            bottoneCaricaDaMainManu.interactable = false;
            bottoneEliminaPartita.interactable = false;
            erroreCaricamento.text = "Non ci sono partite salvate";
            vitaCaricamento.text = "none";
            attaccoCaricamento.text = "none";
            difesaCaricamento.text = "none";
            if (tmpGOPrecedente != null)
            {
                tmpGOPrecedente.transform.position = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].position;
                tmpGOPrecedente.transform.rotation = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].rotation;
            }
            elencoCartelleDropDown.captionText.text = string.Empty;  //NON VISUALIZZA LA STRINGA QUANDO LA LISTA E' VUOTA
        }
    }

    public void RecuperaDatiGiocatore()
    {
        if (elencoCartelleDropDown.options.Count <= 0) return;
        if (nuovaPartita) return;
        Statici.nomePersonaggio = elencoCartelleDropDown.options[elencoCartelleDropDown.value].text;
        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);
        vitaCaricamento.text = datiPersonaggio.Dati.Vita.ToString();
        attaccoCaricamento.text = datiPersonaggio.Dati.Attacco.ToString();
        difesaCaricamento.text = datiPersonaggio.Dati.difesa.ToString();
        if (tmpGOPrecedente != null)
        {
            tmpGOPrecedente.transform.position = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].position;
            tmpGOPrecedente.transform.rotation = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].rotation;
        }
        if (datiPersonaggio == null) return;       
        if(!dizionarioCollegamentoNomiConModelli.ContainsKey(datiPersonaggio.Dati.nomeModello))    
        {
            erroreCaricamento.text = "Errore..Questo personaggio non e' più valido";
            bottoneCaricaDaMainManu.interactable = false;
            return;
        }
        else
        {
            erroreCaricamento.text = "";
            bottoneCaricaDaMainManu.interactable = true;
        }      
        GameObject tmOj = dizionarioCollegamentoNomiConModelli[datiPersonaggio.Dati.nomeModello];
        tmOj.transform.position = posizioneCameraCarica.position;
        tmOj.transform.rotation = posizioneCameraCarica.rotation;        
        tmpGOPrecedente = tmOj;
    }

    public void CancellaPartita()
    {
        if (Directory.Exists(Path.Combine
            (Application.persistentDataPath, Statici.nomePersonaggio)))
        {
            Directory.Delete(Path.Combine
            (Application.persistentDataPath, Statici.nomePersonaggio), true);
            RecuperaElencoCartelle();
        }
    }

    public void RecuperaSesso()
    {
        elencoSessiDropDown.options.Clear();
        for (int i = 0; i < Enum.GetValues(typeof(Sesso)).Length; i++)
        {
            Dropdown.OptionData tmp = new Dropdown.OptionData();
            tmp.text = Enum.GetName(typeof(Sesso), i);
            elencoSessiDropDown.options.Add(tmp);
        }
        elencoCartelleDropDown.value = 0;
    }
}