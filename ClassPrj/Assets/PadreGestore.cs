using UnityEngine;
using System.Collections;

public class PadreGestore : MonoBehaviour {

  
public GestorePercorso this[int adx]
    {
        get
        {
            GestorePercorso tmp = transform.GetChild(adx).GetComponent<GestorePercorso>();
            if (tmp.IndexPercorso == adx) return tmp;
            else return null;

        }
     
    }
    

}
