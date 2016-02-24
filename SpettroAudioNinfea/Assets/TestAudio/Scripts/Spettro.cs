using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Spettro : MonoBehaviour {

    public LineRenderer lineRenderer;
    public LineRenderer lineRenderer2;
    public AudioSource audioSource;   
    public int numeroPuntiInterniLR = 150;
    public AudioClip[] audioClips;
    public Camera cameraPrincipale;
    public Text canzoneInEsecuzioneText;
    public GameObject oggetto;
    public int numeroOggetti = 150;
    public Dropdown dropDownFinestre;
    public Dropdown dropdownOnOff;
    public GameObject Gui;

    private float[] spectrum = new float[1024];
    private GameObject tmpCubo;
    private List<GameObject> oggetti = new List<GameObject>();
    private FFTWindow[] nomeFinestraSpettro = { FFTWindow.Blackman, FFTWindow.BlackmanHarris, FFTWindow.Hamming, FFTWindow.Hanning, FFTWindow.Rectangular, FFTWindow.Triangle };
    private string[] cosaVisualizzo = { "entrambi", "solo LineRenderer", "solo Oggetti"};
    private int indice=0;
    private float coordinataInAlto = 0.75f;
    private float coordinataInBasso = 0.25f;
    private float coordinataCentrale = 0.5f;
    private float coordinataLineRenderer;
    private float coordinataOggetti;
    private float coordinataGui;
    private RectTransform guiRectTransform;
  
    

    // Use this for initialization
    void Start () {
        canzoneInEsecuzioneText.text = audioSource.clip.ToString();

        for(int i = 0; i < numeroOggetti; i++)
        {
            tmpCubo = Instantiate(oggetto, cameraPrincipale.ScreenToWorldPoint(new Vector3(((float)i*Screen.width/numeroOggetti), Screen.height*coordinataInBasso, 20f)), Quaternion.identity) as GameObject;
            oggetti.Add(tmpCubo);
        }

        dropDownFinestre.options.Clear();      
        for (int i = 0; i < nomeFinestraSpettro.Length; i++)
        {
            Dropdown.OptionData opzione = new Dropdown.OptionData();
            opzione.text= nomeFinestraSpettro[i].ToString();
            dropDownFinestre.options.Add(opzione);
        }
        dropDownFinestre.value = 0;

        dropdownOnOff.options.Clear();
        for (int i = 0; i < cosaVisualizzo.Length; i++)
        {
            Dropdown.OptionData opzione = new Dropdown.OptionData();
            opzione.text = cosaVisualizzo[i].ToString();
            dropdownOnOff.options.Add(opzione);
        }
        dropdownOnOff.value = 0;

        guiRectTransform = Gui.GetComponent<RectTransform>();
        guiRectTransform.localPosition = new Vector3(0f,0f,0f);
        
    }
	
	// Update is called once per frame
	void Update () {
        audioSource.GetSpectrumData(spectrum, 0, nomeFinestraSpettro[dropDownFinestre.value]);

        if (dropdownOnOff.value != 2)
        {
            if (!lineRenderer.gameObject.activeInHierarchy)
                lineRenderer.gameObject.SetActive(true);
            if (!lineRenderer2.gameObject.activeInHierarchy)
                lineRenderer2.gameObject.SetActive(true);
            coordinataLineRenderer = (dropdownOnOff.value == 0) ? coordinataInAlto : coordinataCentrale;
            //lineRenderers:
            lineRenderer.SetVertexCount(numeroPuntiInterniLR);
            lineRenderer2.SetVertexCount(numeroPuntiInterniLR);
            lineRenderer.SetPosition(0, cameraPrincipale.ViewportToWorldPoint(new Vector3(0f, coordinataLineRenderer, cameraPrincipale.nearClipPlane)));
            lineRenderer2.SetPosition(0, cameraPrincipale.ViewportToWorldPoint(new Vector3(0f, coordinataLineRenderer, cameraPrincipale.nearClipPlane)));

            for (int i = 1; i < numeroPuntiInterniLR - 1; i++)
            {
                float DatoSpettro = spectrum[i];
                lineRenderer.SetPosition(i, cameraPrincipale.ViewportToWorldPoint(new Vector3((float)i / numeroPuntiInterniLR, coordinataLineRenderer + DatoSpettro * 5, cameraPrincipale.nearClipPlane)));
                lineRenderer2.SetPosition(i, cameraPrincipale.ViewportToWorldPoint(new Vector3((float)i / numeroPuntiInterniLR, coordinataLineRenderer - DatoSpettro * 5, cameraPrincipale.nearClipPlane)));
            }
            lineRenderer.SetPosition(numeroPuntiInterniLR - 1, cameraPrincipale.ViewportToWorldPoint(new Vector3(1f, coordinataLineRenderer, cameraPrincipale.nearClipPlane)));
            lineRenderer2.SetPosition(numeroPuntiInterniLR - 1, cameraPrincipale.ViewportToWorldPoint(new Vector3(1f, coordinataLineRenderer, cameraPrincipale.nearClipPlane)));
        }
        else
        {
            if (lineRenderer.gameObject.activeInHierarchy)
                lineRenderer.gameObject.SetActive(false);
            if (lineRenderer2.gameObject.activeInHierarchy)
                lineRenderer2.gameObject.SetActive(false);
        }


        if (dropdownOnOff.value != 1)
        {
            coordinataOggetti = (dropdownOnOff.value == 0) ? coordinataInBasso : coordinataCentrale;
            //oggetti;
            for (int i = 0; i < numeroOggetti; i++)
            {
                float datoSpettro = spectrum[i];
                Vector3 localScalePrecedente = oggetti[i].transform.localScale;
                localScalePrecedente.y = datoSpettro;
                oggetti[i].transform.position = cameraPrincipale.ScreenToWorldPoint(new Vector3(((float)i * Screen.width / numeroOggetti), Screen.height * coordinataOggetti, 20f));
                oggetti[i].transform.localScale = new Vector3(localScalePrecedente.x, localScalePrecedente.y * 100, localScalePrecedente.z);
                if (!oggetti[i].activeInHierarchy)
                    oggetti[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < numeroOggetti; i++)
            {
                if (oggetti[i].activeInHierarchy)
                    oggetti[i].SetActive(false);
            }
        }

        if(dropdownOnOff.value==0)
            guiRectTransform.localPosition= new Vector3(0f, 0f, 0f);
        else
            guiRectTransform.position = new Vector3(Screen.width*coordinataCentrale, Screen.height*coordinataInAlto, 0f);
    }

    public void CambiaMusica()
    {
        if (indice == audioClips.Length-1)
            indice = 0;
        else
            indice++;
        audioSource.clip = audioClips[indice];
        if (!audioSource.isPlaying)
            audioSource.Play();
        canzoneInEsecuzioneText.text = audioSource.clip.ToString();
    }
}
