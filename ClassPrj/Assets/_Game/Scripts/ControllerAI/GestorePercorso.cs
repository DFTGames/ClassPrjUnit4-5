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


public class GestorePercorso : MonoBehaviour
{
    //public List<Transform> Percorso = new List<Transform>();
    public Transform this [int idx]
    {
        get
        {
            if (idx < transform.childCount)
                return transform.GetChild(idx);
            else return transform.GetChild(0);
        }
    }

    public Color colore = Color.black;
    private RaycastHit hitinfo;
    private Vector3 nuovaPosizione;
    public float offsetSpostaOggetto = 0.5f;

  
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        if (transform.childCount > 0)
        {
            Transform[] path = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                path[i] = transform.GetChild(i);
                Ray raggio = new Ray(new Vector3(this[i].transform.position.x, 1000f, this[i].transform.position.z), Vector3.down);
                if (Physics.Raycast(raggio, out hitinfo))
                {
                    nuovaPosizione = new Vector3(this[i].transform.position.x, hitinfo.point.y + offsetSpostaOggetto, this[i].transform.position.z);
                    this[i].transform.position = nuovaPosizione;
                }      

            }
            
            if (transform.childCount>1)
            iTween.DrawPathGizmos(path, colore);
           // iTween.DrawLineHandles(path, colore);
        }

    }

}