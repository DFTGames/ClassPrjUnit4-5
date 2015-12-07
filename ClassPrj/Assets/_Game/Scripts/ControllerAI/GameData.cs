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
    public classeAmicizie[] matriceAmicizie = new classeAmicizie[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
    //Usato per memorizzare lo stato dei bottoni (cliccato o non cliccato)
//public classeBottoni[] matriceBottoni = new classeBottoni[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
}

[System.Serializable]
public class classeAmicizie
{
    public Amicizie[] elementoAmicizia = new Amicizie[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
}

[System.Serializable]
public class classeBottoni
{
    public bool[] elementoBottoni = new bool[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
}
