/*
1)Start:Per prima cosa raccolgo tutti i tag dei nemici di questo personaggio in base a quanto mostrato nel database
e li raccolgo nella lista di stringhe "Nemici"
2)OnTriggerEnter: ogni volta che entra qualcuno nello sphereCollider controllo se è un nemico e quindi se fa parte della lista Nemici che ho messo da parte,
se trovo il suo tag nella lista dei nemici interrompo la ricerca e lo aggiungo nella lista chiamata "listaNemiciDentroNonVisti"
3)Update:
  a)se ci sono nemici nella listaNemiciDentroNonVisti controllo se è il caso di aggiungerli nella lista "listaNemiciVisti" che valuta se ogni singolo elemento di questa
  lista rientra nell'angolo di visuale, se rientra in quest'angolo(e se il raggio che parte dal personaggio e arriva al suo nemico
  non colpisce un ostacolo) vuol dire che è stato visto e quindi lo sposto in un'altra lista chiamata "listaNemiciVisti" e lo rimuovo
  dalla listaNemiciDentroNonVisti. Altrimenti non lo sposto.
  b)se ci sono elementi nella listaNemiciVisti, devo controllare se si spostano nella zona oltre l'angolo cioè se vanno fuori visuale, quindi
  controllo se il nemico si è spostato o meno nella zona non visibile(ma sempre dentro la sfera), se è così lo trasferisco nella lista
  listaNemiciDentroNonVisti, e lo rimuovo dalla listaNemiciVisti.
  Inoltre, sempre se ci sono elementi nella listaNemiciVisti, se non sto ancora inseguendo nessuno(oppure se la persona che stavo inseguendo esce dalla lista dei visti
  quindi non è più visto), cerco un altro nemico da inseguire che sarà il più vicino tra quelli che sto vedendo.
  inoltre questo valore lo passerò al cervello.
  c)Altrimenti cioè se non ci sono più nemici nella listaNemiciVisti ma poco prima ne stavo inseguendo uno, dico al cervello che può smettere di inseguire e tornare a pattugliare,
  indicandogli che non ci sono più obiettivi nemici e  impostando il suo ObiettivoInVista=false, farò terminare lo stato corrente di inseguimento e farò
  ripartire il prossimo stato cioè il pattugliamento.
4)OnTriggerExit: se un elemento esce dallo spherecollider, se era nella listaNemiciVisti, lo tolgo oppure se era nella listaNemiciDentroNonVisti lo tolgo.

*/

using System.Collections.Generic;
using UnityEngine;

public class Vista : MonoBehaviour
{
    private RaycastHit hit;
    private float alphaGradMezzi;
    private Vector3 vettoreDaTransformAObiettivo;

    private float angoloTraForwardEObiettivo;
    private float prodottoScalare;
    private float prodottoMagnitudini;


    private FSM mioCervello;
    private List<string> Nemici = null;

    public List<Transform> listaNemiciDentroNonVisti;

    public List<Transform> listaNemiciVisti;

    private float distanzaMinore = 0f;
    private List<Transform> tmpDaELiminare;
    private bool amicizieCambiate = false;
    private float distanzaTraPlayerGoblin;
    

    private void Start()
    {
        listaNemiciDentroNonVisti = new List<Transform>();
        mioCervello = GetComponent<FSM>();      
        mioCervello.ObiettivoNemico = null;
        alphaGradMezzi = (mioCervello.alphaGrad) * 0.5f;
        tmpDaELiminare = new List<Transform>();
       
    }

    private void OnTriggerEnter(Collider coll)
    {
        //if (!GameManager.dizionarioDiNemici.ContainsKey(gameObject.tag)) return;
        if (GameManager.dizionarioDiNemici[gameObject.tag].Contains(coll.tag))
            listaNemiciDentroNonVisti.Add(coll.transform);
    }

    private void OnTriggerExit(Collider coll)
    {
        if (listaNemiciDentroNonVisti.Contains(coll.transform))
            listaNemiciDentroNonVisti.Remove(coll.transform);
        else if (listaNemiciVisti.Contains(coll.transform))
            listaNemiciVisti.Remove(coll.transform);
    }

    private void OnTriggerStay(Collider coll)
    {
       // if (!GameManager.dizionarioDiNemici.ContainsKey(gameObject.tag)) return;
        if (GameManager.dizionarioDiNemici[gameObject.tag].Contains(coll.tag) && !listaNemiciDentroNonVisti.Contains(coll.transform) && !listaNemiciVisti.Contains(coll.transform))
            listaNemiciDentroNonVisti.Add(coll.transform);

        else if (listaNemiciVisti.Contains(coll.transform) && !GameManager.dizionarioDiNemici[gameObject.tag].Contains(coll.tag))
            listaNemiciVisti.Remove(coll.transform);

        else if (listaNemiciDentroNonVisti.Contains(coll.transform) && !GameManager.dizionarioDiNemici[gameObject.tag].Contains(coll.tag))
            listaNemiciDentroNonVisti.Remove(coll.transform);
    }

