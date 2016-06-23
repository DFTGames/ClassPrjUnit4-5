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

/**
 *
 * @author Ninfea
 */
public class RichiestaControlloNomeHandler extends BaseClientRequestHandler{

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
       String nomePersonaggioScelto=isfso.getUtfString("nome");
        try {
            IDBManager dbManager = null;
            Connection connection = null;
            dbManager = getParentExtension().getParentZone().getDBManager();
            connection = dbManager.getConnection();            
            String querySql = "SELECT * FROM PersonaggiUtente WHERE nomePersonaggio=? ";
            ISFSArray rit = dbManager.executeQuery(querySql, new Object[]{nomePersonaggioScelto});
            if(rit.size()>0){
                trace("il nome esiste");
              SFSObject objOut = new SFSObject();
              objOut.putBool("nE", true);          
              send("nExist", objOut, user);
            }else{
                trace("il nome NON esiste");
                SFSObject objOut = new SFSObject();
                objOut.putBool("nE", false);          
                send("nExist", objOut, user);                
            }
            connection.close();

        } catch (SQLException ex) {
            trace("errore, non sono riuscito a fare la select in PersonaggiUtente"+ex.getMessage());
        }
    }
    
}
