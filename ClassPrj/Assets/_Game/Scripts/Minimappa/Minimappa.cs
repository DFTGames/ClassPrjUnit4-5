using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Minimappa : MonoBehaviour
{

    public Camera cameraMinimap;
    public float distanzaCamera = 130;
    public RawImage rawImage;
    public Sprite spriteAmico;
    public float misuraSpriteAmico = 20f;
    public Sprite spriteNemico;
    public float misuraSpriteNemico = 20f;
    public Sprite spritePortale;
    public float misuraSpritePortale = 30f;
    public List<DatiMarcatoreMulti> listaUserIdMarcati = new List<DatiMarcatoreMulti>();

    private Vector3 distanzaCameraOggetto;
    private Vector3 distanzaPlayerOggetto;
    private Vector2 nuovaposizioneOggettoInMappa;
    private OggettiDaMarcare oggettoDaMarcare;
    private float scala;
    private bool distanzaMassimaLetta = false;
    private Transform playerT;
    private float larghezzaMinimappa;
    private Vector2 massimo;

    public Transform PlayerT
    {
        get
        {
            return playerT;
        }

        set
        {
            playerT = value;
        }
    }

    public Vector2 Massimo
    {
        get
        {
            return massimo;
        }

        set
        {
            massimo = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        if (!Statici.multigiocatoreOn)
            PlayerT = Statici.PersonaggioPrincipaleT;
        else
            PlayerT = Statici.playerLocaleGO.transform;
        PlayerT.GetComponent<OggettiDaMarcare>().enabled = false;
        larghezzaMinimappa = rawImage.rectTransform.rect.width;
    }

    void Update()
    {
        cameraMinimap.transform.position = new Vector3(PlayerT.transform.position.x, distanzaCamera, PlayerT.transform.position.z);

    }

    public Vector2 CalcolaPosizioneMarcatore(Vector3 posizioneOggettoNelMondo)
    {
        distanzaCameraOggetto = posizioneOggettoNelMondo - cameraMinimap.transform.position;
        distanzaPlayerOggetto = posizioneOggettoNelMondo - PlayerT.position;
        nuovaposizioneOggettoInMappa = new Vector2(distanzaPlayerOggetto.x, distanzaPlayerOggetto.z) * larghezzaMinimappa / distanzaCameraOggetto.magnitude;
        return nuovaposizioneOggettoInMappa;
    }

    public void GestisciVisibilitaMarcatore(OggettiDaMarcare oggettoDaMarcare)
    {
        Vector2 distanzaFrecciaMarcatore = oggettoDaMarcare.Marcatore.transform.localPosition;//dovrei aggiungere: meno posizione della freccia ma la posizione della freccia in locale è sempre 0.     
        Vector3 distanzaAttualePlayerOggetto = oggettoDaMarcare.gameObject.transform.position - PlayerT.position;
        scala = distanzaAttualePlayerOggetto.z / distanzaFrecciaMarcatore.y;
        if (distanzaAttualePlayerOggetto.magnitude > (larghezzaMinimappa * 0.5f * scala))
        {
            if (oggettoDaMarcare.nascondiSeFuoriMappa)
                oggettoDaMarcare.NascondiMarcatore();
            else
            {
                if (!distanzaMassimaLetta)
                {
                    Massimo = distanzaFrecciaMarcatore;
                    distanzaMassimaLetta = true;
                }
                oggettoDaMarcare.BloccaOggetto = true;
            }
        }
        else
        {
            if (oggettoDaMarcare.nascondiSeFuoriMappa)
                oggettoDaMarcare.VisualizzaMarcatore();
            oggettoDaMarcare.BloccaOggetto = false;
            distanzaMassimaLetta = false;
        }
    }

    /// <summary>
    /// caso multiplayer: se un utente esce dalla scena devo distruggere il suo marcatore.
    /// </summary>
    /// <param name="userID"></param>
    public void DistruggiMarcatore(int userID)
    {
        for(int i = 0; i < listaUserIdMarcati.Count; i++)
        {
            if(listaUserIdMarcati[i].idUtente==userID)
            {
                Destroy(listaUserIdMarcati[i].gameObject);
                listaUserIdMarcati.RemoveAt(i);
            }
        }

    }

}
