using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
Descrizione della classe "GeneraPercorso" nel Gdd

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
    public List<Transform> Percorso = new List<Transform>();

    public Color colore = Color.black;
    private RaycastHit hitinfo;
    private Vector3 nuovaPosizione;
    public float offsetSpostaOggetto = 0.5f;

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        if (Percorso.Count > 0)
        {

            for (int i = 0; i < Percorso.Count; i++)
            {

                Ray raggio = new Ray(new Vector3(Percorso[i].transform.position.x, 1000f, Percorso[i].transform.position.z), Vector3.down);
                if (Physics.Raycast(raggio, out hitinfo))
                {
                    nuovaPosizione = new Vector3(Percorso[i].transform.position.x, hitinfo.point.y + offsetSpostaOggetto, Percorso[i].transform.position.z);
                    Percorso[i].transform.position = nuovaPosizione;
                }
                //   Gizmos.DrawIcon(Percorso[i].transform.position, "3.png", true);

            }
        }

    }

    public List<Transform> Itinerario(TipoPercorso sceltaPercorso)
    {

        switch (sceltaPercorso)
        {
            case TipoPercorso.A:
                return Percorso;

            default: return null;


        }


    }

}