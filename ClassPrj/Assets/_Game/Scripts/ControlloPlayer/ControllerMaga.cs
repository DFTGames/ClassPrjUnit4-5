﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]


public class ControllerMaga : MonoBehaviour {

    #region Variabili PUBLIC
    [Range(0.3f,1f)]
    public float distanzaDaTerra = 0.3f;
    [Range(4f, 10f)]
    public float forzaSalto =4f;
    public bool corsaPerDefault = false;
    #endregion

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
    private bool aTerra;
    private bool voglioSaltare;
    private bool abbassato;
    private bool rimaniBasso;
    private bool attacco1 = false;
    private bool attacco2 = false;
    #endregion

    void Start () {
        
        rigidBody = GetComponent<Rigidbody>();
        animatore = GetComponent<Animator>();
        capsula = GetComponent<CapsuleCollider>();
        altezzaCapsula = capsula.height;
        capsulaCentro = new Vector3(0.0f,capsula.center.y,0.0f);
	}
	
	void Update () {

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        movimento = new Vector3(h, 0.0f, v);
        rotazione = Mathf.Atan2(h, v);
        velocitaSpostamento = 0.5f;

        if (!Input.GetKey(KeyCode.LeftShift) && corsaPerDefault || 
            !corsaPerDefault && Input.GetKey(KeyCode.LeftShift))
        {
            velocitaSpostamento = 1f;
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
        if(Input.GetMouseButtonDown(0) && !attacco2 )
        {
            attacco1 = true;
            attacco2 = false;
        }
        else if(Input.GetMouseButtonDown(1) && !attacco1)
        {
            attacco2 = true;
            attacco1 = false;
        }
        //DEBUG Bool Attacco
        Debug.Log(attacco1 + "o" + attacco2);

        if (Input.GetButtonDown("Jump") && !voglioSaltare)
        {
            voglioSaltare = true;
        }

	}

    void FixedUpdate()
    {
        aTerra = false;

        RaycastHit hit;

        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.down * 0.1f), Color.blue);
        if (Physics.Raycast(transform.position + (Vector3.up *0.1f),Vector3.down,out hit , distanzaDaTerra))
        {
            aTerra = true;
            animatore.applyRootMotion = true;
        }
        else
        {
            aTerra = false;
            animatore.applyRootMotion = false;
        }

        /*SALTO
        if(voglioSaltare && aTerra && !abbassato)
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, forzaSalto, rigidBody.velocity.z);
        }
        voglioSaltare = false;
        */
        /*CONTROLLO ABBASSATO
        Ray ray = new Ray(transform.position + (Vector3.up * capsulaCentro.y), Vector3.up);

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
        attacco1 = false;
        animatore.SetBool("attacco2", attacco2);
        attacco2 = false;
        //animatore.SetBool("Crouch", abbassato);

        /*
        if (!aTerra)
            animatore.SetFloat("Jump", rigidBody.velocity.y);
        */
    }
}
