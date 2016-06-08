/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package accessoExtension;

import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.variables.SFSUserVariable;
import com.smartfoxserver.v2.entities.variables.UserVariable;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;
import java.util.Arrays;
import java.util.List;

/**
 *
 * @author Pierluigi
 */
public class PostLoginEventHandler extends BaseServerEventHandler{

    @Override
    public void handleServerEvent(ISFSEvent event) throws SFSException {
    {
        trace("joined, nella postLogin");
        User theUser = (User) event.getParameter(SFSEventParam.USER);
         
        // dbid is a hidden UserVariable, available only server side
        UserVariable uv_dbId = new SFSUserVariable("dbid", theUser.getSession().getProperty("idDB"));
        uv_dbId.setHidden(true);
         
        // Set the variables
        List<UserVariable> vars = Arrays.asList(uv_dbId);
        getApi().setUserVariables(theUser, vars);
         
        // Join the user
        Room lobby = getParentExtension().getParentZone().getRoomByName("The Lobby");
         
        if (lobby == null)
            throw new SFSException("The Lobby Room was not found! Make sure a Room called 'The Lobby' exists in the Zone to make this example work correctly.");
         
        getApi().joinRoom(theUser, lobby);
    }
}
}
