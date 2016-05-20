/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package sferenetsfsextension;

import com.smartfoxserver.v2.entities.User;
import java.util.Dictionary;
import java.util.Hashtable;

/**
 *
 * @author Ninfea
 */
public class Player {
    
    Transform t;
    float vita;
    float vitaMax;            
    String modello;
    String nome;
    String classe;
    String scena; 
    boolean vivo=true;
    int numeroUccisioni=0;
    int numeroSpawn=0;
    boolean postoAssegnato=false;
    boolean pronto=false;
    float mana;
    float manaMax;
    float attacco;
    float difesa;
    boolean giocabile=false;
    float xp;
    float xpMax;
    int livello=0;
     
    //l'ho usato per risorgere passandogli la vita massima ma questo metodo si può usare anche nel caso si beva una pozione:
    public float RiceviVita(float vitaAttuale,float vitaAggiuntiva, float vitaMassima){
        vitaAttuale+=vitaAggiuntiva;
        vitaAttuale=Clamp(vitaAttuale, 0, vitaMassima);
        return vitaAttuale;
    }  
    
    public float RiceviDanno(float vitaAttuale,float danno,float vitaMassima){               
        vitaAttuale-=danno;  
        vitaAttuale=Clamp(vitaAttuale, 0, vitaMassima);
        return vitaAttuale;
    }   
    
      public float Clamp(float valore, float minimo, float massimo){
        return Math.max(minimo, Math.min(massimo, valore));
   }
    
 
}
