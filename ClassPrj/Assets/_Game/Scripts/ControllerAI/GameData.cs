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
}
[System.Serializable]
public class classiAmicizie
{
    public Amicizie[] elementoAmicizia = new Amicizie[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
}

[System.Serializable]
public class classeBottoni
{
    public bool[] elementoBottoni = new bool[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
}
