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
    /// Lo script metodo charlie e serializza percorsi sono due metodi statici perchè vengono richiamati
    /// in due script cioè manager iniziale e gamemanager.
    /// Per evitare di avere due metodi identici dentro entrambi gli script li ho spostati qui e resi statici
    /// in modo che siano scritti una volta sola e richiamati dove si vuole.
    /// </summary>
    /// <param name="databseInizialeAmicizie"></param>
    /// <param name="databaseInizialeProprieta"></param>
    public static void Metodo_Charlie(ref GameData databseInizialeAmicizie,ref caratteristichePersonaggioV2 databaseInizialeProprieta)   //metodo per assegnare gli asset dentro l'inspector... By Luca del dftStudent
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
    }

    public static void SerializzaPercorsi(ref GameData databseInizialeAmicizie, ref Serializzabile<AmicizieSerializzabili> datiDiplomazia, ref Dictionary<string, int> dizionarioPercorsi)   //Controlla e se necessario riserializza i percorsi
    {
        //Controlla i percorsi se sono gia serializzati e se ci sono variazioni li reserializza
        //se non sono serializzati li serializza
        //se non ci sono variazioni non fa niente

        if (databseInizialeAmicizie == null) return;

        if (datiDiplomazia == null)

            datiDiplomazia = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);

        if (datiDiplomazia.Dati.indexPercorsi.Equals(databseInizialeAmicizie.indexPercorsi)) return;  //CONTROLLARE SE METODO E' CORRETTO

        for (int i = 0; i < databseInizialeAmicizie.tagEssere.Length; i++)

        {
            datiDiplomazia.Dati.indexPercorsi[i] = databseInizialeAmicizie.indexPercorsi[i];
        }

        datiDiplomazia.Salva();

        // AGGIUNTO PER IL DIZIONARIO SUI PERCORSO
        dizionarioPercorsi.Clear();

        for (int i = 0; i < datiDiplomazia.Dati.tipoEssere.Length; i++)
            dizionarioPercorsi.Add(datiDiplomazia.Dati.tipoEssere[i], datiDiplomazia.Dati.indexPercorsi[i]);


    }
}




