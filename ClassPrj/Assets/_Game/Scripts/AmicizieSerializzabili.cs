// Questo è un nuovo commento e pure cambiato

using UnityEngine;
using System.Collections; // Commento
using System;

[System.Serializable]
public class AmicizieSerializzabili {
    
    public string[] tipoEssere = new string[Enum.GetValues(typeof(classiPersonaggi)).Length];

    public classiAmicizie[] matriceAmicizie = new classiAmicizie[Enum.GetValues(typeof(classiPersonaggi)).Length];
  
  
}
