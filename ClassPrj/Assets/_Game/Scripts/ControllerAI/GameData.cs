using UnityEngine;


[System.Serializable]
public class GameData : ScriptableObject
{
    public enum Amicizie
    {
        Neutro = 1,
        Alleato = 2,
        Nemico = 3
    }
    public string[] tagEssere = new string[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
    public classeAmicizie[] matriceAmicizie = new classeAmicizie[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
    public classeTexture[] matriceIcone = new classeTexture[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
    public classeBottoni[] matriceBottoni = new classeBottoni[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
}

[System.Serializable]
public class classeAmicizie
{
    public GameData.Amicizie[] elementoAmicizia = new GameData.Amicizie[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
}
[System.Serializable]
public class classeBottoni
{
    public bool[] elementoBottoni = new bool[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
}
[System.Serializable]
public class classeTexture
{
    public Texture[] iconeBottoni = new Texture[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
}