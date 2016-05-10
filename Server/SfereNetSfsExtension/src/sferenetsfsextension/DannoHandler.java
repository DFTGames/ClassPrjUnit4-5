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
import java.util.Dictionary;
import java.util.Hashtable;
import javafx.scene.transform.Transform;

/**
 *
 * @author Ninfea
 */
public class DannoHandler extends BaseClientRequestHandler{
 
     @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
    
         // Mondo mondo=RoomHelper.getWorld(this);
        int utenteColpito = isfso.getInt("uco");
        float mioAttacco = isfso.getFloat("mat");
        float suaDifesa = isfso.getFloat("sdif");
    
        User utenteColpitoUser=user.getLastJoinedRoom().getUserById(utenteColpito);
        
       
        float dannoInflitto=mioAttacco-suaDifesa;    
      
        SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();
        Mondo mondo=ext.world; 
        trace("Danno elementi nel dizionario"+mondo.dizionarioUtentiPlayer.size());
      
       Player player=mondo.dizionarioUtentiPlayer.get(utenteColpitoUser);

        float vitaAttuale=player.vita;
       
        vitaAttuale=player.RiceviDanno(vitaAttuale, dannoInflitto);
        
        mondo.dizionarioUtentiPlayer.get(utenteColpitoUser).vita=vitaAttuale;   
        if(vitaAttuale<=0f){
            mondo.dizionarioUtentiPlayer.get(utenteColpitoUser).vivo=false;
            mondo.dizionarioUtentiPlayer.get(user).numeroUccisioni+=1;
            if(mondo.dizionarioUtentiPlayer.get(user).numeroUccisioni>=5)
                ext.FinePartita(user);
            
        }else
             mondo.dizionarioUtentiPlayer.get(utenteColpitoUser).vivo=true;
        
        
         ISFSObject isfsoOut = new SFSObject();
         isfsoOut.putInt("uci", utenteColpito);
         isfsoOut.putFloat("vita", vitaAttuale);
         isfsoOut.putInt("userI", user.getId());   
        // isfsoOut.putBool("vivo",  mondo.dizionarioUtentiPlayer.get(utenteColpitoUser).vivo);
       
         send("danno", isfsoOut,mondo.GetUtentiInScena(user.getLastJoinedRoom(), mondo.dizionarioUtentiPlayer.get(user).scena));
      
         ext.ComunicaPunteggiAiGiocatori(user);        
        
        
    }
}
