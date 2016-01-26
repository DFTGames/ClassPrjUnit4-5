using UnityEngine;
using UnityEngine.EventSystems;

public class Cursore : MonoBehaviour
{
    public Texture2D immagineCursore;
    public Texture2D immagineNormale;
    public Texture2D immagineToccare;
    public Texture2D immagineParlare;
    public Texture2D immagineCombattere;
    public Texture2D immagineRaccogliere;

    private int cursorSizeX = 32;
    private int cursorSizeY = 32;
    private RaycastHit hit;

    private void Start()
    {
        Cursor.visible = true;
        immagineCursore = immagineNormale;
    }

    private void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, -1, QueryTriggerInteraction.Ignore) && !EventSystem.current.IsPointerOverGameObject())
        {
            switch (hit.transform.gameObject.layer)
            {
                case (10):
                    immagineCursore = immagineToccare;
                    break;
                case (12):
                    immagineCursore = immagineRaccogliere;
                    break;
                case (11):
                    if (GameManager.dizionarioDiNemici["Player"].Contains(hit.transform.tag))
                        immagineCursore = immagineCombattere;
                    else
                        immagineCursore = immagineParlare;
                    break;
                default:
                    immagineCursore = immagineNormale;
                    break;
            }
        }
        else
            immagineCursore = immagineNormale;
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, cursorSizeX, cursorSizeY), immagineCursore);
    }
}