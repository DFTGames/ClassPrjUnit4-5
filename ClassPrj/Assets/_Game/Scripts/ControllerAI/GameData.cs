using UnityEngine;

[System.Serializable]
public class GameData : ScriptableObject
{
    public enum Amicizie
    {
        Neutro,
        Alleato,
        Nemico
    }

    public enum TipoEsseri
    {
        goblinAggressivo,
        goblinNeutro,
        goblinBuono,
        Player,
        Elfo
    }

    public string[] tipoEssere = new string[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];

    public classeAmicizie[] matriceAmicizie = new classeAmicizie[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
}

[System.Serializable]
public class classeAmicizie
{
    public GameData.Amicizie[] elementoAmicizia = new GameData.Amicizie[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
}