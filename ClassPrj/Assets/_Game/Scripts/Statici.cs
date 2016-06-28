using Mono.Data.Sqlite;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//using UnityEditor;

public class Statici
{
    public const string NomeFileAudio = "Audio.dat";
    public const string nomeFileDiplomazia = "diplomazia.dat";
    public const string NomeFilePersonaggio = "Personaggio.dat";
    public const string STR_DatabaseDiGioco = "/dataBaseDiGioco.asset";
    public const string STR_DatabaseDiGioco2 = "/dataBasePercorso.asset";
    public const string STR_DatabaseDiGioco3 = "/dataBasePersonaggioV2.asset";
    public const string STR_PercorsoConfig = "PercorsoConfigurazione";

    public const string CMD_RICHIESTA_PERSONAGGI = "persg";
    public const string CMD_RICHIESTA_PERSONAGGI_UTENTE = "persU";
    public const string CMD_RICHIESTA_DIPLOMAZIA = "dip";
    public const string CMD_RICHIESTA_DIPLOMAZIA_PERSONAGGIO = "dipP";
    public const string CMD_RICHIESTA_TS_UTENTI = "tsUtenti";
    public const string CMD_NESSUN_PERSONAGGIO_TROVATO = "noPers";
    public const string CMD_INSERISCI_NUOVO_PERSONAGGIO = "insP";
    public const string CMD_CANCELLA_PERSONAGGIO = "delP";
    public const string CMD_RICHIESTA_SE_NOME_ESISTE = "nExist";
    

    //fine variabili multigiocatore
    public const string STR_PercorsoConfig2 = "PercorsoConfigurazione";

    //Path memorizzazione del Percorso
    public const string STR_PercorsoConfig3 = "PercorsoConfigurazione";

    public const string tmpPercorsi = "nome Percorso";
    public static string classeDiColuiCheVuoleCambiareAmicizia = string.Empty;
    public static Percorsi databaseInizialePercorsi;
    public static caratteristichePersonaggioV2 databaseInizialeProprieta;
    public static GameData databseInizialeAmicizie;
    public static Serializzabile<ValoriPersonaggioS> datiPersonaggio;
    public static DatiPersonaggio datiPersonaggioLocale;
    public static bool diplomaziaAggiornata = false;
    public static Dictionary<classiPersonaggi, List<classiPersonaggi>> dizionarioDiAmici = new Dictionary<classiPersonaggi, List<classiPersonaggi>>();
    public static Dictionary<classiPersonaggi, List<classiPersonaggi>> dizionarioDiIndifferenti = new Dictionary<classiPersonaggi, List<classiPersonaggi>>();
    public static Dictionary<classiPersonaggi, List<classiPersonaggi>> dizionarioDiNemici = new Dictionary<classiPersonaggi, List<classiPersonaggi>>();
    public static bool finePartita = false;

    //da verificare se servirà ancora o no
    public static bool inGioco = false;

    public static NetworkTransformInterpolation.InterpolationMode inter = NetworkTransformInterpolation.InterpolationMode.INTERPOLATION;

    //usera al posto di buildindex
    public static bool IsPointAndClick = true;

    public static string messaggio = "";

    //inizio variabili per multigiocatore:
    public static bool multigiocatoreOn = false;

    public static string nomeModello = string.Empty;
    public static string nomePersonaggio = string.Empty;
    public static int numeroPostoSpawn = -1;
    public static bool ownerUscito = false;
    public static PadreGestore padreGestore;
    public static bool partenza = false;
    public static DatiPersonaggio personaggio;
    public static Transform PersonaggioPrincipaleT;
    public static GameObject playerLocaleGO;
    public static GestoreInfoClassi playerRemotiGestore = new GestoreInfoClassi();
    public static string posizioneInizialeMulti = string.Empty;
    public static Dictionary<int, DatiPersonaggio> registroDatiPersonaggi = new Dictionary<int, DatiPersonaggio>();
    public static bool sonoPassatoDallaScenaIniziale = false;
    public static float tempoInvioTransform = 0.06f;
    public static int userLocaleId = 0;
    public static int contatoreTimeStampOk = 0;//contatore dei timeStamp aggiornati
    public static int contatoreTabelleDiUtentiOk = 0;//contatore delle tabelle aggiornate dopo il controllo del timestamp della tabella Utenti  
    public static int numeroTabelleAggTimeStamp = 0;//numero delle tabelle locali di cui si deve controllare il loro timeStamp:righe della tabella sincronizzazioneDB+tabella Utenti
    public static SqliteConnection conn;
    private static string destinazione = Application.persistentDataPath + "/dbgioco.db";
    private static string origine = Application.streamingAssetsPath + "/dbgioco.db";
    public static int idDB=0;
    public static SmartFox sfs;
    public static SFSArray arrayPersNewPersUt = new SFSArray();//array da passare ai metodi per aggiornare la tabella locale PersonaggiUtente se si crea un nuovo personaggio
    public static SFSArray arrayPersNewPersDipPers = new SFSArray();//array da passare ai metodi per aggiornare la tabella locale DiplomaziaPersonaggio se si crea un nuovo personaggio
    public static DatiPersonaggioPartenza valoriPersonaggioScelto = new DatiPersonaggioPartenza();//dati iniziali da considerare una volta scelto il personaggio sia da nuovo personaggio che da carica personaggio.

