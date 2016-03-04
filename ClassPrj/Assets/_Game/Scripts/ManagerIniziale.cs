using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagerIniziale : MonoBehaviour
{
    public Animator animatoreMainMenu;
    public Animator animatoreMenuCreazione;
    public Animator animatoreMenuCarica;
    public Button bottoneCaricaDaMainManu;
    public Button bottoneCaricaDaCaricamento;
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
    public Image pannelloImmagineSfondo;
    public Text nomeScenaText;
    public Transform posizioneCameraCarica;
    [Range(0f, 20f)]
    public float altezzaCamera = 1f;
    [Range(-20f, 20f)]
    public float ZOffSet;

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
    private Vector3 targetT;
    private Vector3 velocita = Vector3.zero;
    private Vector3 posizioneCamera;
    private Transform cameraT;
    private Dictionary<string, GameObject> dizionarioCollegamentoNomiConModelli = new Dictionary<string, GameObject>();
    private float fromValue;
    private Vector3[] percorso;
    private Vector3 obiettivoDaInquadrare;
    private bool indiceCambiato = false; //mi serve per capire se sono passato da una classe ad un'altra

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
        fromValue = 0f;
        CambiaAlphaPannelloSfondo(1f);
        nomeScenaText.gameObject.SetActive(false);
        Statici.assegnaAssetDatabase(ref databseInizialeAmicizie, ref databaseInizialeProprieta);
        cameraT = Camera.main.transform;      
        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);
        //istanzio tutti i personaggi
        for (int i = 0; i < databaseInizialeProprieta.matriceProprieta.Count; i++)
        {
            string tmpNomeModelloM = databaseInizialeProprieta.matriceProprieta[i].nomeM;
            string tmpNomeModelloF = databaseInizialeProprieta.matriceProprieta[i].nomeF;
            dizionarioCollegamentoNomiConModelli.Add(tmpNomeModelloM, Instantiate(Resources.Load(tmpNomeModelloM), GameObject.Find("postazione" + i).transform.FindChild("posizioneM").position, new Quaternion(0f, 180f, 0f, 0f)) as GameObject);
            dizionarioCollegamentoNomiConModelli.Add(tmpNomeModelloF, Instantiate(Resources.Load(tmpNomeModelloF), GameObject.Find("postazione" + i).transform.FindChild("posizioneF").position, Quaternion.identity) as GameObject);
            dizionarioPosizioniPrecedenti.Add(dizionarioCollegamentoNomiConModelli[tmpNomeModelloM].name, GameObject.Find("postazione" + i).transform.FindChild("posizioneM"));
            dizionarioPosizioniPrecedenti.Add(dizionarioCollegamentoNomiConModelli[tmpNomeModelloF].name, GameObject.Find("postazione" + i).transform.FindChild("posizioneF"));
        }
        Statici.CopiaIlDB();
    }

    private void CambiaAlphaPannelloSfondo(float alpha)
    {
        iTween.ValueTo(pannelloImmagineSfondo.gameObject, iTween.Hash("from", fromValue, "to", alpha, "time", 3.0f, "easetype",
             iTween.EaseType.easeOutCirc, "onupdatetarget", gameObject, "onupdate", "OnTweenUpdate", "onupdateparams", fromValue));
    }

    public void OnTweenUpdate(float newValue)
    {
        pannelloImmagineSfondo.color = new Color(1f, 1f, 1f, newValue);
    }

    public void NuovaPartita()
    {
        animatoreMenuCreazione.SetBool("Torna", true);
        animatoreMainMenu.SetBool("Via", true);
        nuovaPartita = true;
        VisualizzaValoriPersonaggio();
        fromValue = 1f;
        CambiaAlphaPannelloSfondo(0f);
        //se premo nuova partita inquadro il personaggio deciso dall'indice attuale(classe) e dal sesso:
        if (elencoSessiDropDown.value == 0)
        {
            obiettivoDaInquadrare = dizionarioCollegamentoNomiConModelli[databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeM].transform.position; ;
            cameraT.position = new Vector3(obiettivoDaInquadrare.x, obiettivoDaInquadrare.y + altezzaCamera, obiettivoDaInquadrare.z - ZOffSet);
        }
        else
        {
            obiettivoDaInquadrare = dizionarioCollegamentoNomiConModelli[databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeF].transform.position;
            cameraT.position = new Vector3(obiettivoDaInquadrare.x, obiettivoDaInquadrare.y + altezzaCamera, obiettivoDaInquadrare.z + ZOffSet);
        }
        iTween.LookTo(cameraT.gameObject, iTween.Hash("looktarget", obiettivoDaInquadrare, "time", 0f, "axis", "y", "easetype", iTween.EaseType.linear));
    }

    public void CaricaPartita()
    {
        caricaPartita = true;
        animatoreMenuCarica.SetBool("Torna", true);
        animatoreMainMenu.SetBool("Via", true);
        RecuperaElencoCartelle();
        RecuperaDatiGiocatore();
        fromValue = 1f;
        CambiaAlphaPannelloSfondo(0f);
        //se premo carica partita inquadro la posizione di caricamento del personaggio:
        cameraT.position = new Vector3(posizioneCameraCarica.transform.position.x, posizioneCameraCarica.transform.position.y + altezzaCamera, posizioneCameraCarica.transform.position.z + ZOffSet);
        iTween.LookTo(cameraT.gameObject, iTween.Hash("looktarget", posizioneCameraCarica.transform, "time", 0f, "axis", "y", "easetype", iTween.EaseType.linear));
    }

    public void CaricaScenaDaCaricamento()
    {
        Statici.sonoPassatoDallaScenaIniziale = true;
        Statici.SerializzaPercorsi(ref databseInizialeAmicizie, ref datiDiplomazia, ref GameManager.dizionarioPercorsi);
        StartCoroutine(ScenaInCaricamento(datiPersonaggio.Dati.nomeScena));
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
        StartCoroutine(ScenaInCaricamento(datiPersonaggio.Dati.nomeScena));
    }

    public void AnnullaDaCreazione()
    {
        animatoreMenuCreazione.SetBool("Torna", false);
        animatoreMainMenu.SetBool("Via", false);
        nuovaPartita = false;
        ResettaValoriPerAnnullamento();
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
        ResettaValoriPerAnnullamento();
    }

    private void ResettaValoriPerAnnullamento()
    {      
        fromValue = 0f;
        CambiaAlphaPannelloSfondo(1f);
    }

    public void Precedente()
    {
        IndiceClasseSuccessivaPrecedente--;
        VisualizzaValoriPersonaggio();
        DecidiPercorsoSeCambiClasse();
    }

    public void Sucessivo()
    {
        IndiceClasseSuccessivaPrecedente++;
        VisualizzaValoriPersonaggio();
        DecidiPercorsoSeCambiClasse();
    }

    /// <summary>
    /// decido il percorso della telecamera se devo passare da una classe ad un'altra:
    /// </summary>
    private void DecidiPercorsoSeCambiClasse()
    {
        if (elencoSessiDropDown.value == 0)
        {
            obiettivoDaInquadrare = dizionarioCollegamentoNomiConModelli[databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeM].transform.position; ;
            percorso = new Vector3[] { cameraT.position, new Vector3(obiettivoDaInquadrare.x, obiettivoDaInquadrare.y + altezzaCamera, obiettivoDaInquadrare.z - ZOffSet) };
        }
        else
        {
            obiettivoDaInquadrare = dizionarioCollegamentoNomiConModelli[databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeF].transform.position;
            percorso = new Vector3[] { cameraT.position, new Vector3(obiettivoDaInquadrare.x, obiettivoDaInquadrare.y + altezzaCamera, obiettivoDaInquadrare.z + ZOffSet) };
        }
        indiceCambiato = true;
        MuoviCameraSuPgNuovo();
    }

    public void MuoviCameraSuPgNuovo()
    {
        if (!indiceCambiato)
        {//decido il percorso se l'indice non è cambiato cioè se passo da un sesso all'altro senza cambiare classe:
            if (elencoSessiDropDown.value == 0)
            {
                obiettivoDaInquadrare = dizionarioCollegamentoNomiConModelli[databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeM].transform.position;
                percorso = new Vector3[] { cameraT.position, new Vector3(obiettivoDaInquadrare.x + 5, obiettivoDaInquadrare.y + altezzaCamera, obiettivoDaInquadrare.z - ZOffSet), new Vector3(obiettivoDaInquadrare.x, obiettivoDaInquadrare.y + altezzaCamera, obiettivoDaInquadrare.z - ZOffSet) };
            }
            else
            {
                obiettivoDaInquadrare = dizionarioCollegamentoNomiConModelli[databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeF].transform.position;
                percorso = new Vector3[] { cameraT.position, new Vector3(obiettivoDaInquadrare.x - 5, obiettivoDaInquadrare.y + altezzaCamera, obiettivoDaInquadrare.z + ZOffSet), new Vector3(obiettivoDaInquadrare.x, obiettivoDaInquadrare.y + altezzaCamera, obiettivoDaInquadrare.z + ZOffSet) };
            }
        }
        //muovo la telecamera verso l'obiettivo:
        iTween.MoveTo(cameraT.gameObject, iTween.Hash("path", percorso, "time", 2f, "looktarget", obiettivoDaInquadrare, "looktime", 0f, "axis", "y", "easetype", iTween.EaseType.linear, "oncompletetarget", gameObject, "oncomplete", "ResettaIndiceCambiato"));
    }

    private void ResettaIndiceCambiato()
    {
        indiceCambiato = false;
    }   

    // Update is called once per frame
    private void Update()
    {
    }

    private void VisualizzaValoriPersonaggio()
    {
        casellaTipo.text = databaseInizialeProprieta.classePersonaggio[IndiceClasseSuccessivaPrecedente].ToString();
        valoreVita.text = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Vita.ToString();
        valoreAttacco.text = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Attacco.ToString();
        valoreDifesa.text = databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].difesa.ToString();
    }

    private IEnumerator ScenaInCaricamento(string nomeScena)
    {
        AsyncOperation asynCaricamentoScena = SceneManager.LoadSceneAsync(nomeScena);
        if (!asynCaricamentoScena.isDone && pannelloImmagineSfondo.color.a != 1f)
        {
            animatoreMenuCarica.gameObject.SetActive(false);
            animatoreMenuCreazione.gameObject.SetActive(false);
            animatoreMainMenu.gameObject.SetActive(false);
            nomeScenaText.gameObject.SetActive(true);
            nomeScenaText.text = "Loading... " + datiPersonaggio.Dati.nomeScena;
            pannelloImmagineSfondo.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
        yield return null;
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
        if (!dizionarioCollegamentoNomiConModelli.ContainsKey(datiPersonaggio.Dati.nomeModello))
        {
            erroreCaricamento.text = "Errore..Questo personaggio non e' più valido";
            bottoneCaricaDaCaricamento.interactable = false;
            return;
        }
        else
        {
            erroreCaricamento.text = "";
            bottoneCaricaDaCaricamento.interactable = true;
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
}