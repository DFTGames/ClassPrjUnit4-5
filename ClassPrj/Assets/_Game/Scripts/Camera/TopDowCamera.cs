using UnityEngine;
using System.Collections;

public class TopDowCamera : MonoBehaviour
{

    #region Variabili PUBLIC
    [Range(0f, 20f)]
    public float altezza = 1f;
    [Range(-20f, 20f)]
    public float ZOffSet;
    public float DampTime;
    #endregion

    #region Variabili PRIVATE
    private Transform Target = null;
    private Transform myTransform;
    private Vector3 velocita = Vector3.zero;
    private Vector3 Obiettivo;
    #endregion
    // Use this for initialization
    void Start()
    {

        myTransform = GetComponent<Transform>();
      
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null && GameObject.FindGameObjectWithTag("Player") != null)//+
            Target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        else if (Target != null)
        {
            Obiettivo = new Vector3(Target.position.x, Target.position.y + altezza, Target.position.z + ZOffSet);

            myTransform.position = Vector3.SmoothDamp(myTransform.position, Obiettivo, ref velocita, DampTime);

            if (ZOffSet != 0f)
            {
                myTransform.LookAt(Target.position);
            }
        }
    }
}
