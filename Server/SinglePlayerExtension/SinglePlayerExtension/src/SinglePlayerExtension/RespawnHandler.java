/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package SinglePlayerExtension;

import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import java.security.KeyPair;
import java.util.Dictionary;
import java.util.List;


/**
 *
 * @author Ninfea
 */
public class RespawnHandler extends BaseClientRequestHandler{

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
        float x=isfso.getFloat("x");
        float y=isfso.getFloat("y");
        float z=isfso.getFloat("z");
        float ry=isfso.getFloat("ry");

       
        String scena=isfso.getUtfString("scena");
        SinglePlayerExtension ext = (SinglePlayerExtension) this.getParentExtension();         
        Mondo mondo=ext.world; 
        EssereAIeNon player=new EssereAIeNon();
       
        Room stanza=user.getLastJoinedRoom();
        
        
      
        ISFSObject objOut=new SFSObject();          
           
        send("respawn", objOut,user);  
          
        
      
      //  ext.ComunicaPunteggiAiGiocatori(user,user.getId());
       
      
       
       
    }
    
}
