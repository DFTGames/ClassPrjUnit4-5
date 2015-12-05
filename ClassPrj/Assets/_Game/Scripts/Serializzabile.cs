using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Serializzabile<T> : System.IDisposable where T : class, new()
{
    private T dati = null;
    private string mioFile = string.Empty;
    private bool pronto = false;

    public T Dati
    {
        get
        {
            return dati;
        }
    }

    public Serializzabile(string nomeFile, bool LeggiDatiEsistenti = true)
    {
        PreparaNomeFile(nomeFile);
        if (LeggiDatiEsistenti)
            Carica();

        if (dati == null)
            dati = new T();
    }

    private void PreparaNomeFile(string nome)
    {
        string dir = Path.Combine(Application.persistentDataPath, Statici.nomePersonaggio);

        if (!Directory.Exists(dir))
        {
            try
            {
                DirectoryInfo dirInfo = Directory.CreateDirectory(dir);
            }
            catch (IOException e)
            {
                Debug.LogError("Errore di I/O\n" + e.StackTrace);
            }
            catch (System.UnauthorizedAccessException e)
            {
                Debug.LogError("Errore di Autorizzazione\n" + e.StackTrace);
            }
            catch (System.ArgumentException e)
            {
                Debug.LogError("Errore di Argomento\n" + e.StackTrace);
            }
            catch (System.NotSupportedException e)
            {
                Debug.LogError("Errore di Compatibilità\n" + e.StackTrace);
            }
        }
        if (Directory.Exists(dir))
        {
            mioFile = Path.Combine(dir, nome);

            pronto = true;
        }
    }

    private void Carica()
    {
        if (!pronto)
        {
            Debug.LogError("Errore:Tentativo di salvataggio in assenza di percorso valido");
            return;
        }
        if (File.Exists(mioFile))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream file = File.Open(mioFile, FileMode.Open);

            dati = (T)formatter.Deserialize(file);

            file.Close();
        }
    }

    public void Salva()
    {
        if (!pronto)
        {
            Debug.LogError("Errore:Tentativo di salvataggio in assenza di percorso valido.");
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(mioFile, FileMode.Create);
        formatter.Serialize(file, dati);
        file.Close();
    }

    public void Dispose()
    {
        if (dati is System.IDisposable)
            ((System.IDisposable)dati).Dispose();
        dati = null;
    }
}