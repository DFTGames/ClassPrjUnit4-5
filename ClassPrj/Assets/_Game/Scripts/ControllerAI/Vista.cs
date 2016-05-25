using System.Collections.Generic;
using UnityEngine;

public class Vista : MonoBehaviour
{
    public List<Transform> listaNemiciDentroNonVisti;
    public List<Transform> listaNemiciVisti;

    private float alphaGradMezzi;
    private bool amiciziaCambiata = false;
    private bool amicizieCambiate = false;
    private float angoloTraForwardEObiettivo;
    private bool armaCambiata = false;
    private float conta = 0f;
    private DatiPersonaggio datiAltroPersonaggio;
    private DatiPersonaggio datiPersonaggio;
    private RaycastHit hit;
    private FSM mioCervello;
    private List<string> Nemici = null;
    private float prodottoMagnitudini;
    private float prodottoScalare;
    private List<Transform> tmpDaELiminare;
    private TipoArma tmpTipoArmaPrecedente;
    private Vector3 vettoreDaTransformAObiettivo;

    public bool AmiciziaCambiata
    {
        get
        {
            return amiciziaCambiata;
        }

        set
        {
            amiciziaCambiata = value;
        }
    }

    private void CalcolaAngolo(List<Transform> daTrattare, int i)
    {
        vettoreDaTransformAObiettivo = (daTrattare[i].position - transform.position).normalized;
        prodottoScalare = Vector3.Dot(vettoreDaTransformAObiettivo, transform.forward);
        prodottoMagnitudini = vettoreDaTransformAObiettivo.magnitude * transform.forward.magnitude;
        angoloTraForwardEObiettivo = Mathf.Acos((prodottoScalare / prodottoMagnitudini)) * Mathf.Rad2Deg;
    }

    private void OnTriggerEnter(Collider coll)
    {
        datiAltroPersonaggio = coll.GetComponent<DatiPersonaggio>();
        if (datiAltroPersonaggio != null)
        {
            if (Statici.dizionarioDiNemici[datiPersonaggio.miaClasse].Contains(datiAltroPersonaggio.miaClasse))
                listaNemiciDentroNonVisti.Add(coll.transform);
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (listaNemiciDentroNonVisti.Contains(coll.transform))
            listaNemiciDentroNonVisti.Remove(coll.transform);
        else if (listaNemiciVisti.Contains(coll.transform))
            listaNemiciVisti.Remove(coll.transform);
        datiAltroPersonaggio = null;
    }

    private void OnTriggerStay(Collider coll)
    {
        if (AmiciziaCambiata)
        {
            datiAltroPersonaggio = coll.GetComponent<DatiPersonaggio>();
            if (datiAltroPersonaggio != null)
            {
                if (Statici.dizionarioDiNemici[datiPersonaggio.miaClasse].Contains(datiAltroPersonaggio.miaClasse) && !listaNemiciDentroNonVisti.Contains(coll.transform) && !listaNemiciVisti.Contains(coll.transform))
                    listaNemiciDentroNonVisti.Add(coll.transform);
                else if (listaNemiciVisti.Contains(coll.transform) && !Statici.dizionarioDiNemici[datiPersonaggio.miaClasse].Contains(datiAltroPersonaggio.miaClasse))
                    listaNemiciVisti.Remove(coll.transform);
                else if (listaNemiciDentroNonVisti.Contains(coll.transform) && !Statici.dizionarioDiNemici[datiPersonaggio.miaClasse].Contains(datiAltroPersonaggio.miaClasse))
                    listaNemiciDentroNonVisti.Remove(coll.transform);
                AmiciziaCambiata = false;
            }
        }
    }

    private void Start()
    {
        listaNemiciDentroNonVisti = new List<Transform>();
        mioCervello = GetComponent<FSM>();
        mioCervello.ObiettivoNemico = null;
        alphaGradMezzi = (mioCervello.alphaGrad) * 0.5f;
        tmpDaELiminare = new List<Transform>();
        datiPersonaggio = GetComponent<DatiPersonaggio>();
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
        else // if (mioCervello.ObiettivoNemico != null)
        {
            Debug.DrawLine(transform.position + Vector3.up, mioCervello.ObiettivoNemico.position + Vector3.up, Color.red, Time.deltaTime);
            mioCervello.DistanzaTraPlayerGoblin = Vector3.Distance(mioCervello.Agente.transform.position, mioCervello.ObiettivoNemico.position);
            if (mioCervello.DistanzaTraPlayerGoblin <= mioCervello.distanzaAttaccoGoblinPugno)
            {
                if (tmpTipoArmaPrecedente != TipoArma.Pugno)
                    armaCambiata = true;

                mioCervello.TipoArma = TipoArma.Pugno;
                tmpTipoArmaPrecedente = mioCervello.TipoArma;
            }
            else
            {
                if (tmpTipoArmaPrecedente != TipoArma.Arco)
                    armaCambiata = true;

                mioCervello.TipoArma = TipoArma.Arco;
                tmpTipoArmaPrecedente = mioCervello.TipoArma;
            }

            bool inAttacco = mioCervello.DistanzaTraPlayerGoblin <= mioCervello.DistanzaAttacco ? true : false;

            if (!armaCambiata && inAttacco)
            {
                mioCervello.inZonaAttacco = true;
                conta = 0f;
            }
            else if (armaCambiata || !inAttacco)
            {
                mioCervello.inZonaAttacco = false;
                conta += Time.deltaTime;
                armaCambiata = false;
            }

            if ((!listaNemiciVisti.Contains(mioCervello.ObiettivoNemico) &&
                !mioCervello.inZonaAttacco && Mathf.Approximately(conta, 2f)) ||
                (!listaNemiciVisti.Contains(mioCervello.ObiettivoNemico) &&
                !listaNemiciDentroNonVisti.Contains(mioCervello.ObiettivoNemico)))
                mioCervello.ObiettivoNemico = null;
            else
               if (listaNemiciVisti.Contains(mioCervello.ObiettivoNemico))
                mioCervello.ObiettivoInVista = true;
            else
                mioCervello.ObiettivoInVista = false;
        }
    }
}