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

/**
 *
 * @author Ninfea
 */
public class GetTimeHandler extends BaseClientRequestHandler{

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
                ISFSObject res = new SFSObject();
		Date date = new Date();
		res.putLong("t", date.getTime());
		this.send("time", res, user);
    }
    
}
