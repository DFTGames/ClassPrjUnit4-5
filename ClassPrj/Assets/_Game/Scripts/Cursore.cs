using UnityEngine;

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

    // private RaycastHit hit;
    private bool ignoraTrigger = false;

    private void Start()
    {
        Cursor.visible = true;
        immagineCursore = immagineNormale;
    }

    private void Update()
    {
        immagineCursore = immagineNormale;
        RaycastHit[] hit = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), float.MaxValue, -1, ignoraTrigger ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);
        //if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, -1, QueryTriggerInteraction.Ignore))
        if (hit != null)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                switch (hit[i].transform.gameObject.layer)
                {
                    case (10):
                        immagineCursore = immagineToccare;
                 
                        break;

                    case (12):
                        immagineCursore = immagineRaccogliere;

                        break;

                    case (11):
                        if (hit[i].collider.GetComponent<CapsuleCollider>() && hit[i].collider.GetComponent<DatiPersonaggio>()!=null)
                        {
                            if (GameManager.dizionarioDiNemici[GameManager.personaggio.miaClasse].Contains(hit[i].collider.GetComponent<DatiPersonaggio>().miaClasse))

                                immagineCursore = immagineCombattere;
                            else

                                immagineCursore = immagineParlare;
                        }
                       
                        break;

                    case (9):
                        ignoraTrigger = true;
                        immagineCursore = immagineNormale;
                        break;

                    default:
                        ignoraTrigger = false;
                        immagineCursore = immagineNormale;
                        
                        break;
                }
            }
        }
        else
        {
            ignoraTrigger = false;
            immagineCursore = immagineNormale;
        }
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, cursorSizeX, cursorSizeY), immagineCursore);
    }
}