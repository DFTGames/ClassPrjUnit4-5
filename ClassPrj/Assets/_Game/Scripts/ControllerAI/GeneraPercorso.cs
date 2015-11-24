using UnityEngine;
using System.Collections;

public enum TipoPercorso
{
    A,
    B,
    C
}


public class GeneraPercorso : MonoBehaviour
{

    public Transform[] PercorsoA;
    public Transform[] PercorsoB;
    public Transform[] PercorsoC;
   
    void OnDrawGizmos()
    {
       for (int i=0; i<PercorsoA.Length;i++)
            Gizmos.DrawIcon(PercorsoA[i].position, "3.png", true);
        for (int i = 0; i < PercorsoB.Length; i++)
            Gizmos.DrawIcon(PercorsoB[i].position, "2.png", true);
        for (int i = 0; i < PercorsoC.Length; i++)
            Gizmos.DrawIcon(PercorsoC[i].position, "1.png", true);
    }

    public Transform[] Itinerario(TipoPercorso sceltaPercorso)
    {
        switch (sceltaPercorso)
        {
            case TipoPercorso.A:
                return PercorsoA;         
            case TipoPercorso.B:
               return PercorsoB;              
            case TipoPercorso.C:
                return PercorsoC;               
            default: return null;               
        }    
    }
   
}

