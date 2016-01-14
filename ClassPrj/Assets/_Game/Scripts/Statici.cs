using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;

public class Statici
{
    public const string NomeFilePersonaggio = "Personaggio.dat";
    public const string nomeFileDiplomazia = "diplomazia.dat";
    public static string nomePersonaggio = string.Empty;

    public const string STR_PercorsoConfig = "PercorsoConfigurazione";
    public const string STR_DatabaseDiGioco = "/dataBaseDiGioco.asset";

    public const string STR_PercorsoConfig2 = "PercorsoConfigurazione";  //Path memorizzazione del Percorso
    public const string STR_DatabaseDiGioco2 = "/dataBasePercorso.asset";

    public const string STR_PercorsoConfig3 = "PercorsoConfigurazione";
    public const string STR_DatabaseDiGioco3 = "/dataBasePersonaggioV2.asset";

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
}

/*
namespace DFTGames.Tools.EditorTools
{
    public static class Statici2
    {
        
         public static List<string> percorsiDisponibili = new List<string>();
    }
}
*/


