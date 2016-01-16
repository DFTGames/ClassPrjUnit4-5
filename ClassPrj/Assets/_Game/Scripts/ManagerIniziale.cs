using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
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
    public Animator AnimatoreMenu;
    public Animator AltrieMenu1;
    public Animator AltrieMenu2;
    public Button BottoneCaricaOff;
    public Button EliminaPartita;
    public Text CasellaTipo;
    public Text ValoreVita;
    public Text ValoreAttacco;
    public Text ValoreDifesa;
    public Text TestoNome;
    public Text Errore;
    public Text ErroreCaricamento;
    public Text VitaAttuale;
    public Text AttaccoAtuale;
    public Text DifesaAttuale;
    public GameData databseInizialeAmicizie;
    public caratteristichePersonaggioV2 databaseInizialeProprieta;
    public Dropdown ElencoCartelle;
    public Dropdown ElencoSessi;

    public Transform posizioneInizialeCamera;
    public GameObject pannelloImmagineSfondo;
    public Text nomeScenaText;

    private int indiceButton = 0;
    private Serializzabile<ValoriPersonaggioS> datiPersonaggio;
    private Serializzabile<AmicizieSerializzabili> datiDiplomazia;
    private static ManagerIniziale Me;
    private string scena = string.Empty;
    private GameObject tmpGODaEliminareSeSelezioniAltroGO = null;
    private GameObject tmpGOPrecedente = null;
    private Dictionary<string, Transform> dizionarioPosizioniPrecedenti = new Dictionary<string, Transform>();

    private bool nuovaPartita = false;
    private bool caricaPartita = false;
    private bool personaggiInCarica = false;

    //telecamera:
    public Transform posizioneCameraCarica;

    [Range(0f, 20f)]
    public float altezza = 1f;

    [Range(-20f, 20f)]
    public float ZOffSet;

    public float DampTime;

    private Transform Target = null;
    private Vector3 velocita = Vector3.zero;
    private Vector3 Obiettivo;
    private Transform cameraT;

    public int IndiceButton
    {
        get
        {
            return indiceButton;
        }

        set
        {
            int valoreMinimo = 0;

            int valoreMassimo = databaseInizialeProprieta.classePersonaggio.Count - 1;
            indiceButton = Mathf.Clamp(value, valoreMinimo, valoreMassimo);
            if (value > valoreMassimo)
                indiceButton = valoreMinimo;
            if (value < valoreMinimo)
                indiceButton = valoreMassimo;
        }
    }

    private void Metodo_Charlie()   //metodo per assegnare gli asset dentro l'inspector... By Luca del dftStudent
    {
        if (databseInizialeAmicizie == null)
        {
            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig))
            {
                string percorso = EditorPrefs.GetString(Statici.STR_PercorsoConfig);
                databseInizialeAmicizie = AssetDatabase.LoadAssetAtPath<GameData>(percorso + Statici.STR_DatabaseDiGioco);
            }
        }
        if (databaseInizialeProprieta == null)
        {
            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig3))
            {
                string percorso = EditorPrefs.GetString(Statici.STR_PercorsoConfig3);
                databaseInizialeProprieta = AssetDatabase.LoadAssetAtPath<caratteristichePersonaggioV2>(percorso + Statici.STR_DatabaseDiGioco3);
            }
        }
    }

    // Use this for initialization
    private void Start()
    {
        nomeScenaText.gameObject.SetActive(false);
#if UNITY_EDITOR
        Metodo_Charlie();
#endif

        Me = this;
        cameraT = Camera.main.transform;
        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);

        //istanzio tutti i personaggi
        for (int i = 0; i < databaseInizialeProprieta.matriceProprieta.Count; i++)
        {
            Instantiate(Resources.Load(databaseInizialeProprieta.matriceProprieta[i].nomeM), GameObject.Find("postazione" + i).transform.FindChild("posizioneM").position, new Quaternion(0f, 180f, 0f, 0f));
            Instantiate(Resources.Load(databaseInizialeProprieta.matriceProprieta[i].nomeF), GameObject.Find("postazione" + i).transform.FindChild("posizioneF").position, Quaternion.identity);
            dizionarioPosizioniPrecedenti.Add(databaseInizialeProprieta.matriceProprieta[i].nomeM + "(Clone)", GameObject.Find("postazione" + i).transform.FindChild("posizioneM"));
            dizionarioPosizioniPrecedenti.Add(databaseInizialeProprieta.matriceProprieta[i].nomeF + "(Clone)", GameObject.Find("postazione" + i).transform.FindChild("posizioneF"));
        }

        Statici.CopiaIlDB();
    }

    public void NuovaPartita()
    {
        AltrieMenu1.SetBool("Torna", true);
        AnimatoreMenu.SetBool("Via", true);
        nuovaPartita = true;
        RecuperaSesso();
    }

    public void CaricaPartita()
    {
        caricaPartita = true;
        AltrieMenu2.SetBool("Torna", true);
        AnimatoreMenu.SetBool("Via", true);
        RecuperaElencoCartelle();
        RecuperaDatiGiocatore();
    }

    public void CaricaScena()
    {
        for (int i = 0; i < databaseInizialeProprieta.matriceProprieta.Count; i++)
        {
            Destroy(GameObject.Find(databaseInizialeProprieta.matriceProprieta[i].nomeM + "(Clone)"));
            Destroy(GameObject.Find(databaseInizialeProprieta.matriceProprieta[i].nomeF + "(Clone)"));
        }

        SerializzaPercorsi();
        /*
        l'if else qui sotto, serve per verificare se la scena in cui vogliamo far spuntare il personaggio 
        esiste ancora o no nel build settings. Perchè può capitare di cancellar euna scena o rinominalrla.
        Per evitare di far andar ein errore il gioco, se la scena non esiste più il personaggio viene caricato 
        nell'isola altrimenti nell'ultima scena visitata.
        */
       
        if (!Application.CanStreamedLevelBeLoaded(datiPersonaggio.Dati.nomeScena)) {
            datiPersonaggio.Dati.nomeScena = "Isola";
            datiPersonaggio.Dati.posizioneCheckPoint = "start";
            datiPersonaggio.Salva();
            SceneManager.LoadScene("Isola");
       }else
        SceneManager.LoadScene(datiPersonaggio.Dati.nomeScena);
    
    }

    public void AvviaGioco()
    {
        Errore.text = String.Empty;
        if (TestoNome.text.Trim() == string.Empty)
        {
            Errore.text = "Inserire un nome";
            return;
        }

        DirectoryInfo dI = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] dirs = dI.GetDirectories();
        for (int i = 0; i < dirs.Length; i++)
        {
            if (dirs[i].Name.ToLower() == TestoNome.text.ToLower())
            {
                Errore.text = "Esiste Gia Un Personaggio con questo nome";
                return;
            }
        }

        for (int i = 0; i < databaseInizialeProprieta.matriceProprieta.Count; i++)
        {
            Destroy(GameObject.Find(databaseInizialeProprieta.matriceProprieta[i].nomeM + "(Clone)"));
            Destroy(GameObject.Find(databaseInizialeProprieta.matriceProprieta[i].nomeF + "(Clone)"));
        }
        Statici.nomePersonaggio = TestoNome.text;

        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);

        if (datiPersonaggio.Dati.nomePersonaggio == null)
        {
            datiPersonaggio.Dati.Vita = databaseInizialeProprieta.matriceProprieta[IndiceButton].Vita;
            datiPersonaggio.Dati.Attacco = databaseInizialeProprieta.matriceProprieta[IndiceButton].Attacco;
            datiPersonaggio.Dati.difesa = databaseInizialeProprieta.matriceProprieta[IndiceButton].difesa;
            datiPersonaggio.Dati.Xp = databaseInizialeProprieta.matriceProprieta[IndiceButton].Xp;
            datiPersonaggio.Dati.Livello = databaseInizialeProprieta.matriceProprieta[IndiceButton].Livello;

            if (ElencoSessi.value == 0)
                datiPersonaggio.Dati.nomeModello = databaseInizialeProprieta.matriceProprieta[IndiceButton].nomeM;
            else
                datiPersonaggio.Dati.nomeModello = databaseInizialeProprieta.matriceProprieta[IndiceButton].nomeF;
            datiPersonaggio.Dati.nomePersonaggio = TestoNome.text;
            datiPersonaggio.Dati.classe = CasellaTipo.text;

            datiPersonaggio.Dati.VitaMassima = databaseInizialeProprieta.matriceProprieta[IndiceButton].Vita;
            datiPersonaggio.Dati.ManaMassimo = databaseInizialeProprieta.matriceProprieta[IndiceButton].Mana;
            datiPersonaggio.Dati.XPMassimo = databaseInizialeProprieta.matriceProprieta[IndiceButton].Xp;
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
        SerializzaPercorsi();
        personaggiInCarica = true;
        SceneManager.LoadScene(datiPersonaggio.Dati.nomeScena);
    
    }

    private void SerializzaPercorsi()   //Controlla e se necessario riserializza i percorsi
    {
        //Controlla i percorsi se sono gia serializzati e se ci sono variazioni li reserializza
        //se non sono serializzati li serializza
        //se non ci sono variazioni non fa niente

        if (databseInizialeAmicizie == null) return;

        if (datiDiplomazia == null)

            datiDiplomazia = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);

        if (datiDiplomazia.Dati.indexPercorsi.Equals(databseInizialeAmicizie.indexPercorsi)) return;  //CONTROLLARE SE METODO E' CORRETTO

        for (int i = 0; i < databseInizialeAmicizie.tagEssere.Length; i++)

        {
            datiDiplomazia.Dati.indexPercorsi[i] = databseInizialeAmicizie.indexPercorsi[i];
        }

        datiDiplomazia.Salva();
    }

    public void Anulla1()
    {
        AltrieMenu1.SetBool("Torna", false);
        AnimatoreMenu.SetBool("Via", false);
        nuovaPartita = false;
    }

    public void Anulla2()
    {
        if (tmpGOPrecedente != null)
        {
            tmpGOPrecedente.transform.position = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].position;
            tmpGOPrecedente.transform.rotation = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].rotation;
        }

        AltrieMenu2.SetBool("Torna", false);
        AnimatoreMenu.SetBool("Via", false);
        caricaPartita = false;
    }

    public void Precedente()
    {
        IndiceButton--;

        RecuperaSesso();
    }

    public void Sucessivo()
    {
        IndiceButton++;

        RecuperaSesso();
    }

    // Update is called once per frame
    private void Update()
    {
        CasellaTipo.text = databaseInizialeProprieta.classePersonaggio[IndiceButton].ToString();
        ValoreVita.text = databaseInizialeProprieta.matriceProprieta[IndiceButton].Vita.ToString();
        ValoreAttacco.text = databaseInizialeProprieta.matriceProprieta[IndiceButton].Attacco.ToString();
        ValoreDifesa.text = databaseInizialeProprieta.matriceProprieta[IndiceButton].difesa.ToString();

        if ((!nuovaPartita && !caricaPartita) || (caricaPartita && !personaggiInCarica))
        {
            Image tmpImage = pannelloImmagineSfondo.GetComponent<Image>();
            if (tmpImage.color.a < 1f)                
                tmpImage.color += new Color(0f, 0f, 0f, 0.2f) * Time.deltaTime * 3f;

            Target = posizioneInizialeCamera;
            Obiettivo = new Vector3(Target.position.x, Target.position.y + altezza, Target.position.z + ZOffSet);

            CambiaPosizioneTelecamera();
        }
        else if (caricaPartita && !nuovaPartita)//+
        {

            Image tmpImage = pannelloImmagineSfondo.GetComponent<Image>();
            if (tmpImage.color.a > 0f)
                tmpImage.color -= new Color(0f, 0f, 0f, 0.1f) * Time.deltaTime * 1.5f;
            if (GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)") != null)
            {
                Target = GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)").GetComponent<Transform>();
                Obiettivo = new Vector3(Target.position.x, Target.position.y + altezza, Target.position.z + ZOffSet);

                CambiaPosizioneTelecamera();
            }
        }
        else if (ElencoSessi.value == 0 && nuovaPartita && !caricaPartita)
        {
            Image tmpImage = pannelloImmagineSfondo.GetComponent<Image>();
            if (tmpImage.color.a > 0f)
                tmpImage.color -= new Color(0f, 0f, 0f, 0.1f) * Time.deltaTime * 1.5f;

            Target = GameObject.Find(databaseInizialeProprieta.matriceProprieta[IndiceButton].nomeM + "(Clone)").GetComponent<Transform>();
            Obiettivo = new Vector3(Target.position.x, Target.position.y + altezza, Target.position.z - ZOffSet);

            CambiaPosizioneTelecamera();
        }
        else if (ElencoSessi.value == 1 && nuovaPartita && !caricaPartita)
        {

            Image tmpImage = pannelloImmagineSfondo.GetComponent<Image>();
            if (tmpImage.color.a > 0f)
                tmpImage.color -= new Color(0f, 0f, 0f, 0.1f) * Time.deltaTime * 1.5f;
            Target = GameObject.Find(databaseInizialeProprieta.matriceProprieta[IndiceButton].nomeF + "(Clone)").GetComponent<Transform>();
            Obiettivo = new Vector3(Target.position.x, Target.position.y + altezza, Target.position.z + ZOffSet);

            CambiaPosizioneTelecamera();
        }

        //mentre sto caricando una nuova scena faccio spuntare un immagine di sfondo e il nome della scena facendo scomparire eventuali pannelli attivi:
        if (Application.isLoadingLevel && pannelloImmagineSfondo.GetComponent<Image>().color.a!=1f)
        {
           
            AltrieMenu2.gameObject.SetActive(false);
            AltrieMenu1.gameObject.SetActive(false);
            AnimatoreMenu.gameObject.SetActive(false);           
            nomeScenaText.gameObject.SetActive(true);
            nomeScenaText.text = "Loading... " + datiPersonaggio.Dati.nomeScena;
            pannelloImmagineSfondo.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        }
       
    }

    private void CambiaPosizioneTelecamera()
    {
        cameraT.position = Vector3.SmoothDamp(cameraT.position, Obiettivo, ref velocita, DampTime);

        if (ZOffSet != 0f)
        {
            cameraT.LookAt(Target.position);
        }
    }

    public void RecuperaElencoCartelle()
    {
        ErroreCaricamento.text = string.Empty;
        Dropdown.OptionData Tmp = null;
        ElencoCartelle.options.Clear();
        DirectoryInfo Di = new DirectoryInfo(Application.persistentDataPath);
        DirectoryInfo[] Drs = Di.GetDirectories();
        for (int i = 0; i < Drs.Length; i++)
        {
            Tmp = new Dropdown.OptionData();
            Tmp.text = Drs[i].Name;
            ElencoCartelle.options.Add(Tmp);
        }
        if (Drs.Length > 0)
        {
            personaggiInCarica = true;
            BottoneCaricaOff.interactable = true;
            EliminaPartita.interactable = true;
            ElencoCartelle.value = -1;    //visualizzo sempre il primo elemento della lista

            Statici.nomePersonaggio = ElencoCartelle.options[ElencoCartelle.value].text;
        }
        else
        {
            personaggiInCarica = false;
            Statici.nomePersonaggio = string.Empty;
            BottoneCaricaOff.interactable = false;
            EliminaPartita.interactable = false;
            ErroreCaricamento.text = "Non ci sono partite salvate";
            VitaAttuale.text = "none";
            AttaccoAtuale.text = "none";
            DifesaAttuale.text = "none";
            if (tmpGOPrecedente != null)
            {
                tmpGOPrecedente.transform.position = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].position;
                tmpGOPrecedente.transform.rotation = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].rotation;
            }
            ElencoCartelle.captionText.text = string.Empty;  //NON VISUALIZZA LA STRINGA QUANDO LA LISTA E' VUOTA
        }
    }

    public void RecuperaDatiGiocatore()
    {
        if (ElencoCartelle.options.Count <= 0) return;
        if (nuovaPartita) return;
        Statici.nomePersonaggio = ElencoCartelle.options[ElencoCartelle.value].text;
        datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);

        VitaAttuale.text = datiPersonaggio.Dati.Vita.ToString();
        AttaccoAtuale.text = datiPersonaggio.Dati.Attacco.ToString();
        DifesaAttuale.text = datiPersonaggio.Dati.difesa.ToString();

        if (tmpGOPrecedente != null)
        {
            tmpGOPrecedente.transform.position = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].position;
            tmpGOPrecedente.transform.rotation = dizionarioPosizioniPrecedenti[tmpGOPrecedente.name].rotation;
        }

        if (datiPersonaggio == null) return;

        if (GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)") == null)
        {
            ErroreCaricamento.text = "Errore..Questo personaggio non e' valido";
            return;
        }

        GameObject tmOj = GameObject.Find(datiPersonaggio.Dati.nomeModello + "(Clone)");

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
        ElencoSessi.options.Clear();
        for (int i = 0; i < Enum.GetValues(typeof(Sesso)).Length; i++)
        {
            Dropdown.OptionData tmp = new Dropdown.OptionData();
            tmp.text = Enum.GetName(typeof(Sesso), i);
            ElencoSessi.options.Add(tmp);
        }

        ElencoCartelle.value = 0;
    }
}