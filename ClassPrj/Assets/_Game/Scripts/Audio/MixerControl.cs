using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MixerControl : MonoBehaviour {

    private static MixerControl m_me;
    private static float volumiSfx;
    private static float volumiEnvironment;

    public Slider s_VolumiSfx;
    public Slider s_VolumiEnvironment;

    private FMOD.Studio.Bus SFXBus;
    private FMOD.Studio.Bus EnviromentBus;

    void Awake()
    {
        m_me = this;
    }
    void Start()
    {
        s_VolumiSfx.value = PlayerPrefs.GetFloat("v_Sfx", 0.5f);
    }
    void Update()
    {
        VolumiSfx = VolumiSfx;
        VolumiEnvironment = VolumiEnvironment;
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
            PlayerPrefs.SetFloat("v_Sfx", volumiSfx);
            PlayerPrefs.Save();
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
            PlayerPrefs.SetFloat("v_Enviroment", volumiSfx);
            PlayerPrefs.Save();
        }
    }
}
