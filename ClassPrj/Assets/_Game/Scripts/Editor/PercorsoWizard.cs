using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class PercorsoWizard : ScriptableWizard
{
    private GameObject padrePercorso;
    public GameObject percorso;
    public Color colore=Color.black;

    [MenuItem("Window/AggiungiPercorso Wizard")]
    static void CreateWizard()
    {
        PercorsoWizard me = ScriptableWizard.DisplayWizard<PercorsoWizard>("Genera Percorso", "Esci","Aggiungi Percorsi");

    }

    void OnWizardCreate()
    {
       
    }

    void OnDrawGizmos()
    {


    }

    void OnWizardUpdate()
    {
        errorString = "Per Inserire i WayPoint dentro ai padri..selezionarli , premere ALT+Ctrl e click sulla scena *** Assegnare i nomi dei percorsi dentro al Percorso stesso";
     
    }

    void OnWizardOtherButton()
    {
        padrePercorso = GameObject.Find("PadrePercorso");
        if (!padrePercorso) padrePercorso = new GameObject("PadrePercorso");
        padrePercorso.AddComponent<PadreGestore>();
        GameObject tmpGbj = new GameObject("Percorso");
        tmpGbj.transform.parent = padrePercorso.transform;
        percorso = tmpGbj;
        Selection.activeTransform = percorso.transform;
        percorso.transform.position = Vector3.zero;
        GestorePercorso tmpGeneraPercorso = percorso.AddComponent<GestorePercorso>();
        tmpGeneraPercorso.colore =colore;
  

    }

}