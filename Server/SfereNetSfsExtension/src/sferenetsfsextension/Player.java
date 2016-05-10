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
    float vita=10;
    String modello;
    String nome;
    String scena;
    //String nomeSpawnPoint="";
    boolean vivo=true;
    int numeroUccisioni=0;
    int numeroSpawn=0;
    boolean postoAssegnato=false;
    boolean pronto=false;
     
            
    public float RiceviDanno(float vitaAttuale,float danno){               
        vitaAttuale-=danno;        
        return vitaAttuale;
    }    
    
    public float RiceviVita(float vitaAttuale,float vitaAggiuntiva){
        vitaAttuale+=vitaAggiuntiva;
        return vitaAttuale;
    }
    
   
    
}
