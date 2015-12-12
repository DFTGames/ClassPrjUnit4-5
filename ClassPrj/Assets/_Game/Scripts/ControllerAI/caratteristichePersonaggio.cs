using UnityEngine;
using System;
using System.Collections.Generic;

public enum TipidiMaga
{
    maschile,
    femminile
}

public enum Proprieta
{
    Character,
    Vita,
    Mana,
    Livello,
    Xp,
    Attacco,
    Difesa
}

[System.Serializable]
public class caratteristichePersonaggio : ScriptableObject
{
    public  GameObject character_M;
    public GameObject character_F;

    public string Maschio = "Maschio";
    public string Femmine = "Femmine";
    public string[] schieraProprietaC = new string[Enum.GetValues(typeof(Proprieta)).Length];
    public List<ClasseValorPersonaggio> matriceProprietaM;
    public List<ClasseValorPersonaggio> matriceProprietaF;
    public List<String> classePersonaggio=new List<string>();
}


[System.Serializable]
public class ClasseValorPersonaggio
{
    public string nome="nessun nome";
    public float Vita = 10f;
    public int Livello = 0;
    public float Mana = 10f;
    public float Xp = 0f;
    public float Attacco = 10f;
    public float difesa = 20f;
}


