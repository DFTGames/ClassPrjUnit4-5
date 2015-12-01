/*
1)Start:Per prima cosa raccolgo tutti i tag dei nemici di questo personaggio in base a quanto mostrato nel database
e li raccolgo nella schiera di stringhe "Nemici"
2)OnTriggerEnter: ogni volta che entra qualcuno nello sphereCollider controllo se è un nemico e quindi se fa parte della schiera Nemici che ho messo da parte,
se trovo il suo tag nella lista dei nemici interrompo la ricerca e lo aggiungo nella lista chiamata "listaNemiciDentroNonVisti"
3)Update:
  a)se ci sono nemici nella listaNemiciDentroNonVisti chiamo il metodo "ValutaSeAggiungereInListaVisti" che valuta se ogni singolo elemento di questa
  lista rientra nell'angolo di visuale, se rientra in quest'angolo(e se il raggio che parte dal personaggio e arriva al suo nemico
  non colpisce un ostacolo) vuol dire che è stato visto e quindi lo sposto in un'altra lista chiamata "listaNemiciVisti" e lo rimuovo
  dalla listaNemiciDentroNonVisti. Altrimenti non lo sposto.
  b)se ci sono elementi nella listaNemiciVisti, devo controllare se si spostano nella zona oltre l'angolo cioè se vanno fuori visuale, quindi richiamo il metodo
  "ValutaSeAggiungereInListaNonVisti" in cui controllo se il nemico si è spostato o meno nella zona non visibile(ma sempre dentro la sfera), se è così lo trasferisco nella lista
  listaNemiciDentroNonVisti, e lo rimuovo dalla listaNemiciVisti.
  Inoltre, sempre se ci sono elementi nella listaNemiciVisti, se non sto ancora inseguendo nessuno(oppure se la persona che stavo inseguendo esce dalla lista dei visti
  quindi non è più visto), cerco un altro nemico da inseguire che sarà il più vicino tra quelli che sto vedendo.
  Per trovare il più vicino, richiamo il metodo "ChiInseguire" che restituisce un trasform, che sarà il nemico che voglio inseguire, inoltre questo valore lo passerò al cervello
  per fargli capire chi deve inseguire e dirò al cervello che la sua variabile obiettivoInVista=true, che gli permetterà di attivare l'inseguimento.
  c)Altrimenti cioè se non ci sono più nemici nella listaNemiciVisti ma poco prima ne stavo inseguendo uno, dico al cervello che può smettere di inseguire e tornare a pattugliare,
  indicandogli che non ci sono più obiettivi nemici e  impostando il suo ObiettivoInVista=false, farò terminare lo stato corrente di inseguimento e farò
  ripartire il prossimo stato cioè il pattugliamento.
4)OnTriggerExit: se un elemento esce dallo spherecollider, se era nella listaNemiciVisti, lo tolgo oppure se era nella listaNemiciDentroNonVisti lo tolgo.

*/

using System.Collections.Generic;
using UnityEngine;

public class Vista : MonoBehaviour
{
    public Transform obiettivoDaInseguire;

    private RaycastHit hit;
    private float alphaGradMezzi;
    private Vector3 vettoreDaTransformAObiettivo;

    private float angoloTraForwardEObiettivo;
    private float prodottoScalare;
    private float prodottoMagnitudini;

    private SphereCollider colliderSfera;
    private FSM mioCervello;
    private string[] Nemici;

    public List<Transform> listaNemiciDentroNonVisti;

    public List<Transform> listaNemiciVisti;

    public bool hoSceltoChiInseguire = false;
    private float distanzaMinore = 0f;

    private Transform elemento;

    private void Start()
    {
        listaNemiciDentroNonVisti = new List<Transform>();
        mioCervello = GetComponent<FSM>();
        colliderSfera = GetComponent<SphereCollider>();
        obiettivoDaInseguire = null;

        Nemici = new string[GetComponent<VisualizzaAmicizie>().nemici.Length];

        for (int i = 0; i < Nemici.Length; i++)
            Nemici[i] = GetComponent<VisualizzaAmicizie>().nemici[i];
    }

