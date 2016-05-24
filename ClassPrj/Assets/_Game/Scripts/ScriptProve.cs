using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScriptProve : MonoBehaviour {

    public Slider sliderTempoTransform;
    public Slider sliderTempoAnimation;
    public Slider suonoAmbiente;
    public Slider suonoSFX;
    public Toggle isPointClick;
    public Dropdown inter;
    public Text textTimeTrans;
    public Text textTimeAnim;
    private NetworkPlayer netPlayer;

    void Start()
    {
        sliderTempoTransform.value = Statici.tempoInvioTransform;
        textTimeTrans.text = (Mathf.Round(Statici.tempoInvioTransform * 100) / 100).ToString();
        sliderTempoAnimation.value = Statici.tempoInvioAnimazione;
        textTimeAnim.text = (Mathf.Round(Statici.tempoInvioTransform * 100) / 100).ToString();
        isPointClick.isOn = Statici.IsPointAndClick;
        inter.value = 0;
      //  suonoAmbiente.value = ManagerIniziale.datiAudio.Dati.volEnvironment;
      //  suonoSFX.value = ManagerIniziale.datiAudio.Dati.volSFX;

        
        
    }
 
    public void AssegnaSliderTempoNetworkTransform(float g)
    {       
        Statici.tempoInvioTransform =g;
      //  Debug.Log("invio transform valore" + Statici.tempoInvioTransform);
        textTimeTrans.text = (Mathf.Round(g*100)/100).ToString();
    }

    public void AssegnaSliderTempoNetworkAnimation(float g)
    {
        Statici.tempoInvioAnimazione = g;
       //Debug.Log("invio transform valore" + Statici.tempoInvioAnimazione);
        textTimeAnim.text = (Mathf.Round(g * 100) / 100).ToString();
    }

    public void UsaTastiera(bool t)
    {
        Statici.IsPointAndClick = t;
      //  Debug.Log("Invio isPoint click " + Statici.IsPointAndClick);

    }
    public void TipoINterpolazione(int ob)
    {
        Statici.inter =(NetworkTransformInterpolation.InterpolationMode)ob;
    //    Debug.Log("camio interpolazione" + Statici.inter); 

    }
    public void cambiaSfx(float p)
    {
     //   ManagerIniziale.cambiaSfx_(p);
    }
    public void cambiaAmbiente(float p)
    {
     //  ManagerIniziale.cambiaAmbiente_(p);
    }
}
