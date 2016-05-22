using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwitchVivoMortoAI : MonoBehaviour {

    private Collider[] ColliderRagdollSchiera;
    private List<Collider> ColliderRagdoll=new List<Collider>();
    private NavMeshAgent agente;
    private FSM cervello;
    private Animator animatore;
    private Rigidbody[] rbFigli;    
    private Collider colliderPadre;    
    
    // Use this for initialization
    void Start()
    {
        cervello = GetComponent<FSM>();
        animatore = GetComponent<Animator>();
        ColliderRagdollSchiera = GetComponentsInChildren<Collider>();        
        colliderPadre = GetComponent<Collider>();
        for(int i=1;i<ColliderRagdollSchiera.Length;i++)
        {
            if (ColliderRagdollSchiera[i].transform.name!="SferaVista")
                ColliderRagdoll.Add(ColliderRagdollSchiera[i]);            
        }        
        rbFigli = GetComponentsInChildren<Rigidbody>();             
        rbFigli[0].isKinematic = true;
        DisattivaRagdoll();     

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            cervello.RiceviDanno(cervello.DatiPersonaggio.VitaMassima);
        else
            if (Input.GetKeyDown(KeyCode.G))        
            cervello.Resuscita(cervello.DatiPersonaggio.VitaMassima);       
      
    }

    public void AttivaRagdoll()
    {            
        agente = GetComponent<NavMeshAgent>();
        agente.enabled = false;      
        colliderPadre.enabled = false;          
        for (int i = 0; i < ColliderRagdoll.Count-1; i++)
        {
            ColliderRagdoll[i].enabled = true;
            rbFigli[i].useGravity = true;            
        }     
        animatore.enabled = false;   
     
    }
    public void DisattivaRagdoll()
    {       
        for (int i = 0; i < ColliderRagdoll.Count-1; i++)
        {
            ColliderRagdoll[i].enabled = false;           
            rbFigli[i].useGravity = false;
            
        }
        colliderPadre.enabled = true;       
        agente = GetComponent<NavMeshAgent>();
        agente.enabled = true;
        animatore.enabled = true;
    }
}
