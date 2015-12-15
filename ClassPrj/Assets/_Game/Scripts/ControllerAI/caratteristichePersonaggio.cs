using System;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject character_M;
    public GameObject character_F;
    public string Maschio = "Maschio";
    public string Femmine = "Femmina";
    public string[] schieraProprietaC = new string[Enum.GetValues(typeof(Proprieta)).Length];
    public List<ClasseValorPersonaggio> matriceProprietaM;
    public List<ClasseValorPersonaggio> matriceProprietaF;
}

[System.Serializable]
public class ClasseValorPersonaggio
{
    public string razza = string.Empty;
    public string nome = "nessun nome";
    public float Vita = 10f;
    public int Livello = 0;
    public float Mana = 10f;
    public float Xp = 0f;
    public float Attacco = 10f;
    public float difesa = 20f;
}