using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class PercorsoWizard : ScriptableWizard
{

    public GameObject padrePercorso;
    public Color colorePadre=Color.black;
   

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
        errorString = "Per Inserire i WayPoint dentro ai padri..selezionarli , premere ALT+Ctrl e click sulla scena";
      
    }

    void OnWizardOtherButton()
    {
        GameObject tmpGbj = new GameObject("Percorso");
        padrePercorso = tmpGbj;
        Selection.activeTransform = padrePercorso.transform;
        tmpGbj.transform.position = Vector3.zero;
        GestorePercorso tmpGeneraPercorso = tmpGbj.AddComponent<GestorePercorso>();
        tmpGeneraPercorso.colore =colorePadre;
  

    }

}