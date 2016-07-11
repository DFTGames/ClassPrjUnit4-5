/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package sferenetsfsextension;

import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.SFSRoomRemoveMode;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.SFSRoomVariable;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;
import java.util.ArrayList;
import java.util.List;

/**
 *
 * @author Ninfea
 */
public class OnUserGoneHandler extends BaseServerEventHandler{
 
    @Override
    public void handleServerEvent(ISFSEvent event) throws SFSException {
        User user = (User) event.getParameter(SFSEventParam.USER);       
        Room room=(Room) event.getParameter(SFSEventParam.ROOM);
        
        
       SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();
        Mondo mondo=ext.world;     
        
        if(user==mondo.utenteOwner)     {    
             
           send("ownOut", new SFSObject(),room.getUserList());
            trace("owner Uscito");
        }
        Player player=mondo.dizionarioUtentiPlayer.get(user);
        if(player!=null)  {  
            if ( player.SalvaDati(ext)){
                trace("dati player salvati");
            }else
                trace("dati player non salvati");
            int numeroSpawnLibero=player.numeroSpawn;
            mondo.listaSpawnPointLiberi.add(numeroSpawnLibero);
            mondo.RiordinaListaSpawnPoint();
             if(mondo.listaUtentiPronti.contains(user.getId())){
                 mondo.numeroUtentiPronti--;
                 player.pronto=false;
               mondo.listaUtentiPronti.remove(user);
               
            }
            mondo.dizionarioUtentiPlayer.remove(user);
           
            trace("l'utente: "+user+" è stato rimosso dal dizionario utenti/player perchè non è più in gioco.");
            
        }
    }
       
         
    }
    

