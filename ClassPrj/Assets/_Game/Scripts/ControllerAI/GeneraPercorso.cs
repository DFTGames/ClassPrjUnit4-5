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

    private RaycastHit hitinfo;
    private Vector3 nuovaPosizione;
    public float offsetSpostaOggetto = 0.5f;

    void OnDrawGizmos()
    {
        for (int i = 0; i < PercorsoA.Length; i++)
        {
            Gizmos.DrawIcon(PercorsoA[i].position, "3.png", true);
            Ray raggio = new Ray(new Vector3(PercorsoA[i].position.x, 1000f, PercorsoA[i].position.z), Vector3.down);
            if (Physics.Raycast(raggio, out hitinfo))
            {
                nuovaPosizione = new Vector3(PercorsoA[i].position.x, hitinfo.point.y + offsetSpostaOggetto, PercorsoA[i].position.z);
                PercorsoA[i].position = nuovaPosizione;
            }

        }
        for (int i = 0; i < PercorsoB.Length; i++)
        {
            Gizmos.DrawIcon(PercorsoB[i].position, "2.png", true);
            Ray raggio = new Ray(new Vector3(PercorsoB[i].position.x, 1000f, PercorsoB[i].position.z), Vector3.down);
            if (Physics.Raycast(raggio, out hitinfo))
            {
                nuovaPosizione = new Vector3(PercorsoB[i].position.x, hitinfo.point.y + offsetSpostaOggetto, PercorsoB[i].position.z);
                PercorsoB[i].position = nuovaPosizione;
            }
        }

        for (int i = 0; i < PercorsoC.Length; i++)
        {
            Gizmos.DrawIcon(PercorsoC[i].position, "1.png", true);
            Ray raggio = new Ray(new Vector3(PercorsoC[i].position.x, 1000f, PercorsoC[i].position.z), Vector3.down);
            if (Physics.Raycast(raggio, out hitinfo))
            {
                nuovaPosizione = new Vector3(PercorsoC[i].position.x, hitinfo.point.y + offsetSpostaOggetto, PercorsoC[i].position.z);
                PercorsoC[i].position = nuovaPosizione;
            }

        }
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

