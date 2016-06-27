using UnityEngine;

public class SwitchVivoMorto : MonoBehaviour
{
    private NavMeshAgent agente;
    private Animator animatore;
    private Collider[] ColliderRagdoll;
    private ControllerMaga controller;
    private ManagerNetwork managerNetwork;
    private Rigidbody[] rbFigli;

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
        if (agente != null)
            agente.enabled = true;
        animatore.enabled = true;
    }

    // Use this for initialization
    private void Start()
    {
        controller = GetComponent<ControllerMaga>();
        animatore = GetComponent<Animator>();
        ColliderRagdoll = GetComponentsInChildren<Collider>();
        rbFigli = GetComponentsInChildren<Rigidbody>();
        DisattivaRagdoll();
        if (!Statici.multigiocatoreOn || !Statici.inGioco)
            return;
        managerNetwork = GameObject.Find("ManagerNetwork").GetComponent<ManagerNetwork>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Statici.inGioco)
            return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!Statici.multigiocatoreOn)
                controller.RiceviDanno((float)controller.DatiPersonaggio.VitaMassima);
            /*  else
              {
                  if (Statici.datiPersonaggioLocale.SonoUtenteLocale)
                      managerNetwork.NemicoColpito(Statici.userLocaleId);
              }*/
        }
        else
            if (Input.GetKeyDown(KeyCode.G))
        {
            if (!Statici.multigiocatoreOn)
                controller.Resuscita((float)controller.DatiPersonaggio.VitaMassima);
        }
    }
}