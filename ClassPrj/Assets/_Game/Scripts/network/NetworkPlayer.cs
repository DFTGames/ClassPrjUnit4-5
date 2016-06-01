using UnityEngine;
using System.Collections;

public enum azioniPlayer
{
    attacco1=1,
    attacco2=2,
}

public class NetworkPlayer : MonoBehaviour
{
    
    private int user = -2; //inizializzato alla cazzum...(e' fuori dal intervallo che verra usato)
    public bool playerLocale { get; set; }
    // private float tempoInvioTransorm = 0.1f;
    private float timeCorrente;
    private float timeAfterStop;
    private bool movimentoDirty;
    private float rotazione;

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

        if (Statici.IsPointAndClick) rotazione = transform.rotation.eulerAngles.y;  //unico comportamento diverso se siamo punta & clicca o tastiera
               else rotazione = controller.Rotazione;
        attacchi = 0;
        if (controller.Attacco1) attacchi =(int)azioniPlayer.attacco1;
        if (controller.Attacco2) attacchi =(int)azioniPlayer.attacco2;

        if (attacchi > 0) movimentoDirty = true;


        if (Statici.multigiocatoreOn && Statici.inGioco)
        {
            if (controller.Forward > 0)
            {
                movimentoDirty = true;
                timeAfterStop = Time.time;
            }
        }

        if (!movimentoDirty) return;

        if ((timeCorrente + Statici.tempoInvioTransform) < Time.time)
        {

            NetworkTransform net = NetworkTransform.CreaOggettoNetworktransform(transform.position, rotazione, controller.Forward, attacchi, 0,controller.Jump,controller.JumpLeg);  //nel invio messo time a zero in quanto non lo uso (esempuio del shooter che lo manda sul server ma non viene usato)
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

        if ((net.attacchi & (int)azioniPlayer.attacco1) == (int)azioniPlayer.attacco1) anim.SetTrigger("attacco1");    //ho usato operatori binari 
        else if ((net.attacchi & (int)azioniPlayer.attacco2) == (int)azioniPlayer.attacco2) anim.SetTrigger("attacco2");  //ho usato operatori binari 

        if (Statici.IsPointAndClick)
        {
            anim.SetFloat("Jump", net.jump);
            anim.SetFloat("JumpLeg", net.jumpLeg);

        }

    }

}
