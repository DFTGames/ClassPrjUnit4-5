/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package accessoExtension;

import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.db.IDBManager;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSArray;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSDataWrapper;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.SFSUserVariable;
import com.smartfoxserver.v2.entities.variables.UserVariable;
import com.smartfoxserver.v2.exceptions.SFSErrorCode;
import com.smartfoxserver.v2.exceptions.SFSErrorData;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.exceptions.SFSLoginException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;
import java.sql.Connection;
import java.sql.SQLException;
import java.sql.Timestamp;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Calendar;
import java.util.List;

/**
 *
 * @author Pierluigi
 */
public class PostLoginEventHandler extends BaseServerEventHandler {

    @Override
    public void handleServerEvent(ISFSEvent event) throws SFSException {
        {
            IDBManager dbManager = null;
            Connection connection = null;
            trace("joined, nella postLogin");
            User theUser = (User) event.getParameter(SFSEventParam.USER);
            int idUtente = (int) theUser.getSession().getProperty("idDB");
            UserVariable uv_dbId = new SFSUserVariable("dbid", idUtente);
            //  uv_dbId.setHidden(true);
            List<UserVariable> vars = new ArrayList();
            vars.add(uv_dbId);

            try {
                trace("recupero dati sincronizzazione");
                dbManager = getParentExtension().getParentZone().getDBManager();
                connection = dbManager.getConnection();

                String querySql = "SELECT nomeTabella, aggiornatoAl FROM SincronizzazioneDB";
                ISFSArray rit = dbManager.executeQuery(querySql, new Object[]{});

                querySql = "SELECT timeStampUtente FROM Utenti WHERE idUtente=?";
                ISFSArray rit2 = dbManager.executeQuery(querySql, new Object[]{idUtente});
                if (rit2.size() > 0) {
                    ISFSObject obj = rit2.getSFSObject(0);
                    UserVariable uv_tsU = new SFSUserVariable("tsU", obj.getUtfString("timeStampUtente"));//registro il ts attuale di Utenti come UserVarible
                    vars.add(uv_tsU);

                }
                getApi().setUserVariables(theUser, vars, true, true); 

                //mando l'auttale timeStamp delle tabelle base
                SFSObject objOut = new SFSObject();
                objOut.putSFSArray("timeList", rit);
                send("timeS", objOut, theUser);
                
                //calcolo il nuovo timestamp della tabella Utenti e lo metto nel dbRemoto
                Calendar calendar = Calendar.getInstance();
                Timestamp currentTimestamp = new Timestamp(calendar.getTime().getTime());
                querySql = "UPDATE Utenti SET timeStampUtente=? WHERE idUtente=?";
                dbManager.executeUpdate(querySql, new Object[]{currentTimestamp.toString(),idUtente});
               
                connection.close();

            } catch (SQLException e) {
                try {
                    trace("Errore SQL: " + e.getMessage());
                    connection.close();
                    // throw new SFSLoginException("Errore accesso al database: " + soprannome, data);
                } catch (SQLException ex) {
                    // TODO Auto-generated catch block
                    ex.getMessage();
                }
            }

        }
    }
}
