/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package sferenetsfsextension;

import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.RoomVariable;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import java.util.Dictionary;
import java.util.Hashtable;

/**
 *
 * @author Ninfea
 */
public class SpawnMeHandler extends BaseClientRequestHandler{
    
  
    
    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {       
       
        int userDaAvvisare=isfso.getInt("usIn");
        String scena=isfso.getUtfString("scena");
        
        //String nomeSpawnPoint=isfso.getUtfString("nSpawn");
       
        SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();
        Mondo mondo=ext.world; 
        Player player=new Player();
       // player.t=new Transform(x, y, z, rotazione);
        player.vita=isfso.getFloat("vita");
        player.vitaMax=isfso.getFloat("vitaM");
        player.modello=isfso.getUtfString("model");
        player.nome=isfso.getUtfString("nome");
        player.classe=isfso.getUtfString("classe");
        player.scena=isfso.getUtfString("scena");
        player.attacco=isfso.getFloat("att");
        player.difesa=isfso.getFloat("dif");
        player.mana=isfso.getFloat("mana");
        player.manaMax=isfso.getFloat("manaM");
        player.livello=isfso.getInt("liv");
        player.xp=isfso.getFloat("exp");
        player.xpMax=isfso.getFloat("expM");
        player.giocabile=isfso.getBool("gioc");
        player.vivo=true;
        player.numeroUccisioni=0;        
        
      
        
        
        if(mondo.dizionarioUtentiPlayer.get(user)==null){       
            
            player.numeroSpawn=mondo.listaSpawnPointLiberi.get(0);
            mondo.listaSpawnPointLiberi.remove(0);
            mondo.dizionarioUtentiPlayer.put(user, player); 
            mondo.utenteOwner=user.getLastJoinedRoom().getOwner();
          //  mondo.listaSpawnPointLiberi.add(player.nomeSpawnPoint);
       }
          
       
       
        
        ISFSObject objOut=new SFSObject();
        objOut.putUtfString("model", mondo.dizionarioUtentiPlayer.get(user).modello);
        objOut.putUtfString("nome", mondo.dizionarioUtentiPlayer.get(user).nome);
      /*  objOut.putFloat("x",  mondo.dizionarioUtentiPlayer.get(user).t.x);
        objOut.putFloat("y",  mondo.dizionarioUtentiPlayer.get(user).t.y);
        objOut.putFloat("z",  mondo.dizionarioUtentiPlayer.get(user).t.z);
        objOut.putFloat("rot",  mondo.dizionarioUtentiPlayer.get(user).t.rot);*/
        objOut.putInt("ut", user.getId());
        objOut.putInt("nSpawn", mondo.dizionarioUtentiPlayer.get(user).numeroSpawn);
        if(userDaAvvisare==-1)//avviso tutti quelli dentro la stanza
          send("spawnMe", objOut,user.getLastJoinedRoom().getUserList());
        else//avviso solo quello specificato cioè quello appena entrato
          send("spawnMe", objOut,user.getLastJoinedRoom().getUserById(userDaAvvisare));  
        
   
    }
    
}
