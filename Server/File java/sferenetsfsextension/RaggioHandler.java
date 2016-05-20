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

/**
 *
 * @author Ninfea
 */
public class RaggioHandler extends BaseClientRequestHandler{

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
         SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();
        Mondo mondo=ext.world; 
       float raggio=isfso.getFloat("dr");
       ISFSObject sfsObjOut=new SFSObject();
       sfsObjOut.putFloat("r", raggio);
       sfsObjOut.putInt("ur",user.getId());
       send("raggio",sfsObjOut,mondo.GetUtentiInScena(user.getLastJoinedRoom(), mondo.dizionarioUtentiPlayer.get(user).scena));
    }
    
}
