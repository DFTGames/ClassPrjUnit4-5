using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager me;
    private Collider attuale = null;
    private string classeEssereSelezionato = string.Empty;
    private Serializzabile<AmicizieSerializzabili> datiDiplomazia;
    private RaycastHit hit;
    private OggettiDaMarcare oggettoDaMarcare;
    private Collider precedente = null;
    private Vista vistaGoblin;



    private void Awake()
    {
        me = this;
        Statici.inGioco = true;
    }

    private void Start()
    {
        GameObject tmpPdr = GameObject.Find("PadrePercorso");
        if (tmpPdr != null)
            Statici.padreGestore = tmpPdr.GetComponent<PadreGestore>();
        else
            Debug.LogError("Ma ci fai o ci sei ????..sei un cazzone....manca il padrePercorso");
        Statici.assegnaAssetDatabase();
    
        if (Statici.nomePersonaggio.Equals(string.Empty))
            Statici.nomePersonaggio = "PersonaggioDiProva";
        Statici.datiPersonaggio = new Serializzabile<ValoriPersonaggioS>(Statici.NomeFilePersonaggio);
        datiDiplomazia = new Serializzabile<AmicizieSerializzabili>(Statici.nomeFileDiplomazia);

        if (Statici.multigiocatoreOn) return;
        if (Statici.sonoPassatoDallaScenaIniziale)
        {
            GameObject tmpObjP = Instantiate(Resources.Load(Statici.datiPersonaggio.Dati.nomeModello), GameObject.Find(Statici.datiPersonaggio.Dati.posizioneCheckPoint).transform.position, Quaternion.identity) as GameObject;
           
            Statici.PersonaggioPrincipaleT = tmpObjP.transform;
            RecuperaDizionariDiplomazia();
        }
        else
        {
            if (Statici.datiPersonaggio.Dati.nomePersonaggio == null)
            {
                Statici.datiPersonaggio.Dati.Vita = 10;
                Statici.datiPersonaggio.Dati.Attacco = 20;
                Statici.datiPersonaggio.Dati.difesa = 5;
                Statici.datiPersonaggio.Dati.Xp = 19;
                Statici.datiPersonaggio.Dati.Livello = 1;
                Statici.datiPersonaggio.Dati.nomeModello = "MagoBluM";
                Statici.datiPersonaggio.Dati.nomePersonaggio = "PersonaggioDiProva";
                Statici.datiPersonaggio.Dati.classe = classiPersonaggi.magoAcqua.ToString();
                Statici.datiPersonaggio.Dati.VitaMassima = 10;
                Statici.datiPersonaggio.Dati.ManaMassimo = 20;
                Statici.datiPersonaggio.Dati.XPMassimo = 100;
                Statici.datiPersonaggio.Dati.posizioneCheckPoint = "start";
                Statici.datiPersonaggio.Dati.nomeScena = SceneManager.GetActiveScene().name;
                Statici.datiPersonaggio.Salva();
            }
            if (datiDiplomazia.Dati.tipoEssere[0] == null)
            {
                for (int i = 0; i < Statici.databseInizialeAmicizie.classiEssere.Length; i++)
                {
                    datiDiplomazia.Dati.tipoEssere[i] = Statici.databseInizialeAmicizie.classiEssere[i];
                }
                for (int i = 0; i < Statici.databseInizialeAmicizie.classiEssere.Length; i++)
                {
                    datiDiplomazia.Dati.matriceAmicizie[i] = Statici.databseInizialeAmicizie.matriceAmicizie[i];

                    for (int j = 0; j < Statici.databseInizialeAmicizie.classiEssere.Length; j++)
                    {
                        datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j] = Statici.databseInizialeAmicizie.matriceAmicizie[i].elementoAmicizia[j];
                    }
                }
                datiDiplomazia.Salva();
            }
            GameObject tmpObjP = Instantiate(Resources.Load(Statici.datiPersonaggio.Dati.nomeModello), GameObject.Find(Statici.datiPersonaggio.Dati.posizioneCheckPoint).transform.position, Quaternion.identity) as GameObject;
            Statici.PersonaggioPrincipaleT = tmpObjP.transform;
            RecuperaDizionariDiplomazia();
            Statici.CopiaIlDB();
            Statici.sonoPassatoDallaScenaIniziale = true;
        }
    }

    

    public void RecuperaDizionariDiplomazia()
    {
        Statici.dizionarioDiNemici.Clear();
        Statici.dizionarioDiIndifferenti.Clear();
        Statici.dizionarioDiAmici.Clear();
        List<classiPersonaggi> tmpNemici = null;
        List<classiPersonaggi> tmpAmici = null;
        List<classiPersonaggi> tmpIndifferenti = null;


        /*
        for (int i = 0; i < datiDiplomazia.Dati.tipoEssere.Length; i++)
        {
            tmpNemici = new List<classiPersonaggi>();
            tmpIndifferenti = new List<classiPersonaggi>();
            tmpAmici = new List<classiPersonaggi>();
            for (int j = 0; j < datiDiplomazia.Dati.tipoEssere.Length; j++)
            {
                switch (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j])
                {
                    case Amicizie.Neutro:
                        if (!tmpIndifferenti.Contains((classiPersonaggi)j))
                            tmpIndifferenti.Add((classiPersonaggi)j);                      
                        break;

                    case Amicizie.Alleato:
                        if (!tmpAmici.Contains((classiPersonaggi)j))
                            tmpAmici.Add((classiPersonaggi)j);
                        break;

                    case Amicizie.Nemico:
                        if (!tmpNemici.Contains((classiPersonaggi)j))
                            tmpNemici.Add((classiPersonaggi)j);
                        break;

                    default:
                        break;
                }
            }
            */

        for (int i = 0; i < datiDiplomazia.Dati.tipoEssere.Length; i++)
        {
            tmpNemici = new List<classiPersonaggi>();
            tmpIndifferenti = new List<classiPersonaggi>();
            tmpAmici = new List<classiPersonaggi>();
            for (int j = 0; j < datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia.Length; j++)
            {
                switch (datiDiplomazia.Dati.matriceAmicizie[i].elementoAmicizia[j])
                {
                    case Amicizie.Neutro:
                        if (!tmpIndifferenti.Contains((classiPersonaggi)j))
                            tmpIndifferenti.Add((classiPersonaggi)j);
                        break;

                    case Amicizie.Alleato:
                        if (!tmpAmici.Contains((classiPersonaggi)j))
                            tmpAmici.Add((classiPersonaggi)j);
                        break;

                    case Amicizie.Nemico:
                        if (!tmpNemici.Contains((classiPersonaggi)j))
                            tmpNemici.Add((classiPersonaggi)j);
                        break;

                    default:
                        break;
                }
            }


            if (!Statici.dizionarioDiNemici.ContainsKey((classiPersonaggi)i))
                Statici.dizionarioDiNemici.Add((classiPersonaggi)i, tmpNemici);
            if (!Statici.dizionarioDiAmici.ContainsKey((classiPersonaggi)i))
                Statici.dizionarioDiAmici.Add((classiPersonaggi)i, tmpAmici);
            if (!Statici.dizionarioDiIndifferenti.ContainsKey((classiPersonaggi)i))
                Statici.dizionarioDiIndifferenti.Add((classiPersonaggi)i, tmpIndifferenti);

            /*
            foreach (KeyValuePair<classiPersonaggi, List<classiPersonaggi>> pair in Statici.dizionarioDiNemici)
            {
                Debug.Log("chiave  " + pair.Key + "valore   " + pair.Value[0]);
            }
            */

     
        }
        if (vistaGoblin != null)
            vistaGoblin.AmiciziaCambiata = true;
        if (oggettoDaMarcare != null)
            oggettoDaMarcare.ControllaAmicizia = true;
        Statici.diplomaziaAggiornata = true;
    }


}