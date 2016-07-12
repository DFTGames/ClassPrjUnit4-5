using UnityEngine;

public class Cursore : MonoBehaviour
{
    public Texture2D immagineCombattere;
    public Texture2D immagineCursore;
    public Texture2D immagineNormale;
    public Texture2D immagineParlare;
    public Texture2D immagineRaccogliere;
    public Texture2D immagineToccare;

    private static Cursore me;
    private classiPersonaggi classePlayer;
    private int cursorSizeX = 32;
    private int cursorSizeY = 32;

    // private RaycastHit hit;
    private bool ignoraTrigger = false;

    /// <summary>
    /// Questo metodo serve per cambiare l'immagine del cursore.
    /// </summary>
    /// <param name="numeroLayer"></param>
    /// <param name="miaClasse"></param>
    public static void CambiaCursore(int numeroLayer, int miaClasse)
    {
        switch (numeroLayer)
        {
            case (10)://layer Toccare: se è un oggetto cliccabile e vuoi che spunti la mano col dito.
                me.immagineCursore = me.immagineToccare;
                break;

            case (12)://layer Raccogliere: l'immagine è di una mano quasi chiusa come se devi raccogliere qualcosa.
                me.immagineCursore = me.immagineRaccogliere;
                break;

            case (11)://layer EssereVivente: l'immagine può essere un fumetto se si è amici o una spadina se si è nemici.
                if (!Statici.multigiocatoreOn)
                {
                    if (Statici.dizionarioDiNemici[Statici.personaggio.IdMiaClasse].Contains(miaClasse))
                        me.immagineCursore = me.immagineCombattere;
                    else
                        me.immagineCursore = me.immagineParlare;
                }
                else
                {      //immagine combattere perchè nel death match sono tutti contro tutti, e comunque le amicizie in multiplayer dovrebbero eventualmente essere decise dal server.
                    me.immagineCursore = me.immagineCombattere;
                }
                break;

            default://altri layer : l'immagine è quella di default cioè una freccia dorata.
                me.immagineCursore = me.immagineNormale;
                break;
        }
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, cursorSizeX, cursorSizeY), immagineCursore);
    }

    private void Start()
    {
        me = this;
        Cursor.visible = true;
        immagineCursore = immagineNormale;
    }
}