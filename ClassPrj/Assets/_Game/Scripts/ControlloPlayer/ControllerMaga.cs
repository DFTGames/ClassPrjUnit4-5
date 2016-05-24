using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CapsuleCollider)), RequireComponent(typeof(Animator)), RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(EventoAudio))]
public class ControllerMaga : MonoBehaviour
{

    #region Variabili PUBLIC
    [Range(0.3f, 1f)]
    public float distanzaDaTerra = 0.44f;
    [Range(4f, 10f)]
    public float forzaSalto = 4f;
    public float distanzaMassimaClick = 20f;

    public bool corsaPerDefault = false;
    //public bool IsPointAndClick;

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
    private SwitchVivoMorto switchVivoMorto;
    private ManagerNetwork managerNetwork;
    private GestoreCanvasNetwork gestoreCanvas;

    //AGGIUNTO PER MULTIPLAYER
    private AnimSyncronizeSender syncAnimS = null;
    private bool at1, at2 = false;
    //
    public DatiPersonaggio DatiPersonaggio
    {
        get
        {
            return datiPersonaggio;
        }

        set
        {
            datiPersonaggio = value;
        }
    }

    public Rigidbody RigidBody
    {
        get
        {
            return rigidBody;
        }


    }

    public NavMeshAgent NavMeshAgent
    {
        get
        {
            return navMeshAgent;
        }


    }

    public AnimSyncronizeSender SyncAnimS
    {
        get
        {
            return syncAnimS;
        }

        set
        {
            syncAnimS = value;
        }
    }
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
        if (ev_Audio == null)
            ev_Audio = gameObject.AddComponent<EventoAudio>();

        audioZona = GetComponent<AudioZona>();
        if (audioZona == null)
            audioZona = gameObject.AddComponent<AudioZona>();

        animatore = GetComponent<Animator>();
        capsula = GetComponent<CapsuleCollider>();
        altezzaCapsula = capsula.height;
        capsulaCentro = new Vector3(0.0f, capsula.center.y, 0.0f);
        //IsPointAndClick = true;
        layerAlberi = ~layerAlberi;
        switchVivoMorto = GetComponent<SwitchVivoMorto>();
        if (!Statici.inGioco)
            return;
        DatiPersonaggio = GetComponent<DatiPersonaggio>();

