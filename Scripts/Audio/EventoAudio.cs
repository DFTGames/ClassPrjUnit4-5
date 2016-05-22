using UnityEngine;
using System.Collections;

public class EventoAudio : MonoBehaviour {

public void Evento_Audio()
    {
        FMODUnity.RuntimeManager.PlayOneShot(GestoreAudio.EventoPassi(), transform.position);
    }
}
