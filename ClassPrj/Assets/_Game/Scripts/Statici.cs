using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;
using UnityEditor;

public class Statici
{
    public const string NomeFilePersonaggio = "Personaggio.dat";
    public const string nomeFileDiplomazia = "diplomazia.dat";
    public const string nomeFilePercorsi = "percorsi.dat";
    public const string NomeFileAudio = "Audio.dat";
    public static string nomePersonaggio = string.Empty;

    public const string STR_PercorsoConfig = "PercorsoConfigurazione";
    public const string STR_DatabaseDiGioco = "/dataBaseDiGioco.asset";

    public const string STR_PercorsoConfig2 = "PercorsoConfigurazione";  //Path memorizzazione del Percorso
    public const string STR_DatabaseDiGioco2 = "/dataBasePercorso.asset";

    public const string STR_PercorsoConfig3 = "PercorsoConfigurazione";
    public const string STR_DatabaseDiGioco3 = "/dataBasePersonaggioV2.asset";

    public static bool sonoPassatoDallaScenaIniziale = false;
   

    static string origine = Application.streamingAssetsPath + "/dbgioco.db";
    static string destinazione = Application.persistentDataPath + "/dbgioco.db";
    static SqliteConnection conn;
    public static void CopiaIlDB(bool sovrascrivi = false)
    {
        if (!File.Exists(destinazione) || sovrascrivi)
        {
            Debug.Log("Copio il DB in " + Application.persistentDataPath);
            File.Copy(origine, destinazione, sovrascrivi);
        }
        conn = new SqliteConnection("URI=file:" + destinazione);
        conn.StateChange += Conn_StateChange;
        conn.Open();
        while (conn.State != System.Data.ConnectionState.Open)
        {
            // Aspetta
        }
        conn.Close();
    }

    private static void Conn_StateChange(object sender, System.Data.StateChangeEventArgs e)
    {
        if (e.CurrentState == System.Data.ConnectionState.Open)
        {
            Debug.Log("Aperta!");
        }
        else if (e.CurrentState == System.Data.ConnectionState.Closed)
        {
            Debug.Log("Chiusa!");
        }
    }


    /// <summary>
    /// Lo script metodo assegnaAssetDatabase e serializza percorsi sono due metodi statici perchè vengono richiamati
    /// in due script cioè manager iniziale e gamemanager.
    /// Per evitare di avere due metodi identici dentro entrambi gli script li ho spostati qui e resi statici
    /// in modo che siano scritti una volta sola e richiamati dove si vuole.
    /// </summary>
    /// <param name="databseInizialeAmicizie"></param>
    /// <param name="databaseInizialeProprieta"></param>
    public static void assegnaAssetDatabase(ref GameData databseInizialeAmicizie,ref caratteristichePersonaggioV2 databaseInizialeProprieta,ref PercorsiClass databaseInizialePercorsi)   //metodo per assegnare gli asset dentro l'inspector... By Luca del dftStudent
    {
        if (databseInizialeAmicizie == null)
        {
            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig))
            {
                string percorso = EditorPrefs.GetString(Statici.STR_PercorsoConfig);
                databseInizialeAmicizie = AssetDatabase.LoadAssetAtPath<GameData>(percorso + Statici.STR_DatabaseDiGioco);
            }
        }
        if (databaseInizialeProprieta == null)
        {
            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig3))
            {
                string percorso = EditorPrefs.GetString(Statici.STR_PercorsoConfig3);
                databaseInizialeProprieta = AssetDatabase.LoadAssetAtPath<caratteristichePersonaggioV2>(percorso + Statici.STR_DatabaseDiGioco3);
            }
        }
        if (databaseInizialePercorsi == null)
        {
            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig2))
            {
                string percorso = EditorPrefs.GetString(Statici.STR_PercorsoConfig2);
                databaseInizialePercorsi = AssetDatabase.LoadAssetAtPath<PercorsiClass>(percorso + Statici.STR_DatabaseDiGioco2);
            }
        }
    }

    public static void SerializzaPercorsi(ref PercorsiClass databaseInizialePercorsi,ref Serializzabile<PercorsiClass> datiPercorsi)   //Controlla e se necessario riserializza i percorsi
    {
        //Controlla i percorsi se sono gia serializzati e se ci sono variazioni li reserializza
        //se non sono serializzati li serializza
        //se non ci sono variazioni non fa niente  

        //IMPLEMENTARE IL CONTROLLO..CHE SE C'E UN CAMBIAMENTO ...LO RISERIALIZZA..(un campo salvato nella classe)..chiedere al prof
  
        if (databaseInizialePercorsi == null) return;

        if (datiPercorsi == null)   //SI PUO' TOGLIERE LA PAARTE DELLA SERIALIZAZIONE DAL MANAGERINIZIALE E DAL GAMEMANAGER E LASCIARLO SOLO QUA?

            datiPercorsi = new Serializzabile<PercorsiClass>(Statici.nomeFilePercorsi);
        for (int i = 0; i < databaseInizialePercorsi.per.Count; i++)    //li trasferisco dal asset database al file serializzato
            datiPercorsi.Dati.per[i] = databaseInizialePercorsi.per[i];
        datiPercorsi.Salva();
 
    }
}




