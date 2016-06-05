
using System;
using System.Collections;
using UnityEngine;

// This script is used to interpolate and predict values to make the position smooth
// And correspond to the real one.
public class NetworkTransformInterpolation : MonoBehaviour
{
    public enum InterpolationMode
    {
        INTERPOLATION,
        EXTRAPOLATION,
        NESSUNA
    }

   // public InterpolationMode mode = Statici.inter;    //InterpolationMode.INTERPOLATION; //cambiato da Luca

    private double interpolationBackTime = 200;

    // The maximum time we try to extrapolate
    private float extrapolationForwardTime = 1000; // Can make this depend on ping if needed

    private bool running = false;
    private Animator anim;
    private ControllerMaga contr;
    private NetworkTransform net;
    // We store twenty states with "playback" information
    NetworkTransform[] bufferedStates = new NetworkTransform[20];
    // Keep track of what slots are used
    int statesCount = 0;

    public Animator Anim
    { 
        set
        {
            anim = value;
        }
    }

    public ControllerMaga Contr
    {

        set
        {
            contr = value;
        }
    }

    // We call it on remote player to start receiving his transform
    public void StartReceiving()
    {
        running = true;
    }

    public void ReceivedTransform(NetworkTransform ntransform)
    {
        if (!running) return;
        if (Statici.inter == InterpolationMode.NESSUNA)
        {          
            net = ntransform;
           return;
        }

        // When receiving, buffer the information
        // Receive latest state information
        //	Vector3 pos = ntransform.position;
        //	Quaternion rot = ntransform.rotation;     		
        // Shift buffer contents, oldest data erased, 18 becomes 19, ... , 0 becomes 1
        for (int i = bufferedStates.Length - 1; i >= 1; i--)
        {
            bufferedStates[i] = bufferedStates[i - 1];
        }

        // Save currect received state as 0 in the buffer, safe to overwrite after shifting
        bufferedStates[0] = ntransform;

        // Increment state count but never exceed buffer size
        statesCount = Mathf.Min(statesCount + 1, bufferedStates.Length);

        // Check integrity, lowest numbered state in the buffer is newest and so on
        for (int i = 0; i < statesCount - 1; i++)
        {
            if (bufferedStates[i].timeStamp < bufferedStates[i + 1].timeStamp)
            {
                Debug.Log("State inconsistent");  //riordonarli..
            }
        }
    }


    // This only runs where the component is enabled, which is only on remote peers (server/clients)
    void Update()
    {
        
        if (!running) return;
        if (statesCount == 0 && Statici.inter != InterpolationMode.NESSUNA) return; // || (Statici.inter == InterpolationMode.NESSUNA && net==null)) return;
        
        UpdateValues();
        double currentTime = TimeManager.Instance.NetworkTime;// Debug.Log("currentTime" + currentTime);
        double interpolationTime = currentTime - interpolationBackTime;

        // We have a window of interpolationBackTime where we basically play 
        // By having interpolationBackTime the average ping, you will usually use interpolation.
        // And only if no more data arrives we will use extrapolation



        // Use interpolation
        // Check if latest state exceeds interpolation time, if this is the case then
        // it is too old and extrapolation should be used

      //  Debug.Log("son qua" + net.position);


        switch (Statici.inter)
        {
            case InterpolationMode.INTERPOLATION:
             //   Debug.Log("interpolazione ");
                if (bufferedStates[0].timeStamp > interpolationTime)
                {
                   
                    for (int i = 0; i < statesCount; i++)
                    {
                        // Find the state which matches the interpolation time (time+0.1) or use last state
                        if (bufferedStates[i].timeStamp <= interpolationTime || i == statesCount - 1)
                        {

                            // The state one slot newer (<100ms) than the best playback state
                            NetworkTransform rhs = bufferedStates[Mathf.Max(i - 1, 0)];
                            // The best playback state (closest to 100 ms old (default time))
                            NetworkTransform lhs = bufferedStates[i];

                            // Use the time between the two slots to determine if interpolation is necessary
                            double length = rhs.timeStamp - lhs.timeStamp;
                            float t = 0.0F;
                            // As the time difference gets closer to 100 ms t gets closer to 1 in 
                            // which case rhs is only used
                            if (length > 0.0001)
                            {
                                t = (float)((interpolationTime - lhs.timeStamp) / length);
                            }

                            // if t=0 => lhs is used directly
                            transform.position = Vector3.Lerp(lhs.position, rhs.position, t);
                            transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, lhs.rotation, 0), Quaternion.Euler(0, rhs.rotation, 0), t);
                            float lerpAnim = Mathf.Lerp(lhs.forward, rhs.forward, t);
                            anim.SetFloat("Forward", lerpAnim);

                            return;
                        }
                    }
                }
                break;

