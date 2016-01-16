using UnityEngine;
using System.Collections;

public class TriggerPortale : MonoBehaviour {

    public string destinazione = "Villaggio";
    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            GestoreCanvasAltreScene.nomeProssimaScena = destinazione;
            GameManager.MemorizzaProssimaScena(destinazione,transform.name+"RitornoPortale");
            
        }
    }
}
