using UnityEngine;
using System;
using System.Collections.Generic;

public enum ProprietaPersonaggio
{
   
    CharacterM,
    CharacterF,
    Classe,
    Vita,
    Livello,
    Mana,  
    Xp,
    Attacco,
    Difesa
}


[System.Serializable]
public class caratteristichePersonaggioV2 : ScriptableObject
{
   
    public  GameObject character_M;
    public GameObject character_F;

   
    public string[] schieraProprietaC = new string[Enum.GetValues(typeof(ProprietaPersonaggio)).Length];
    public List<ClasseValorPersonaggioV2> matriceProprieta;
    
    public List<String> classePersonaggio = new List<string>();
}

[System.Serializable]
public class ClasseValorPersonaggioV2
{
    
    public string nomeM="nessun nome";
    public string nomeF = "nessun nome";
    public string classe = "nessuna classe";
    public float Vita = 10f;
    public int Livello = 0;
    public float Mana = 10f;
    public float Xp = 0f;
    public float Attacco = 10f;
    public float difesa = 20f;
}


