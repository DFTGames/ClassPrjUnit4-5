using UnityEngine;
using System.Collections;

public class NetworkPlayer : MonoBehaviour
{

    private int user = -2; //inizializzato alla cazzum...(e' fuori dal intervallo che verra usato)
    public bool playerLocale { get; set; }
    // private float tempoInvioTransorm = 0.1f;
    private float timeCorrente;
    private float timeAfterStop;
    private bool movimentoDirty;

    private Animator anim;
    private ControllerMaga controller;
    private NetworkTransformInterpolation inter;


    private byte attacchi;

    public int User
    {
        set
        {
            user = value;
        }
    }

    public NetworkTransformInterpolation Inter
    {
        set
        {
            inter = value;
        }
    }

    void Start()
    {

        controller = GetComponent<ControllerMaga>();
        if (controller != null) anim = controller.GetComponent<Animator>();
        timeCorrente = Time.time;
    }



    void Update()
    { //QUA DENTRO C'E LA LOGICA PER I PLAYER LOCALI

          if (!playerLocale || (playerLocale && !Statici.partenza)) return; //Statici.partenza controllar
      
        attacchi = 0;
        if (controller.Attacco1) attacchi = 1;
        if (controller.Attacco2) attacchi = 2;

        if (attacchi > 0) movimentoDirty = true;
        /*//METODO RIPORTATO DAL CONTROLLER MAGA.....PER RICORDARMI COME L'AVEVO FATTO...

        if (Statici.multigiocatoreOn && net != null)
        {
            net.alimentaAnimazione(forward, rotazione, aTerra, jump, jumpLeg, attacco1, attacco2);
        }
        attacco1 = false;
        attacco2 = false;
    */

        if (Statici.multigiocatoreOn && Statici.inGioco)
        {
            if ((controller.NavMeshAgent.velocity.magnitude > 0))
            {
                movimentoDirty = true;
                timeAfterStop = Time.time;
            }
        }

        if (!movimentoDirty) return;

        if ((timeCorrente + Statici.tempoInvioTransform) < Time.time)
        {
         //   Debug.Log("sto inviando " + "MOVIMENTO dIRTY " + movimentoDirty);
            NetworkTransform net = NetworkTransform.CreaOggettoNetworktransform(transform.position, transform.rotation.eulerAngles.y, controller.Forward, attacchi, 0);  //nel invio messo time a zero in quanto non lo uso (esempuio del shooter che lo manda sul server ma non viene usato)
            ManagerNetwork.InviaTransformAnimazioniLocali(net);
            attacchi = 0;  //azzero l'attacco
            timeCorrente = Time.time;
            if ((timeAfterStop + 0.5f) < Time.time) movimentoDirty = false;  //mi aspetta tot tempo per fare in modo che mi prenda con sicurezza anche l'arresto..considerando che siamo in UDP..e non tutti i pacchetti arrivano(vedere lezione 11 modulo 7)
        }

    }



    //QUESTO METODO E' PER I PLAYER REMOTI
    public void ricevitransform(NetworkTransform net, int netUser)
    {
        if (playerLocale || user != netUser) return;

        if (inter != null && Statici.inter != NetworkTransformInterpolation.InterpolationMode.NESSUNA)
        {
            inter.ReceivedTransform(net, anim);
            Debug.Log("non sono nullo " + " Inter  " + Statici.inter);
        }
        else
        {
            transform.position = net.position;
            transform.rotation = Quaternion.Euler(0, net.rotation, 0);
            anim.SetFloat("Forward", net.forward);       
            Debug.Log(" sono nullo " + " Inter  " + Statici.inter);
            Debug.Log("Net position" + net.position + "transform position " + transform.position);
        }

        if (net.attacchi == 1) anim.SetTrigger("attacco1");
        else if (net.attacchi == 2) anim.SetTrigger("attacco2");

    }

}
