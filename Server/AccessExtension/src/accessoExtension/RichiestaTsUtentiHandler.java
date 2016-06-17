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
public class RichiestaTsUtentiHandler extends BaseClientRequestHandler{

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
       try {
           trace("sono dentro RichiestaTsUtentiHandler");
            IDBManager dbManager = null;
            Connection connection = null;           
            dbManager = getParentExtension().getParentZone().getDBManager();
            connection = dbManager.getConnection();
            int idDB=user.getVariable("dbid").getIntValue();
            String querySql = "SELECT timeStampUtente FROM Utenti WHERE idUtente=?";
            ISFSArray rit = dbManager.executeQuery(querySql, new Object[]{idDB});
            trace("rit.size di Utenti:"+ rit.size());
            if(rit.size()>0){
                 ISFSObject obj = rit.getSFSObject(0);
                trace("nuovo timeStamp Utenti trovato"+obj.getUtfString("timeStampUtente")+" per l'utente id: "+idDB);
              SFSObject objOut = new SFSObject();
              objOut.putUtfString("ts", obj.getUtfString("timeStampUtente"));           
              send("tsUtenti", objOut, user);
            }else
                trace("non ho trovato l'idUtente corrispondente nella tabella Utenti quendi non posso inviare il nuovo ts");
            connection.close();

        } catch (SQLException ex) {
        
            trace("errore, non sono riuscito a fare la select in Utenti"+ex.getMessage());
        }
    }
    
}
