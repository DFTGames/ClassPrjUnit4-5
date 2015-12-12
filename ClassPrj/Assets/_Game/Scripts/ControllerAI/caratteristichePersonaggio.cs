using UnityEngine;
using System;
using System.Collections.Generic;

public enum TipidiMaga
{
    maschile,
    Femminile
}
//SI
// Come diceva Pino per ogni razza o tipo maschio femmina 
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
    public GameObject character;
    public string[] schieraTipiR = new string[Enum.GetValues(typeof(TipidiMaga)).Length];
    public string[] schieraProprietaC = new string[Enum.GetValues(typeof(Proprieta)).Length];
    public valoriPropieta[] matriceProprieta = new valoriPropieta[Enum.GetValues(typeof(TipidiMaga)).Length];
    public List<String> classePersonaggio;
}

[System.Serializable]
public class valoriPropieta
{
    public ClasseValorPersonaggio[] elementoProprieta = new ClasseValorPersonaggio[Enum.GetValues(typeof(Proprieta)).Length];
}

[System.Serializable]
public class ClasseValorPersonaggio
{
    public string nome;
    public float Vita = 10f;
    public int Livello = 0;
    public float Mana = 10f;
    public float Xp = 0f;
    public float Attacco = 10f;
    public float difesa = 20f;
}


