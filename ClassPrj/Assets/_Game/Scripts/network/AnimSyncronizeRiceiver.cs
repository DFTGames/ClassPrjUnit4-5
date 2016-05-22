using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class AnimSyncronRiceiver : MonoBehaviour
{

    float forward;
    float turn;
    bool onGround;
    float jump;
    float jumpLeg;
    bool attacco1;
    bool attacco2;

    private Animator anim;
    public ControllerMaga controller { get; set; }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    { 
            anim.SetFloat("Forward", forward);
            if (attacco1) anim.SetBool("attacco1", attacco1);
            if (attacco2) anim.SetBool("attacco2", attacco2);
            attacco1 = false; attacco2 = false;

        if (!(controller.IsPointAndClick))
        {
            anim.SetFloat("turn", turn);
            anim.SetBool("onGround", onGround);
            anim.SetFloat("jump", jump);
            anim.SetFloat("jumpLeg", jumpLeg);
        }
       //**************************************

    }
    public void eseguiAnimazioniRemoteT(ISFSObject sfsObjIn)  //esegue animazioni remote Tastiera
    {
        forward=sfsObjIn.GetFloat("f");
        turn=sfsObjIn.GetFloat("t");
        onGround=sfsObjIn.GetBool("o");
        jump=sfsObjIn.GetFloat("j");
        jumpLeg=sfsObjIn.GetFloat("jL");
        attacco1=sfsObjIn.GetBool("a1");
        attacco2=sfsObjIn.GetBool("a2");

    }

    public void eseguiAnimazioniRemoteC(ISFSObject sfsObjIn)  //esegue animazioni remote Punta e clicca
    {
        forward = sfsObjIn.GetFloat("f");
        attacco1 = sfsObjIn.GetBool("a1");
        attacco2 = sfsObjIn.GetBool("a2");

    }
}
