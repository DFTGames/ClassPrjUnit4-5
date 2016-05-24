using UnityEngine;
using System.Collections;

public class NetworkPlayer : MonoBehaviour {

    private int user=-2; //inizializzato alla cazzum...(e' fuori dal intervallo che verra usato)
    public bool playerLocale { get; set; }  
   // private float tempoInvioTransorm = 0.1f;
    private float timeCorrente;
    private bool movimentoDirty;
    private ControllerMaga controller;
    private NetworkTransformInterpolation inter;

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
        timeCorrente = Time.time;        
    }

	void Update () { //QUA DENTRO C'E LA LOGICA PER I PLAYER LOCALI
        if (!playerLocale || !Statici.partenza) return;

        if (Statici.multigiocatoreOn && Statici.inGioco)
        {
            if (controller.RigidBody.velocity.magnitude > 0 || controller.NavMeshAgent.velocity.magnitude > 0)
                movimentoDirty = true;
        }

     if (!movimentoDirty) return;

        if (Time.time + Statici.tempoInvioTransform>timeCorrente)  
        {
            NetworkTransform net = NetworkTransform.CreaOggettoNetworktransform(transform.position, transform.rotation.eulerAngles);
            ManagerNetwork.InviaTransformLocali(net);
            movimentoDirty = false;
            timeCorrente = Time.time;  
        }
    
    }

    //QUESTO METODO E' PER I PLAYER REMOTI
    public  void ricevitransform(NetworkTransform net, int netUser)
    {
        if (playerLocale || user != netUser) return;

      if (inter == null )
        {
            transform.position = net.position;
            transform.rotation = Quaternion.Euler(net.rotation.x, net.rotation.y, net.rotation.z);
        }
       else
        {
         inter.ReceivedTransform(net);
        }
    }
    
}
