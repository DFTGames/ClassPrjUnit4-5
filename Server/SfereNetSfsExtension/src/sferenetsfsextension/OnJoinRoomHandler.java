/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package sferenetsfsextension;

import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;

/**
 *
 * @author Ninfea
 */
public class OnJoinRoomHandler extends BaseServerEventHandler{

    @Override
    public void handleServerEvent(ISFSEvent evento) throws SFSException {
            User user = (User) evento.getParameter(SFSEventParam.USER); 
        SfereNetSfsExtension ext=(SfereNetSfsExtension)this.getParentExtension();
        Mondo mondo=ext.world;
        SFSObject objOut=new SFSObject();
        objOut.putIntArray("listUP", mondo.listaUtentiPronti);
        for(int i=0;i<mondo.listaUtentiPronti.size();i++)
            trace("utente Pronto  "+i+" "+Integer.toString(mondo.listaUtentiPronti.get(i)));
        send("lPrt",objOut,user);
    }
    
}
