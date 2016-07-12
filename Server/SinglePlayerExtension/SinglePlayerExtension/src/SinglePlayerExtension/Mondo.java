/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package SinglePlayerExtension;

import com.smartfoxserver.v2.db.IDBManager;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSArray;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSArray;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.RoomVariable;
import com.smartfoxserver.v2.entities.variables.SFSRoomVariable;
import java.sql.Connection;
import java.sql.SQLException;
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
    private SinglePlayerExtension extension;
    public Dictionary<Integer,EssereAIeNon> dizionarioAI=new Hashtable();
    public EssereAIeNon datiGiocatore=new EssereAIeNon();   
    public User utenteOwner;
    public List<Integer> listaSpawnPointLiberiAI=new ArrayList<Integer>();
    public User userWinner;
    public int numeroMaxUccisioni=0;   
  
    public final int NUMERO_MAX_AI=5;
   
   
 

    Mondo(SinglePlayerExtension extension) {
       this.extension = extension;   
    }

  
    public User GetWin(Room room){
        
        for(int i=0;i<room.getUserList().size();i++){
            User utente=room.getUserList().get(i);
            if(datiGiocatore.numeroUccisioni > numeroMaxUccisioni){
                numeroMaxUccisioni=datiGiocatore.numeroUccisioni;                
                userWinner=utente;                
            }
                
        }
        
        return userWinner;
    }
    
   
    
        public void RiempiSpawnAI(int numeroMassimo){
            for(int i=0;i<numeroMassimo;i++){
                listaSpawnPointLiberiAI.add(i);
            }            
        }
        
       /* public void RiordinaListaSpawnPoint(){
           
             Collections.sort(listaSpawnPointLiberiAI);
        }*/
    
        
       
        
       
        
      
}
