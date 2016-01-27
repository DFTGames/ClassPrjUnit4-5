using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class ControllerMaga : MonoBehaviour
{

    #region Variabili PUBLIC
    [Range(0.3f, 1f)]
    public float distanzaDaTerra = 0.3f;
    [Range(4f, 10f)]
    public float forzaSalto = 4f;
    public float distanzaMassimaClick = 20f;
    public float distanzaAnnullaClick = 1f;

    public bool corsaPerDefault = false;
    public bool SwitchController;

    #endregion Variabili PUBLIC

    #region Variabili PRIVATE
    private float h, v, rotazione;
    private float velocitaSpostamento;
    private float altezzaCapsula;
    private const float meta = 0.5f;
    private Rigidbody rigidBody;
    private CapsuleCollider capsula;
    private Animator animatore;
    private Vector3 capsulaCentro;
    private Vector3 movimento;
    private Transform transform_m;
    private bool aTerra;
    private bool voglioSaltare = false;
    private bool abbassato;
    private bool rimaniBasso;
    private bool attacco1 = false;
    private bool attacco2 = false;
    private float cicloOffset = 0.2f;
    private float jumpLeg;
    private RaycastHit hit;
    private Vector3 posMouse;
    private NavMeshAgent Controller;
    private bool Destinazione = false;
    #endregion Variabili PRIVATE

    private void Start()
    {
        transform_m = GetComponent<Transform>();

        if(Controller == null)
        {
            Controller = gameObject.AddComponent<NavMeshAgent>();
            Controller = GetComponent<NavMeshAgent>();
        }

        Controller.height = 2f;
        Controller.stoppingDistance = 1f;
        Controller.enabled = false;

        if(rigidBody == null && !SwitchController)
        {
            rigidBody = gameObject.AddComponent<Rigidbody>();
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

        animatore = GetComponent<Animator>();
        capsula = GetComponent<CapsuleCollider>();
        altezzaCapsula = capsula.height;
        capsulaCentro = new Vector3(0.0f, capsula.center.y, 0.0f);
    }

    private void Update()
    {
        if (Application.loadedLevel == 0) return;

        if (SwitchController)
        {
            if (Controller.enabled == false || rigidBody != null)
            {
                Destroy(rigidBody);
                capsula.enabled = false;
                Controller.enabled = true;
            }
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, distanzaMassimaClick))
                {
                    posMouse = hit.point;
                    posMouse.y = transform_m.position.y;
                    if (Vector3.Distance(transform_m.position, posMouse) > distanzaAnnullaClick)
                    {
                        Destinazione = true;
                    }
                }
            }

            if (Destinazione && Vector3.Distance(transform_m.position, posMouse) > distanzaAnnullaClick)
            {
                Controller.SetDestination(posMouse);
                animatore.SetFloat("Forward", 1f);
            }
            else
            {
                Destinazione = false;
            }

        }
        else if (!SwitchController)
        {
            if (rigidBody == null && !SwitchController)
            {
                rigidBody = gameObject.AddComponent<Rigidbody>();
                rigidBody = GetComponent<Rigidbody>();
                rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
            capsula.enabled = true;
            Controller.enabled = false;

            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            velocitaSpostamento = 0.5f;
            movimento = new Vector3(h, 0.0f, v);
            rotazione = Mathf.Atan2(h, v);

            if (!Input.GetKey(KeyCode.LeftShift) && corsaPerDefault ||
                !corsaPerDefault && Input.GetKey(KeyCode.LeftShift))
            {
                velocitaSpostamento = 1f;
            }
            if (!EventSystem.current.IsPointerOverGameObject())
            {

                if (Input.GetMouseButtonDown(0) && !voglioSaltare && aTerra && !attacco1)
                {
                    attacco1 = true;
                }
                else if (animatore.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && !animatore.IsInTransition(0) && attacco1)
                {
                    attacco1 = false;
                }
                if (Input.GetMouseButtonDown(1) && !voglioSaltare && aTerra && !attacco2)
                {
                    attacco2 = true;
                }
                else if (animatore.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f && !animatore.IsInTransition(0) && attacco2)
                {

                    attacco2 = false;
                }
            }
            if (Input.GetButtonDown("Jump") && aTerra && !voglioSaltare &&
       !animatore.GetCurrentAnimatorStateInfo(0).IsName("Attacco1") && !attacco1 &&
       !animatore.GetCurrentAnimatorStateInfo(0).IsName("Attacco2") && !attacco2)
            {
                voglioSaltare = true;
            }
            /* ACCOVACCIAMENTO
            if (!voglioSaltare && aTerra && Input.GetKey(KeyCode.C))
             {
                 abbassato = true;
                 capsula.center = capsulaCentro * meta;
                 capsula.height = capsula.height * meta;
             }
             else
             {
                 if (!rimaniBasso)
                 {
                     abbassato = false;
                     capsula.height = altezzaCapsula;
                     capsula.center = capsulaCentro;
                     rimaniBasso = false;
                 }
             }
             */
        }

    }

    private void FixedUpdate()
    {
        aTerra = false;
        RaycastHit hit;

        Debug.DrawLine(transform_m.position + (Vector3.up * 0.1f), transform_m.position + (Vector3.down * 0.1f), Color.blue);
        if (Physics.Raycast(transform_m.position + (Vector3.up * 0.1f), Vector3.down, out hit, distanzaDaTerra))
        {
            aTerra = true;
            if (SwitchController)
            {
                animatore.applyRootMotion = false;
            }
            else
            {
                animatore.applyRootMotion = true;
            }
        }
        else
        {
            aTerra = false;
            animatore.applyRootMotion = false;
            voglioSaltare = false;
        }
        ///*SALTO
        if (voglioSaltare && aTerra && !abbassato)
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, forzaSalto, rigidBody.velocity.z);
            float cicloCamminata = Mathf.Repeat(animatore.GetCurrentAnimatorStateInfo(0).normalizedTime + cicloOffset, 1);
            jumpLeg = (cicloCamminata < 0.5f ? 1 : -1) * movimento.z;
        }
        // */
        /*CONTROLLO ABBASSATO
        Ray ray = new Ray(transform_m.position + (Vector3.up * capsulaCentro.y), Vector3.up);

        float lunghezzaRay = capsulaCentro.y;

        if (Physics.SphereCast(ray, capsula.radius * meta, lunghezzaRay) && abbassato)
            rimaniBasso = true;
        else
            rimaniBasso = false;
         */
        /* ANIMATORE */
        animatore.SetBool("OnGround", aTerra);
        animatore.SetFloat("Forward", movimento.z * velocitaSpostamento);
        animatore.SetFloat("Turn", rotazione);
        animatore.SetBool("attacco1", attacco1);
        animatore.SetBool("attacco2", attacco2);
        animatore.SetFloat("JumpLeg", jumpLeg);
        //animatore.SetBool("Crouch", abbassato);
        if (!aTerra && !SwitchController)
            animatore.SetFloat("Jump", rigidBody.velocity.y);
    }
}