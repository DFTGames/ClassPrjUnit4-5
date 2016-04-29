using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PercorsiClass : ScriptableObject
//PROGETTO GEROLAMO 401.....Dove le stelline sono d'obbligo..
{
    const int NON_ESISTE = -1;

    public bool cambiatoAlmenoUnaScena;

    public bool scenaDaSalvare;

    public Percorsi per =null;


    public PercorsiClass()     //chiedere se va bene fatto in questo modo per evitare di creare istanza da dentro editor
    {
        per= new Percorsi();
    }

    [System.Serializable]
    public class Percorso
    {
        public string nomePercorsi = string.Empty;
        public int idx = NON_ESISTE;
        public classiPersonaggi[] classi;

    }


    [System.Serializable]
    public class Percorsi : IEnumerable
    {
        private Percorso[] percorsi;

        public void Add(Percorso percorso)
        {

            for (int i = 0; i < percorsi.Length; i++)
            {
                if (percorsi[i].nomePercorsi.Contains(percorso.nomePercorsi))
                {
                    Debug.LogError("Capra !! Capra !! Capra !  percorso gia usato..Riprova..la prox volta sarai piu' fortunato ");
                    return;
                }
            }

            percorso.idx = trovaIndexLibero(percorsi);
            Array.Resize<Percorso>(ref percorsi, percorsi.Length + 1);
            percorsi[percorsi.Length - 1] = percorso;

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
            int idx2 = percorsi.Length - 1;

            if (idx2 >= 0 && idx2 < percorsi.Length)
            {

                for (int i = idx2; i < percorsi.Length - 1; i++)
                    percorsi[i] = percorsi[i + 1];


                Array.Resize<Percorso>(ref percorsi, percorsi.Length - 1);
                return true;
            }
            else return false;
        }
        public void RemoveAll()  // da implementarlo correttamente..
        {
            percorsi = null;

        }
        public IEnumerator GetEnumerator()
        {
            return percorsi.GetEnumerator();
        }

        public Percorso this[int idx]
        {
            get
            {
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


    /*
    public List<int> trovaPercorsiDaPersonaggio(classiPersonaggi per)
    {
        List<int> lista = new List<int>();

        for (int i = 0; i < percorsi.Length; i++)

            for (int ii = 0; ii < percorsi[i].classi.Length; ii++)
                if (per == percorsi[i].classi[ii]) lista.Add(percorsi[i].idx);

        return lista;
    }

    public List<classiPersonaggi> trovaPersonaggiDaPercorsi(string percorso)
    {
        List<classiPersonaggi> lista = new List<classiPersonaggi>();



        return lista;

    }
    */
    public void ControlloIndexPercorsi()
    {

        GameObject tmpObj = GameObject.Find("PadrePercorso");

        if (tmpObj != null)
            for (int i = 0; i < tmpObj.transform.childCount; i++)
            {
                Transform tmpPercorso = tmpObj.transform.GetChild(i);
                GestorePercorso gp = tmpPercorso.GetComponent<GestorePercorso>();
                if (gp.IndexPercorso == NON_ESISTE ) //|| (gp.IndexPercorso != NON_ESISTE && !indexPercorsi.Contains(gp.IndexPercorso)))  IMPLEMENTARLO CORRETTAMENTE..
                {
                    tmpPercorso.name = "PERCORSO ERRATO";
                    gp.IndexPercorso = NON_ESISTE;
                    scenaDaSalvare = true;
                }

            }
    }
}
