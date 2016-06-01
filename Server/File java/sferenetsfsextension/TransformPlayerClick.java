/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package sferenetsfsextension;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import java.util.ArrayList;
import java.util.Date;
import java.util.Dictionary;
import java.util.Hashtable;
import java.util.List;

/**
 *
 * @author Ninfea
 */
public class TransformPlayer extends BaseClientRequestHandler{

 //  private Dictionary<User,Player> dizionarioUtentiPlayer=new Hashtable();
    
    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
     
            trace("sono nel 1");    
        float x=isfso.getFloat("x");
       float y=isfso.getFloat("y");
       float z=isfso.getFloat("z");
       float ry=isfso.getFloat("ry");
       float forw=isfso.getFloat("forw");
        byte a1=isfso.getByte("a1");
        Player player=new Player();        
        player.t=new Transform(x, y, z, ry);
        
         SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();
        Mondo mondo=ext.world; 
      
        if(mondo.dizionarioUtentiPlayer.get(user)==null) return;
             
        mondo.dizionarioUtentiPlayer.get(user).t=player.t;
     
        ISFSObject objOut=new SFSObject();
        objOut.putFloat("x", mondo.dizionarioUtentiPlayer.get(user).t.x);
        objOut.putFloat("y", mondo.dizionarioUtentiPlayer.get(user).t.y);
        objOut.putFloat("z", mondo.dizionarioUtentiPlayer.get(user).t.z); 
        objOut.putFloat("ry", mondo.dizionarioUtentiPlayer.get(user).t.ry);  
        objOut.putFloat("forw",forw); 
        objOut.putLong("time",new Date().getTime());  //viene aggiunto il time nel InviO transform
        objOut.putByte("a1",a1); 
 
  
        objOut.putInt("u", user.getId()); 
        trace("sono nel 2");
       //  send("regT", objOut ,user.getLastJoinedRoom().getUserList());}
        send("regT", objOut ,mondo.GetUtentiInScena(user.getLastJoinedRoom(), mondo.dizionarioUtentiPlayer.get(user).scena),true);
    }

  
    
    
}
