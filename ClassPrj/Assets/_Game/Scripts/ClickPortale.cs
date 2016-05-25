using UnityEngine;
using UnityEngine.UI;

public class ClickPortale : MonoBehaviour
{
    private string destinazione = string.Empty;
    private Vector3 distanza;
    private ParticleSystem particelle;
    private Transform playerT = null;
    private bool portaleAttivato = false;

    /// <summary>
    /// se clicco col mouse sul portale e il player è vicino, cambio scena.
    /// la distanza tra portale e player viene calcolata in onmouseover.
    /// </summary>
    private void OnMouseDown()
    {
        Cursore.CambiaCursore(0, 0);
        if (portaleAttivato)//se ho cliccato una volta ed sono a meno di 5 metri se riclicco il portale non deve funzionare
            return;
        if (distanza.magnitude < 5f)
        {
            portaleAttivato = true;
            if (!Statici.multigiocatoreOn)
            {
                GestoreCanvasAltreScene.nomeProssimaScena = destinazione;
                Statici.datiPersonaggio.Dati.posizioneCheckPoint = transform.parent.name + "RitornoPortale";
                Statici.datiPersonaggio.Dati.nomeScena = destinazione;
                Statici.datiPersonaggio.Salva();
                GameObject immagineCaricamento = GestoreCanvasAltreScene.ImmagineCaricamento;
                Text casellaScrittaCaricamento = GestoreCanvasAltreScene.NomeScenaText;
                StartCoroutine(GestoreCanvasAltreScene.ScenaInCarica(destinazione, destinazione, immagineCaricamento, casellaScrittaCaricamento));
            }
            else
            {
                GestoreCanvasAltreScene.nomeProssimaScena = destinazione;
                Statici.posizioneInizialeMulti = transform.parent.name + "RitornoPortale";
                ManagerNetwork.CambiaScenaPortale(destinazione);
            }
        }
    }

    private void OnMouseExit()
    {
        particelle.maxParticles = 1;
    }

    /// <summary>
    /// se il mouse è sopra il portale, controllo se il giocatore è ad una distanza dal portale
    /// inferiore ad un certo valore che sarebbe il valore necessario a renderlo funzionante.
    /// nel caso si trovi al di sotto di questo valore, quindi il player è abbastanza vicino al portale,
    /// il portale diventa più intenso, altrimenti se tolgo il mouse o non sono alla distanza corretta,
    /// l'intensità del portale torna normale.
    /// </summary>
    private void OnMouseOver()
    {
        if (portaleAttivato)
            return;
        distanza = (playerT.position - transform.position);
        if (distanza.magnitude < 5f && particelle.maxParticles != 2)
            particelle.maxParticles = 2;
        else if (distanza.magnitude > 5f && particelle.maxParticles != 1)
            particelle.maxParticles = 1;
    }

    // Use this for initialization
    private void Start()
    {
        destinazione = gameObject.GetComponentInParent<TriggerPortale>().destinazione;
        particelle = GetComponent<ParticleSystem>();
        if (!Statici.multigiocatoreOn)
            playerT = Statici.PersonaggioPrincipaleT;
        else
            playerT = Statici.playerLocaleGO.transform;
    }
}