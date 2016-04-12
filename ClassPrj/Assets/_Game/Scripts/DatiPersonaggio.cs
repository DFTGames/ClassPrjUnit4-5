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
    private string nome;

    public float Vita
    {
        get
        {
            return vita;
        }

        set
        {        
            vita= Mathf.Clamp(value, 0, vitaMassima);
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
            mana = Mathf.Clamp(value, 0, manaMassimo);
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
            xp = Mathf.Clamp(value, 0, xpMassimo);
        }
    }

    private float CalcolaAttaccoCorrente()
    {
        //aggiungere tutta la logica per calcolare l'attacco in base agli oggetti che indossa il personaggio che aumentano l'attacco base
        //come l'arma, o che ne so un cappello particolare ecc...
        return attacco;
    }

    public float Attacco
    {
        get
        {
            return CalcolaAttaccoCorrente();
        }

        set
        {
            attacco = value;
        }
    }

    private float CalcolaDifesaCorrente()
    {
        //aggiungere tutta la logica per calcolare la difesa in base agli oggetti che indossa il personaggio che aumentano l'attacco base
        //come la difesa che ha la tunica, o che ne so un cappello particolare ecc...
        return difesa;
    }

    public float Difesa
    {
        get
        {
            return CalcolaDifesaCorrente();
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

    public string Nome
    {
        get
        {
            return nome;
        }

        set
        {
            nome = value;
        }
    }

    // Use this for initialization
    void Start () {     
     
      
      
    }

  
}
