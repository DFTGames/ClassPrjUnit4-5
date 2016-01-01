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
    public string[] tagEssere = new string[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
    public classiAmicizie[] matriceAmicizie = new classiAmicizie[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
    public int[] indexPercorsi; //index riferimento al nome percorso (vedi EditorPercorsiClass) 
}
[System.Serializable]
public class classiAmicizie
{
    public Amicizie[] elementoAmicizia = new Amicizie[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
}


