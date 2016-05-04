using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScegliPersonaggio : MonoBehaviour {

    private Serializzabile<ValoriPersonaggioS> datiPersonaggio;
    private Dictionary<string, GameObject> dizionarioCollegamentoNomiConModelli = new Dictionary<string, GameObject>();

    int contatoreGiocabili = -1;
    // Use this for initialization
    void Start () {
        //Statici.assegnaAssetDatabase();
        Statici.sonoPassatoDallaScenaIniziale = true;
         datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);
        for (int i = 0; i < Statici.databaseInizialeProprieta.matriceProprieta.Count; i++)
        {
            if (!Statici.databaseInizialeProprieta.matriceProprieta[i].giocabile)
                continue;
            string tmpNomeModelloM = Statici.databaseInizialeProprieta.matriceProprieta[i].nomeM;
            string tmpNomeModelloF = Statici.databaseInizialeProprieta.matriceProprieta[i].nomeF;
            if (tmpNomeModelloM!=null )
            {
                Debug.Log("son qua");
                contatoreGiocabili += 1;
                // dizionarioCollegamentoNomiConModelli.Add(tmpNomeModelloM, Instantiate(Resources.Load(tmpNomeModelloM), GameObject.Find("ScegliPersonaggio").transform.FindChild("Posizione" + contatoreGiocabili).position, Quaternion.identity) as GameObject);
               GameObject tmp=Instantiate(Resources.Load(tmpNomeModelloM), GameObject.Find("ScegliPersonaggio").transform.FindChild("Posizione" + contatoreGiocabili).position, Quaternion.identity) as GameObject;
                tmp.GetComponent<Rigidbody>().isKinematic = true;
            }
            if (tmpNomeModelloF != null)
            {
                //  dizionarioCollegamentoNomiConModelli.Add(tmpNomeModelloF, Instantiate(Resources.Load(tmpNomeModellof), GameObject.Find("ScegliPersonaggio").transform.FindChild("Posizione" + contatoreGiocabili).position, Quaternion.identity) as GameObject);
          //      Instantiate(Resources.Load(tmpNomeModelloF), GameObject.Find("ScegliPersonaggio").transform.FindChild("Posizione" + contatoreGiocabili).position, Quaternion.identity);
       // contatoreGiocabili += 1;
            }
            

            //      dizionarioCollegamentoNomiConModelli.Add(tmpNomeModelloF, Instantiate(Resources.Load(tmpNomeModelloF), GameObject.Find("postazione" + contatoreGiocabili).transform.FindChild("posizioneF").position, Quaternion.identity) as GameObject);
            //    dizionarioPosizioniPrecedenti.Add(dizionarioCollegamentoNomiConModelli[tmpNomeModelloM].name, GameObject.Find("postazione" + contatoreGiocabili).transform.FindChild("posizioneM"));
            //    dizionarioPosizioniPrecedenti.Add(dizionarioCollegamentoNomiConModelli[tmpNomeModelloF].name, GameObject.Find("postazione" + contatoreGiocabili).transform.FindChild("posizioneF"));
            //  contatoreGiocabili += 1;
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
