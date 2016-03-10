using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CapsuleCollider)),RequireComponent(typeof(Animator)), RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(EventoAudio))]
public class ControllerMaga : MonoBehaviour
{

    #region Variabili PUBLIC
    [Range(0.3f, 1f)]
    public float distanzaDaTerra = 0.44f;
    [Range(4f, 10f)]
    public float forzaSalto = 4f;
    public float distanzaMassimaClick = 20f;

    public bool corsaPerDefault = false;
    public bool IsPointAndClick;

    #endregion Variabili PUBLIC

    #region Variabili PRIVATE
    private float h, v, rotazione;
    private float velocitaSpostamento;
    private float altezzaCapsula;
    private const float meta = 0.5f;
    private Rigidbody rigidBody;
    private EventoAudio ev_Audio;
    private AudioZona audioZona;
    private CapsuleCollider capsula;
    private Animator animatore;
    private Vector3 capsulaCentro;
    private Vector3 movimento;
    private Transform transform_m;
    private bool aTerra;
    private bool voglioSaltare = false;
    private bool abbassato;
    private bool rimaniBasso;
    private float cicloOffset = 0.2f;
    private float jumpLeg;
    private LayerMask layer = 1 << 13;
    private LayerMask layerAlberi = 1 << 13;
    private RaycastHit hit;
    private Vector3 posMouse;
    private NavMeshAgent navMeshAgent;
    private bool Destinazione = false;
    private DatiPersonaggio datiPersonaggio;
    #endregion Variabili PRIVATE

    private void Start()
    {
        transform_m = GetComponent<Transform>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.height = 2f;
            navMeshAgent.stoppingDistance = 1f;
        }
        navMeshAgent.enabled = true;

        rigidBody = GetComponent<Rigidbody>();
        if (rigidBody == null)
        {
            rigidBody = gameObject.AddComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        rigidBody.isKinematic = true;

        ev_Audio = GetComponent<EventoAudio>();
        if(ev_Audio == null)
            ev_Audio = gameObject.AddComponent<EventoAudio>();

        audioZona = GetComponent<AudioZona>();
        if (audioZona == null)
            audioZona = gameObject.AddComponent<AudioZona>();

        animatore = GetComponent<Animator>();
        capsula = GetComponent<CapsuleCollider>();
        altezzaCapsula = capsula.height;
        capsulaCentro = new Vector3(0.0f, capsula.center.y, 0.0f);
        IsPointAndClick = true;
        layerAlberi = ~layerAlberi;
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;
        datiPersonaggio = GetComponent<DatiPersonaggio>();
        GameManager.RegistraDatiPersonaggio(datiPersonaggio);
    }

    private void Update()
    {        
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {           
            rigidBody.isKinematic = false;
            capsula.enabled = true;
            navMeshAgent.enabled = false;
            return;
        }       

        if (IsPointAndClick)
        {
            if (navMeshAgent.enabled == false)
            {
                rigidBody.isKinematic = true;
                capsula.enabled = false;
                navMeshAgent.enabled = true;
            }
            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !EventSystem.current.IsPointerOverGameObject() 
                && !animatore.GetCurrentAnimatorStateInfo(0).IsName("Attacco1") 
                && !animatore.GetCurrentAnimatorStateInfo(0).IsName("Attacco2"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, distanzaMassimaClick,layerAlberi, QueryTriggerInteraction.Ignore))
                {
                    posMouse = hit.point;

                    switch (hit.transform.gameObject.layer) //Bozza da correggere ... Ho usato Tag Goblin per provare .
                    {
                        case 0:
                            if (Vector3.Distance(transform_m.position, posMouse) > navMeshAgent.stoppingDistance)
                                navMeshAgent.SetDestination(posMouse);
                            break;
                        case 11:
                            if (hit.collider.CompareTag("goblin"))
                                Attacco();
                            break;
                    }
                }      
            }
            animatore.SetFloat("Forward", navMeshAgent.velocity.normalized.magnitude);

        }
        else // Not Point & Click
        {
            if(rigidBody.isKinematic == true)
            {
                rigidBody.isKinematic = false;
                capsula.enabled = true;
                navMeshAgent.enabled = false;
            }

            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            movimento = new Vector3(h, 0.0f, v);
            rotazione = Mathf.Atan2(h, v)*Mathf.Rad2Deg;
            velocitaSpostamento = !Input.GetKey(KeyCode.LeftShift) && corsaPerDefault ||
                                  !corsaPerDefault && Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f;
          
            if (Input.GetButtonDown("Jump") && aTerra && !voglioSaltare &&!animatore.GetCurrentAnimatorStateInfo(0).IsName("Attacco1") &&
                !animatore.GetCurrentAnimatorStateInfo(0).IsName("Attacco2"))
                voglioSaltare = true;

            if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                Attacco();
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
            animatore.applyRootMotion = !IsPointAndClick;
        }
        else
        {
            aTerra = false;
            animatore.applyRootMotion = false;
            voglioSaltare = false;
        }
        ///*SALTO
        if (voglioSaltare && aTerra)
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
        animatore.SetFloat("JumpLeg", jumpLeg);
        //animatore.SetBool("Crouch", abbassato);
        if (!aTerra && !IsPointAndClick)
            animatore.SetFloat("Jump", rigidBody.velocity.y);
    }

    void Attacco ()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && !animatore.GetCurrentAnimatorStateInfo(0).IsName("Attacco1") && !animatore.GetCurrentAnimatorStateInfo(0).IsName("Attacco2"))
        {
            if (Input.GetMouseButtonDown(0) && !voglioSaltare && aTerra )
                animatore.SetTrigger("attacco1");

            if (Input.GetMouseButtonDown(1) && !voglioSaltare && aTerra )
                animatore.SetTrigger("attacco2");
        }
    }

    //richiamare questo metodo come evento dell'animazione attacco nel frame finale
    public void FaiDanno()
    {
        //controllare se il personaggio è girato verso il nemico e quindi se è nel suo arco di attacco
        //farsi dare la percentuale di resistere all'attacco dal nemico
        //calcolare un numero random da 1 a 100, per esempio supponendo che la percentuale del nemico di resistere sia del 20%,
        //se il  numero random è un numero inferiore a 20 l'attacco non è andato a buon fine se invece è un numero da 21 a 100 è andato a buonfine.
        //se l'attacco è andato a buonfine:
        //recuperare attaccobase del personaggio da DatiPersonaggio, e recuperare tutti i dati del nemico relativi alla sua difesa
        //calcolare il danno da effettuare in base a tutti i valori sopra citati(secondo una qualche equazione che li lega)
        //mandare un messaggio al metodo del nemico RiceviDanno passando come parametro la quantità di danno inflitta
    }    
    
    public void RiceviDanno(float quanto)
    {
        datiPersonaggio.Vita -= quanto;
        SalvaDatiVita();
    }

    private void SalvaDatiVita()
    {
        GameManager.datiPersonaggio.Dati.Vita = datiPersonaggio.Vita;
        GameManager.datiPersonaggio.Salva();
        GestoreCanvasAltreScene.AggiornaVita();
    }

    public void PozioneVita(float quanto)
    {
        datiPersonaggio.Vita += quanto;
        SalvaDatiVita();
    }

    

}