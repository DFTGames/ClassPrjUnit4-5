using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class AnimSyncronRiceiver : MonoBehaviour {

     float forward;
    float turn;
     bool onGround;
     float jump;
     float jumpLeg;
    bool attacco1;
    bool attacco2;

    private Animator anim;

	void Start () {
        anim = GetComponent<Animator>();
	}
	
	void Update () {
        anim.SetFloat("Forward", forward);
        anim.SetBool("Attacco1", attacco1);
        anim.SetBool("Attacco2", attacco2);

    }
    public void eseguiAnimazioniRemoteT(ISFSObject sfsObjIn)  //esegue animazioni remote Tastiera
    {
        anim.SetFloat("Forward",sfsObjIn.GetFloat("f"));
        anim.SetFloat("Turn", sfsObjIn.GetFloat("t"));
        anim.SetBool("OnGround", sfsObjIn.GetBool("o"));
        anim.SetFloat("Jump", sfsObjIn.GetFloat("j"));
        anim.SetFloat("JumpLeg", sfsObjIn.GetFloat("jL"));
        anim.SetBool("Attacco1", sfsObjIn.GetBool("a1"));
        anim.SetBool("Attacco2", sfsObjIn.GetBool("a2"));

    }

    public void eseguiAnimazioniRemoteC(ISFSObject sfsObjIn)  //esegue animazioni remote Punta e clicca
    {
        forward= sfsObjIn.GetFloat("f");
        attacco1=sfsObjIn.GetBool("a1");
        attacco2=sfsObjIn.GetBool("a2");

    }
}
