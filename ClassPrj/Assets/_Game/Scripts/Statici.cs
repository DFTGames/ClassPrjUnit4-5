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
    public static Dictionary<classiPersonaggi, List<classiPersonaggi>> dizionarioDiNemici = new Dictionary<classiPersonaggi, List<classiPersonaggi>>();
    public static Dictionary<classiPersonaggi, List<classiPersonaggi>> dizionarioDiAmici = new Dictionary<classiPersonaggi, List<classiPersonaggi>>();
    public static Dictionary<classiPersonaggi, List<classiPersonaggi>> dizionarioDiIndifferenti = new Dictionary<classiPersonaggi, List<classiPersonaggi>>();
    public static Transform PersonaggioPrincipaleT;
    public static PadreGestore padreGestore;
   // public static Dictionary<string, int> dizionarioPercorsi = new Dictionary<string, int>();
    public static DatiPersonaggio personaggio;
    public static Serializzabile<ValoriPersonaggioS> datiPersonaggio;
    public static Dictionary<int, DatiPersonaggio> registroDatiPersonaggi = new Dictionary<int, DatiPersonaggio>();
    public static caratteristichePersonaggioV2 databaseInizialeProprieta;
    public static Percorsi databaseInizialePercorsi;
    public static  GameData databseInizialeAmicizie;
    public static string classeDiColuiCheVuoleCambiareAmicizia = string.Empty;//da verificare se servirà ancora o no

    public const string STR_PercorsoConfig = "PercorsoConfigurazione";
    public const string STR_DatabaseDiGioco = "/dataBaseDiGioco.asset";

    public const string STR_PercorsoConfig2 = "PercorsoConfigurazione";  //Path memorizzazione del Percorso
    public const string STR_DatabaseDiGioco2 = "/dataBasePercorso.asset";

    public const string STR_PercorsoConfig3 = "PercorsoConfigurazione";
    public const string STR_DatabaseDiGioco3 = "/dataBasePersonaggioV2.asset";

    public static bool sonoPassatoDallaScenaIniziale = false;

    public const string tmpPercorsi = "nome Percorso";

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
    /// <summary>
    /// Lo script metodo assegnaAssetDatabase e serializza percorsi sono due metodi statici perchè vengono richiamati
    /// in due script cioè manager iniziale e gamemanager.
    /// Per evitare di avere due metodi identici dentro entrambi gli script li ho spostati qui e resi statici
    /// in modo che siano scritti una volta sola e richiamati dove si vuole.
    /// </summary>
    /// <param name="databseInizialeAmicizie"></param>
    /// <param name="databaseInizialeProprieta"></param>
   
    public static void assegnaAssetDatabase()   //metodo per assegnare gli asset dentro l'inspector... By Luca del dftStudent
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
                databaseInizialePercorsi = AssetDatabase.LoadAssetAtPath<Percorsi>(percorso + Statici.STR_DatabaseDiGioco2);
           
            }
        }
    }

  
    /// <summary>
    /// salva in un dizionario il personaggio e le sue caratteristiche (vita, livello ecc)
    /// </summary>
    /// <param name="datiPersonaggioDaRegistrare"></param>
     public static void RegistraDatiPersonaggio(DatiPersonaggio datiPersonaggioDaRegistrare)
    {
        registroDatiPersonaggi.Add(datiPersonaggioDaRegistrare.GetInstanceID(), datiPersonaggioDaRegistrare);
        RecuperaDati(datiPersonaggioDaRegistrare);
    }
    /// <summary>
    /// recupera i dati del personaggio(giocatore o AI) e li assegna
    /// </summary>
    /// <param name="datiStatistici"></param>
    public static void RecuperaDati(DatiPersonaggio datiStatistici)
    {
        int tmpID = datiStatistici.GetInstanceID();
        int indice = databaseInizialeProprieta.classePersonaggio.IndexOf(registroDatiPersonaggi[tmpID].miaClasse.ToString());
        registroDatiPersonaggi[tmpID].Giocabile = databaseInizialeProprieta.matriceProprieta[indice].giocabile;
        if (!registroDatiPersonaggi[tmpID].Giocabile)
        { //se è un personaggio AI recupero i dati dallo scriptble object
            registroDatiPersonaggi[tmpID].VitaMassima = databaseInizialeProprieta.matriceProprieta[indice].Vita;
            registroDatiPersonaggi[tmpID].Vita = databaseInizialeProprieta.matriceProprieta[indice].Vita;
            registroDatiPersonaggi[tmpID].ManaMassimo = databaseInizialeProprieta.matriceProprieta[indice].Mana;
            registroDatiPersonaggi[tmpID].Mana = databaseInizialeProprieta.matriceProprieta[indice].Mana;
            registroDatiPersonaggi[tmpID].Livello = databaseInizialeProprieta.matriceProprieta[indice].Livello;
            registroDatiPersonaggi[tmpID].XpMassimo = databaseInizialeProprieta.matriceProprieta[indice].Xp;
            registroDatiPersonaggi[tmpID].Xp = databaseInizialeProprieta.matriceProprieta[indice].Xp;
            registroDatiPersonaggi[tmpID].Attacco = databaseInizialeProprieta.matriceProprieta[indice].Attacco;
            registroDatiPersonaggi[tmpID].Difesa = databaseInizialeProprieta.matriceProprieta[indice].difesa;
        }
        else
        {  //se è giocabile recupero i dati dal file serializzato
            registroDatiPersonaggi[tmpID].VitaMassima = Statici.datiPersonaggio.Dati.VitaMassima;
            registroDatiPersonaggi[tmpID].Vita = Statici.datiPersonaggio.Dati.Vita;
            registroDatiPersonaggi[tmpID].ManaMassimo = Statici.datiPersonaggio.Dati.ManaMassimo;
            registroDatiPersonaggi[tmpID].Mana = Statici.datiPersonaggio.Dati.Mana;
            registroDatiPersonaggi[tmpID].Livello = Statici.datiPersonaggio.Dati.Livello;
            registroDatiPersonaggi[tmpID].XpMassimo = Statici.datiPersonaggio.Dati.XPMassimo;
            registroDatiPersonaggi[tmpID].Xp = Statici.datiPersonaggio.Dati.Xp;
            registroDatiPersonaggi[tmpID].Attacco = Statici.datiPersonaggio.Dati.Attacco;
            registroDatiPersonaggi[tmpID].Difesa = Statici.datiPersonaggio.Dati.difesa;
            registroDatiPersonaggi[tmpID].Nome = Statici.datiPersonaggio.Dati.nomePersonaggio;
            PersonaggioPrincipaleT.GetComponentInChildren<TextMesh>().text = registroDatiPersonaggi[tmpID].Nome;
            GestoreCanvasAltreScene.AggiornaDati(datiStatistici);
            Statici.personaggio = datiStatistici;
            classeDiColuiCheVuoleCambiareAmicizia = datiStatistici.miaClasse.ToString();
        }
    }

}





