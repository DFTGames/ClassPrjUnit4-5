/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package sferenetsfsextension;

import com.smartfoxserver.v2.entities.Room;
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
        float ry=isfso.getFloat("ry");

       
        String scena=isfso.getUtfString("scena");
        SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();         
        Mondo mondo=ext.world; 
        Player player=new Player();
        player.t=new Transform(x, y, z,ry);
        
        mondo.dizionarioUtentiPlayer.get(user).t=player.t;
        mondo.dizionarioUtentiPlayer.get(user).scena=scena;
        Room stanza=user.getLastJoinedRoom();
        
        List<User> listaUtentiInScena=mondo.GetUtentiInScena(stanza, mondo.dizionarioUtentiPlayer.get(user).scena);  
        
       //raccolgo le informazioni degli utenti in scena e li invio. 
       //se le informazioni riguardano l'utente appena entrato le invio a tutti quelli in scena
       //se invece riguardano quelli già presenti in scena li mando solo all'utente appena entrato perchè gli altri già le conoscono.
        for(int i=0;i<listaUtentiInScena.size();i++){
             ISFSObject objOut=new SFSObject();
            User utenteDiCuiMiServonoInfo=listaUtentiInScena.get(i);
            objOut.putUtfString("model", mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).modello);
            objOut.putUtfString("nome", mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).nome);
             objOut.putUtfString("classe", mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).classe);
             objOut.putInt("idClasse", mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).idClasse);
            objOut.putDouble("vita",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).vita);
            objOut.putDouble("vitaM",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).vitaMax);
            objOut.putDouble("mana",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).mana);
            objOut.putDouble("manaM",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).manaMax);
            objOut.putDouble("xp",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).xp);
            objOut.putDouble("xpM",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).xpMax);
            objOut.putDouble("att",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).attacco);
            objOut.putDouble("dif",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).difesa);
            objOut.putDouble("liv",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).livello);  
            objOut.putBool("gioc", mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).giocabile);
             objOut.putFloat("x",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).t.x);
             objOut.putFloat("y",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).t.y);
             objOut.putFloat("z",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).t.z);
             objOut.putFloat("ry",  mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).t.ry);
            objOut.putUtfString("scena", mondo.dizionarioUtentiPlayer.get(utenteDiCuiMiServonoInfo).scena);
            objOut.putInt("ut", utenteDiCuiMiServonoInfo.getId());
            if(utenteDiCuiMiServonoInfo==user)//se le informazioni riguardano l'utente che è entrato appena in scena le mando a tutti quelli in scena
                send("respawn", objOut,listaUtentiInScena);
            else//se le informazioni riguardano gli altri già in scena le mando solo all'utente che è entrato in scena da poco perchè gli altri le sanno già
                send("respawn", objOut,user);  
        }    
        
       //comunico a quello che è appena entrato in scena il punteggio di tutti quelli in stanza:
       for(int i=0;i<stanza.getUserList().size();i++){
           User utenteDiCuiMiServePunteggio=stanza.getUserList().get(i);
           ext.ComunicaPunteggiAiGiocatori(utenteDiCuiMiServePunteggio,user.getId());
       }
      
       
       
    }
    
}
