using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tipi di relazione possibili
/// </summary>
public enum Amicizie
{
    Neutro = 1,
    Alleato = 2,
    Nemico = 3
}

[System.Serializable]
public class GameData : ScriptableObject
{
    public string[] classiEssere = new string[Enum.GetValues(typeof(classiPersonaggi)).Length];
    public classiAmicizie[] matriceAmicizie = new classiAmicizie[Enum.GetValues(typeof(classiPersonaggi)).Length];
    public int[] indexPercorsi; //index riferimento al nome percorso (vedi EditorPercorsiClass) 
}
[System.Serializable]
public class classiAmicizie
{
    public Amicizie[] elementoAmicizia = new Amicizie[Enum.GetValues(typeof(classiPersonaggi)).Length];
}


