using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]

public class PercorsiClass : ScriptableObject
{
   // public Dictionary<int, string> nomePercorsi = new Dictionary<int, string>();  //sostituita il dizionario con 2 liste per problemi di serializzazione

    public List<int> indexPercorsi = new List<int>();
    public List<string> nomePercorsi = new List<string>();
    //POTEVO USARE SOLTANTO LA STRINGA CON I NOMI DEI PERCORSI...NON SO PERCHE PINO MI HA DETTO CHE DOVEVO FARE 2 LISTE...(I NUMERI CON A FIANCO I NOMI PERCORSI)
    //TUTTAVIA IN QUESTO MODO MEMORIZZO SOLTANTO NUMERI ANZICHE STRINGHE NEL GAMEDATE (Chissa che grande risparmio :(( )

}
