using UnityEngine;
using System.Collections;

public class InterpolazioneRemota : MonoBehaviour {

    private Vector3 posizioneDesiderata;
    private Quaternion rotazioneDesiderata;

    private float dampingFactor = 10f;

    void Start()
    {
        posizioneDesiderata = this.transform.position;
        rotazioneDesiderata = this.transform.rotation;
    }

    public void SetTransform(Vector3 posizione, Quaternion rotazione, bool interpolate)
    {
        if (interpolate)
        {
            posizioneDesiderata = posizione;
            rotazioneDesiderata = rotazione;
        }
        else
        {
            this.transform.position = posizione;
            this.transform.rotation = rotazione;
        }
    }

    void Update()
    {
        this.transform.position = Vector3.Lerp(transform.position, posizioneDesiderata, Time.deltaTime * dampingFactor);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, rotazioneDesiderata, Time.deltaTime * dampingFactor);
    }
}
