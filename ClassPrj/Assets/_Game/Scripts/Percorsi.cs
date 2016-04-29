using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Percorsi : ScriptableObject , IEnumerable, IDisposable
{
   const int NON_ESISTE = -1;

    [System.Serializable]
    public class Percorso : IDisposable
    {
        public string nomePercorsi = string.Empty;
        public List<classiPersonaggi> classi = new List<classiPersonaggi>();

        public void Dispose()
        {
            classi.Clear();
            classi = null;
            nomePercorsi = null;

        }
    }


    public Percorsi()         //metto il costruttore oppure quando dichiaro la variabile privata la metto = new Percorso[0];
    {
        percorsi = new Percorso[0];
    }

    [SerializeField]
    private Percorso[] percorsi;


    public int Count
    {
        get
        {
            return percorsi.Length;
        }

    }


    public void Add(string nomePercorso)
    {
        Percorso tmp = new Percorso();
        tmp.nomePercorsi = nomePercorso;

        Add(tmp);

    }
    public void Add(Percorso percorso)
    {      
        Array.Resize<Percorso>(ref percorsi, percorsi.Length + 1);
        percorsi[percorsi.Length - 1] = percorso;

        percorsi[percorsi.Length - 1].classi.Add(classiPersonaggi.goblin);
    }

    public bool ControlloNomiPercorso(string nomePercorso)  //ritorna  false se il nome e' gia presente
    {
        for (int i = 0; i < percorsi.Length; i++)
        {
            if (percorsi[i].nomePercorsi.Contains(nomePercorso) && nomePercorso != Statici.tmpPercorsi)
            {
                Debug.LogError("Capra !! Capra !! Capra !  percorso gia usato..Riprova..la prox volta sarai piu' fortunato ");
                return (false);
            }

        }
        return (true);
    }


    public void EliminaPercorsiVuoti() 
    {                                      
        if (percorsi.Length == 0) return;         //QUANDO NE METTO 2 VUOTI.ME NE LASCIA UNO..VEDERE dove sta il buco..

        for (int i=0; i< percorsi.Length; i++)
        {
            if (percorsi[i].nomePercorsi.Equals(Statici.tmpPercorsi))
            {
                RemoveAt(i);
                if (i == 0) i--;   //Perdono Pino..e' poco leggibile lo so...pero' se il percorso e' al primo giro non funziona..perche lo metterei di nuovo a 0..ma poi lui lo incrementa alla fine..quindi non me li toglie tutti
                     else i = 0;
            }
        }
    }

    public void EliminaClassiDoppie()
    {
        

        for (int i = 0; i < percorsi.Length; i++)
        {
            if (percorsi[i].classi.Count < 2) continue;
              else
                      
                for (int c = 0; c < percorsi[i].classi.Count; c++)
                {
                    classiPersonaggi tmp = percorsi[i].classi[c];
                    List<classiPersonaggi> tmpList = new List<classiPersonaggi>(percorsi[i].classi);
                    tmpList.Remove(tmp);
                    if (tmpList.Contains(tmp))
                    {
                        RemoveAtClass(i, c);
                        if (c == 1) c--;   //Continuo del Percorso (uguale a sopra)
                        else c = 0;
                    }
             
                }        
           
        }
    }


    public bool RemoveAt(int idx)   //mi rimuove l'idx del percorso
    {

        if (idx >= 0 && idx < percorsi.Length)
        {
            percorsi[idx].Dispose();
            percorsi[idx] = null;
            for (int i = idx; i < percorsi.Length - 1; i++)
                percorsi[i] = percorsi[i + 1];

            Array.Resize<Percorso>(ref percorsi, percorsi.Length - 1);
            return true;
        }
        else return false;
    }


    public bool RemoveAtClass(int idx, int idxClasse)   //mi rimuove l'idx della classe al percorso[idx]
    {
        if ((idx >= 0 && idx < percorsi.Length) && (idxClasse >= 0 && idxClasse < percorsi[idx].classi.Count))
        {
            percorsi[idx].classi.RemoveAt(idxClasse);
            return true;
        }
        return false;
    }

    public void Clear()
    {
        for (int i = 0; i < percorsi.Length; i++)
        {
            percorsi[i].Dispose();
            percorsi[i] = null;
        }
        Array.Resize(ref percorsi, 0);
    }

    public IEnumerator GetEnumerator()   //LA USO A FARE QUALCOSA OPPURE SERVE SOLO PER LA SERIALIZZAZIONE? (IN QUESTO CASO HO SERIALIZZATO NEL ASSET)
    {
        return percorsi.GetEnumerator();
    }

    public void Dispose()
    {

        Clear();
        percorsi = null;
    }

    public Percorso this[int idx]
    {
        get
        {
            if (idx >= 0 && idx < percorsi.Length)
                return percorsi[idx];
            Debug.LogError("Capra2 !! Capra2 !! numero fuori valori amessi");
            return null;
        }
       
        set   //NON USATO
        {
            if (idx >= 0 && idx < percorsi.Length)
            {
                percorsi[idx] = value;
            }
            else Debug.LogError("Fesso!! numero fuori valori ammessi!!!");
        }
        
    }


    /* FUNZIONE ELIMINATA DA PINO...PERCHE INUTILE..(SOSTITUITO IDX CON LA POSIZIONE NEL ARRAY)..MA IO LA TENGO QUA COME RICORDO...Tra i migliori script del .net da me creati..potenza e velocita in unico script
    public static int trovaIndexLibero(Percorso[] way)   //mi trova l'index libero (metodo da me brevettato...e' veloce e funzionale e non fa uso di ricorsione)
    {
        if (way.Length == 0) return 0;

        List<int> tmpOrdino = new List<int>();  //creata lista provvisoria

        for (int i = 0; i < way.Length; i++)  //alimenta la lista e poi la ordina
            tmpOrdino.Add(way[i].idx);

        tmpOrdino.Sort();

        int primo = 0;

        for (int fi = 0; fi < tmpOrdino.Count; fi++)  //trova il primo indice libero
        {
            if (primo == tmpOrdino[fi]) primo++;
            else break;
        }
        return primo;
    }
    */

    public List<string> elencaPercorsi()
    {
        List<string> arrayPer = new List<string>();

        for (int i = 0; i < Count; i++)
        {
            arrayPer.Add(percorsi[i].nomePercorsi);
        }

        return arrayPer;

    }

    
    public List<int> elencaIdxPercorsi()
    {
        List<int> arrayPer = new List<int>();

        for (int i = 0; i < Count; i++)
            arrayPer.Add(i);
        return arrayPer;

    }
    

    public List<int> trovaPercorsiDaPersonaggio(classiPersonaggi classe)
    {
        List<int> elenco = new List<int>();


        for (int i = 0; i < Count; i++)  
            for (int ii = 0; ii < percorsi[i].classi.Count; ii++)
            {
                if (classe == percorsi[i].classi[ii]) elenco.Add(i);
                break;
            }

        return elenco;
    }

    public List<classiPersonaggi> trovaPersonaggiDaIndicePercorsi(int idx)
    {
        List<classiPersonaggi> elenco = new List<classiPersonaggi>();

        for (int i = 0; i < Count; i++)   
        {
            if (idx == i)
                for (int ii = 0; ii < percorsi[i].classi.Count; ii++)
                    elenco.Add(percorsi[i].classi[ii]);
        }
        return elenco;
    }


    public bool ControlloIndexPercorsi()
    {
        bool scenaDaSalvare = false; 
        GameObject tmpObj = GameObject.Find("PadrePercorso");

        if (tmpObj != null)
            for (int i = 0; i < tmpObj.transform.childCount; i++)
            {
                Transform tmpPercorso = tmpObj.transform.GetChild(i);
                GestorePercorso gp = tmpPercorso.GetComponent<GestorePercorso>();
                if ((gp.IndexPercorso == NON_ESISTE) || (gp.IndexPercorso != NON_ESISTE && !elencaIdxPercorsi().Contains(gp.IndexPercorso)))  
                {
                    tmpPercorso.name = "PERCORSO ERRATO";
                    gp.IndexPercorso = NON_ESISTE;
                    scenaDaSalvare = true;
                }

            }
        return scenaDaSalvare;
    }






}