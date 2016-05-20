using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcoOnOff : MonoBehaviour {

    public GameObject armi;

    private FSM cervello;
    private bool arcoAttivo = true;
    
	// Use this for initialization
	void Start () {
        cervello =GetComponent<FSM>();
	}

    // Update is called once per frame
    void Update()
    {
        if (cervello.DatiPersonaggio.Vita <= 0 && arcoAttivo)
        {          
            armi.SetActive(false);
            arcoAttivo = false;
        }
        else if (cervello.DatiPersonaggio.Vita > 0 && !arcoAttivo)
        {           
            armi.SetActive(true);
            arcoAttivo = true;
        }
    }
}
