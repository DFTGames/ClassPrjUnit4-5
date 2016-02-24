using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MixerControl : MonoBehaviour {

    public Slider s_VolumiSfx;
    public Slider s_VolumiEnvironment;

    private static MixerControl m_me;
    private Serializzabile<ClasseAudio> datiAudio;
    private GestoreCanvasAltreScene gc_AltreScene;
    private static float volumiSfx;
    private static float volumiEnvironment;
    private float volumePrecedente;
    private FMOD.Studio.Bus SFXBus;
    private FMOD.Studio.Bus EnviromentBus;

    void Awake()
    {
        m_me = this;
    }
    void Start()
    {
        Gc_AltreScene = GameObject.Find("GestoreCanvas").GetComponent<GestoreCanvasAltreScene>();
        Inizializza();
        if (Gc_AltreScene.pannelloTest.activeInHierarchy)
        {
            if (s_VolumiEnvironment == null || s_VolumiSfx == null)
            {
                s_VolumiSfx = GameObject.Find("s_VolumiSfx").GetComponent<Slider>();
                s_VolumiEnvironment = GameObject.Find("s_VolAmbiente").GetComponent<Slider>();
                Carica();
            }
        }
    }
    void Update()
    {
        if(Gc_AltreScene.pannelloTest.activeInHierarchy)
        {
            if (s_VolumiEnvironment == null || s_VolumiSfx == null)
            {
                s_VolumiSfx = GameObject.Find("s_VolumiSfx").GetComponent<Slider>();
                s_VolumiEnvironment = GameObject.Find("s_VolAmbiente").GetComponent<Slider>();
                Carica();
            }
            if (s_VolumiSfx.value != VolumiSfx ||
                s_VolumiEnvironment.value != VolumiEnvironment)
            {
                VolumiSfx = volumiSfx;
                VolumiEnvironment = volumiEnvironment;
            }
        }
    }

    public static float VolumiSfx
    {
        get
        {
            return volumiSfx;
        }
        set
        {
            volumiSfx = m_me.s_VolumiSfx.value;
            m_me.SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
            m_me.SFXBus.setFaderLevel(value);
            m_me.datiAudio.Dati.volSFX = volumiSfx;
            m_me.datiAudio.Salva();
        }
    }

    public static float VolumiEnvironment
    {
        get
        {
            return volumiEnvironment;
        }
        set
        {
            volumiEnvironment = m_me.s_VolumiEnvironment.value;
            m_me.EnviromentBus = FMODUnity.RuntimeManager.GetBus("bus:/Environment");
            m_me.EnviromentBus.setFaderLevel(value);
            m_me.datiAudio.Dati.volEnvironment = volumiEnvironment;
            m_me.datiAudio.Salva();
        }
    }

    public GestoreCanvasAltreScene Gc_AltreScene
    {
        get
        {
            return gc_AltreScene;
        }

        set
        {
            gc_AltreScene = value;
        }
    }

    public void Inizializza()
    {
        datiAudio = new Serializzabile<ClasseAudio>(Statici.NomeFileAudio);
        if (!datiAudio.Dati.inizializzato)
        {
            datiAudio.Dati.volEnvironment = 0.5f;
            datiAudio.Dati.volSFX = 0.5f;
            datiAudio.Dati.inizializzato = true;
            datiAudio.Salva();
        }
    }
    public void Carica()
    {
        if (datiAudio.Dati != null)
        {
            volumiEnvironment = datiAudio.Dati.volEnvironment;
            volumiSfx = datiAudio.Dati.volSFX;
            if (s_VolumiEnvironment != null && s_VolumiSfx != null)
            {
                s_VolumiEnvironment.value = VolumiEnvironment;
                s_VolumiSfx.value = VolumiSfx;
            }
            datiAudio.Salva();
        }
    }
}
