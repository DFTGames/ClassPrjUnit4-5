using UnityEngine;
using System.Collections;

public class AudioZona : MonoBehaviour {

	private TipiPassi t_passiPrecedente ;

    void OnTriggerEnter(Collider Other)
    {
        Area area = Other.GetComponent<Area>();
        if (area != null)
        {
            Debug.Log("Entro!");

            t_passiPrecedente = GestoreAudio.T_PassiCorrente;
            GestoreAudio.T_PassiCorrente = area.tipiPassiArea;
        }
    }

    void OnTriggerExit(Collider Other)
    {
        Area area = Other.GetComponent<Area>();
        if (area != null)
        {
            GestoreAudio.T_PassiCorrente = t_passiPrecedente;
        }
    }
}
