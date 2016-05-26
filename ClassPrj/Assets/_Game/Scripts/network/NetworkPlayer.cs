using UnityEngine;
using System.Collections;

public class NetworkPlayer : MonoBehaviour {

    private int user=-2; //inizializzato alla cazzum...(e' fuori dal intervallo che verra usato)
    public bool playerLocale { get; set; }  
   // private float tempoInvioTransorm = 0.1f;
    private float timeCorrente;
    private bool movimentoDirty;
    private bool animazioneDirty;
    private Animator anim;
    private ControllerMaga controller;
    private NetworkTransformInterpolation inter;

    //animazione
    float forward;
    float turn;
    bool onGround;
    float jump;
    float jumpLeg;
    bool attacco1;
    bool attacco2;
    //fine animazioni

    private byte attacchi;

    public int User
    { 
        set
        {
            user = value;
        }
    }



    void Awake()
    {
        inter = GetComponent<NetworkTransformInterpolation>();
        if (inter != null)
        {
            inter.StartReceiving();
        }
    }
        void Start () {

        controller = GetComponent<ControllerMaga>();
        if (controller != null) anim = controller.GetComponent<Animator>();
        timeCorrente = Time.time;        
    }

    public void alimentaAnimazione(float forw,float tur,bool onGr,float jum,float jumL,bool att1,bool att2)
    {
        forw=forward;
        tur= turn;
        onGr=onGround;
        jum=jump;
        jumL=jumpLeg;
        att1=attacco1;
        att2=attacco2;

        if (forw > 0) animazioneDirty = true;   //PER ORA CONTROLLO SOLO PUNTA E CLICCKA IL FORWARD....SUCESSIVAMENTE VEDO UN PO PER IL CONTROLLO CON TASTIERA..E GLI AALTRI PARAMETRI
    }
	void Update () { //QUA DENTRO C'E LA LOGICA PER I PLAYER LOCALI
      

        if (!playerLocale || !Statici.partenza) return;

        if (Statici.multigiocatoreOn && Statici.inGioco)
        {
            if ((controller.RigidBody.velocity.magnitude > 0 || controller.NavMeshAgent.velocity.magnitude > 0) && animazioneDirty)
                movimentoDirty = true;                            
        }



     if (!movimentoDirty) return;

        if (Time.time + Statici.tempoInvioTransform>timeCorrente)
        {
            NetworkTransform net = NetworkTransform.CreaOggettoNetworktransform(transform.position, transform.rotation.eulerAngles.y,forward,attacco1,attacco2);
            ManagerNetwork.InviaTransformAnimazioniLocali(net);
            StartCoroutine(AspettaTempo());   //mi aspetta tot tempo per fare in modo che mi prenda con sicurezza anche l'arresto..considerando che siamo in UDP..e non tutti i pacchetti arrivano(vedere lezione 11 modulo 7)
            timeCorrente = Time.time;
        }

    }

    private IEnumerator AspettaTempo()
    {
        yield return new WaitForSeconds(0.5f);
        movimentoDirty = false;
    }

    //QUESTO METODO E' PER I PLAYER REMOTI
    public  void ricevitransform(NetworkTransform net, int netUser)
    {
        if (playerLocale || user != netUser) return;

      if (inter == null )
        {
            transform.position = net.position;
            transform.rotation = Quaternion.Euler(0, net.rotation,0);
            anim.SetFloat("Forward", net.forward);
            if (net.attacco1) anim.SetTrigger("attacco1");
            if (net.attacco2) anim.SetTrigger("attacco2");
        }
       else
        {
         inter.ReceivedTransform(net,anim);
        }
    }
    
}
