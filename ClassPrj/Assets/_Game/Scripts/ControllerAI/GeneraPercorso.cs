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

    private Transform[] percorso;


    public Transform[] Itinerario(TipoPercorso sceltaPercorso)
    {
        switch (sceltaPercorso)
        {
            case TipoPercorso.A:
                percorso = PercorsoA;
                break;
            case TipoPercorso.B:
                percorso = PercorsoB;
                break;
            case TipoPercorso.C:
                percorso = PercorsoC;
                break;
            default:
                break;
        }

        return (percorso);
    }


}

