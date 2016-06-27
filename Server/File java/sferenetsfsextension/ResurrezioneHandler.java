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
public class ResurrezioneHandler extends BaseClientRequestHandler {

    
    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
    float vitaAggiuntiva=isfso.getFloat("vitaM");
     
    SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();
    Mondo mondo=ext.world; 
    
    Player player=mondo.dizionarioUtentiPlayer.get(user); 
    double vitaAttuale= player.RiceviVita(player.vita, vitaAggiuntiva,player.vitaMax);
    mondo.dizionarioUtentiPlayer.get(user).vita=vitaAttuale;//salvo la vita attuale dopo averla aggiunta.
    ISFSObject objOut=new SFSObject();
    objOut.putDouble("vita", vitaAttuale);
    objOut.putInt("u", user.getId());
    send("res",objOut,mondo.GetUtentiInScena(user.getLastJoinedRoom(), mondo.dizionarioUtentiPlayer.get(user).scena));
    }
    
}
