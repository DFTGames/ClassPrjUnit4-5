using UnityEngine;
using System.Collections;

public class AnimSyncronizeSender : MonoBehaviour {

    //Animazioni Locali Da inviare
    private float forward;
    private float turn;
    private bool onGround;
    private float jump;
    private float jumpLeg;
    private bool attacco1;
    private bool attacco2;


    // valori precedenti..(per il controllo del dirty)
    float Pforward;
    float Pturn;
    bool PonGround;
     float Pjump;
     float PjumpLeg;
     bool Pattacco1;
     bool Pattacco2;
    
    private int user = Statici.userLocaleId;
    private float timeLastSending = 0.0f;
    //  public ControllerMaga controller {get; set; }


    public float Forward
    {
        get
        {
            return forward;
        }

        set
        {
            forward = value;
        }
    }

    public float Turn
    {
        get
        {
            return turn;
        }

        set
        {
            turn = value;
        }
    }

    public bool OnGround
    {
        get
        {
            return onGround;
        }

        set
        {
            onGround = value;
        }
    }

    public float Jump
    {
        get
        {
            return jump;
        }

        set
        {
            jump = value;
        }
    }

    public float JumpLeg
    {
        get
        {
            return jumpLeg;
        }

        set
        {
            jumpLeg = value;
        }
    }

    public bool Attacco1
    {
        get
        {
            return attacco1;
        }

        set
        {
            attacco1 = value;
        }
    }

    public bool Attacco2
    {
        get
        {
            return attacco2;
        }

        set
        {
            attacco2 = value;
        }
    }

    public void controlloDirty()
    {
        if (timeLastSending >= Statici.tempoInvioAnimazione)   
        {
            timeLastSending = 0;

            if (Statici.IsPointAndClick)
            {
                if ((forward != Pforward) || (attacco1 != Pattacco1) || (attacco2 != Pattacco2))
                {
                    Pforward = forward;
                    Pattacco1 = attacco1;
                    Pattacco2 = attacco2;
                    ManagerNetwork.InviaAnimazioneControllerClick(forward, attacco1, attacco2);
                }

            }
            else if ((forward != Pforward) || (turn != Pturn) || (onGround != PonGround) || (jump != Pjump) || (jumpLeg != PjumpLeg) || (attacco1 != Pattacco1) || (attacco2 != Pattacco2))
            {
                Pforward = forward;
                Pturn = turn;
                PonGround = onGround;
                Pjump = jump;
                PjumpLeg = jumpLeg;
                Pattacco1 = attacco1;
                Pattacco2 = attacco2;
                ManagerNetwork.InviaAnimazioneControllerTast(forward, turn, onGround, jump, jumpLeg, attacco1, attacco2);
            }
        }
        timeLastSending += Time.deltaTime;
    }
}
