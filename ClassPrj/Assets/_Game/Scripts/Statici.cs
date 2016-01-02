using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;

public static class Statici
{
    public const string NomeFilePersonaggio = "Personaggio.dat";
    public const string nomeFileDiplomazia = "diplomazia.dat";
    public static string nomePersonaggio = "nomePersonaggio";

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
    }

    private static void Conn_StateChange(object sender, System.Data.StateChangeEventArgs e)
    {
        if (e.CurrentState == System.Data.ConnectionState.Open)
        {
            Debug.Log("Aperta!");
            conn.Close();
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


