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
//import javafx.scene.transform.Transform;

/**
 *
 * @author Ninfea
 */
public class DannoHandler extends BaseClientRequestHandler{
 
        
    
     @Override
    public void handleClientRequest(User user, ISFSObject isfso) {    
      
        int utenteColpito = isfso.getInt("uco");  
        User utenteColpitoUser=user.getLastJoinedRoom().getUserById(utenteColpito); 
        
        SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();
        Mondo mondo=ext.world;    
        
        
        double dannoInflitto=mondo.dizionarioUtentiPlayer.get(user).attacco-mondo.dizionarioUtentiPlayer.get(utenteColpitoUser).difesa;      
        if(dannoInflitto<0)//se l'attacco è minore della difesa avrei un danno negativo e non posso avere un danno negativo
            dannoInflitto=0;
          
        Player player=mondo.dizionarioUtentiPlayer.get(utenteColpitoUser);   
        double vitaAttuale=player.RiceviDanno(player.vita, dannoInflitto,player.vitaMax);//calcolo il nuovo valore della vita in base al danno inflitto
        
        //salvo il nuovo valore della vita calcolato dopo aver fatto danno:
        mondo.dizionarioUtentiPlayer.get(utenteColpitoUser).vita=vitaAttuale;
        
        //verifico se la partita è finita(la partita finisce quando uno degli utenti fa per primo 5 kill:
        if(vitaAttuale<=0f){
            mondo.dizionarioUtentiPlayer.get(utenteColpitoUser).vivo=false;
            mondo.dizionarioUtentiPlayer.get(user).numeroUccisioni+=1;            
            if(mondo.dizionarioUtentiPlayer.get(user).numeroUccisioni>=5)
                ext.FinePartita(user);
        }else
             mondo.dizionarioUtentiPlayer.get(utenteColpitoUser).vivo=true;
        
         ISFSObject isfsoOut = new SFSObject();
         isfsoOut.putInt("uci", utenteColpito);
         isfsoOut.putDouble("vita", vitaAttuale);
         isfsoOut.putInt("userI", user.getId());   
       
         send("danno", isfsoOut,mondo.GetUtentiInScena(user.getLastJoinedRoom(), mondo.dizionarioUtentiPlayer.get(user).scena));
      
         ext.ComunicaPunteggiAiGiocatori(user, mondo.TUTTI_QUELLI_IN_STANZA);      
        
        
    }
    
  
}
