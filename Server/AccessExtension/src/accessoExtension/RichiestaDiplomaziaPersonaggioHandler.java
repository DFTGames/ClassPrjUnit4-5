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
public class RichiestaDiplomaziaPersonaggioHandler extends BaseClientRequestHandler{

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
         try {
            IDBManager dbManager = null;
            Connection connection = null;
            dbManager = getParentExtension().getParentZone().getDBManager();
            connection = dbManager.getConnection();
            int idDB=user.getVariable("dbid").getIntValue();
            String querySql = "SELECT * FROM DiplomaziaPersonaggio WHERE PersonaggiUtente_Utenti_idUtente=? ";
            ISFSArray rit = dbManager.executeQuery(querySql, new Object[]{idDB});
            if(rit.size()>0){
               SFSObject objOut = new SFSObject();
               objOut.putSFSArray("dipL", rit);           
               send("dipP", objOut, user);
            }else{
                 trace("idUtente non corrispondente in DiplomaziaPersonaggio,idUtente"+idDB);                 
                 send("noPers",new SFSObject(),user);
            }
            connection.close();

        } catch (SQLException ex) {
            trace("errore, non sono riuscito a fare la select in DiplomaziaPersonaggio"+ex.getMessage());
        }
    }
  }
    

