/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package accessoExtension;

import com.smartfoxserver.v2.db.IDBManager;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSArray;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.SFSUserVariable;
import com.smartfoxserver.v2.entities.variables.UserVariable;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.Arrays;
import java.util.List;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author Ninfea
 */
public class RichiestaPersonaggiHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
        try {
            IDBManager dbManager = null;
            Connection connection = null;
            dbManager = getParentExtension().getParentZone().getDBManager();
            connection = dbManager.getConnection();
            String querySql = "SELECT * FROM Personaggi";
            ISFSArray rit = dbManager.executeQuery(querySql, new Object[]{});
            SFSObject objOut = new SFSObject();
            objOut.putSFSArray("persL", rit);
            querySql = "SELECT aggiornatoAl FROM SincronizzazioneDB WHERE nomeTabella=?";            
            rit = dbManager.executeQuery(querySql, new Object[]{"Personaggi"});
            
            if(rit.size()>0)
            {
                ISFSObject obj = rit.getSFSObject(0);                 
                objOut.putUtfString("tStamp", obj.getUtfString("aggiornatoAl"));
                  send("persg", objOut, user);
            }
          
            connection.close();
           

        } catch (SQLException ex) {
             trace("errore, non sono riuscito a fare la select in Personaggi o SincronizzazioneDB"+ex.getMessage());
        }

    }

}
