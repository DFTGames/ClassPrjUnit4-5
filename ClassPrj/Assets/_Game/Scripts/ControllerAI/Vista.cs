using UnityEngine;


//N.B.DA MODIFICARE A SECONDA DELLE AMICIZIE/INIMICIZIE
public class Vista : MonoBehaviour
{
    private RaycastHit hit;
    private float alphaGradMezzi;
    private Vector3 vettoreDaTransformAObiettivo;
    private bool visto = false;
    private Transform obiettivo;
    private float angoloTraForwardEObiettivo;
    private float prodottoScalare;
    private float prodottoMagnitudini;
    private bool magaEntrata = false;

    private void OnTriggerEnter(Collider coll)
    {
        
        if (coll.CompareTag("Player"))
            magaEntrata = true;
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            magaEntrata = false;
            visto = false;
            gameObject.SendMessage("ControlloVista", visto, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnTriggerStay(Collider coll)
    {
        if (!magaEntrata) return;

        if (magaEntrata)
        {
            alphaGradMezzi = (gameObject.GetComponent<FSM>().alphaGrad) * 0.5f;
            vettoreDaTransformAObiettivo = (obiettivo.position - transform.position).normalized;
            prodottoScalare = Vector3.Dot(vettoreDaTransformAObiettivo, transform.forward);
            prodottoMagnitudini = vettoreDaTransformAObiettivo.magnitude * transform.forward.magnitude;
            angoloTraForwardEObiettivo = Mathf.Acos((prodottoScalare / prodottoMagnitudini)) * Mathf.Rad2Deg;

            if ((angoloTraForwardEObiettivo < alphaGradMezzi))
            {
                visto = true;
                if (Physics.Raycast(transform.position + transform.up * 0.5f, vettoreDaTransformAObiettivo, out hit, gameObject.GetComponent<SphereCollider>().radius))
                {
                    if (hit.collider.CompareTag("Player") && !visto)
                        visto = true;
                    else if (!hit.collider.CompareTag("Player") && visto)
                        visto = false;
                }
            }
            else if (visto)
                visto = false;

            gameObject.SendMessage("ControlloVista", visto, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void Start()
    {
        //da modificare in base alle amicizie/inimicizie.
        obiettivo = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (visto)
            Debug.DrawLine(transform.position, obiettivo.position, Color.red);
        else
            Debug.DrawLine(transform.position, obiettivo.position, Color.gray);
    }
}