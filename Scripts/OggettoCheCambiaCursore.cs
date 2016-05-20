using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Questa classe deve essere attaccata ad un oggetto su cui vogliamo che l'immagine del cursore cambi,
/// come un personaggio, un oggetto da raccogliere o un oggetto interattivo come un portale.
/// Tali oggetti devono far parte di determinati Layer: layer Toccare, EssereVivente o Raccogliere.
/// Se l'oggetto non fa parte di uno di questi layer, il cursore rimarrà con l'immagine di default
/// cioè una freccia dorata.
/// In più, questa classe fa anche questo:se l'oggetto è un personaggio, comparirà anche un cerchio 
/// ai piedi, quando il mouse si trova sopra ad esso.
/// </summary>
public class OggettoCheCambiaCursore : MonoBehaviour
{
    private DatiPersonaggio datiPersonaggio;
    private LayerMask layer;
    private GameObject luceSelezioneGO;
    private classiPersonaggi miaClasse;

    private void OnMouseExit()
    {
       
        if (!Statici.inGioco)
            return;
        Cursore.CambiaCursore(0, 0);
        if (luceSelezioneGO != null)
            luceSelezioneGO.SetActive(false);
    }

    private void OnMouseOver()
    {
        if (!Statici.inGioco)
            return;
        Cursore.CambiaCursore(gameObject.layer, miaClasse);
        if (luceSelezioneGO != null)
            luceSelezioneGO.SetActive(true);
    }
  

    private void Start()
    {
        if (!Statici.inGioco)
            return;
        datiPersonaggio = GetComponent<DatiPersonaggio>();
        if (datiPersonaggio != null)
        {
            miaClasse = GetComponent<DatiPersonaggio>().miaClasse;
            luceSelezioneGO = transform.FindChild("luceSelezione").gameObject;
        }
    }
   
}