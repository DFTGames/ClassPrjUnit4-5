using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]

public class PercorsiClass : ScriptableObject
{
   // public Dictionary<int, string> nomePercorsi = new Dictionary<int, string>();  //sostituita il dizionario con 2 liste per problemi di serializzazione

    public List<int> indexPercorsi = new List<int>();
    public List<string> nomePercorsi = new List<string>();


}
