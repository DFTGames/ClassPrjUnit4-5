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
    public Animator animatoreMultiplayer;
    public Button bottoneCaricaDaMainManu;
    public Button bottoneCaricaDaCaricamento;
    public Button bottoneEliminaPartita;
    public Slider volumiSFX;
    public Slider volumiAmbiente;
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
    public Dropdown elencoCartelleDropDown;
    public Dropdown elencoSessiDropDown;
    public Image pannelloImmagineSfondo;
    public Text nomeScenaText;
    public Transform posizionePersonaggioCarica;
    public Transform posizioneCameraCarica;
    [Range(0f, 20f)]
    public float altezzaCamera = 1f;
    [Range(-20f, 20f)]
    public float ZOffSet;
    public CanvasGroup canvasGroupCreazione;
    public CanvasGroup canvasGroupCarica;

    private Serializzabile<ClasseAudio> datiAudio;
    private static ManagerIniziale me;
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
    private float zeta;
    private float ics;
    private float alpha;
    private List<string> cartelleLocali = new List<string>();
    private int contatoreGiocabili = 0;


    FMOD.Studio.Bus SFXBus;
    FMOD.Studio.Bus EnviromentBus;

    public int IndiceClasseSuccessivaPrecedente
    {
        get
        {
            return indiceClasseSuccessivaPrecedente;
        }
        set
        {
            int valoreMinimo = 0;
            int valoreMassimo = Statici.databaseInizialeProprieta.classePersonaggio.Count - 1;
            indiceClasseSuccessivaPrecedente = Mathf.Clamp(value, valoreMinimo, valoreMassimo);
            if (value > valoreMassimo)
                indiceClasseSuccessivaPrecedente = valoreMinimo;
            if (value < valoreMinimo)
                indiceClasseSuccessivaPrecedente = valoreMassimo;
        }
    }

    public void cambiaSfx(float p)
    {
        SFXBus.setFaderLevel(p);
        datiAudio.Dati.volSFX = p;
        datiAudio.Salva();
    }
    public void cambiaAmbiente(float p)
    {
        EnviromentBus.setFaderLevel(p);
        datiAudio.Dati.volEnvironment = p;
        datiAudio.Salva();
    }


    private void Start()
    {
        me = this;
        CambiaAlphaPannelloSfondo();
        nomeScenaText.gameObject.SetActive(false);
        Statici.assegnaAssetDatabase();
        cameraT = Camera.main.transform;
        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);
        for (int i = 0; i < Statici.databaseInizialeProprieta.matriceProprieta.Count; i++)
        {
            if (!Statici.databaseInizialeProprieta.matriceProprieta[i].giocabile)
                continue;
            string tmpNomeModelloM = Statici.databaseInizialeProprieta.matriceProprieta[i].nomeM;
            string tmpNomeModelloF = Statici.databaseInizialeProprieta.matriceProprieta[i].nomeF;
            dizionarioCollegamentoNomiConModelli.Add(tmpNomeModelloM, Instantiate(Resources.Load(tmpNomeModelloM), GameObject.Find("postazione" + contatoreGiocabili).transform.FindChild("posizioneM").position, new Quaternion(0f, 180f, 0f, 0f)) as GameObject);
            dizionarioCollegamentoNomiConModelli.Add(tmpNomeModelloF, Instantiate(Resources.Load(tmpNomeModelloF), GameObject.Find("postazione" + contatoreGiocabili).transform.FindChild("posizioneF").position, Quaternion.identity) as GameObject);
            dizionarioPosizioniPrecedenti.Add(dizionarioCollegamentoNomiConModelli[tmpNomeModelloM].name, GameObject.Find("postazione" + contatoreGiocabili).transform.FindChild("posizioneM"));
            dizionarioPosizioniPrecedenti.Add(dizionarioCollegamentoNomiConModelli[tmpNomeModelloF].name, GameObject.Find("postazione" + contatoreGiocabili).transform.FindChild("posizioneF"));
            contatoreGiocabili += 1;
        }
        Statici.CopiaIlDB();
        DirectoryInfo dI = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] dirs = dI.GetDirectories();
        for (int i = 0; i < dirs.Length; i++)
        {
            cartelleLocali.Add(dirs[i].Name);
        }

        SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        EnviromentBus = FMODUnity.RuntimeManager.GetBus("bus:/Environment");

        datiAudio = new Serializzabile<ClasseAudio>(Statici.NomeFileAudio, true);
        if (!datiAudio.Dati.inizializzato)
        {
            SFXBus.getFaderLevel(out datiAudio.Dati.volSFX);
            EnviromentBus.getFaderLevel(out datiAudio.Dati.volEnvironment);
            datiAudio.Dati.inizializzato = true;
            datiAudio.Salva();
        }
        else
        {
            SFXBus.setFaderLevel(datiAudio.Dati.volSFX);
            EnviromentBus.setFaderLevel(datiAudio.Dati.volEnvironment);
        }

        volumiAmbiente.value = datiAudio.Dati.volEnvironment;
        volumiSFX.value = datiAudio.Dati.volSFX;

    }

    public void NuovaPartita()
    {
        animatoreMenuCreazione.SetBool("Torna", true);
        animatoreMainMenu.SetBool("Via", true);
        nuovaPartita = true;
        erroreCreazioneText.text = string.Empty;
        VisualizzaValoriPersonaggio();
        CambiaAlphaPannelloSfondo();
        ObiettivoDaInquadrareXZ();
        cameraT.position = new Vector3(obiettivoDaInquadrare.x, obiettivoDaInquadrare.y + altezzaCamera, zeta);
        iTween.LookTo(cameraT.gameObject, iTween.Hash("looktarget", obiettivoDaInquadrare, "time", 0f, "axis", "y", "easetype", iTween.EaseType.linear));

    }



    public void CaricaPartita()
    {
        caricaPartita = true;
        animatoreMenuCarica.SetBool("Torna", true);
        animatoreMainMenu.SetBool("Via", true);
        RecuperaElencoCartelle();
        RecuperaDatiGiocatore();
        CambiaAlphaPannelloSfondo();
        cameraT.position = new Vector3(posizioneCameraCarica.transform.position.x, posizioneCameraCarica.transform.position.y + altezzaCamera, posizioneCameraCarica.transform.position.z + ZOffSet);
        cameraT.rotation = posizioneCameraCarica.rotation;
    }

    public void CaricaScenaDaCaricamento()
    {
        canvasGroupCreazione.interactable = false;
        canvasGroupCarica.interactable = false;
        Statici.sonoPassatoDallaScenaIniziale = true;
        if (!Statici.multigiocatoreOn)//SOLO SINGLEPLAYER 
            StartCoroutine(ScenaInCaricamento(datiPersonaggio.Dati.nomeScena));
        else//solo multiplayer
            ScenaInizialeNetwork.VaiAlleStanze();
      
    }

    public void CaricaPartitaDaCreazione()
    {
       
        if (testoNome.text.Trim() == string.Empty)
            erroreCreazioneText.text = "Inserire un nome";
        else
        {
            erroreCreazioneText.text = string.Empty;
            if (cartelleLocali.Contains(testoNome.text.Trim()))
                erroreCreazioneText.text = "Esiste Gia Un Personaggio con questo nome";
            else
            {
                canvasGroupCreazione.interactable = false;
                canvasGroupCarica.interactable = false;
                erroreCaricamento.text = string.Empty;
                Statici.sonoPassatoDallaScenaIniziale = true;
                Statici.nomePersonaggio = testoNome.text;
                datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);
                if (datiPersonaggio.Dati.nomePersonaggio == null)
                {
                    datiPersonaggio.Dati.Vita = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Vita;
                    datiPersonaggio.Dati.Attacco = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Attacco;
                    datiPersonaggio.Dati.difesa = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].difesa;
                    datiPersonaggio.Dati.Xp = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Xp;
                    datiPersonaggio.Dati.Livello = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Livello;
                    if (elencoSessiDropDown.value == 0)
                        datiPersonaggio.Dati.nomeModello = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeM;
                    else
                        datiPersonaggio.Dati.nomeModello = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeF;
                    datiPersonaggio.Dati.nomePersonaggio = testoNome.text;
                    datiPersonaggio.Dati.classe = casellaTipo.text;
                    datiPersonaggio.Dati.VitaMassima = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Vita;
                    datiPersonaggio.Dati.ManaMassimo = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Mana;
                    datiPersonaggio.Dati.XPMassimo = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Xp;
                    datiPersonaggio.Dati.posizioneCheckPoint = "start";
                    datiPersonaggio.Dati.nomeScena = "Isola";
                    datiPersonaggio.Salva();
                }
                datiDiplomazia = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);
                if (datiDiplomazia.Dati.tipoEssere[0] == null)
                {
                    for (int i = 0; i < Statici.databseInizialeAmicizie.classiEssere.Length; i++)
                    {
                        datiDiplomazia.Dati.tipoEssere[i] = Statici.databseInizialeAmicizie.classiEssere[i];
                    }
                    for (int i = 0; i < Statici.databseInizialeAmicizie.classiEssere.Length; i++)
                    {
                        datiDiplomazia.Dati.matriceAmicizie[i] = Statici.databseInizialeAmicizie.matriceAmicizie[i];

                        for (int j = 0; j < Statici.databseInizialeAmicizie.classiEssere.Length; j++)
                        {
                            datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] = Statici.databseInizialeAmicizie.matriceAmicizie[i].elementoAmicizia[j];
                        }
                    }
                    datiDiplomazia.Salva();
                }
                if (!Statici.multigiocatoreOn)//SOLO SINGLEPLAYER
                    StartCoroutine(ScenaInCaricamento(datiPersonaggio.Dati.nomeScena));
                else//solo multiplayer
                    ScenaInizialeNetwork.VaiAlleStanze();
            }
        }
    }

    public void AnnullaDaCreazione()
    {
        animatoreMenuCreazione.SetBool("Torna", false);
        animatoreMainMenu.SetBool("Via", false);
        nuovaPartita = false;
        CambiaAlphaPannelloSfondo();
    }

    public void AnnullaDaNetwork()
    {
        animatoreMultiplayer.SetBool("Torna", false);
        animatoreMainMenu.SetBool("Via", false);
        nuovaPartita = false;
        CambiaAlphaPannelloSfondo();
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
        CambiaAlphaPannelloSfondo();
    }

    private void CambiaAlphaPannelloSfondo()
    {
        fromValue = (!nuovaPartita && !caricaPartita) ? 0f : 1f;
        alpha = (!nuovaPartita && !caricaPartita) ? 1f : 0f;
        iTween.ValueTo(pannelloImmagineSfondo.gameObject, iTween.Hash("from", fromValue, "to", alpha, "time", 5f, "delay", 0.1f, "easetype",
             iTween.EaseType.easeOutCirc, "onupdatetarget", gameObject, "onupdate", "OnTweenUpdate", "onupdateparams", fromValue));
    }

    public void OnTweenUpdate(float newValue)
    {
        pannelloImmagineSfondo.color = new Color(1f, 1f, 1f, newValue);
    }

    public void Precedente()
    {
        IndiceClasseSuccessivaPrecedente--;
        while (!Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].giocabile)
            IndiceClasseSuccessivaPrecedente--;
        VisualizzaValoriPersonaggio();
        indiceCambiato = true;
        DecisionePercorsoCambioClasse();
    }

    public void Sucessivo()
    {
        IndiceClasseSuccessivaPrecedente++;
        while (!Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].giocabile)
            IndiceClasseSuccessivaPrecedente++;
        VisualizzaValoriPersonaggio();
        indiceCambiato = true;
        DecisionePercorsoCambioClasse();
    }

    private void ObiettivoDaInquadrareXZ()
    {
        obiettivoDaInquadrare = (elencoSessiDropDown.value == 0) ?
           dizionarioCollegamentoNomiConModelli[Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeM].transform.position :
           dizionarioCollegamentoNomiConModelli[Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeF].transform.position;
        zeta = (elencoSessiDropDown.value == 0) ? (obiettivoDaInquadrare.z - ZOffSet) : obiettivoDaInquadrare.z + ZOffSet;
        ics = (elencoSessiDropDown.value == 0) ? (obiettivoDaInquadrare.x + 5) : (obiettivoDaInquadrare.x - 5);
    }

    private void DecisionePercorsoCambioClasse()
    {
        ObiettivoDaInquadrareXZ();
        percorso = new Vector3[] { cameraT.position, new Vector3(obiettivoDaInquadrare.x, obiettivoDaInquadrare.y + altezzaCamera, zeta) };
        InquadraPersonaggioInNuovaPartita();
    }

    public void DecisionePercorsoCambioSesso()
    {
        ObiettivoDaInquadrareXZ();
        percorso = new Vector3[] { cameraT.position, new Vector3(ics, obiettivoDaInquadrare.y + altezzaCamera, zeta),
            new Vector3(obiettivoDaInquadrare.x, obiettivoDaInquadrare.y + altezzaCamera, zeta) };
        InquadraPersonaggioInNuovaPartita();
    }

    private void InquadraPersonaggioInNuovaPartita()
    {
        iTween.MoveTo(cameraT.gameObject, iTween.Hash("path", percorso, "time", 2f, "looktarget", obiettivoDaInquadrare,
            "looktime", 0f, "axis", "y", "easetype", iTween.EaseType.linear, "oncompletetarget", gameObject, "oncomplete", "ResettaIndiceCambiato"));
    }

    private void ResettaIndiceCambiato()
    {
        indiceCambiato = false;
    }

    private void VisualizzaValoriPersonaggio()
    {
        casellaTipo.text = Statici.databaseInizialeProprieta.classePersonaggio[IndiceClasseSuccessivaPrecedente].ToString();
        valoreVita.text = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Vita.ToString();
        valoreAttacco.text = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].Attacco.ToString();
        valoreDifesa.text = Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].difesa.ToString();
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
        personaggioProvaEsiste = false;
        erroreCaricamento.text = string.Empty;
        Dropdown.OptionData Tmp = null;
        elencoCartelleDropDown.options.Clear();

        for (int i = 0; i < cartelleLocali.Count; i++)
        {
            Tmp = new Dropdown.OptionData();

            Tmp.text = cartelleLocali[i];

            if (cartelleLocali[i] == "PersonaggioDiProva")
            {
                personaggioProvaEsiste = true;
                continue;
            }

            elencoCartelleDropDown.options.Add(Tmp);
        }
        int numeroCartelleMinimo = 0;
        numeroCartelleMinimo = !personaggioProvaEsiste ? 0 : 1;
        if (cartelleLocali.Count > numeroCartelleMinimo)
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
            bottoneCaricaDaCaricamento.interactable = false;
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
        tmOj.transform.position = posizionePersonaggioCarica.position;
        tmOj.transform.rotation = posizionePersonaggioCarica.rotation;
        tmpGOPrecedente = tmOj;
    }

    public void CancellaPartita()
    {
        if (Directory.Exists(Path.Combine
            (Application.persistentDataPath, Statici.nomePersonaggio)))
        {
            cartelleLocali.Remove(Statici.nomePersonaggio);
            Directory.Delete(Path.Combine
            (Application.persistentDataPath, Statici.nomePersonaggio), true);
            RecuperaElencoCartelle();
        }
    }
}