    public static bool EliminatoPersonaggioDaDbLocale(string nomePersonaggioDaEliminare)
    {
        bool valoreRitorno = false;
        SqliteTransaction tr = null;
        try
        {
            tr = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.Transaction = tr;
            int numeroRighe = 0;
         
           
            cmd.CommandText = @"UPDATE PersonaggiUtente SET eliminato = @eliminato WHERE nomePersonaggio = @nomePersonaggio";

            cmd.Parameters.Add(new SqliteParameter("@nomePersonaggio", nomePersonaggioDaEliminare));
            cmd.Parameters.Add(new SqliteParameter("@eliminato", 1));
            numeroRighe = cmd.ExecuteNonQuery();

            if (numeroRighe == 0)
            {
                ManagerIniziale.SollevaErroreScenaInizialeCaricamentoPg("nessuna riga modificata nella tabella Diplomazia");
                Debug.LogError("nessuna riga modificata nella tabella PersonaggiUtenti");
            }
            else
            {
                Debug.Log("ho aggiornato una riga della tabella PersonaggiUtenti");
                cmd.CommandText = @"UPDATE DiplomaziaPersonaggio SET eliminato = @eliminato WHERE PersonaggiUtente_nomePersonaggio = @nomePersonaggio";
                cmd.Parameters.Add(new SqliteParameter("@nomePersonaggio", nomePersonaggioDaEliminare));
                cmd.Parameters.Add(new SqliteParameter("@eliminato", 1));
                numeroRighe = cmd.ExecuteNonQuery();

                if (numeroRighe > 0)
                {
                    Debug.Log("ho aggiornato una riga della tabella DiplomaziaPersonaggio");
                    valoreRitorno = true;
                    tr.Commit();
                }else
                {
                    ManagerIniziale.SollevaErroreScenaInizialeCaricamentoPg("fallita eliminazione personaggio da DiplomaziaPersonaggio perchè il personaggio non esiste");
                    Debug.LogError("fallita eliminazione personaggio da DiplomaziaPersonaggio perchè il personaggio non esiste");
                }
            }

        }
        catch (Exception ex)
        {

            Debug.LogError("errore aggiornamento PersonaggiUtenti o DiplomaziaPersonaggio durante l'eliminazione personaggio: " + ex.Message);
            ManagerIniziale.SollevaErroreScenaInizialeCaricamentoPg("errore aggiornamento PersonaggiUtenti o DiplomaziaPersonaggio durante l'eliminazione personaggio: " + ex.Message);
            if (tr != null)
            {
                try
                {
                    tr.Rollback();

                }
                catch (SqliteException ex2)
                {
                    Debug.LogError("rollback della transazione di eliminazione personaggio fallito: " + ex2.ToString());
                    ManagerIniziale.SollevaErroreScenaInizialeCaricamentoPg("rollback della transazione di eliminazione personaggio fallito: " + ex2.ToString());
                }
                finally
                {
                    tr.Dispose();
                }
            }
        }

        return valoreRitorno;
    }

    public static bool AggiornaPersonaggi(SFSArray arrayPers)
    {
        bool valoreRitorno = false;
        SqliteTransaction tr = null;
        try
        {         
            tr = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.Transaction = tr;
            int numeroRighe = 0;
            int recordAgg = 0;
            for (int i = 0; i < arrayPers.Size(); i++)
            {
                SFSObject objArr = (SFSObject)arrayPers.GetSFSObject(i);
                int idPersonaggio = objArr.GetInt("idPersonaggio");
                int idClasse = objArr.GetInt("ClassiPersonaggi_idClassiPersonaggi");
                int eliminato = objArr.GetInt("eliminato");
                int giocabile = objArr.GetInt("giocabile");
                string modelloM = objArr.GetUtfString("modelloM");
                string modelloF = objArr.GetUtfString("modelloF");
                string nome = objArr.GetUtfString("nome");
                double vitaMassima = objArr.GetDouble("vitaMassima");
                double vitaAttuale = objArr.GetDouble("vitaAttuale");
                double manaMassimo = objArr.GetDouble("manaMassimo");
                double manaAttuale = objArr.GetDouble("manaAttuale");
                double livelloPartenza = objArr.GetDouble("livelloPartenza");
                double xpPartenza = objArr.GetDouble("xpPartenza");
                double attaccoBase = objArr.GetDouble("attaccoBase");
                double difesaBase = objArr.GetDouble("difesaBase");

                try
                {
                    cmd.CommandText = @"INSERT INTO Personaggi VALUES (@idPersonaggio,@idClasse, @eliminato,  @giocabile," +
                    " @modelloM, @modelloF, @nome, @vitaMassima, @vitaAttuale,  @manaMassimo, @manaAttuale, @livelloPartenza, @xpPartenza, @attaccoBase, @difesaBase)";
                    cmd.Parameters.Add(new SqliteParameter("@idPersonaggio", idPersonaggio));
                    cmd.Parameters.Add(new SqliteParameter("@idClasse", idClasse));
                    cmd.Parameters.Add(new SqliteParameter("@eliminato", eliminato));
                    cmd.Parameters.Add(new SqliteParameter("@giocabile", giocabile));
                    cmd.Parameters.Add(new SqliteParameter("@modelloM", modelloM));
                    cmd.Parameters.Add(new SqliteParameter("@modelloF", modelloF));
                    cmd.Parameters.Add(new SqliteParameter("@nome", nome));
                    cmd.Parameters.Add(new SqliteParameter("@vitaMassima", vitaMassima));
                    cmd.Parameters.Add(new SqliteParameter("@vitaAttuale", vitaAttuale));
                    cmd.Parameters.Add(new SqliteParameter("@manaMassimo", manaMassimo));
                    cmd.Parameters.Add(new SqliteParameter("@manaAttuale", manaAttuale));
                    cmd.Parameters.Add(new SqliteParameter("@livelloPartenza", livelloPartenza));
                    cmd.Parameters.Add(new SqliteParameter("@xpPartenza", xpPartenza));
                    cmd.Parameters.Add(new SqliteParameter("@attaccoBase", attaccoBase));
                    cmd.Parameters.Add(new SqliteParameter("@difesaBase", difesaBase));
                    numeroRighe = cmd.ExecuteNonQuery();
                    Debug.Log("Inserimento nella tabella Personaggi della riga: " + idPersonaggio);
                    recordAgg++;
                }
                catch
                {
                    
                    try
                    {
                        cmd.CommandText = @"UPDATE Personaggi SET idPersonaggio = @idPersonaggio, ClassiPersonaggi_idClassiPersonaggi = @idClasse ,eliminato = @eliminato" +
                                ", giocabile = @giocabile, modelloM = @modelloM, modelloF = @modelloF, nome = @nome, vitaMassima = @vitaMassima" +
                                ", vitaAttuale = @vitaAttuale, manaMassimo = @manaMassimo, manaAttuale = @manaAttuale, livelloPartenza = @livelloPartenza" +
                                ", xpPartenza = @xpPartenza, attaccoBase = @attaccoBase, difesaBase = @difesaBase WHERE idPersonaggio = @idPersonaggio";
                        cmd.Parameters.Add(new SqliteParameter("@idPersonaggio", idPersonaggio));
                        cmd.Parameters.Add(new SqliteParameter("@idClasse", idClasse));
                        cmd.Parameters.Add(new SqliteParameter("@eliminato", eliminato));
                        cmd.Parameters.Add(new SqliteParameter("@giocabile", giocabile));
                        cmd.Parameters.Add(new SqliteParameter("@modelloM", modelloM));
                        cmd.Parameters.Add(new SqliteParameter("@modelloF", modelloF));
                        cmd.Parameters.Add(new SqliteParameter("@nome", nome));
                        cmd.Parameters.Add(new SqliteParameter("@vitaMassima", vitaMassima));
                        cmd.Parameters.Add(new SqliteParameter("@vitaAttuale", vitaAttuale));
                        cmd.Parameters.Add(new SqliteParameter("@manaMassimo", manaMassimo));
                        cmd.Parameters.Add(new SqliteParameter("@manaAttuale", manaAttuale));
                        cmd.Parameters.Add(new SqliteParameter("@livelloPartenza", livelloPartenza));
                        cmd.Parameters.Add(new SqliteParameter("@xpPartenza", xpPartenza));
                        cmd.Parameters.Add(new SqliteParameter("@attaccoBase", attaccoBase));
                        cmd.Parameters.Add(new SqliteParameter("@difesaBase", difesaBase));
                        numeroRighe = cmd.ExecuteNonQuery();

                        Debug.Log("Aggiornamento tabella personaggi riga :" + idPersonaggio);
                        if (numeroRighe == 0)
                            ControllerLogin.SollevaErroreAggiornamentoDB("nessuna riga modificata nella tabella Personaggi");
                        else
                            recordAgg++;
                       
                    }
                    catch (Exception ex)
                    {
                        ControllerLogin.SollevaErroreAggiornamentoDB("errore nell'aggiornamento del db locale tabella Personaggi" + ex.Message);
                        Debug.LogError("errore nell'aggiornamento del db locale tabella Personaggi" + ex.Message);
                    }
                
                }
            }

            if (recordAgg == arrayPers.Size())
            {
                valoreRitorno = true;
                tr.Commit();
            }
      
        }
        catch (Exception ex)
        {

            Debug.LogError("errore aggiornamento archivio Personaggi: " + ex.Message);
            ControllerLogin.SollevaErroreAggiornamentoDB("errore aggiornamento archivio Personaggi: " + ex.Message);
            if (tr != null)
            {
                try
                {
                    tr.Rollback();

                }
                catch (SqliteException ex2)
                {
                    Debug.LogError("rollback della transazione riguardante l'aggiornamento della tabella Personaggi fallito: " + ex2.ToString());

                }
                finally
                {
                    tr.Dispose();
                }
            }
        }

        return valoreRitorno;
    }

   