            case InterpolationMode.EXTRAPOLATION:
             //   Debug.Log("Estrpolazione ");

                {
                    // If the value we have is too old, use extrapolation based on 2 latest positions
                    float extrapolationLength = Convert.ToSingle(currentTime - bufferedStates[0].timeStamp) / 1000.0f;
                    if (extrapolationLength < extrapolationForwardTime && statesCount > 1)
                    {
                        Debug.Log("extrapolation");
                        Vector3 dif = bufferedStates[0].position - bufferedStates[1].position;
                        float distance = Vector3.Distance(bufferedStates[0].position, bufferedStates[1].position);
                        float timeDif = Convert.ToSingle(bufferedStates[0].timeStamp - bufferedStates[1].timeStamp) / 1000.0f;

                        if (Mathf.Approximately(distance, 0) || Mathf.Approximately(timeDif, 0))
                        {
                            transform.position = bufferedStates[0].position;
                            transform.rotation = Quaternion.Euler(0, bufferedStates[0].rotation, 0);
                            anim.SetFloat("Forward", bufferedStates[0].forward);
                            return;
                        }

                        float speed = distance / timeDif;

                        dif = dif.normalized;
                        Vector3 expectedPosition = bufferedStates[0].position + dif * extrapolationLength * speed;
                        transform.position = Vector3.Lerp(transform.position, expectedPosition, Time.deltaTime * speed);

                        float lerpAnim = Mathf.Lerp(anim.GetFloat("Forward"), bufferedStates[0].forward, Time.deltaTime * speed);
                        // non penso sia corretto..ma non mi veniva in mente nient'altro..su come estrapolare una animazione di una transform da proiettare nel futuro
                        anim.SetFloat("Forward", lerpAnim);
                    }
                    else
                    {
                        transform.position = bufferedStates[0].position;
                        anim.SetFloat("Forward", bufferedStates[0].forward);
                    }
                    // No extrapolation for the rotation
                    transform.rotation = Quaternion.Euler(0, bufferedStates[0].rotation, 0);
                }
                break;
            case InterpolationMode.NESSUNA:
               if (net == null) return;
                Debug.Log("son qua******" + net.position);
          //     if (net == null) Debug.Log("net e' nullo");
              
                transform.position = net.position;
                transform.rotation = Quaternion.Euler(0, net.rotation, 0);
                anim.SetFloat("Forward", net.forward);
                Debug.Log("Net position" + net.position + "rotazione " + transform.rotation + "Contro rotaz " + net.rotation);
                Debug.Log(" Interpolazione NEssuna");
                break;

            default:
                Debug.LogError("Modo non implementato");
                break;

        }


        Debug.Log("net attacchi" + net.attacchi);
        if ((net != null) && !net.isPointAndClick && ((net.attacchi & (byte)azioniPlayer.salto)==(byte)azioniPlayer.salto)) 
        {
            anim.SetFloat("Jump", net.jump); Debug.Log("jump  " + net.jump);
            anim.SetFloat("JumpLeg", net.jumpLeg); Debug.Log("jumpLeg  " + net.jumpLeg);

        }

    }

    private void UpdateValues()
    {
        double ping = TimeManager.Instance.AveragePing;
        if (ping < 50)
        {
            interpolationBackTime = 50;
        }
        else if (ping < 100)
        {
            interpolationBackTime = 100;
        }
        else if (ping < 200)
        {
            interpolationBackTime = 200;
        }
        else if (ping < 400)
        {
            interpolationBackTime = 400;
        }
        else if (ping < 600)
        {
            interpolationBackTime = 600;
        }
        else
        {
            interpolationBackTime = 1000;
        }
    }
}
