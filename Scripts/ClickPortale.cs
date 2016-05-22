using UnityEngine;
using System.Collections;

public class ClickPortale : MonoBehaviour {
    
    private string destinazione = string.Empty;
    private Vector3 distanza;
    private ParticleSystem particelle;
    private Transform playerT=null;

	// Use this for initialization
	void Start () {
    
        destinazione = gameObject.GetComponentInParent<TriggerPortale>().destinazione;
        particelle = GetComponent<ParticleSystem>();
        if (!Statici.multigiocatoreOn)
            playerT = Statici.PersonaggioPrincipaleT;
        else
            playerT = Statici.playerLocaleGO.transform;
    }

  
    /// <summary>
    /// se clicco col mouse sul portale e il player è vicino, cambio scena.
    /// la distanza tra portale e player viene calcolata in onmouseover.
    /// </summary>
    void OnMouseDown()
    {      

        if (distanza.magnitude < 5f)
        {
            if (!Statici.multigiocatoreOn)
            {
                GestoreCanvasAltreScene.nomeProssimaScena = destinazione;
                Statici.datiPersonaggio.Dati.posizioneCheckPoint = transform.parent.name + "RitornoPortale";
                Statici.datiPersonaggio.Dati.nomeScena = destinazione;
                Statici.datiPersonaggio.Salva();
                StartCoroutine(GestoreCanvasAltreScene.ScenaInCarica(destinazione));
            }
            else
            {
                GestoreCanvasAltreScene.nomeProssimaScena = destinazione;
                Statici.posizioneInizialeMulti = transform.parent.name + "RitornoPortale";
                ManagerNetwork.CambiaScenaPortale(destinazione);
            }
        }
     
    }
 

    /// <summary>
    /// se il mouse è sopra il portale, controllo se il giocatore è ad una distanza dal portale 
    /// inferiore ad un certo valore che sarebbe il valore necessario a renderlo funzionante.
    /// nel caso si trovi al di sotto di questo valore, quindi il player è abbastanza vicino al portale,
    /// il portale diventa più intenso, altrimenti se tolgo il mouse o non sono alla distanza corretta,
    /// l'intensità del portale torna normale.   
    /// </summary>
    void OnMouseOver()
    {     
            distanza = (playerT.position - transform.position);
            if (distanza.magnitude < 5f && particelle.maxParticles != 2)
                particelle.maxParticles = 2;
            else if (distanza.magnitude > 5f && particelle.maxParticles != 1)
                particelle.maxParticles = 1;

      
    }
    void OnMouseExit()
    {
        particelle.maxParticles = 1;
       
    }

}
