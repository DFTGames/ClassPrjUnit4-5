using UnityEngine;
using System.Collections;

/*
Descrizione eella classe "GeneraPercorso" nel Gdd

Questo script viene attaccato a un gameobject ,"GeneraPercorso", nella hierarchy e al suo interno 
vengono messi dei gameobject vuoti che corrispondono a punti posizione nella scena .
L'ordine di sequenza di tali punti mi delinea il percorso che verra seguito dal soggetto designato

Variabili :
-una enumerazione sul TipoPercorso , con dentro il nome di ciascun percorso
-Schiera di gameobject contententi i posizionatori

Lo script viene eseguito nella scena in quanto c'e il metodo void OnDrawGizmos()
che me lo esegue in questo contesto.

Ogni qualvolta si attiva la scena cliccandoci sopra viene eseguito un ciclo dove ciascuna 
scheera di posizionatori viene sistemata a un offset di distanza sopra l'oggetto che sta
sopra la sua verticale, in modo da avere un allineamento corretto anche al di sopra di supercifi

NOMI VARIABILI :
 - Transform[] PercorsoA,PercorsoB,PercorsoC

ENUMERAZIONE :
 - TipoPercorso.A,TipoPercorso.B,TipoPercorso.C

*/


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
        if (Application.isPlaying)
            return;
        {

            for (int i = 0; i < PercorsoA.Length; i++)
            {
               
                Ray raggio = new Ray(new Vector3(PercorsoA[i].position.x, 1000f, PercorsoA[i].position.z), Vector3.down);
                if (Physics.Raycast(raggio, out hitinfo))
                {
                    nuovaPosizione = new Vector3(PercorsoA[i].position.x, hitinfo.point.y + offsetSpostaOggetto, PercorsoA[i].position.z);
                    PercorsoA[i].position = nuovaPosizione;
                }
                Gizmos.DrawIcon(PercorsoA[i].position, "3.png", true);

            }
            for (int i = 0; i < PercorsoB.Length; i++)
            {
                Ray raggio = new Ray(new Vector3(PercorsoB[i].position.x, 1000f, PercorsoB[i].position.z), Vector3.down);
                if (Physics.Raycast(raggio, out hitinfo))
                {
                    nuovaPosizione = new Vector3(PercorsoB[i].position.x, hitinfo.point.y + offsetSpostaOggetto, PercorsoB[i].position.z);
                    PercorsoB[i].position = nuovaPosizione;
                }
                Gizmos.DrawIcon(PercorsoB[i].position, "2.png", true);
            }

            for (int i = 0; i < PercorsoC.Length; i++)
            {
                Ray raggio = new Ray(new Vector3(PercorsoC[i].position.x, 1000f, PercorsoC[i].position.z), Vector3.down);
                if (Physics.Raycast(raggio, out hitinfo))
                {
                    nuovaPosizione = new Vector3(PercorsoC[i].position.x, hitinfo.point.y + offsetSpostaOggetto, PercorsoC[i].position.z);
                    PercorsoC[i].position = nuovaPosizione;
                }
                Gizmos.DrawIcon(PercorsoC[i].position, "1.png", true);
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

