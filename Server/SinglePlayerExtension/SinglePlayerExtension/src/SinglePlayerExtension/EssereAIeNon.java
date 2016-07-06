/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package SinglePlayerExtension;

import com.smartfoxserver.v2.entities.User;
import java.util.Dictionary;
import java.util.Hashtable;

/**
 *
 * @author Ninfea
 */
public class EssereAIeNon {
    
    Transform t;
    double vita;
    double vitaMax;            
    String modello;
    String nome;
    String classe;
    int idClasse;
    String scena; 
    String checkPoint;
    boolean vivo=true;
    int numeroUccisioni=0;
    int numeroSpawn=0;
    boolean postoAssegnato=false;
    boolean pronto=false;
    double mana;
    double manaMax;
    double attacco;
    double difesa;
    boolean giocabile=false;
    double xp;
    double xpMax;
    double livello=0;
     
    //l'ho usato per risorgere passandogli la vita massima ma questo metodo si può usare anche nel caso si beva una pozione:
    public double RiceviVita(double vitaAttuale,double vitaAggiuntiva, double vitaMassima){
        vitaAttuale+=vitaAggiuntiva;
        
        vitaAttuale= clamp(vitaAttuale, 0d, vitaMassima);
        return vitaAttuale;
    }  
    
    public double RiceviDanno(double vitaAttuale,double danno,double vitaMassima){               
        vitaAttuale-=danno;  
        vitaAttuale=clamp(vitaAttuale, 0d, vitaMassima);
        return vitaAttuale;
    }   
    
    //clamp di float
      public float Clamp(float valore, float minimo, float massimo){
        return Math.max(minimo, Math.min(massimo, valore));
   }
    
      //clamp di qualsiasi tipo
  public static <T extends Comparable<T>> T clamp(T val, T min, T max) {
    if (val.compareTo(min) < 0) return min;
    else if (val.compareTo(max) > 0) return max;
    else return val;
}
}
