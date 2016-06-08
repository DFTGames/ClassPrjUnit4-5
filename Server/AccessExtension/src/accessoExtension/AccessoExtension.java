/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package accessoExtension;

/**
 *
 * @author Pierluigi
 */
import com.smartfoxserver.v2.SmartFoxServer;
import com.smartfoxserver.v2.core.SFSEventType;
import com.smartfoxserver.v2.db.IDBManager;
import com.smartfoxserver.v2.extensions.SFSExtension;

public class AccessoExtension extends SFSExtension {

    @Override
    public void init() {
        trace("Nella estensione Accesso");

        addEventHandler(SFSEventType.USER_LOGIN, LoginEventHandler.class);
        addEventHandler(SFSEventType.USER_JOIN_ZONE, PostLoginEventHandler.class);

        addRequestHandler("regUt", RegistraUtenteHandler.class);    
    }
}