    private void OnTriggerEnter(Collider coll)
    {
        for (int i = 0; i < Nemici.Length; i++)
        {
            if (coll.CompareTag(Nemici[i]))
            {
                listaNemiciDentroNonVisti.Add(coll.transform);

                break;
            }
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (listaNemiciDentroNonVisti.Contains(coll.transform))
            listaNemiciDentroNonVisti.Remove(coll.transform);
        else if (listaNemiciVisti.Contains(coll.transform))
            listaNemiciVisti.Remove(coll.transform);
    }

    private void Update()
    {
        if (listaNemiciDentroNonVisti.Count > 0)
            ValutaSeAggiungereInListaVisti();

        if (listaNemiciVisti.Count > 0)
        {
            ValutaSeAggiungereInListaNonVisti();
            if (!listaNemiciVisti.Contains(obiettivoDaInseguire))

            {
                obiettivoDaInseguire = ChiInseguire();
                mioCervello.obiettivoNemico = obiettivoDaInseguire;
                mioCervello.obiettivoInVista = true;
            }
        }
        else if (hoSceltoChiInseguire)
        {
            obiettivoDaInseguire = null;
            mioCervello.obiettivoNemico = obiettivoDaInseguire;
            mioCervello.obiettivoInVista = false;
            hoSceltoChiInseguire = false;
        }

        if (hoSceltoChiInseguire)
            Debug.DrawLine(transform.position, obiettivoDaInseguire.position, Color.red);
    }

    private void ValutaSeAggiungereInListaVisti()
    {
        for (int i = 0; i < listaNemiciDentroNonVisti.Count; i++)
        {
            alphaGradMezzi = (mioCervello.alphaGrad) * 0.5f;
            vettoreDaTransformAObiettivo = (listaNemiciDentroNonVisti[i].position - transform.position).normalized;
            prodottoScalare = Vector3.Dot(vettoreDaTransformAObiettivo, transform.forward);
            prodottoMagnitudini = vettoreDaTransformAObiettivo.magnitude * transform.forward.magnitude;
            angoloTraForwardEObiettivo = Mathf.Acos((prodottoScalare / prodottoMagnitudini)) * Mathf.Rad2Deg;

            if (angoloTraForwardEObiettivo < alphaGradMezzi)
            {
                if (Physics.Raycast(transform.position + transform.up * 0.5f, vettoreDaTransformAObiettivo, out hit, colliderSfera.radius))
                {
                    if (hit.collider.name.Equals(listaNemiciDentroNonVisti[i].name))
                    {
                        elemento = listaNemiciDentroNonVisti[i];
                        listaNemiciVisti.Add(elemento);
                        listaNemiciDentroNonVisti.RemoveAt(i);
                    }
                }
            }
        }
    }

    private void ValutaSeAggiungereInListaNonVisti()
    {
        for (int i = 0; i < listaNemiciVisti.Count; i++)
        {
            alphaGradMezzi = (mioCervello.alphaGrad) * 0.5f;
            vettoreDaTransformAObiettivo = (listaNemiciVisti[i].position - transform.position).normalized;
            prodottoScalare = Vector3.Dot(vettoreDaTransformAObiettivo, transform.forward);
            prodottoMagnitudini = vettoreDaTransformAObiettivo.magnitude * transform.forward.magnitude;
            angoloTraForwardEObiettivo = Mathf.Acos((prodottoScalare / prodottoMagnitudini)) * Mathf.Rad2Deg;

            if (angoloTraForwardEObiettivo > alphaGradMezzi)
            {
                elemento = listaNemiciVisti[i];
                listaNemiciDentroNonVisti.Add(elemento);
                listaNemiciVisti.RemoveAt(i);
            }
            else
            {
                if (Physics.Raycast(transform.position + transform.up * 0.5f, vettoreDaTransformAObiettivo, out hit, colliderSfera.radius))
                {
                    if (!hit.collider.name.Equals(listaNemiciVisti[i].name))
                    {
                        elemento = listaNemiciVisti[i];
                        listaNemiciDentroNonVisti.Add(elemento);
                        listaNemiciVisti.RemoveAt(i);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Metodo che seleziona l'obiettivo più vicino da inseguire.
    /// </summary>
    /// <returns></returns>
    private Transform ChiInseguire()
    {
        Transform obiettivoTemporaneoDaInseguire = null;
        hoSceltoChiInseguire = true;
        if (listaNemiciVisti.Count > 1)
        {
            for (int j = 0; j < listaNemiciVisti.Count - 1; j++)
            {
                float distanzaElementoAttuale = (listaNemiciVisti[j].position - transform.position).magnitude;
                float distanzaElementoSuccessivo = (listaNemiciVisti[j + 1].position - transform.position).magnitude;
                if (distanzaElementoAttuale <= distanzaElementoSuccessivo)

                    obiettivoTemporaneoDaInseguire = listaNemiciVisti[j];
                else

                    obiettivoTemporaneoDaInseguire = listaNemiciVisti[j + 1];
            }
        }
        else if (listaNemiciVisti.Count == 1)
        {
            obiettivoTemporaneoDaInseguire = listaNemiciVisti[0];
        }
        else
        {
            hoSceltoChiInseguire = false;
            obiettivoTemporaneoDaInseguire = null;
        }
        return obiettivoTemporaneoDaInseguire;
    }
}