using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SwitchVivoMorto : MonoBehaviour {
      
    private Collider[] ColliderRagdoll;
    private NavMeshAgent agente;
    private ControllerMaga controller;
    private Animator animatore; 
    private Rigidbody[] rbFigli;

	// Use this for initialization
	void Start () {       
        controller = GetComponent<ControllerMaga>();       
        animatore = GetComponent<Animator>();    
        ColliderRagdoll = GetComponentsInChildren<Collider>();
        rbFigli= GetComponentsInChildren<Rigidbody>();
        DisattivaRagdoll();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;
        if (Input.GetKeyDown(KeyCode.F))
            controller.RiceviDanno(controller.DatiPersonaggio.VitaMassima);
        else
            if (Input.GetKeyDown(KeyCode.G))       
            controller.Resuscita(controller.DatiPersonaggio.VitaMassima);      
        
           
	}

    public void AttivaRagdoll()
    {
        for (int i = 1; i < ColliderRagdoll.Length; i++)
        {
            ColliderRagdoll[i].enabled = true;
            rbFigli[i].useGravity = true;
        }
        ColliderRagdoll[0].enabled = false;      
        controller.enabled = false;        
        agente = GetComponent<NavMeshAgent>();
        agente.enabled = false;
        rbFigli[0].isKinematic = true;
        animatore.enabled = false;
        rbFigli[0].useGravity = false;  
    }
    public void DisattivaRagdoll()
    {
        ColliderRagdoll[0].enabled = true;
        rbFigli[0].useGravity = true;
        for (int i = 1; i < ColliderRagdoll.Length; i++)
        {
            ColliderRagdoll[i].enabled = false;
            rbFigli[i].useGravity = false;
        }
        controller.enabled = true;
        agente = GetComponent<NavMeshAgent>();
        agente.enabled = true;     
        animatore.enabled = true; 
    }
}
