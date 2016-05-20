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
        float rx=isfso.getFloat("rx");
        float ry=isfso.getFloat("ry");
        float rz=isfso.getFloat("rz");

        int userDaAvvisare=isfso.getInt("usIn");
        String scena=isfso.getUtfString("scena");
        SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();         
        Mondo mondo=ext.world; 
        Player player=new Player();
        player.t=new Transform(x, y, z,rx,ry,rz);
        
        mondo.dizionarioUtentiPlayer.get(user).t=player.t;
        mondo.dizionarioUtentiPlayer.get(user).scena=scena;
        
        List<User> listaUtentiInScena=mondo.GetUtentiInScena(user.getLastJoinedRoom(), mondo.dizionarioUtentiPlayer.get(user).scena);
        
        trace("l'utente"+user.getName()+"vita:"+mondo.dizionarioUtentiPlayer.get(user).vita);
        ISFSObject objOut=new SFSObject();
        objOut.putUtfString("model", mondo.dizionarioUtentiPlayer.get(user).modello);
        objOut.putUtfString("nome", mondo.dizionarioUtentiPlayer.get(user).nome);
         objOut.putUtfString("classe", mondo.dizionarioUtentiPlayer.get(user).classe);
        objOut.putFloat("vita",  mondo.dizionarioUtentiPlayer.get(user).vita);
        objOut.putFloat("vitaM",  mondo.dizionarioUtentiPlayer.get(user).vitaMax);
        objOut.putFloat("mana",  mondo.dizionarioUtentiPlayer.get(user).mana);
        objOut.putFloat("manaM",  mondo.dizionarioUtentiPlayer.get(user).manaMax);
        objOut.putFloat("xp",  mondo.dizionarioUtentiPlayer.get(user).xp);
        objOut.putFloat("xpM",  mondo.dizionarioUtentiPlayer.get(user).xpMax);
        objOut.putFloat("att",  mondo.dizionarioUtentiPlayer.get(user).attacco);
        objOut.putFloat("dif",  mondo.dizionarioUtentiPlayer.get(user).difesa);
        objOut.putInt("liv",  mondo.dizionarioUtentiPlayer.get(user).livello);  
        objOut.putBool("gioc", mondo.dizionarioUtentiPlayer.get(user).giocabile);
        objOut.putFloat("x",  mondo.dizionarioUtentiPlayer.get(user).t.x);
        objOut.putFloat("y",  mondo.dizionarioUtentiPlayer.get(user).t.y);
        objOut.putFloat("z",  mondo.dizionarioUtentiPlayer.get(user).t.z);
         objOut.putFloat("rx",  mondo.dizionarioUtentiPlayer.get(user).t.rx);
        objOut.putFloat("ry",  mondo.dizionarioUtentiPlayer.get(user).t.ry);
        objOut.putFloat("rz",  mondo.dizionarioUtentiPlayer.get(user).t.rz);
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
