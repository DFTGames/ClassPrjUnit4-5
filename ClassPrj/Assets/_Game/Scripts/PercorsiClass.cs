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

    public List<int> indexPercorsi = new List<int>();

    public List<string> nomePercorsi = new List<string>();


    public void ResettaIndexGameData1(int key,GameData gameData1)  //mi resetta(default = NON_ESISTE) i valori del indexPercorso GameData
    {                                    // se key=NON_ESISTE mi resetta tutti i valori..altrimenti solo il valore corrispondente

        for (int i = 0; i < gameData1.indexPercorsi.Length; i++)
        {
            if ((key == NON_ESISTE) || (gameData1.indexPercorsi[i] == key))
            {
                gameData1.indexPercorsi[i] = NON_ESISTE;
                cambiatoAlmenoUnaScena = true;
            }
        }
        ControlloIndexPercorsi();
    }


   public void ordinaClasseListaDouble(ref List<int> indexPercorsi, ref List<string> nomePercorsi) //mi effettua lo sort del index e stringa(questo costruito io)
    {
        List<int> tmpindex = new List<int>(indexPercorsi); //passaggio per valore IMPORTANTE
        List<string> tmpPercorsi = new List<string>(nomePercorsi);  //passaggio per valore IMPORTANTE

        indexPercorsi.Sort();

        for (int i = 0; i < tmpindex.Count; i++)
            nomePercorsi[i] = tmpPercorsi[tmpindex.IndexOf(indexPercorsi[i])];           //mi ordina la lista percorso sulla base della lista index riordinata con lo sort
    }

    public int trovaIndexLibero(List<int> indexPercorsi)   //mi trova l'index libero ciclando
    {
        int tmp = 0;
        if (indexPercorsi.Count > 0)
        {
            for (int i = 0; i < indexPercorsi.Count; i++)
            {
                if (tmp < indexPercorsi[i] - 1) break;
                tmp = indexPercorsi[i];
            }
        }
        return ++tmp;
    }

    public void ControlloIndexPercorsi()
    {

        GameObject tmpObj = GameObject.Find("PadrePercorso");

        if (tmpObj != null)
            for (int i = 0; i < tmpObj.transform.childCount; i++)
            {
                Transform tmpPercorso = tmpObj.transform.GetChild(i);
                GestorePercorso gp = tmpPercorso.GetComponent<GestorePercorso>();
                if (gp.IndexPercorso == NON_ESISTE || (gp.IndexPercorso != NON_ESISTE && !indexPercorsi.Contains(gp.IndexPercorso)))
                {
                    tmpPercorso.name = "PERCORSO ERRATO";
                    gp.IndexPercorso = NON_ESISTE;
                    scenaDaSalvare = true;
                }

            }
    }

    public void resetta(GameData gameData1)
    {
        nomePercorsi.Clear();
        indexPercorsi.Clear();
        ResettaIndexGameData1(NON_ESISTE, gameData1);

    }

}
