using UnityEngine;

public class DatiPersonaggio : MonoBehaviour
{
    public classiPersonaggi miaClasse;

    private float attacco;
    private float difesa;
    private GestoreCanvasNetwork gestoreCanvas;
    private bool giocabile;
    private int indicePunteggio = 0;
    private int livello;
    private float mana;
    private float manaMassimo;
    private string nome;
    private bool sonoUtenteLocale = false;
    private int utente;
    private float vita;
    private float vitaMassima;
    private float xp;
    private float xpMassimo;

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

    public int IndicePunteggio
    {
        get
        {
            return indicePunteggio;
        }

        set
        {
            indicePunteggio = value;
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

    public bool SonoUtenteLocale
    {
        get
        {
            return sonoUtenteLocale;
        }

        set
        {
            sonoUtenteLocale = value;
        }
    }

    public int Utente
    {
        get
        {
            return utente;
        }

        set
        {
            utente = value;
        }
    }

    public float Vita
    {
        get
        {
            return vita;
        }

        set
        {
            vita = Mathf.Clamp(value, 0, vitaMassima);
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

    private float CalcolaAttaccoCorrente()
    {
        //aggiungere tutta la logica per calcolare l'attacco in base agli oggetti che indossa il personaggio che aumentano l'attacco base
        //come l'arma, o che ne so un cappello particolare ecc...
        return attacco;
    }

    private float CalcolaDifesaCorrente()
    {
        //aggiungere tutta la logica per calcolare la difesa in base agli oggetti che indossa il personaggio che aumentano l'attacco base
        //come la difesa che ha la tunica, o che ne so un cappello particolare ecc...
        return difesa;
    }
}