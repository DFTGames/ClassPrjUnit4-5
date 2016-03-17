using UnityEngine;
using System.Collections;

public class TriggerCheckPoint : MonoBehaviour {

    private DatiPersonaggio datiGiocatore;   
    void OnTriggerEnter(Collider coll)
    {       
        datiGiocatore = coll.GetComponent<DatiPersonaggio>();
      if (datiGiocatore!=null && datiGiocatore.Giocabile)
        {
            GameManager.datiPersonaggio.Dati.posizioneCheckPoint = transform.name;
            GameManager.datiPersonaggio.Salva();
        }

    }  
}
