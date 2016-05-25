/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package sferenetsfsextension;

import com.smartfoxserver.v2.core.SFSEventType;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.SFSRoomRemoveMode;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.RoomVariable;
import com.smartfoxserver.v2.entities.variables.SFSRoomVariable;
import com.smartfoxserver.v2.extensions.SFSExtension;
import java.util.Dictionary;
import java.util.Hashtable;

/**
 *
 * @author Ninfea
 */
public class SfereNetSfsExtension extends SFSExtension{
  
    
   public final Mondo world=new Mondo(this);

	
    @Override
    public void init() {
      
        trace("chiamata a estensione");
        world.RiempiSpawn(getParentRoom().getMaxUsers());
       
        this.getParentRoom().getVariable("gameStarted").setGlobal(true);
        this.getParentRoom().getVariable("gameStarted").setPersistent(true);
       
        
        this.getParentRoom().setAutoRemoveMode(SFSRoomRemoveMode.WHEN_EMPTY);
        
        addRequestHandler("spawnMe", SpawnMeHandler.class); 
        addRequestHandler("respawn", RespawnHandler.class);
        addRequestHandler("regT", TransformPlayer.class);      
        addRequestHandler("danno", DannoHandler.class);
        addRequestHandler("raggio", RaggioHandler.class);
        addRequestHandler("res", ResurrezioneHandler.class);
        addRequestHandler("del", DeleteAvatarHandler.class);     
        addRequestHandler("ready", ReadyHandler.class);
        addRequestHandler("getTime", GetTimeHandler.class);
        addRequestHandler("SanT", GetAnimTHandler.class);
        addRequestHandler("SanC", GetAnimCHandler.class);
        
      
        
        addEventHandler(SFSEventType.USER_DISCONNECT, OnUserGoneHandler.class);
        addEventHandler(SFSEventType.USER_LEAVE_ROOM, OnUserGoneHandler.class);
        addEventHandler(SFSEventType.USER_LOGOUT, OnUserGoneHandler.class);
        addEventHandler(SFSEventType.USER_JOIN_ROOM, OnJoinRoomHandler.class);
      
      
    }
    
    
     public void FinePartita(User user){
               
        Mondo mondo=this.world; 
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
 
     public void ComunicaPunteggiAiGiocatori(User utenteP, int userDaAvvisareId){
         Mondo mondo=this.world; 
         ISFSObject objOut2=new SFSObject();           
             
         objOut2.putInt("nUcc", mondo.dizionarioUtentiPlayer.get(utenteP).numeroUccisioni);        
         objOut2.putInt("nSpawn", mondo.dizionarioUtentiPlayer.get(utenteP).numeroSpawn);
         objOut2.putUtfString("nome", mondo.dizionarioUtentiPlayer.get(utenteP).nome);
         if(userDaAvvisareId==mondo.TUTTI_QUELLI_IN_STANZA)//se faccio danno comunico il nuovo punteggio a tutti
          send("datiUcc", objOut2,utenteP.getLastJoinedRoom().getUserList()); 
         else//se invece entro in scena viene comunicato solo a me stesso il punteggio di ogni utente in stanza
            send("datiUcc", objOut2,utenteP.getLastJoinedRoom().getUserById(userDaAvvisareId));          
   }
   
  
}
