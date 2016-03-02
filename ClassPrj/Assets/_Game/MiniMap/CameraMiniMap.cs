using UnityEngine;
using System.Collections;

public class CameraMiniMap : MonoBehaviour {

   public int altezzaCamera=50;

	void Update () {

        if (GameManager.PersonaggioPrincipaleT != null)
            
            transform.position = new Vector3(GameManager.PersonaggioPrincipaleT.position.x, altezzaCamera, GameManager.PersonaggioPrincipaleT.position.z);
        else Debug.LogError("Un somaro vicino a te sembra una volpe.....NON RIESCE A CERCARE L'ISTANZA DEL PERSONAGGIO...che mi combini ..Pirlun??");
    }
}
