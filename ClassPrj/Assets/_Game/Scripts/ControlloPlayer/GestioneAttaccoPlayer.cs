using UnityEngine;

public class GestioneAttaccoPlayer : MonoBehaviour
{
    private DatiPersonaggio datiPersonaggio;
    private GestoreCanvasNetwork gestoreCanvasNetowork;
    private ManagerNetwork managerNetwork;

    private void OnMouseExit()
    {
        if (Statici.inGioco && Statici.multigiocatoreOn)
            gestoreCanvasNetowork.ResettaScrittaNemicoAttaccato(false);
    }

    private void OnMouseOver()
    {
        if (!Statici.inGioco || !Statici.multigiocatoreOn)
            return;
        gestoreCanvasNetowork.VisualizzaDatiUserSelezionato(datiPersonaggio.Nome, datiPersonaggio.Vita);
    }

    private void OnMouseUp()
    {
        if (!Statici.inGioco || !Statici.multigiocatoreOn || (Statici.multigiocatoreOn && datiPersonaggio.SonoUtenteLocale) || datiPersonaggio.Vita <= 0f)
            return;
        managerNetwork.NemicoColpito(datiPersonaggio.Utente);
    }

    // Use this for initialization
    private void Start()
    {
        if (!Statici.inGioco)
            return;
        datiPersonaggio = GetComponent<DatiPersonaggio>();
        if (Statici.multigiocatoreOn)
        {
            gestoreCanvasNetowork = GameObject.Find("ManagerCanvasMultiplayer").GetComponent<GestoreCanvasNetwork>();
            managerNetwork = GameObject.Find("ManagerNetwork").GetComponent<ManagerNetwork>();
        }
    }
}