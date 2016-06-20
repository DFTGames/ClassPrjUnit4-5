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
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author Ninfea
 */
public class RichiestaDiplomaziaHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
          try {
            IDBManager dbManager = null;
            Connection connection = null;
            dbManager = getParentExtension().getParentZone().getDBManager();
            connection = dbManager.getConnection();
            String querySql = "SELECT * FROM Diplomazia";
            ISFSArray rit = dbManager.executeQuery(querySql, new Object[]{});
            SFSObject objOut = new SFSObject();
            objOut.putSFSArray("dipL", rit);
            querySql = "SELECT aggiornatoAl FROM SincronizzazioneDB WHERE nomeTabella=?";            
            rit = dbManager.executeQuery(querySql, new Object[]{"Diplomazia"});
            
            if(rit.size()>0)
            {
                ISFSObject obj = rit.getSFSObject(0);                 
                objOut.putUtfString("tStamp", obj.getUtfString("aggiornatoAl"));
            }         
            send("dip", objOut, user);
            connection.close();
           

        } catch (SQLException ex) {
             trace("errore durante la select o di diplmazia o di sincronizzazioneDB"+ex.getMessage());
        }

    }
    
}
