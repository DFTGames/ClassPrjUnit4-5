using UnityEngine;

public class SeguiPersonaggioMain : MonoBehaviour
{
    private Transform playerT;

    // Use this for initialization
    private void Start()
    {
        if (!Statici.multigiocatoreOn)
            playerT = Statici.PersonaggioPrincipaleT;
        else
            playerT = Statici.playerLocaleGO.transform;
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerT == null)
            return;
        transform.position = new Vector3(playerT.position.x, transform.position.y, playerT.position.z - 5f);
    }
}