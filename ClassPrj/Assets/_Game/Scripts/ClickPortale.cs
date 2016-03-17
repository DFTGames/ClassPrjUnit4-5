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
        playerT = GameManager.PersonaggioPrincipaleT;
    }

  
    /// <summary>
    /// se clicco col mouse sul portale e il player è vicino, cambio scena.
    /// la distanza tra portale e player viene calcolata in onmouseover.
    /// </summary>
    void OnMouseDown()
    {      

        if (distanza.magnitude < 5f)
        {
            GestoreCanvasAltreScene.nomeProssimaScena = destinazione;        
            GameManager.datiPersonaggio.Dati.posizioneCheckPoint = transform.parent.name + "RitornoPortale";
            GameManager.datiPersonaggio.Dati.nomeScena = destinazione;
            GameManager.datiPersonaggio.Salva();
            StartCoroutine(GestoreCanvasAltreScene.ScenaInCarica(destinazione));
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
