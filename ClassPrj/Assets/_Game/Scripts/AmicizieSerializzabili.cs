using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class AmicizieSerializzabili {
    
    public string[] tipoEssere = new string[Enum.GetValues(typeof(classiPersonaggi)).Length];

    public classiAmicizie[] matriceAmicizie = new classiAmicizie[Enum.GetValues(typeof(classiPersonaggi)).Length];
  
    public int[] indexPercorsi = new int[Enum.GetValues(typeof(classiPersonaggi)).Length];  //Aggiunto index per memorizzazione percorso
  
}
