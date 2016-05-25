/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package sferenetsfsextension;

import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.RoomVariable;
import com.smartfoxserver.v2.entities.variables.SFSRoomVariable;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Date;
import java.util.Dictionary;
import java.util.Hashtable;
import java.util.List;
import java.util.Random;

/**
 *
 * @author Ninfea
 */
public class Mondo {
    private SfereNetSfsExtension extension;
    public Dictionary<User,Player> dizionarioUtentiPlayer=new Hashtable();
    //public List<User> listaUtentiInScenaEInStanza=new ArrayList();
    public User utenteOwner;
    public List<Integer> listaSpawnPointLiberi=new ArrayList<Integer>();
    public User userWinner;
    public int numeroMaxUccisioni=0;
    public List<Integer> listaUtentiPronti=new ArrayList<Integer>(); 
    public int numeroUtentiPronti=0;
    public final int TUTTI_QUELLI_IN_STANZA=-1;
   
   
 

    Mondo(SfereNetSfsExtension extension) {
       this.extension = extension;   
    }

  
    public User GetWin(Room room){
        
        for(int i=0;i<room.getUserList().size();i++){
            User utente=room.getUserList().get(i);
            if(dizionarioUtentiPlayer.get(utente).numeroUccisioni > numeroMaxUccisioni){
                numeroMaxUccisioni=dizionarioUtentiPlayer.get(utente).numeroUccisioni;                
                userWinner=utente;                
            }
                
        }
        
        return userWinner;
    }
    
   
    
        public void RiempiSpawn(int numeroMassimo){
            for(int i=0;i<numeroMassimo;i++){
                listaSpawnPointLiberi.add(i);
            }
        }
        
        public void RiordinaListaSpawnPoint(){
           
             Collections.sort(listaSpawnPointLiberi);
        }
        public List<User> GetUtentiInScena(Room room,String scena){
            List<User> listaUtentiInScena=new ArrayList();
            for(int i=0;i<room.getUserList().size();i++){
            User utenteDaVerificare=room.getUserList().get(i);            
            if(dizionarioUtentiPlayer.get(utenteDaVerificare).scena.equals(scena)){                
                if(!listaUtentiInScena.contains(utenteDaVerificare))
                   listaUtentiInScena.add(utenteDaVerificare);                
            }
            else if(listaUtentiInScena.contains(utenteDaVerificare))
                listaUtentiInScena.remove(utenteDaVerificare);
        }
        
            return listaUtentiInScena;
        }
        
        public boolean TuttiGliUtentiSonoPronti(Room room){
            boolean sonoPronti=false;
            numeroUtentiPronti++;
            if(numeroUtentiPronti==room.getMaxUsers())
                sonoPronti=true;
            return sonoPronti;
        }
        
      
}
