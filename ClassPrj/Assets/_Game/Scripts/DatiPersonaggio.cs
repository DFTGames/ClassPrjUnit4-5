using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DatiPersonaggio : MonoBehaviour {
    public classiPersonaggi miaClasse;

    private float vita;
    private float vitaMassima;
    private float manaMassimo;
    private float xpMassimo;
    private bool giocabile;
    private int livello;
    private float mana;
    private float xp;
    private float attacco;
    private float difesa; 

    public float Vita
    {
        get
        {
            return vita;
        }

        set
        {
            vita = value;
        }
    }

    public bool Giocabile
    {
        get
        {
            return giocabile;
        }

        set
        {
            giocabile = value;
        }
    }

    public int Livello
    {
        get
        {
            return livello;
        }

        set
        {
            livello = value;
        }
    }

    public float Mana
    {
        get
        {
            return mana;
        }

        set
        {
            mana = value;
        }
    }

    public float Xp
    {
        get
        {
            return xp;
        }

        set
        {
            xp = value;
        }
    }

    public float Attacco
    {
        get
        {
            return attacco;
        }

        set
        {
            attacco = value;
        }
    }

    public float Difesa
    {
        get
        {
            return difesa;
        }

        set
        {
            difesa = value;
        }
    }

    public float VitaMassima
    {
        get
        {
            return vitaMassima;
        }

        set
        {
            vitaMassima = value;
        }
    }

    public float ManaMassimo
    {
        get
        {
            return manaMassimo;
        }

        set
        {
            manaMassimo = value;
        }
    }

    public float XpMassimo
    {
        get
        {
            return xpMassimo;
        }

        set
        {
            xpMassimo = value;
        }
    }

    // Use this for initialization
    void Start () {
        
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;
        GameManager.RegistraDatiPersonaggio(this);
        GameManager.RecuperaDati(this);
      
    }
	
	// Update is called once per frame
	void Update () {
       
    }
  
}
