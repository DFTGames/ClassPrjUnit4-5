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
import java.security.KeyPair;
import java.util.Dictionary;
import java.util.List;


/**
 *
 * @author Ninfea
 */
public class RespawnHandler extends BaseClientRequestHandler{

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
        float x=isfso.getFloat("x");
        float y=isfso.getFloat("y");
        float z=isfso.getFloat("z");
        float rotazione=isfso.getFloat("rot");
        int userDaAvvisare=isfso.getInt("usIn");
        String scena=isfso.getUtfString("scena");
        SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();         
        Mondo mondo=ext.world; 
        Player player=new Player();
        player.t=new Transform(x, y, z, rotazione);
        
        mondo.dizionarioUtentiPlayer.get(user).t=player.t;
        mondo.dizionarioUtentiPlayer.get(user).scena=scena;
        
        List<User> listaUtentiInScena=mondo.GetUtentiInScena(user.getLastJoinedRoom(), mondo.dizionarioUtentiPlayer.get(user).scena);
        
        
        ISFSObject objOut=new SFSObject();
        objOut.putUtfString("model", mondo.dizionarioUtentiPlayer.get(user).modello);
        objOut.putUtfString("nome", mondo.dizionarioUtentiPlayer.get(user).nome);
        objOut.putFloat("vita",  mondo.dizionarioUtentiPlayer.get(user).vita);
        objOut.putFloat("x",  mondo.dizionarioUtentiPlayer.get(user).t.x);
        objOut.putFloat("y",  mondo.dizionarioUtentiPlayer.get(user).t.y);
        objOut.putFloat("z",  mondo.dizionarioUtentiPlayer.get(user).t.z);
        objOut.putFloat("rot",  mondo.dizionarioUtentiPlayer.get(user).t.rot);
        objOut.putUtfString("scena", mondo.dizionarioUtentiPlayer.get(user).scena);
       // objOut.putInt("nUcc", mondo.dizionarioUtentiPlayer.get(user).numeroUccisioni);
        objOut.putInt("ut", user.getId());
        //objOut.putInt("nSpawn", mondo.dizionarioUtentiPlayer.get(user).numeroSpawn);
        if(userDaAvvisare==-1)//avviso tutti quelli dentro la stanza
          send("respawn", objOut,listaUtentiInScena);
        else//avviso solo quello specificato cioè quello appena entrato
          send("respawn", objOut,user.getLastJoinedRoom().getUserById(userDaAvvisare));  
        
       ext.ComunicaPunteggiAiGiocatori(user);
    }
    
}