        if (!Statici.multigiocatoreOn)
        {
            Statici.RegistraDatiPersonaggio(DatiPersonaggio);
            //se all'inizio della partita si ritrova a 0 di vita, gli do 1 di vita così non nasce morto.
            if (DatiPersonaggio.Vita <= 0f)
            {
                DatiPersonaggio.Vita = 1f;
                SalvaDatiVita();
            }
        }
        else
        {

            managerNetwork = GameObject.Find("ManagerNetwork").GetComponent<ManagerNetwork>();
            gestoreCanvas = GameObject.Find("ManagerCanvasMultiplayer").GetComponent<GestoreCanvasNetwork>();
        }

    }

    private void Update()
    {

        //AGGIUNTA MULTIPLAYER
        if (!Statici.inGioco || (Statici.multigiocatoreOn && !DatiPersonaggio.SonoUtenteLocale))
        {
            rigidBody.isKinematic = false;
            capsula.enabled = true;
            navMeshAgent.enabled = false;
            return;
        }
        //
        if (Statici.IsPointAndClick)
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
                if (Physics.Raycast(ray, out hit, distanzaMassimaClick, layerAlberi, QueryTriggerInteraction.Ignore))
                {
                    posMouse = hit.point;

                    switch (hit.transform.gameObject.layer) //Bozza da correggere ... Ho usato Tag Goblin per provare .
                    {
                        case 0:
                            if (Vector3.Distance(transform_m.position, posMouse) > navMeshAgent.stoppingDistance)
                                navMeshAgent.SetDestination(posMouse);
                            break;
                        case 11:
                            if (hit.collider.transform != gameObject.transform) //se non sono io                          
                                Attacco();
                            break;
                    }
                }
            }
            animatore.SetFloat("Forward", navMeshAgent.velocity.normalized.magnitude);
            //AGGIUNTA MULTIPLAYER
            //  if (Statici.multigiocatoreOn && syncAnimS!=null) syncAnimS.forward = navMeshAgent.velocity.normalized.magnitude;
        }
        else // Not Point & Click
        {
            if (rigidBody.isKinematic == true)
            {
                rigidBody.isKinematic = false;
                capsula.enabled = true;
                navMeshAgent.enabled = false;
            }

            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            movimento = new Vector3(h, 0.0f, v);
            rotazione = Mathf.Atan2(h, v) * Mathf.Rad2Deg;
            velocitaSpostamento = !Input.GetKey(KeyCode.LeftShift) && corsaPerDefault ||
                                  !corsaPerDefault && Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f;

            if (Input.GetButtonDown("Jump") && aTerra && !voglioSaltare && !animatore.GetCurrentAnimatorStateInfo(0).IsName("Attacco1") &&
                !animatore.GetCurrentAnimatorStateInfo(0).IsName("Attacco2"))
                voglioSaltare = true;

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
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
            animatore.applyRootMotion = !Statici.IsPointAndClick;
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
        if (!aTerra && !Statici.IsPointAndClick)
            animatore.SetFloat("Jump", rigidBody.velocity.y);

        //AGGIUNTA MULTIPLAYER
        if (Statici.multigiocatoreOn && syncAnimS != null)
        {
            if (!Statici.IsPointAndClick)//DA CONTROLLARE IL NON PUNTA E CLICCA...
                syncAnimS.Forward = movimento.z * velocitaSpostamento;
            else
                syncAnimS.Forward = navMeshAgent.velocity.normalized.magnitude;

            syncAnimS.Turn = rotazione;
            syncAnimS.OnGround = aTerra;
            syncAnimS.Jump = rigidBody.velocity.y;
            syncAnimS.JumpLeg = jumpLeg;
            syncAnimS.Attacco1 = at1;
            syncAnimS.Attacco2 = at2;

            syncAnimS.controlloDirty();
            at1 = false;  //essendo trigger dopo essere passato a syncANims lo riporto a false
            at2 = false;   //essendo trigger dopo essere passato a syncANims lo riporto a false
        }

    }

    void Attacco()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && !animatore.GetCurrentAnimatorStateInfo(0).IsName("Attacco1") && !animatore.GetCurrentAnimatorStateInfo(0).IsName("Attacco2"))
        {
            if (Input.GetMouseButtonDown(0) && !voglioSaltare && aTerra)
            {
                animatore.SetTrigger("attacco1");
                at1 = true;
            }

            if (Input.GetMouseButtonDown(1) && !voglioSaltare && aTerra)
            {
                animatore.SetTrigger("attacco2");
                at2 = true;
            }
        }
    }

    //richiamare questo metodo come evento dell'animazione attacco nel frame finale o se viene lanciaato qualcosa appena viene colpito il bersaglio
    //quindi a seconda dell'attacco.
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
        DatiPersonaggio.Vita -= quanto;
        if (DatiPersonaggio.Vita <= 0f)
            switchVivoMorto.AttivaRagdoll();
        SalvaDatiVita();
    }
    public void Resuscita(float quanto)
    {
        DatiPersonaggio.Vita += quanto;
        switchVivoMorto.DisattivaRagdoll();
        SalvaDatiVita();
        Statici.PersonaggioPrincipaleT.position = GameObject.Find(Statici.datiPersonaggio.Dati.posizioneCheckPoint).transform.position;
    }

    private void SalvaDatiVita()
    {
        Statici.datiPersonaggio.Dati.Vita = DatiPersonaggio.Vita;
        Statici.datiPersonaggio.Salva();
        GestoreCanvasAltreScene.AggiornaVita();
    }

    public void PozioneVita(float quanto)
    {
        if (DatiPersonaggio.Vita > 0f)
        {
            DatiPersonaggio.Vita += quanto;
            SalvaDatiVita();
        }
    }

    /*
      void OnMouseDown()
      {
          if (!Statici.multigiocatoreOn || (Statici.multigiocatoreOn && DatiPersonaggio.SonoUtenteLocale))
              return;
          managerNetwork.NemicoColpito(DatiPersonaggio.Utente);
          gestoreCanvas.ResettaScrittaNemicoAttaccato(true);
          gestoreCanvas.UserDaVisualizzareNemico = DatiPersonaggio.Utente;
          gestoreCanvas.VitaDaVisualizzareNemico = DatiPersonaggio.Vita.ToString();

      }
      void OnMouseExit()
      {
         
            if(Statici.inGioco)
              gestoreCanvas.ResettaScrittaNemicoAttaccato(false);
      }*/


}