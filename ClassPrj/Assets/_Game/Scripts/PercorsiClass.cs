using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class PercorsiClass : ScriptableObject
{
    const int NON_ESISTE = -1;

    public bool cambiatoAlmenoUnaScena;

    public bool scenaDaSalvare;

    public Percorsi per = null;


    public PercorsiClass()     //chiedere se va bene fatto in questo modo per evitare di creare istanza da dentro editor
    {
        per = new Percorsi();
    }

    [System.Serializable]
    public class Percorso
    {
        public string nomePercorsi = string.Empty;
        public int idx = NON_ESISTE;
        public List<classiPersonaggi> classi = new List<classiPersonaggi>();

    }


    [System.Serializable]
    public class Percorsi : IEnumerable
    {
        private Percorso[] percorsi;

        public Percorsi()
        {
            percorsi = new Percorso[0];
        }
        public void Add(Percorso percorso)
        {
            if (!ControlloNomiPercorso(percorso.nomePercorsi)) return;
            percorso.idx = trovaIndexLibero(percorsi);
            Array.Resize<Percorso>(ref percorsi, percorsi.Length + 1);
            percorsi[percorsi.Length - 1] = percorso;
        }

        public bool ControlloNomiPercorso(string nomePercorso)  //ritorna  false se il nome e' gia presente
        {
            // if (percorsi.Length == 0) return false;
            //   if (percorsi == null) return true;
            for (int i = 0; i < percorsi.Length; i++)
            {
                // Debug.Log(" percorso Aggiunto " + percorso.nomePercorsi);
                //Debug.Log(" percorsi storici " +  percorsi[i].nomePercorsi);
                if (percorsi[i].nomePercorsi.Contains(nomePercorso))
                {
                    Debug.LogError("Capra !! Capra !! Capra !  percorso gia usato..Riprova..la prox volta sarai piu' fortunato ");
                    return (false);
                }
            }
            return (true);
        }

        public int Count
        {
            get
            {
                //provvosorio
                if (percorsi == null) return 0;
                //***********
                return percorsi.Length;
            }

        }

        public bool RemoveAt(int idx)
        {

            if (idx >= 0 && idx < percorsi.Length)
            {

                for (int i = idx; i < percorsi.Length - 1; i++)
                    percorsi[i] = percorsi[i + 1];


                Array.Resize<Percorso>(ref percorsi, percorsi.Length - 1);
                return true;
            }
            else return false;
        }
        public void RemoveAll()  // da implementarlo correttamente..
        {
            percorsi = new Percorso[0];   //ho pensato di resettarlo in questo modo...chiedere a pino se va bene...

        }
        public IEnumerator GetEnumerator()
        {
            return percorsi.GetEnumerator();
        }

        public Percorso this[int idx]
        {
            get
            {
                //    Debug.Log("son nel get");
                if (idx >= 0 && idx < percorsi.Length)
                    return percorsi[idx];
                throw new IndexOutOfRangeException("Capra2 !! Capra2 !! numero fuori valori amessi");
            }


            set
            {

                if (idx >= 0 && idx < percorsi.Length)
                {

                    percorsi[idx] = value;
                }
                else throw new IndexOutOfRangeException("Fesso!! numero fuori valori ammessi!!!");
            }

        }

    }

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


    public List<string> elencaPercorsi()
    {
        List<string> arrayPer = new List<string>();

        for (int i = 0; i < per.Count; i++)
        {        
            arrayPer.Add(per[i].nomePercorsi);          
        }

        return arrayPer;

    }
    public List<int> elencaIdxPercorsi()
    {
        List<int> arrayPer = new List<int>();

        for (int i = 0; i < per.Count; i++)
            arrayPer.Add(per[i].idx);
        return arrayPer;

    }


    public classiPersonaggi[] elencaClassiPersonaggi()  //ritorna array di ClassePersonaggi usate nel assegnazione percorsi   VALUTARE SE  TENERE O NO
    {
        List<classiPersonaggi> cl = new List<classiPersonaggi>();

        for (int i = 0; i < per.Count; i++)
            for (int c = 0; c < per[i].classi.Count; c++)
                if (!(cl.Contains(per[i].classi[c]))) cl.Add(per[i].classi[c]);

        return cl.ToArray();

    }


    public List<int> trovaPercorsiDaPersonaggio(classiPersonaggi classe)
    {
        List<int> elenco = new List<int>();


        for (int i = 0; i < per.Count; i++)   //VA BENE COSI' OPPURE IMPLEMENTO ENUMERATOR ?  (  while (per.GetEnumerator().MoveNext))
            for (int ii = 0; ii < per[i].classi.Count; ii++)
            {
                if (classe == per[i].classi[ii]) elenco.Add(per[i].idx);
                break;
            }

        return elenco;
    }

    public List<classiPersonaggi> trovaPersonaggiDaPercorsi(string percorso)
    {
        List<classiPersonaggi> elenco = new List<classiPersonaggi>();


        for (int i = 0; i < per.Count; i++)   //VA BENE COSI' OPPURE IMPLEMENTO ENUMERATOR ?  (  while (per.GetEnumerator().MoveNext))           
        {
            if (percorso == per[i].nomePercorsi)
                for (int ii = 0; ii < per[i].classi.Count; ii++)
                    elenco.Add(per[i].classi[ii]);

        }

        return elenco;
    }


    public void ControlloIndexPercorsi()
    {

        GameObject tmpObj = GameObject.Find("PadrePercorso");

        if (tmpObj != null)
            for (int i = 0; i < tmpObj.transform.childCount; i++)
            {
                Transform tmpPercorso = tmpObj.transform.GetChild(i);
                GestorePercorso gp = tmpPercorso.GetComponent<GestorePercorso>();
                if (gp.IndexPercorso == NON_ESISTE) //|| (gp.IndexPercorso != NON_ESISTE && !indexPercorsi.Contains(gp.IndexPercorso)))  IMPLEMENTARLO CORRETTAMENTE..
                {
                    tmpPercorso.name = "PERCORSO ERRATO";
                    gp.IndexPercorso = NON_ESISTE;
                    scenaDaSalvare = true;
                }

            }
    }
}
