using UnityEngine;
using System.Collections;

public class DataBase : MonoBehaviour
{

    private static DataBase me;

    public GameData databseInizialeAmicizie;
    public Percorsi databaseInizialePercorsi;
    public caratteristichePersonaggioV2 databaseInizialeProprieta;

    public static void Inizializza()
    {
        if (GameObject.Find("DataBaseSingleton(Clone)") == null)
            Instantiate(Resources.Load("DataBaseSingleton"));
    }
    public static GameData GiveMeAmicizie()
    {
        return me.databseInizialeAmicizie;
    }
    public static Percorsi GiveMePercorsi()
    {
        return me.databaseInizialePercorsi;
    }
    public static caratteristichePersonaggioV2 GiveMeProprieta()
    {
        return me.databaseInizialeProprieta;
    }
    private void Awake()
    {

        me = this;
    }

}
