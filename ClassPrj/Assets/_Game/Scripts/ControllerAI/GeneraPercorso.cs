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

    public LayerMask layersToFade ;
    private RaycastHit hitinfo;
    private Vector3 nuovaPosizione;
    public bool aderenzaTerreno = false;
    private float quantoSpostaOggettoSeColpito=0.5f;
    //RISOLVERE IL PROBLEMA DEL LAYERMASK NEL RAYCAST
    void OnDrawGizmos()
    {
          //CHIEDERE A PINO SE E' CORRETTO QUESTO PROCEDIMENTO...E DOVE C'E 0 METTERE LA Y DEL GOBLIN
            if (!aderenzaTerreno)
            {
                for (int i = 0; i < PercorsoA.Length; i++)
                {
                    Vector3 tempPos = new Vector3(PercorsoA[i].position.x, 0, PercorsoA[i].position.z);
                    PercorsoA[i].position = tempPos;
                }
                for (int i = 0; i < PercorsoB.Length; i++)
                {         
                    Vector3 tempPos = new Vector3(PercorsoB[i].position.x, 0, PercorsoB[i].position.z);
                    PercorsoB[i].position = tempPos;
                }
                for (int i = 0; i < PercorsoC.Length; i++)
                {
                    Vector3 tempPos = new Vector3(PercorsoC[i].position.x, 0, PercorsoC[i].position.z);
                    PercorsoC[i].position = tempPos;
                }
                aderenzaTerreno = true;
            }
       //PINO PERDONAMI
        for (int i = 0; i < PercorsoA.Length; i++)
        {
            Gizmos.DrawIcon(PercorsoA[i].position, "3.png", true);
            Ray raggio =new Ray(new Vector3(PercorsoA[i].position.x, 1000f, PercorsoA[i].position.z), Vector3.down);
            if (Physics.Raycast(raggio, out hitinfo, layersToFade.value))
            {
                nuovaPosizione = new Vector3(PercorsoA[i].position.x, PercorsoA[i].position.y, hitinfo.point.z + quantoSpostaOggettoSeColpito);
                PercorsoA[i].position = nuovaPosizione;
            }
        }
        for (int i = 0; i < PercorsoB.Length; i++)
        {
            Gizmos.DrawIcon(PercorsoB[i].position, "2.png", true);
            Ray raggio = new Ray(new Vector3(PercorsoB[i].position.x, 1000f, PercorsoB[i].position.z), Vector3.down);
            if (Physics.Raycast(raggio, out hitinfo))
            {
                nuovaPosizione = new Vector3(PercorsoB[i].position.x, PercorsoB[i].position.y, hitinfo.point.z + quantoSpostaOggettoSeColpito);
                PercorsoB[i].position = nuovaPosizione;
            }
        }
        for (int i = 0; i < PercorsoC.Length; i++)
        {
            Gizmos.DrawIcon(PercorsoC[i].position, "1.png", true);
            Ray raggio = new Ray(new Vector3(PercorsoC[i].position.x, 1000f, PercorsoC[i].position.z), Vector3.down);
            if (Physics.Raycast(raggio, out hitinfo))
            {
                nuovaPosizione = new Vector3(PercorsoC[i].position.x, PercorsoC[i].position.y, hitinfo.point.z + quantoSpostaOggettoSeColpito);
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

