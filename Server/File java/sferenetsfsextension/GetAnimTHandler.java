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
import java.util.Date;
import java.util.List;

/**
 * Animazione tramite tastiera
 * @author Luca
 */
public class GetAnimTHandler extends BaseClientRequestHandler{
    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
             //   ISFSObject res = new SFSObject();
                 SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();         
                Mondo mondo=ext.world; 
                List<User> listaUtentiInScena=mondo.GetUtentiInScena(user.getLastJoinedRoom(), mondo.dizionarioUtentiPlayer.get(user).scena);
                ISFSObject obj = new SFSObject();
                obj.putFloat("f",isfso.getFloat("f"));
                obj.putFloat("t",isfso.getFloat("t"));
                obj.putBool("o",isfso.getBool("o"));
                obj.putFloat("j",isfso.getFloat("j"));
                obj.putFloat("jL",isfso.getFloat("jL"));
                obj.putBool("a1",isfso.getBool("a1"));
                obj.putBool("a2",isfso.getBool("a2"));
                obj.putInt("id",user.getId());
		this.send("anT",obj, listaUtentiInScena);
                       
    }

    
}
