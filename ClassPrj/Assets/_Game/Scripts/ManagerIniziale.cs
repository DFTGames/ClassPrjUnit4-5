using Mono.Data.Sqlite;
using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerIniziale : MonoBehaviour
{
    public static GestoreAudio audioSingleton;

    [Range(0f, 20f)]
    public float altezzaCamera = 1f;

    public Animator animatoreMainMenu;
    public Animator animatoreMenuCarica;
    public Animator animatoreMenuCreazione;
    public Text attaccoCaricamento;
    public Button bottoneCaricaDaCaricamento;
    public Button bottoneCaricaDaMainManu;
    public Button bottoneEliminaPartita;
    public CanvasGroup canvasGroupCarica;
    public CanvasGroup canvasGroupCreazione;
    public Text casellaTipo;
    public Text difesaCaricamento;
    public Dropdown elencoCartelleDropDown;
    public Dropdown elencoSessiDropDown;
    public Text erroreCaricamento;
    public Text erroreCreazioneText;
    public Text nomeScenaText;
    public Image pannelloImmagineSfondo;
    public Transform posizioneCameraCarica;
    public Transform posizionePersonaggioCarica;
    public Text testoNome;
    public Text valoreAttacco;
    public Text valoreDifesa;
    public Text valoreVita;
    public Text vitaCaricamento;
    public Slider volumiAmbiente;
    public Slider volumiSFX;

    [Range(-20f, 20f)]
    public float ZOffSet;

    private static ManagerIniziale me;
    private float alpha;
    private Transform cameraT;
    private bool caricaPartita = false;
    private int contatoreGiocabili = 0;
    private Serializzabile<ClasseAudio> datiAudio;
    private Dictionary<string, GameObject> dizionarioCollegamentoNomiConModelli = new Dictionary<string, GameObject>();
    private Dictionary<string, Transform> dizionarioPosizioniPrecedenti = new Dictionary<string, Transform>();
    private FMOD.Studio.Bus EnviromentBus;
    private float fromValue;
    private float ics;
    private bool indiceCambiato = false;
    private int indiceClasseSuccessivaPrecedente = 0;

    //liste per carica personaggio
    private List<DatiPersonaggioPartenza> listaDatiPersLoad = new List<DatiPersonaggioPartenza>();

    //liste per nuovo personaggio
    private List<DatiPersonaggioPartenza> listaDatiPersNew = new List<DatiPersonaggioPartenza>();

    private bool nuovaPartita = false;
    private Vector3 obiettivoDaInquadrare;
    private Vector3[] percorso;
    private bool personaggiInCarica = false;
    private bool personaggioProvaEsiste = false;
    private Vector3 posizioneCamera;
    private string scena = string.Empty;
    private FMOD.Studio.Bus SFXBus;
    private Vector3 targetT;
    private GameObject tmpGODaEliminareSeSelezioniAltroGO = null;
    private GameObject tmpGOPrecedente = null;
    private int valoreMassimo = 0;
    private Vector3 velocita = Vector3.zero;
    private float zeta;

    public int IndiceClasseSuccessivaPrecedente
    {
        get
        {
            return indiceClasseSuccessivaPrecedente;
        }
        set
        {
            int valoreMinimo = 0;
            indiceClasseSuccessivaPrecedente = Mathf.Clamp(value, valoreMinimo, valoreMassimo);
            if (value > valoreMassimo)
                indiceClasseSuccessivaPrecedente = valoreMinimo;
            if (value < valoreMinimo)
                indiceClasseSuccessivaPrecedente = valoreMassimo;
        }
    }

    public Text NomeScenaText
    {
        get
        {
            return nomeScenaText;
        }

        set
        {
            nomeScenaText = value;
        }
    }

    public Image PannelloImmagineSfondo
    {
        get
        {
            return pannelloImmagineSfondo;
        }

        set
        {
            pannelloImmagineSfondo = value;
        }
    }

    public static void AggiornaElencoPersonaggiEsistenti()
    {
        int indiceDaRimuovere = 0;
        for (int i = 0; i < me.listaDatiPersLoad.Count; i++)
        {
            if (me.listaDatiPersLoad[i].NomePersonaggio == Statici.nomePersonaggio)
            {
                indiceDaRimuovere = i;
                break;
            }
        }
        me.listaDatiPersLoad.RemoveAt(indiceDaRimuovere);
        me.RecuperaElencoPersonaggiEsistenti();
    }

    public static void CaricaScena(string nomeScena, string scrittaCaricamento)
    {
        me.animatoreMenuCarica.gameObject.SetActive(false);
        me.animatoreMenuCreazione.gameObject.SetActive(false);
        me.animatoreMainMenu.gameObject.SetActive(false);
        me.NomeScenaText.gameObject.SetActive(true);
        me.PannelloImmagineSfondo.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        me.listaDatiPersNew.Clear();
        me.listaDatiPersLoad.Clear();
        me.listaDatiPersLoad = null;
        me.listaDatiPersNew = null;
        me.StartCoroutine(GestoreCanvasAltreScene.ScenaInCarica(nomeScena, scrittaCaricamento, me.PannelloImmagineSfondo.gameObject, me.NomeScenaText));
    }

    public static void SollevaErroreScenaInizialeCaricamentoPg(string testo)
    {
        me.erroreCaricamento.text = testo;
    }

    public static void SollevaErroreScenaInizialeCreazionePg(string testo)
    {
        me.erroreCreazioneText.text = testo;
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

    public void AnnullaDaCreazione()
    {
        animatoreMenuCreazione.SetBool("Torna", false);
        animatoreMainMenu.SetBool("Via", false);
        nuovaPartita = false;
        CambiaAlphaPannelloSfondo();
    }

    public void cambiaAmbiente(float p)
    {
        EnviromentBus.setFaderLevel(p);
        datiAudio.Dati.volEnvironment = p;
        datiAudio.Salva();
    }

    public void cambiaSfx(float p)
    {
        SFXBus.setFaderLevel(p);
        datiAudio.Dati.volSFX = p;
        datiAudio.Salva();
    }

    public void CancellaPartita()
    {
        ScenaInizialeNetwork.CancellaPersonaggio(Statici.nomePersonaggio);
    }

    public void CaricaPartita()
    {
        caricaPartita = true;
        animatoreMenuCarica.SetBool("Torna", true);
        animatoreMainMenu.SetBool("Via", true);
        RecuperaElencoPersonaggiEsistenti();
        RecuperaDatiGiocatore();
        CambiaAlphaPannelloSfondo();
        cameraT.position = new Vector3(posizioneCameraCarica.transform.position.x, posizioneCameraCarica.transform.position.y + altezzaCamera, posizioneCameraCarica.transform.position.z + ZOffSet);
        cameraT.rotation = posizioneCameraCarica.rotation;
    }

    public void CaricaPartitaDaCreazione()
    {
        if (testoNome.text.Trim() == string.Empty)
            erroreCreazioneText.text = "Inserire un nome";
        else
        {
            erroreCreazioneText.text = string.Empty;
            ScenaInizialeNetwork.ControllaSeNomeEsiste(testoNome.text.Trim());
        }
    }

    public void CaricaScenaDaCaricamento()
    {
        canvasGroupCreazione.interactable = false;
        canvasGroupCarica.interactable = false;
        Statici.sonoPassatoDallaScenaIniziale = true;
        if (!Statici.multigiocatoreOn)
            ScenaInizialeNetwork.VaiAlleStanze(listaDatiPersLoad[elencoCartelleDropDown.value].NomeScena, listaDatiPersLoad[elencoCartelleDropDown.value].NomeScena);
        else
            ScenaInizialeNetwork.VaiAlleStanze("ScenaStanze", "The Lobby");
    }

    public void DecisionePercorsoCambioSesso()
    {
        ObiettivoDaInquadrareXZ();
        percorso = new Vector3[] { cameraT.position, new Vector3(ics, obiettivoDaInquadrare.y + altezzaCamera, zeta),
            new Vector3(obiettivoDaInquadrare.x, obiettivoDaInquadrare.y + altezzaCamera, zeta) };
        InquadraPersonaggioInNuovaPartita();
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

    public void OnTweenUpdate(float newValue)
    {
        PannelloImmagineSfondo.color = new Color(1f, 1f, 1f, newValue);
    }

    public void Precedente()
    {
        IndiceClasseSuccessivaPrecedente--;
        VisualizzaValoriPersonaggio();
        indiceCambiato = true;
        DecisionePercorsoCambioClasse();
    }

    public void RecuperaDatiGiocatore()
    {
        if (elencoCartelleDropDown.options.Count <= 0) return;
        if (nuovaPartita) return;
        Statici.nomePersonaggio = elencoCartelleDropDown.options[elencoCartelleDropDown.value].text;
        vitaCaricamento.text = listaDatiPersLoad[elencoCartelleDropDown.value].VitaMassima.ToString();
        attaccoCaricamento.text = listaDatiPersLoad[elencoCartelleDropDown.value].AttaccoBase.ToString();
        difesaCaricamento.text = listaDatiPersLoad[elencoCartelleDropDown.value].DifesaBase.ToString();
        if (tmpGOPrecedente != null)
        {
            tmpGOPrecedente.transform.position = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].position;
            tmpGOPrecedente.transform.rotation = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].rotation;
        }

        if (!dizionarioCollegamentoNomiConModelli.ContainsKey(listaDatiPersLoad[elencoCartelleDropDown.value].NomeModello))
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

        GameObject tmOj = dizionarioCollegamentoNomiConModelli[listaDatiPersLoad[elencoCartelleDropDown.value].NomeModello];
        tmOj.transform.position = posizionePersonaggioCarica.position;
        tmOj.transform.rotation = posizionePersonaggioCarica.rotation;
        tmpGOPrecedente = tmOj;
    }

    public void RecuperaElencoPersonaggiEsistenti()
    {
        personaggioProvaEsiste = false;
        erroreCaricamento.text = string.Empty;
        Dropdown.OptionData Tmp = null;
        elencoCartelleDropDown.options.Clear();

        for (int i = 0; i < listaDatiPersLoad.Count; i++)
        {
            Tmp = new Dropdown.OptionData();

            Tmp.text = listaDatiPersLoad[i].NomePersonaggio;

            if (listaDatiPersLoad[i].NomePersonaggio == "PersonaggioDiProva")
            {
                personaggioProvaEsiste = true;
                continue;
            }

            elencoCartelleDropDown.options.Add(Tmp);
        }
        int numeroPersonaggiLoadMinimo = 0;
        numeroPersonaggiLoadMinimo = !personaggioProvaEsiste ? 0 : 1;
        if (listaDatiPersLoad.Count > numeroPersonaggiLoadMinimo)
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

    public void Sucessivo()
    {
        IndiceClasseSuccessivaPrecedente++;
        VisualizzaValoriPersonaggio();
        indiceCambiato = true;
        DecisionePercorsoCambioClasse();
    }

    internal static void InserimentoNuovoPersonaggio()
    {
        me.canvasGroupCreazione.interactable = false;
        me.canvasGroupCarica.interactable = false;
        me.erroreCaricamento.text = string.Empty;
        Statici.sonoPassatoDallaScenaIniziale = true;
        Statici.nomePersonaggio = me.testoNome.text;
        byte sesso = 0;
        if (me.elencoSessiDropDown.value == 0)
        {
            sesso = 0;
            Statici.nomeModello = me.listaDatiPersNew[me.IndiceClasseSuccessivaPrecedente].NomeModelloM;
        }
        else
        {
            sesso = 1;
            Statici.nomeModello = me.listaDatiPersNew[me.IndiceClasseSuccessivaPrecedente].NomeModelloF;
        }
        SFSObject obj = new SFSObject();
        obj.PutUtfString("nomePersonaggio", Statici.nomePersonaggio);
        obj.PutInt("Utenti_idUtente", Statici.idDB);
        obj.PutUtfString("nomeModello", Statici.nomeModello);
        obj.PutInt("eliminato", 0);
        obj.PutDouble("vitaMassima", (double)me.listaDatiPersNew[me.IndiceClasseSuccessivaPrecedente].VitaMassima);
        obj.PutDouble("vitaAttuale", (double)me.listaDatiPersNew[me.IndiceClasseSuccessivaPrecedente].VitaAttuale);
        obj.PutDouble("manaMassimo", (double)me.listaDatiPersNew[me.IndiceClasseSuccessivaPrecedente].ManaMassimo);
        obj.PutDouble("manaAttuale", (double)me.listaDatiPersNew[me.IndiceClasseSuccessivaPrecedente].ManaAttuale);
        obj.PutDouble("livelloPartenza", (double)me.listaDatiPersNew[me.IndiceClasseSuccessivaPrecedente].LivelloPartenza);
        obj.PutDouble("xpPartenza", (double)me.listaDatiPersNew[me.IndiceClasseSuccessivaPrecedente].XpPartenza);
        obj.PutUtfString("nomeScena", "Isola");
        obj.PutUtfString("checkPoint", "start");
        Statici.arrayPersNewPersUt.AddSFSObject(obj);
        SqliteDataReader reader = Statici.GiveMeDiplomaziaLocale(me.listaDatiPersNew[me.IndiceClasseSuccessivaPrecedente].IdClassePersonaggio);
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                SFSObject obj2 = new SFSObject();
                obj2.PutUtfString("PersonaggiUtente_nomePersonaggio", Statici.nomePersonaggio);
                obj2.PutInt("PersonaggiUtente_Utenti_idUtente", Statici.idDB);
                obj2.PutInt("Diplomazia_ClassiPersonaggi_idClasse", (int)reader["ClassiPersonaggi_idClasse"]);
                obj2.PutInt("Diplomazia_ClassiPersonaggi2_idClasse", (int)reader["ClassiPersonaggi2_idClasse"]);
                obj2.PutInt("eliminato", 0);
                obj2.PutInt("relazione", (int)reader["relazione"]);
                Statici.arrayPersNewPersDipPers.AddSFSObject(obj2);
            }
        }
        ScenaInizialeNetwork.RichiestaCreazionePersonaggio(me.listaDatiPersNew[me.IndiceClasseSuccessivaPrecedente].NomeClasse, sesso);
    }

    private void CambiaAlphaPannelloSfondo()
    {
        fromValue = (!nuovaPartita && !caricaPartita) ? 0f : 1f;
        alpha = (!nuovaPartita && !caricaPartita) ? 1f : 0f;
        iTween.ValueTo(PannelloImmagineSfondo.gameObject, iTween.Hash("from", fromValue, "to", alpha, "time", 5f, "delay", 0.1f, "easetype",
             iTween.EaseType.easeOutCirc, "onupdatetarget", gameObject, "onupdate", "OnTweenUpdate", "onupdateparams", fromValue));
    }

    private void DecisionePercorsoCambioClasse()
    {
        ObiettivoDaInquadrareXZ();
        percorso = new Vector3[] { cameraT.position, new Vector3(obiettivoDaInquadrare.x, obiettivoDaInquadrare.y + altezzaCamera, zeta) };
        InquadraPersonaggioInNuovaPartita();
    }

    private void InquadraPersonaggioInNuovaPartita()
    {
        iTween.MoveTo(cameraT.gameObject, iTween.Hash("path", percorso, "time", 2f, "looktarget", obiettivoDaInquadrare,
            "looktime", 0f, "axis", "y", "easetype", iTween.EaseType.linear, "oncompletetarget", gameObject, "oncomplete", "ResettaIndiceCambiato"));
    }

    private void ObiettivoDaInquadrareXZ()
    {
        obiettivoDaInquadrare = (elencoSessiDropDown.value == 0) ?
           dizionarioCollegamentoNomiConModelli[Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeM].transform.position :
           dizionarioCollegamentoNomiConModelli[Statici.databaseInizialeProprieta.matriceProprieta[IndiceClasseSuccessivaPrecedente].nomeF].transform.position;
        zeta = (elencoSessiDropDown.value == 0) ? (obiettivoDaInquadrare.z - ZOffSet) : obiettivoDaInquadrare.z + ZOffSet;
        ics = (elencoSessiDropDown.value == 0) ? (obiettivoDaInquadrare.x + 5) : (obiettivoDaInquadrare.x - 5);
    }

    private void ResettaIndiceCambiato()
    {
        indiceCambiato = false;
    }

    private void Start()
    {
        Statici.inGioco = false;
        me = this;
        CambiaAlphaPannelloSfondo();
        NomeScenaText.gameObject.SetActive(false);
        Statici.assegnaAssetDatabase();
        PannelloImmagineSfondo = pannelloImmagineSfondo;
        NomeScenaText = nomeScenaText;
        cameraT = Camera.main.transform;

        //mi faccio dare i dati della tabella Personaggi da cui prendo i dati in caso di nuovo personaggio
        SqliteDataReader reader = Statici.GiveMePersonaggiDBLocale();
        listaDatiPersNew.Clear();
        listaDatiPersNew = null;

        valoreMassimo = 0;
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                DatiPersonaggioPartenza dpp = new DatiPersonaggioPartenza();
                dpp.IdPersonaggio = (int)reader["idPersonaggio"];
                dpp.IdClassePersonaggio = (int)reader["ClassiPersonaggi_idClassiPersonaggi"];
                dpp.VitaMassima = (decimal)reader["vitaMassima"];
                dpp.VitaAttuale = (decimal)reader["vitaAttuale"];
                dpp.ManaMassimo = (decimal)reader["manaMassimo"];
                dpp.ManaAttuale = (decimal)reader["manaAttuale"];
                dpp.LivelloPartenza = (decimal)reader["livelloPartenza"];
                dpp.XpPartenza = (decimal)reader["xpPartenza"];
                dpp.NomeClasse = (string)reader["nome"];
                dpp.AttaccoBase = (decimal)reader["attaccoBase"];
                dpp.DifesaBase = (decimal)reader["difesaBase"];
                dpp.NomeModelloM = (string)reader["modelloM"];
                dpp.NomeModelloF = (string)reader["modelloF"];

                listaDatiPersNew.Add(dpp);
                string tmpNomeModelloM = (string)reader["modelloM"];
                string tmpNomeModelloF = (string)reader["modelloF"];
                dizionarioCollegamentoNomiConModelli.Add(tmpNomeModelloM, Instantiate(Resources.Load(tmpNomeModelloM), GameObject.Find("postazione" + contatoreGiocabili).transform.FindChild("posizioneM").position, new Quaternion(0f, 180f, 0f, 0f)) as GameObject);
                dizionarioCollegamentoNomiConModelli.Add(tmpNomeModelloF, Instantiate(Resources.Load(tmpNomeModelloF), GameObject.Find("postazione" + contatoreGiocabili).transform.FindChild("posizioneF").position, Quaternion.identity) as GameObject);
                dizionarioPosizioniPrecedenti.Add(dizionarioCollegamentoNomiConModelli[tmpNomeModelloM].name, GameObject.Find("postazione" + contatoreGiocabili).transform.FindChild("posizioneM"));
                dizionarioPosizioniPrecedenti.Add(dizionarioCollegamentoNomiConModelli[tmpNomeModelloF].name, GameObject.Find("postazione" + contatoreGiocabili).transform.FindChild("posizioneF"));
                contatoreGiocabili += 1;
                valoreMassimo++;
            }
        }

        valoreMassimo--;
        if (!reader.IsClosed)
            reader.Close();

        //mi facio dare i dati dalla tabella PersonaggiUtente in caso di personaggi già esistenti
        listaDatiPersLoad.Clear();
        listaDatiPersLoad = null;
        reader = Statici.GiveMePersonaggiUtenteDBLocale();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                DatiPersonaggioPartenza dpp = new DatiPersonaggioPartenza();
                dpp.VitaMassima = (decimal)reader["vitaMassima"];
                dpp.AttaccoBase = (decimal)reader["attaccoBase"];
                dpp.DifesaBase = (decimal)reader["difesaBase"];
                dpp.NomePersonaggio = (string)reader["nomePersonaggio"];
                dpp.NomeModello = (string)reader["nomeModello"];
                dpp.NomeScena = (string)reader["nomeScena"];
                dpp.CheckPoint = (string)reader["checkPoint"];
                listaDatiPersLoad.Add(dpp);
            }
        }
        if (!reader.IsClosed)
            reader.Close();

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

    private void VisualizzaValoriPersonaggio()
    {
        casellaTipo.text = listaDatiPersNew[IndiceClasseSuccessivaPrecedente].NomeClasse;
        valoreVita.text = listaDatiPersNew[IndiceClasseSuccessivaPrecedente].VitaMassima.ToString();
        valoreAttacco.text = listaDatiPersNew[IndiceClasseSuccessivaPrecedente].AttaccoBase.ToString();
        valoreDifesa.text = listaDatiPersNew[IndiceClasseSuccessivaPrecedente].DifesaBase.ToString();
    }
}