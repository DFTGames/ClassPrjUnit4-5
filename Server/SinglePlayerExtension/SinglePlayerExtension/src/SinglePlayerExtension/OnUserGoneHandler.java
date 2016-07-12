/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package SinglePlayerExtension;

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
        
        
       SinglePlayerExtension ext = (SinglePlayerExtension) this.getParentExtension();
        Mondo mondo=ext.world;     
        
        if(user==mondo.utenteOwner)     {    
             
           send("ownOut", new SFSObject(),room.getUserList());
            trace("owner Uscito");
        }
      
    }
       
         
    }
    

