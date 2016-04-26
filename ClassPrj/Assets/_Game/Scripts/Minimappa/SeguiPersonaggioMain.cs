using UnityEngine;
using System.Collections;

public class SeguiPersonaggioMain : MonoBehaviour {

    private Transform playerT;
    // Use this for initialization
    void Start()
    {
        playerT = Statici.PersonaggioPrincipaleT;        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerT.position.x, transform.position.y, playerT.position.z-5f);        
    }
}
