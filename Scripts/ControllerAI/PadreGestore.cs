using UnityEngine;
using System.Collections;

public class PadreGestore : MonoBehaviour
{
    public GestorePercorso this[int idx]
    {
        get
        {
            GestorePercorso tmp = null;
          
            for (int i = 0; i < transform.childCount; i++)
            {

                if (transform.GetChild(i).GetComponent<GestorePercorso>().IndexPercorso == idx)
                {
                    tmp = transform.GetChild(i).GetComponent<GestorePercorso>();
                    return tmp;
                }
            }
            return tmp;
        }
    }
}