    internal static void AggiornaTsUtenti(string timeStampRemotoNew)
    {
       
        SqliteTransaction tr = null;

        try
        {           
            tr = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.Transaction = tr;
            int numeroRighe = 0;

            cmd.CommandText = @"UPDATE Utenti SET timeStampUtente = @timeStampUtente WHERE idUtente=@idUtente";
            cmd.Parameters.Add(new SqliteParameter("@timeStampUtente", timeStampRemotoNew));
            cmd.Parameters.Add(new SqliteParameter("@idUtente", Statici.idDB));
            numeroRighe = cmd.ExecuteNonQuery();


            if (numeroRighe == 0)
                ControllerLogin.SollevaErroreAggiornamentoDB("errore nell'aggiornamento del timeStamp Utente");
            else
            {
                tr.Commit();            
                contatoreTimeStampOk++;
                ControllaNumeroTimeStampAggiornati();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Errore nell'aggiornamento del timeStamp Utente: " + ex.Message);
            ControllerLogin.SollevaErroreAggiornamentoDB("Errore nell'aggiornamento del timeStamp Utente: " + ex.Message);
            if (tr != null)
            {
                try
                {
                    tr.Rollback();

                }
                catch (SqliteException ex2)
                {
                    Debug.LogError("rollback della transazione riguardante l'aggiornamento della tabella SincronizzazioneDB fallito: " + ex2.ToString());

                }
                finally
                {
                    tr.Dispose();
                }
            }

        }     
        
    }

    internal static bool AggiornaDiplomaziaPersonaggio(SFSArray arrayPers)
    {
        bool valoreRitorno = false;
        SqliteTransaction tr = null;
        try
        {
            tr = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.Transaction = tr;
            int numeroRighe = 0;
            int recordAgg = 0;
            for (int i = 0; i < arrayPers.Size(); i++)
            {
                SFSObject objArr = (SFSObject)arrayPers.GetSFSObject(i);
                string nomePers = objArr.GetUtfString("PersonaggiUtente_nomePersonaggio");
                int idUtente = objArr.GetInt("PersonaggiUtente_Utenti_idUtente");                
                int idClasse1 = objArr.GetInt("Diplomazia_ClassiPersonaggi_idClasse");
                int idClasse2 = objArr.GetInt("Diplomazia_ClassiPersonaggi2_idClasse");
                int eliminato = objArr.GetInt("eliminato");
                int relazione = objArr.GetInt("relazione");

                try
                {
                    cmd.CommandText = @"INSERT INTO DiplomaziaPersonaggio VALUES (@nomePersonaggio, @PersonaggiUtente_Utenti_idUtente,"+
                        "@Diplomazia_ClassiPersonaggi_idClasse, @Diplomazia_ClassiPersonaggi2_idClasse, @eliminato,  @relazione)";
                    cmd.Parameters.Add(new SqliteParameter("@nomePersonaggio", nomePers));
                    cmd.Parameters.Add(new SqliteParameter("@PersonaggiUtente_Utenti_idUtente", idUtente));                   
                    cmd.Parameters.Add(new SqliteParameter("@Diplomazia_ClassiPersonaggi_idClasse", idClasse1));
                    cmd.Parameters.Add(new SqliteParameter("@Diplomazia_ClassiPersonaggi2_idClasse", idClasse2));
                    cmd.Parameters.Add(new SqliteParameter("@eliminato", eliminato));
                    cmd.Parameters.Add(new SqliteParameter("@relazione", relazione));

                    numeroRighe = cmd.ExecuteNonQuery();
                    if(numeroRighe>0)
                     Debug.Log("ho inserito 1 riga nella tabella DiplomaziaPersonaggio:nome personaggio:"+ nomePersonaggio+" idUtente: "+idUtente+" idClasse1: "+idClasse1+" idClasse2: "+idClasse2);
                    recordAgg++;
                }
                catch
                {
                    try
                    {
                        cmd.CommandText = @"UPDATE DiplomaziaPersonaggio SET eliminato = @eliminato, relazione = @relazione " +
                            "WHERE PersonaggiUtente_nomePersonaggio = @nomePersonaggio AND PersonaggiUtente_Utenti_idUtente = @idUtente " +
                            "AND Diplomazia_ClassiPersonaggi_idClasse = @idClasse1 AND Diplomazia_ClassiPersonaggi2_idClasse = @idClasse2";
                        cmd.Parameters.Add(new SqliteParameter("@nomePersonaggio", nomePers));
                        cmd.Parameters.Add(new SqliteParameter("@idUtente", idUtente));                        
                        cmd.Parameters.Add(new SqliteParameter("@idClasse1", idClasse1));
                        cmd.Parameters.Add(new SqliteParameter("@idClasse2", idClasse2));
                        cmd.Parameters.Add(new SqliteParameter("@eliminato", eliminato));
                        cmd.Parameters.Add(new SqliteParameter("@relazione", relazione));
                        numeroRighe = cmd.ExecuteNonQuery();

                        if (numeroRighe == 0)
                        {
                            ControllerLogin.SollevaErroreAggiornamentoDB("errore:impossibile aggiornare la riga nella tabella DiplomaziaPersonaggio:nome personaggio:" + nomePersonaggio + " idUtente: " + idUtente + " idClasse1: " + idClasse1 + " idClasse2: " + idClasse2);
                            Debug.LogError("errore: impossibile aggiornare la riga nella tabella DiplomaziaPersonaggio:nome personaggio:" + nomePersonaggio + " idUtente: " + idUtente + " idClasse1: " + idClasse1 + " idClasse2: " + idClasse2);
                        }
                        else
                        {
                            Debug.Log("ho aggiornato 1 riga nella tabella DiplomaziaPersonaggio:nome personaggio:" + nomePersonaggio + " idUtente: " + idUtente + " idClasse1: " + idClasse1 + " idClasse2: " + idClasse2);
                            recordAgg++;
                        }

                    }
                    catch (Exception ex)
                    {
                        ControllerLogin.SollevaErroreAggiornamentoDB("errore nell'aggiornamento del db locale DiplomaziaPersonaggio" + ex.Message);
                        Debug.LogError("errore nell'aggiornamento del db locale DiplomaziaPersonaggio" + ex.Message);
                    }

                }
            }

            if (recordAgg == arrayPers.Size())
            {
                contatoreTabelleDiUtentiOk++;
                Debug.Log("numero tabelle di utenti aggiornate:" + contatoreTabelleDiUtentiOk);
                tr.Commit();
                valoreRitorno = true;
               
            }

        }
        catch (Exception ex)
        {

            Debug.LogError("errore aggiornamento archivio DiplomaziaPersonaggio: " + ex.Message);
            ControllerLogin.SollevaErroreAggiornamentoDB("errore aggiornamento archivio DiplomaziaPersonaggio: " + ex.Message);
            if (tr != null)
            {
                try
                {
                    tr.Rollback();

                }
                catch (SqliteException ex2)
                {
                    Debug.LogError("rollback della transazione riguardante l'aggiornamento della tabella DiplomaziaPersonaggio fallito: " + ex2.ToString());

                }
                finally
                {
                    tr.Dispose();
                }
            }
        }

        return valoreRitorno;
    }

    public static bool AggiornaDiplomazia(SFSArray arrayPers)
    {
        bool valoreRitorno = false;
        SqliteTransaction tr = null;
        try
        {
            tr = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.Transaction = tr;
            int numeroRighe = 0;
            int recordAgg = 0;
            for (int i = 0; i < arrayPers.Size(); i++)
            {
                SFSObject objArr = (SFSObject)arrayPers.GetSFSObject(i);
                int idClasse1 = objArr.GetInt("ClassiPersonaggi_idClasse");
                int idClasse2 = objArr.GetInt("ClassiPersonaggi2_idClasse");
                int eliminato = objArr.GetInt("eliminato");
                int relazione = objArr.GetInt("relazione");             

                try
                {
                    cmd.CommandText = @"INSERT INTO Diplomazia VALUES (@idClasse1,@idClasse2, @eliminato,  @relazione)";
                    cmd.Parameters.Add(new SqliteParameter("@idClasse1", idClasse1));
                    cmd.Parameters.Add(new SqliteParameter("@idClasse2", idClasse2));
                    cmd.Parameters.Add(new SqliteParameter("@eliminato", eliminato));
                    cmd.Parameters.Add(new SqliteParameter("@relazione", relazione));                  
                    numeroRighe = cmd.ExecuteNonQuery();
                    Debug.Log("ho inserito 1 riga nella tabella Diplomazia");
                    recordAgg++;
                }
                catch
                {

                    try
                    {
                        cmd.CommandText = @"UPDATE Diplomazia SET ClassiPersonaggi_idClasse = @idClasse1, ClassiPersonaggi2_idClasse = @idClasse2 ," +
                            "eliminato = @eliminato, relazione=@relazione WHERE ClassiPersonaggi_idClasse = @idClasse1 AND ClassiPersonaggi2_idClasse=@idClasse2";

                        cmd.Parameters.Add(new SqliteParameter("@idClasse1", idClasse1));
                        cmd.Parameters.Add(new SqliteParameter("@idClasse2", idClasse2));
                        cmd.Parameters.Add(new SqliteParameter("@eliminato", eliminato));
                        cmd.Parameters.Add(new SqliteParameter("@relazione", relazione));
                        numeroRighe = cmd.ExecuteNonQuery();

                        if (numeroRighe == 0)
                        {
                            ControllerLogin.SollevaErroreAggiornamentoDB("nessuna riga modificata nella tabella Diplomazia");
                            Debug.LogError("nessuna riga modificata nella tabella Diplomazia");
                        }
                        else
                        {
                            Debug.Log("ho aggiornato una riga della tabella Diplomazia");
                            recordAgg++;
                        }

                    }
                    catch (Exception ex)
                    {
                        ControllerLogin.SollevaErroreAggiornamentoDB("errore nell'aggiornamento del db locale Diplomazia" + ex.Message);
                        Debug.LogError("errore nell'aggiornamento del db locale Diplomazia" + ex.Message);
                    }

                }
            }

            if (recordAgg == arrayPers.Size())
            {
                valoreRitorno = true;
                tr.Commit();
            }

        }
        catch (Exception ex)
        {

            Debug.LogError("errore aggiornamento archivio Diplomazia: " + ex.Message);
            ControllerLogin.SollevaErroreAggiornamentoDB("errore aggiornamento archivio Diplomazia: " + ex.Message);
            if (tr != null)
            {
                try
                {
                    tr.Rollback();

                }
                catch (SqliteException ex2)
                {
                    Debug.LogError("rollback della transazione riguardante l'aggiornamento della tabella Diplomazia fallito: " + ex2.ToString());
                }
                finally
                {
                    tr.Dispose();
                }
            }
        }

        return valoreRitorno;
    }

    internal static SqliteDataReader GiveMeDiplomaziaLocale(int idClasse)
    {
        SqliteDataReader _reader = null;
        try
        {

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ClassiPersonaggi_idClasse, ClassiPersonaggi2_idClasse, relazione FROM VistaDiplomaziaValidi WHERE ClassiPersonaggi_idClasse = @idClasse OR ClassiPersonaggi2_idClasse =  @idClasse";
            cmd.Parameters.Add(new SqliteParameter("@idClasse", idClasse));
            _reader = cmd.ExecuteReader();

        }
        catch (Exception ex)
        {
            Debug.LogError("errore lettura dalla tabella Diplomazia" + ex.Message);
            ManagerIniziale.SollevaErroreScenaInizialeCreazionePg("errore lettura dalla tabella Diplomazia" + ex.Message);

        }

        return _reader;
    }

    internal static SqliteDataReader GiveMePersonaggiUtenteDBLocale()
    {
        SqliteDataReader _reader = null;
        try
        {

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT Personaggi_idPersonaggio, nomeModello, nomePersonaggio, vitaMassima, vitaAttuale, manaMassimo, manaAttuale, "+
                "livelloAttuale, xpAttuale,attaccoBase, difesaBase, nomeScena, checkPoint " +
                "FROM VistaPersonaggiUtenteValidi WHERE Utenti_idUtente = @idUtente";
            cmd.Parameters.Add(new SqliteParameter("@idUtente", idDB));
            _reader = cmd.ExecuteReader();

        }
        catch (Exception ex)
        {
            Debug.LogError("errore lettura dalla tabella PersonaggiUtente" + ex.Message);
            ManagerIniziale.SollevaErroreScenaInizialeCreazionePg("errore lettura dalla tabella PersonaggiUtente" + ex.Message);

        }

        return _reader;
    }

    internal static SqliteDataReader GiveMePersonaggiDBLocale()
    {
        SqliteDataReader _reader = null;
        try
        {

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT idPersonaggio, ClassiPersonaggi_idClassiPersonaggi, modelloM, modelloF, nome, vitaMassima, vitaAttuale, manaMassimo, manaAttuale, livelloPartenza, xpPartenza, "+
                "attaccoBase, difesaBase FROM VistaPersonaggiGiocabiliValidi";         
            _reader = cmd.ExecuteReader();          
      
        }
        catch (Exception ex)
        {
            Debug.LogError("errore lettura dalla tabella Personaggi" + ex.Message);
            ManagerIniziale.SollevaErroreScenaInizialeCreazionePg("errore lettura dalla tabella Personaggi" + ex.Message);

        }
   
        return _reader;
    }

    internal static void ControllaTSUtenti(int idDB, string timeStampRemoto, bool isRegistrazione)
    {
        SqliteTransaction tr = null;
       
        SqliteCommand cmd = conn.CreateCommand();
       
        if (isRegistrazione)
        {
            try
            {
                tr = conn.BeginTransaction();
                cmd.Transaction = tr;
                int numeroRighe = 0;                
                cmd.CommandText = @"INSERT INTO Utenti VALUES (@idUtente,@timeStampUtente)";
                cmd.Parameters.Add(new SqliteParameter("@idUtente", idDB));
                cmd.Parameters.Add(new SqliteParameter("@timeStampUtente", timeStampRemoto));
                numeroRighe = cmd.ExecuteNonQuery();
                if (numeroRighe > 0)
                {
                    Debug.Log("Inserimento nella tabella Utenti della riga: " + idDB);
                    tr.Commit();
                    RichiediTSUtentiNew();
                  
                }            
                
            }
            catch (Exception ex)
            {
                Debug.LogError("errore aggiornamento archivio Utenti: " + ex.Message);
                ControllerLogin.SollevaErroreAggiornamentoDB("errore aggiornamento archivio Utenti: " + ex.Message);
                if (tr != null)
                {
                    try
                    {
                        tr.Rollback();

                    }
                    catch (SqliteException ex2)
                    {
                        Debug.LogError("rollback della transazione riguardante l'aggiornamento della tabella Utenti fallito: " + ex2.ToString());

                    }
                    finally
                    {
                        tr.Dispose();
                    }
                }
            }
        }
        else//if !isRegistrazione
        {
            SqliteDataReader _reader = null;
            try
            {

                cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT timeStampUtente FROM Utenti WHERE idUtente= @idUtente";
                cmd.Parameters.Add(new SqliteParameter("@idUtente", idDB));
                _reader = cmd.ExecuteReader();
                if (_reader.HasRows)
                {
                    while (_reader.Read())
                    {
                        string tsLocale = (string)_reader["timeStampUtente"];

                        if (String.Compare(timeStampRemoto, tsLocale) != 0)
                        {//se sono diversi si deve aggiornare
                            RichiestaDatiTabellaRemota("PersonaggiUtente");
                            RichiestaDatiTabellaRemota("DiplomaziaPersonaggio");
                            Debug.Log("Le tabelle PersonaggiUtente e DiplomaziaPersonaggio necessitano di aggiornamento, timeStampUtente  locale vecchio!");
                        }
                        else
                        {
                            Debug.Log("Le tabelle PersonaggiUtente e DiplomaziaPersonaggio NON necessitano di aggiornamento, timeStampUtente  locale aggiornato!");
                            RichiediTSUtentiNew();
                        }
                    }
                }
                else//se non lo trovo
                {
                    if (!_reader.IsClosed)
                        _reader.Close();
                    try
                    {
                        tr = conn.BeginTransaction();
                        cmd.Transaction = tr;
                        cmd = conn.CreateCommand();
                        cmd.CommandText = @"INSERT INTO Utenti VALUES (@idUtente, @timeStampUtente)";
                        cmd.Parameters.Add(new SqliteParameter("@idUtente", idDB));
                        cmd.Parameters.Add(new SqliteParameter("@timeStampUtente", timeStampRemoto));
                        int numeroRighe = cmd.ExecuteNonQuery();
                        if (numeroRighe > 0)
                        {
                            Debug.Log("Inserimento nella tabella Utenti della riga: " + idDB);
                            tr.Commit();
                            RichiestaDatiTabellaRemota("PersonaggiUtente");
                            RichiestaDatiTabellaRemota("DiplomaziaPersonaggio");

                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("errore aggiornamento archivio Utenti: " + ex.Message);
                        ControllerLogin.SollevaErroreAggiornamentoDB("errore aggiornamento archivio Utenti: " + ex.Message);
                        if (tr != null)
                        {
                            try
                            {
                                tr.Rollback();

                            }
                            catch (SqliteException ex2)
                            {
                                Debug.LogError("rollback della transazione riguardante l'aggiornamento della tabella Utenti fallito: " + ex2.ToString());

                            }
                            finally
                            {
                                tr.Dispose();
                            }
                        }
                    }
                }
                                
            }
            catch (Exception ex)
            {
                Debug.LogError("errore lettura dalla tabella Utenti"+ex.Message);
                ControllerLogin.SollevaErroreAggiornamentoDB("errore lettura dalla tabella Utenti" + ex.Message);

            }
            finally
            {
                if (!_reader.IsClosed)
                    _reader.Close();                
            }

        }
    }
    public static void RichiediTSUtentiNew()
    {
        Statici.sfs.Send(new ExtensionRequest(Statici.CMD_RICHIESTA_TS_UTENTI, new SFSObject()));
    }

    public static void RichiestaDatiTabellaRemota(string nomeTabella)
    {
        if (nomeTabella == "Personaggi")
            Statici.sfs.Send(new ExtensionRequest(CMD_RICHIESTA_PERSONAGGI, new SFSObject()));
        else if (nomeTabella == "PersonaggiUtente")
            Statici.sfs.Send(new ExtensionRequest(CMD_RICHIESTA_PERSONAGGI_UTENTE, new SFSObject()));

        else if (nomeTabella == "Diplomazia")
            Statici.sfs.Send(new ExtensionRequest(CMD_RICHIESTA_DIPLOMAZIA, new SFSObject()));
        else if (nomeTabella == "DiplomaziaPersonaggio")
            Statici.sfs.Send(new ExtensionRequest(CMD_RICHIESTA_DIPLOMAZIA_PERSONAGGIO, new SFSObject()));

        //aggiungere altri else if se necessario
    }

    public static string GiveMeNomeClasse(int idClasse)
    {
        string nomeClasse = string.Empty;
        SqliteDataReader _reader = null;
        try
        {

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT nome FROM VistaPersonaggiGiocabiliValidi WHERE ClassiPersonaggi_idClassiPersonaggi = @idClasse";
            cmd.Parameters.Add(new SqliteParameter("@idClasse", idClasse));
            _reader = cmd.ExecuteReader();
            if (_reader.HasRows)
            {
                while (_reader.Read())
                {
                    nomeClasse = (string)_reader["nome"];
                    
                }
            }
            else
            {
                if (!_reader.IsClosed)
                    _reader.Close();
                Debug.LogError("personaggio non trovato durante la ricerca della classe");
            }

            }
        catch (Exception ex)
        {
            Debug.LogError("errore lettura nomeClasse dalla tabella Personaggi" + ex.Message);
           

        }
        finally
        {
            if (!_reader.IsClosed)
                _reader.Close();

        }

        return nomeClasse;
    }

    public static bool AggiornaPersonaggiUtente(SFSArray arrayPers)
    {
        bool valoreRitorno = false;
        SqliteTransaction tr = null;       
        try
        {
            tr = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.Transaction = tr;
            int numeroRighe = 0;
            int recordAgg = 0;
            for (int i = 0; i < arrayPers.Size(); i++)
            {
                //Legge i dati provenienti dal remoto (record per record)
                SFSObject objArr = (SFSObject)arrayPers.GetSFSObject(i);
                string nomePers = objArr.GetUtfString("nomePersonaggio");
                int Utenti_idUtente = objArr.GetInt("Utenti_idUtente");
                int idPersonaggio = objArr.GetInt("Personaggi_idPersonaggio");
                string nomeModello = objArr.GetUtfString("nomeModello");               
                int eliminato = objArr.GetInt("eliminato");
                double vitaMassima = objArr.GetDouble("vitaMassima");
                double vitaAttuale = objArr.GetDouble("vitaAttuale");
                double manaMassimo = objArr.GetDouble("manaMassimo");
                double manaAttuale = objArr.GetDouble("manaAttuale");
                double livelloPartenza = objArr.GetDouble("livelloAttuale");
                double xpPartenza = objArr.GetDouble("xpAttuale");
                double attaccoBase = objArr.GetDouble("attaccoBase");
                double difesaBase = objArr.GetDouble("difesaBase");
                string nomeScena = objArr.GetUtfString("nomeScena");
                string checkPoint = objArr.GetUtfString("checkPoint");

                try
                {
                    cmd.CommandText = @"INSERT INTO PersonaggiUtente VALUES (@nomePersonaggio,@Utenti_idUtente,@idPersonaggio,@nomeModello, @eliminato," +
                    " @vitaMassima, @vitaAttuale,  @manaMassimo, @manaAttuale, @livelloPartenza, @xpPartenza, @attaccoBase, @difesaBase, @nomeScena, @checkPoint)";
                    cmd.Parameters.Add(new SqliteParameter("@nomePersonaggio", nomePers));
                    cmd.Parameters.Add(new SqliteParameter("@Utenti_idUtente", Utenti_idUtente));
                    cmd.Parameters.Add(new SqliteParameter("@idPersonaggio", idPersonaggio));
                    cmd.Parameters.Add(new SqliteParameter("@nomeModello", nomeModello));                 
                    cmd.Parameters.Add(new SqliteParameter("@eliminato", eliminato));
                    cmd.Parameters.Add(new SqliteParameter("@vitaMassima", vitaMassima));
                    cmd.Parameters.Add(new SqliteParameter("@vitaAttuale", vitaAttuale));
                    cmd.Parameters.Add(new SqliteParameter("@manaMassimo", manaMassimo));
                    cmd.Parameters.Add(new SqliteParameter("@manaAttuale", manaAttuale));
                    cmd.Parameters.Add(new SqliteParameter("@livelloPartenza", livelloPartenza));
                    cmd.Parameters.Add(new SqliteParameter("@xpPartenza", xpPartenza));
                    cmd.Parameters.Add(new SqliteParameter("@attaccoBase", attaccoBase));
                    cmd.Parameters.Add(new SqliteParameter("@difesaBase", difesaBase));
                    cmd.Parameters.Add(new SqliteParameter("@nomeScena", nomeScena));
                    cmd.Parameters.Add(new SqliteParameter("@checkPoint", checkPoint));
                    numeroRighe = cmd.ExecuteNonQuery();
                    Debug.Log("Inserimento nella tabella PersonaggiUtente nomePersonaggio"+ nomePersonaggio+", id Utente" +Utenti_idUtente +" idPersonaggio,"+idPersonaggio);
                    recordAgg++;
                }
                catch
                {
                    try
                    {
                        cmd.CommandText = "UPDATE PersonaggiUtente SET nomeModello=@nomeModello, eliminato = @eliminato, vitaMassima = @vitaMassima" +
                                ", vitaAttuale = @vitaAttuale, manaMassimo = @manaMassimo, manaAttuale = @manaAttuale, livelloAttuale = @livelloPartenza" +
                                ", xpAttuale = @xpPartenza, attaccoBase = @attaccoBase, difesaBase = @difesaBase, nomeScena = @nomeScena, checkPoint = @checkPoint "+
                                "WHERE nomePersonaggio = @nomePersonaggio";
                        cmd.Parameters.Add(new SqliteParameter("@nomePersonaggio", nomePers));                       
                        cmd.Parameters.Add(new SqliteParameter("@nomeModello", nomeModello));                      
                        cmd.Parameters.Add(new SqliteParameter("@eliminato", eliminato));
                        cmd.Parameters.Add(new SqliteParameter("@vitaMassima", vitaMassima));
                        cmd.Parameters.Add(new SqliteParameter("@vitaAttuale", vitaAttuale));
                        cmd.Parameters.Add(new SqliteParameter("@manaMassimo", manaMassimo));
                        cmd.Parameters.Add(new SqliteParameter("@manaAttuale", manaAttuale));
                        cmd.Parameters.Add(new SqliteParameter("@livelloPartenza", livelloPartenza));
                        cmd.Parameters.Add(new SqliteParameter("@xpPartenza", xpPartenza));
                        cmd.Parameters.Add(new SqliteParameter("@attaccoBase", attaccoBase));
                        cmd.Parameters.Add(new SqliteParameter("@difesaBase", difesaBase));
                        cmd.Parameters.Add(new SqliteParameter("@nomeScena", nomeScena));
                        cmd.Parameters.Add(new SqliteParameter("@checkPoint", checkPoint));
                        numeroRighe = cmd.ExecuteNonQuery();


                        //Update buono come comando MA non ha aggiornato righe
                        if (numeroRighe == 0)
                        {
                            ControllerLogin.SollevaErroreAggiornamentoDB("errore:impossibile aggiornare la riga della tabella PersonaggiUtente: nomePersonaggio" + nomePersonaggio);
                            Debug.LogError("errore:impossibile aggiornare la riga della tabella PersonaggiUtente: nomePersonaggio" + nomePersonaggio);
                        }
                        else
                        {
                            Debug.Log("Aggiornamento tabella personaggiUtente idUtente :" + Utenti_idUtente + " idPersonaggio: " + idPersonaggio);
                            recordAgg++;
                        }
                    }
                    catch (Exception ex)
                    {
                        ControllerLogin.SollevaErroreAggiornamentoDB("errore nell'aggiornamento del db locale tabella PersonaggiUtente" + ex.Message);
                        Debug.LogError("errore nell'aggiornamento del db locale tabella PersonaggiUtente" + ex.Message);
                    }
                }

            }//fine for

            if (recordAgg == arrayPers.Size())
            {
                contatoreTabelleDiUtentiOk++;
                Debug.Log("numero tabelle di utenti aggiornate:" + contatoreTabelleDiUtentiOk);
                tr.Commit();
                valoreRitorno = true;
               
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("errore aggiornamento archivio PersonaggiUtente: " + ex.Message);
            ControllerLogin.SollevaErroreAggiornamentoDB("errore aggiornamento archivio PersonaggiUtente: " + ex.Message);
            if (tr != null)
            {
                try
                {
                    tr.Rollback();

                }
                catch (SqliteException ex2)
                {
                    Debug.LogError("rollback della transazione riguardante l'aggiornamento della tabella PersonaggiUtente fallito: " + ex2.ToString());                    

                }
                finally
                {
                    tr.Dispose();
                }
            }
        }
       
        return valoreRitorno;
    }

    internal static bool AggiornaTimeStampTabelleBase(string nomeTab, string timestamp)
    {
        bool valoreRitorno = false;
        SqliteTransaction tr = null;
     
        try
        {
            tr = conn.BeginTransaction();
            SqliteCommand cmd = conn.CreateCommand();
            cmd.Transaction = tr;
            int numeroRighe = 0;

            cmd.CommandText = @"UPDATE SincronizzazioneDB SET nomeTabella = @nomeTabella, aggiornatoAl = @timestamp  WHERE nomeTabella = @nomeTabella";
            cmd.Parameters.Add(new SqliteParameter("@nomeTabella", nomeTab));
            cmd.Parameters.Add(new SqliteParameter("@timeStamp", timestamp));
            numeroRighe = cmd.ExecuteNonQuery();

            if (numeroRighe == 0)
            {
                ControllerLogin.SollevaErroreAggiornamentoDB("errore nell'aggiornamento del db di sincronizzazione locale");
                Debug.LogError("errore nell'aggiornamento del db di sincronizzazione locale");
            }
            else
            {
                tr.Commit();
                valoreRitorno = true;
                contatoreTimeStampOk++;
                ControllaNumeroTimeStampAggiornati();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Errore nell'aggiornamento del db di sincronizzazione locale: " + ex.Message);
            ControllerLogin.SollevaErroreAggiornamentoDB("Errore nell'aggiornamento del db di sincronizzazione locale: " + ex.Message);
            if (tr != null)
            {
                try
                {
                    tr.Rollback();

                }
                catch (SqliteException ex2)
                {
                    Debug.LogError("rollback della transazione riguardante l'aggiornamento della tabella SincronizzazioneDB fallito: " + ex2.ToString());

                }
                finally
                {
                    tr.Dispose();
                }
            }

        }
     
        return valoreRitorno;
    }

    public static void assegnaAssetDatabase()
    {
        DataBase.Inizializza();
        databseInizialeAmicizie = DataBase.GiveMeAmicizie();
        databaseInizialeProprieta = DataBase.GiveMeProprieta();
        databaseInizialePercorsi = DataBase.GiveMePercorsi();
    }

    public static void ControllaTimeStampTabelleBase(string nomeTabella, string tsRemoto)
    {
        SqliteDataReader _reader = null;
        try
        {
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT aggiornatoAl FROM SincronizzazioneDB WHERE nomeTabella= @nomeTabella";
            cmd.Parameters.Add(new SqliteParameter("@nomeTabella", nomeTabella));
            _reader = cmd.ExecuteReader();
            if (_reader.HasRows)
            {
                while (_reader.Read())
                {
                    string tsLocale = (string)_reader["aggiornatoAl"];

                    if (String.Compare(tsRemoto, tsLocale) != 0)
                    {//se sono diversi si deve aggiornare
                        RichiestaDatiTabellaRemota(nomeTabella);
                        Debug.Log("la tabella necessita aggiornamento " + nomeTabella);
                    }
                    else
                    {
                        Debug.Log("la tabella NON necessita aggiornamento " + nomeTabella);
                        contatoreTimeStampOk++;                       
                        ControllaNumeroTimeStampAggiornati();
                    }
                }
            }
            else
            {
                if (!_reader.IsClosed)
                    _reader.Close();
                cmd.CommandText = @"INSERT INTO SincronizzazioneDB VALUES(@nomeTabella,@aggiornatoAl)";
                cmd.Parameters.Add(new SqliteParameter("@nomeTabella", nomeTabella));
                cmd.Parameters.Add(new SqliteParameter("@aggiornatoAl", "SENZA DATA"));
                cmd.ExecuteNonQuery();
                Debug.Log("il nome tabella era MANCANTE nella Tabella SincronizzazioneDB ma è stata aggiunta e aggiornata: " + nomeTabella);
                RichiestaDatiTabellaRemota(nomeTabella);
                
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Errore nel controllo TimeStamp tabelle SincronizzazioneDB:" +ex.Message);
            ControllerLogin.SollevaErroreAggiornamentoDB("Errore nel controllo TimeStamp tabelle SincronizzazioneDB:" + ex.Message);

        }
        finally
        {
            if (!_reader.IsClosed)
                _reader.Close();
        
        }
    }
    public static void ControllaNumeroTimeStampAggiornati()
    {
        if (contatoreTimeStampOk >= numeroTabelleAggTimeStamp)
            ControllerLogin.EntraNellaLobby();
    }

    public static void CopiaIlDB(bool sovrascrivi = false)
    {
        if (!File.Exists(destinazione) || sovrascrivi)
        {
            Debug.Log("Copio il DB in " + Application.persistentDataPath);
            File.Copy(origine, destinazione, sovrascrivi);
        }
        ConnettiEdApriSQLite();    
    }

    public static void provaErrore(string scriviNomeVariabile, object oggetto)
    {
        if (oggetto == null)
            Debug.Log("$Debug.Automatico : L'oggetto **" + scriviNomeVariabile + " **che mi hai passato e' : NULLO  ");
        else Debug.Log("$Debug.Automatico : Ecco valore del oggetto **" + scriviNomeVariabile + " **che mi hai passato :  " + oggetto.ToString());
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
    /// <summary>
    /// salva in un dizionario il personaggio e le sue caratteristiche (vita, livello ecc)
    /// </summary>
    /// <param name="datiPersonaggioDaRegistrare"></param>
    public static void RegistraDatiPersonaggio(DatiPersonaggio datiPersonaggioDaRegistrare)
    {
        registroDatiPersonaggi.Add(datiPersonaggioDaRegistrare.GetInstanceID(), datiPersonaggioDaRegistrare);
        RecuperaDati(datiPersonaggioDaRegistrare);
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
    /// Crea una connessione a SQLite e la apre
    /// </summary>
    private static void ConnettiEdApriSQLite()
    {
        conn = new SqliteConnection("URI=file:" + destinazione);
        conn.StateChange += Conn_StateChange;
        conn.Open();
    }

    public static double ClampDouble(double val, double min, double max) 
    {
        if (val.CompareTo(min)<0) return min;
        else if (val.CompareTo(max) > 0) return max;
        else return val;
    }

   
}