    private void Update()
    {
        if (listaNemiciDentroNonVisti.Count > 0)
        {  //valuto se aggiungere in listaNemiciVisti
            for (int i = 0; i < listaNemiciDentroNonVisti.Count; i++)
            {
                CalcolaAngolo(listaNemiciDentroNonVisti, i);

                if (angoloTraForwardEObiettivo < alphaGradMezzi)
                {
                    if (Physics.Raycast(transform.position + transform.up * 0.5f, vettoreDaTransformAObiettivo, out hit, mioCervello.ColliderSferaVista.radius))
                    {
                        if (hit.collider.transform == listaNemiciDentroNonVisti[i])
                        {
                            listaNemiciVisti.Add(listaNemiciDentroNonVisti[i]);
                            tmpDaELiminare.Add(listaNemiciDentroNonVisti[i]);
                        }
                    }
                }
            }
            for (int i = 0; i < tmpDaELiminare.Count; i++)
            {
                listaNemiciDentroNonVisti.Remove(tmpDaELiminare[i]);
            }
            tmpDaELiminare.Clear();
        }

        if (listaNemiciVisti.Count > 0)
        {  //valuto se aggiungere in listaDentroNemiciNonVisti
            for (int i = 0; i < listaNemiciVisti.Count; i++)
            {
                CalcolaAngolo(listaNemiciVisti, i);

                if (angoloTraForwardEObiettivo > alphaGradMezzi)
                {
                    listaNemiciDentroNonVisti.Add(listaNemiciVisti[i]);
                    tmpDaELiminare.Add(listaNemiciVisti[i]);
                }
                else
                {
                    if (Physics.Raycast(transform.position + transform.up * 0.5f, vettoreDaTransformAObiettivo, out hit, mioCervello.ColliderSferaVista.radius))
                    {
                        if (!hit.collider.transform == listaNemiciVisti[i])
                        {
                            listaNemiciDentroNonVisti.Add(listaNemiciVisti[i]);
                            tmpDaELiminare.Add(listaNemiciVisti[i]);
                        }
                    }
                }
            }
            for (int i = 0; i < tmpDaELiminare.Count; i++)
            {
                listaNemiciVisti.Remove(tmpDaELiminare[i]);
            }
            tmpDaELiminare.Clear();
        }

        if (mioCervello.ObiettivoNemico == null)
        {   //scelgo chi inseguire:
            Transform obiettivoTemporaneoDaInseguire = null;
            int vicino = -1;
            float distanzaMinore = float.MaxValue;
            if (listaNemiciVisti.Count > 1)
            {
                for (int j = 0; j < listaNemiciVisti.Count; j++)
                {
                    float distanzaElementoAttuale = (listaNemiciVisti[j].position - transform.position).magnitude;
                    if (distanzaElementoAttuale < distanzaMinore)
                    {
                        distanzaMinore = distanzaElementoAttuale;
                        vicino = j;
                    }
                }
                obiettivoTemporaneoDaInseguire = listaNemiciVisti[vicino];
            }
            else if (listaNemiciVisti.Count == 1)
            {
                obiettivoTemporaneoDaInseguire = listaNemiciVisti[0];
            }
            else
            {
                obiettivoTemporaneoDaInseguire = null;
            }
            mioCervello.ObiettivoNemico = obiettivoTemporaneoDaInseguire;


        }
        else
        
        {
            Debug.DrawLine(transform.position + Vector3.up, mioCervello.ObiettivoNemico.position + Vector3.up, Color.red, Time.deltaTime);
            distanzaTraPlayerGoblin = Vector3.Distance(mioCervello.Agente.transform.position, mioCervello.ObiettivoNemico.position);
            if (distanzaTraPlayerGoblin <= mioCervello.distanzaAttacco)            
                mioCervello.inZonaAttacco = true;            
            else
                mioCervello.inZonaAttacco = false;

            if (!listaNemiciVisti.Contains(mioCervello.ObiettivoNemico))
                mioCervello.ObiettivoNemico = null;
        }
    }

    private void CalcolaAngolo(List<Transform> daTrattare, int i)
    {
        vettoreDaTransformAObiettivo = (daTrattare[i].position - transform.position).normalized;
        prodottoScalare = Vector3.Dot(vettoreDaTransformAObiettivo, transform.forward);
        prodottoMagnitudini = vettoreDaTransformAObiettivo.magnitude * transform.forward.magnitude;
        angoloTraForwardEObiettivo = Mathf.Acos((prodottoScalare / prodottoMagnitudini)) * Mathf.Rad2Deg;
    }
}