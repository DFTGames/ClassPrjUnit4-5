/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package sferenetsfsextension;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.SFSRoomVariable;
import com.smartfoxserver.v2.exceptions.SFSVariableException;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author Ninfea
 */
 public class ReadyHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject isfso){
        
           SfereNetSfsExtension ext=(SfereNetSfsExtension)this.getParentExtension();
           Mondo mondo=ext.world;
           mondo.dizionarioUtentiPlayer.get(user).pronto=true;
           mondo.listaUtentiPronti.add(user.getId());
           SFSObject objOut=new SFSObject();
           objOut.putInt("uReady", user.getId());
           send("uR",objOut,user.getLastJoinedRoom().getUserList());
           if(mondo.TuttiGliUtentiSonoPronti(user.getLastJoinedRoom())) { 
               SFSRoomVariable startGame=new SFSRoomVariable("gameStarted", true,false,true,true);
                startGame.setGlobal(true);
                startGame.setPersistent(true);
                
                if(user.getLastJoinedRoom().getMaxUsers()==user.getLastJoinedRoom().getUserList().size())   { 
                    try {
                        trace("sono in try");
                        user.getLastJoinedRoom().setVariable(startGame);
                        
                    } catch (SFSVariableException ex) {
                        trace("sono in catch");
                        Logger.getLogger(ReadyHandler.class.getName()).log(Level.SEVERE, null, ex);
                    }
                     getApi().setRoomVariables(user.getLastJoinedRoom().getOwner(), user.getLastJoinedRoom(), user.getLastJoinedRoom().getVariables());
                     
                }
                send("ready",new SFSObject(),user.getLastJoinedRoom().getUserList());
           }
           
           
    }
    
   
    
}
