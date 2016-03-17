using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]

public class PercorsiClass : ScriptableObject
{
    // public Dictionary<int, string> nomePercorsi = new Dictionary<int, string>();  //sostituita il dizionario con 2 liste per problemi di serializzazione

    public const int NON_ESISTE = -1;  //messo dal Prof...cosi' si capisce meglio

    public bool cambiatoAlmenoUnaScena;

    public bool scenaDaSalvare;

    //public List<string> nomeClassi = new List<string>();

    public List<GruppoPercorsi> percorsi = new List<GruppoPercorsi>();

    public List<Road> road;

    [System.Serializable]
    public class Road
    {
        public string nomeClassi = string.Empty;
        public int indice = -1;

    }

    [System.Serializable]
    public class GruppoPercorsi
    {
        public string nomePercorsi = string.Empty;
        public int indice = NON_ESISTE;

    }



    public void ResettaIndicePercorso(int key)  //mi resetta(default = NON_ESISTE) i valori del indexPercorso GameData
    {                                    // se key=NON_ESISTE mi resetta tutti i valori..altrimenti solo il valore corrispondente

        for (int i = 0; i < road.Count; i++)
        {
            if ((key == NON_ESISTE) || (road[i].indice == key))
            {
                road[i].indice = NON_ESISTE;
                cambiatoAlmenoUnaScena = true;
            }
        }
        ControlloIndexPercorsi();
    }

    /*  FUNZIONE INUTILE PER PINO..DA TOGLIERE
    public void ordinaClasseListaDouble(ref List<int> indexPercorsi, ref List<string> nomePercorsi) //mi effettua lo sort del index e stringa(questo costruito io)
    {
        List<int> tmpindex = new List<int>(indexPercorsi); //passaggio per valore IMPORTANTE
        List<string> tmpPercorsi = new List<string>(nomePercorsi);  //passaggio per valore IMPORTANTE

        indexPercorsi.Sort();

        for (int i = 0; i < tmpindex.Count; i++)
            nomePercorsi[i] = tmpPercorsi[tmpindex.IndexOf(indexPercorsi[i])];           //mi ordina la lista percorso sulla base della lista index riordinata con lo sort
    }
    */

    public int trovaIndexLibero()   //mi trova l'index libero (metodo da me brevettato...e' veloce e funzionale e non fa uso di ricorsione)
    {
        if (percorsi.Count == 0) return 0;

        List<int> tmpOrdino = new List<int>();  //creata lista provvisoria

        for (int i = 0; i < percorsi.Count; i++)  //alimenta la lista e poi la ordina
            tmpOrdino.Add(percorsi[i].indice);

        tmpOrdino.Sort();

        int primo = 0;

        for (int fi = 0; fi < tmpOrdino.Count; fi++)  //trova il primo indice libero
        {
            if (primo == tmpOrdino[fi]) primo++;
            else break;
        }
        return primo;
    }



    public void ControlloIndexPercorsi()
    {

        GameObject tmpObj = GameObject.Find("PadrePercorso");

        if (tmpObj != null)
            for (int i = 0; i < tmpObj.transform.childCount; i++)
            {
                Transform tmpPercorso = tmpObj.transform.GetChild(i);
                GestorePercorso gp = tmpPercorso.GetComponent<GestorePercorso>();
                if (gp.IndexPercorso == NON_ESISTE) //ESCLUSO DAL CONTROLLO MOMENTANEAMENTE..VEDERE SE CI STA O MENO || (gp.IndexPercorso != NON_ESISTE && !indexPercorsi.Contains(gp.IndexPercorso)))
                {
                    tmpPercorso.name = "PERCORSO ERRATO";
                    gp.IndexPercorso = NON_ESISTE;
                    scenaDaSalvare = true;
                }

            }
    }

    public void resetta()
    {
        percorsi.Clear();
        road.Clear();
        //   ResettaIndexGameData1(NON_ESISTE, gameData1);
        //DA SISTEMARE...

    }

}
