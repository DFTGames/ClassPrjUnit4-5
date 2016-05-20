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
     
        float x=isfso.getFloat("x");
       float y=isfso.getFloat("y");
       float z=isfso.getFloat("z");
       float rx=isfso.getFloat("rx");
       float ry=isfso.getFloat("ry");
       float rz=isfso.getFloat("rz");
   
        Player player=new Player();        
        player.t=new Transform(x, y, z, rx,ry,rz);
        
         SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();
        Mondo mondo=ext.world; 
      
        mondo.dizionarioUtentiPlayer.get(user).t=player.t;
     
        ISFSObject objOut=new SFSObject();
        objOut.putFloat("x", mondo.dizionarioUtentiPlayer.get(user).t.x);
        objOut.putFloat("y", mondo.dizionarioUtentiPlayer.get(user).t.y);
        objOut.putFloat("z", mondo.dizionarioUtentiPlayer.get(user).t.z);
        objOut.putFloat("rx", mondo.dizionarioUtentiPlayer.get(user).t.rx);   
         objOut.putFloat("ry", mondo.dizionarioUtentiPlayer.get(user).t.ry);  
          objOut.putFloat("rz", mondo.dizionarioUtentiPlayer.get(user).t.rz);  
        objOut.putInt("u", user.getId());        
       //  send("regT", objOut ,user.getLastJoinedRoom().getUserList());}
        send("regT", objOut ,mondo.GetUtentiInScena(user.getLastJoinedRoom(), mondo.dizionarioUtentiPlayer.get(user).scena));
    }
    
    
}
