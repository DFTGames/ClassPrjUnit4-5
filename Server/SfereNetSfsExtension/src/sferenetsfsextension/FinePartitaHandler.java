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
public class FinePartitaHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
         SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();         
        Mondo mondo=ext.world; 
          ISFSObject isfsoOut = new SFSObject();
           User winner=mondo.GetWin(user.getLastJoinedRoom());
           if(winner!=null){              
               isfsoOut.putInt("nWin", mondo.dizionarioUtentiPlayer.get(winner).numeroUccisioni);                   
               isfsoOut.putInt("win", winner.getId());
               isfsoOut.putUtfString("nomeW", mondo.dizionarioUtentiPlayer.get(winner).nome);
           }
           else{
             isfsoOut.putInt("win", -1);
              isfsoOut.putInt("nWin", 0);     
           }
         send("fine", isfsoOut,user.getLastJoinedRoom().getUserList());
    }
    
}
