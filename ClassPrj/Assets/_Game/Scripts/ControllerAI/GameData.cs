using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;

[System.Serializable]
public class GameData : ScriptableObject
{

    public enum Amicizie
    {
        Neutro,
        Alleato,
        Nemico
    }

    public enum TipiDiEsseriA
    {
        goblinAggressivo =1,
        goblinNeutro  =2,
        goblinBuono =3,
        Player =4,
    }
    public List<classeSogg> listaClasse = new List<classeSogg>();
    
}

[System.Serializable]
public class classeSogg 
{
    public GameData.TipiDiEsseriA EsseriA;
    public GameData.Amicizie[,] Amicizie = new GameData.Amicizie[10,10];
    
}