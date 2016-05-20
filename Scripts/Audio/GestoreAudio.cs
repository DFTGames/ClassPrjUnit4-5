using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum TipiPassi
{
    Terreno,
    Ghiaia,
    Pietra,
    Acqua,
    Fango,
    legno
}

public class GestoreAudio : MonoBehaviour
{

    [FMODUnity.EventRef]
    public string passiTerreno = "event:/FootSteps/FootStepsGround";
    [FMODUnity.EventRef]
    public string passiGhiaia = "event:/FootSteps/FootStepsGravel";
    [FMODUnity.EventRef]
    public string passiLegno = "event:/FootSteps/FootStepsWood";
    [FMODUnity.EventRef]
    public string passiPietra = "event:/FootSteps/FootStepsConcrete";
    [FMODUnity.EventRef]
    public string passiAcqua = "event:/FootSteps/FootStepsWater";
    [FMODUnity.EventRef]
    public string passiFango = "event:/FootSteps/FootStepsMud";

    private static GestoreAudio g_Audio = null;

    private TipiPassi t_passiCorrente = TipiPassi.Terreno;

    public static TipiPassi T_PassiCorrente
    {
        get
        {
            return G_Audio.t_passiCorrente;
        }
        set
        {
            G_Audio.t_passiCorrente = value;
        }
    }
    public static string EventoPassi()
    {
        switch (T_PassiCorrente)
        {
            case TipiPassi.Terreno:
                return G_Audio.passiTerreno;
            case TipiPassi.Fango:
                return G_Audio.passiFango;
            case TipiPassi.legno:
                return G_Audio.passiLegno;
            case TipiPassi.Ghiaia:
                return G_Audio.passiGhiaia;
            case TipiPassi.Pietra:
                return G_Audio.passiPietra;
            case TipiPassi.Acqua:
                return G_Audio.passiAcqua;
            default:
                return G_Audio.passiTerreno;
        }
    }

    public static GestoreAudio G_Audio
    {
        get
        {
            if (g_Audio == null)
            {
                GameObject GestoreAudio = new GameObject("Gestore_Audio");
                g_Audio = GestoreAudio.AddComponent<GestoreAudio>();
                GameObject.DontDestroyOnLoad(GestoreAudio);
            }
            return g_Audio;
        }
    }

    void OnDestroy()
    {
        g_Audio = null;
    }
}
