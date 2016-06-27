using UnityEngine;

public class DatiPersonaggio : MonoBehaviour
{
    public classiPersonaggi miaClasse;

    private double attacco;
    private double difesa;
    private GestoreCanvasNetwork gestoreCanvas;
    private bool giocabile;
    private int indicePunteggio = 0;
    private double livello;
    private double mana;
    private double manaMassimo;
    private string nome;
    private bool sonoUtenteLocale = false;
    private int utente;
    private double vita;
    private double vitaMassima;
    private double xp;
    private double xpMassimo;

    public double Attacco
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

    public double Difesa
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

    public double Livello
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

    public double Mana
    {
        get
        {
            return mana;
        }

        set
        {
            mana = Statici.ClampDouble(value, 0, manaMassimo);
        }
    }

    public double ManaMassimo
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

    public double Vita
    {
        get
        {
            return vita;
        }

        set
        {
            vita = Statici.ClampDouble(value, 0, vitaMassima);
        }
    }

    public double VitaMassima
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

    public double Xp
    {
        get
        {
            return xp;
        }

        set
        {
            xp = Statici.ClampDouble(value, 0, xpMassimo);
        }
    }

    public double XpMassimo
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

    private double CalcolaAttaccoCorrente()
    {
        //aggiungere tutta la logica per calcolare l'attacco in base agli oggetti che indossa il personaggio che aumentano l'attacco base
        //come l'arma, o che ne so un cappello particolare ecc...
        return attacco;
    }

    private double CalcolaDifesaCorrente()
    {
        //aggiungere tutta la logica per calcolare la difesa in base agli oggetti che indossa il personaggio che aumentano l'attacco base
        //come la difesa che ha la tunica, o che ne so un cappello particolare ecc...
        return difesa;
    }
}