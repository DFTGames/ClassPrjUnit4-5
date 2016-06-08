/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package accessoExtension;

/*
 * @author Pierluigi
 */
import com.smartfoxserver.bitswarm.sessions.Session;
import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.db.IDBManager;
import com.smartfoxserver.v2.entities.data.ISFSArray;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.exceptions.SFSErrorCode;
import com.smartfoxserver.v2.exceptions.SFSErrorData;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.exceptions.SFSLoginException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Objects;

public class LoginEventHandler extends BaseServerEventHandler {

    @Override
    public void handleServerEvent(ISFSEvent event) throws SFSException {
        IDBManager dbManager = null;
        Connection connection = null;
        String soprannome = (String) event.getParameter(SFSEventParam.LOGIN_NAME);

        try {
            Session sessione = (Session) event.getParameter(SFSEventParam.SESSION);
            ISFSObject sfso = (ISFSObject) event.getParameter(SFSEventParam.LOGIN_IN_DATA);
            boolean isRegistrazione = false;
            String password = "";
            String email = "";

            if (sfso.containsKey("isReg")) {
                isRegistrazione = sfso.getBool("isReg");
                trace("Registrazione? " + (isRegistrazione ? "SI" : "NO"));
            }
            if (sfso.containsKey("pwd")) {
                password = sfso.getUtfString("pwd");
                trace("Password ricevuta: " + password);
            }
            if (sfso.containsKey("email")) {
                email = sfso.getUtfString("email");
                trace("Email ricevuta: " + email);
            }

            dbManager = getParentExtension().getParentZone().getDBManager();
            connection = dbManager.getConnection();

            PreparedStatement stmt = null;
            ResultSet res = null;
            if (isRegistrazione) {
                trace("Siamo in registrazione utente");
                String querySql = "SELECT username FROM Utenti WHERE username = ?";
                ISFSArray rit = dbManager.executeQuery(querySql, new Object[]{soprannome});
                if (rit.size() > 0) {
                    trace("Ha trovato un utente gia' presente");
                    SFSErrorData errData = new SFSErrorData(SFSErrorCode.LOGIN_BAD_USERNAME);
                    errData.addParameter(soprannome);
                    connection.close();
                    throw new SFSLoginException("Username gia' presente (" + soprannome + ")", errData);
                }
                trace("Utente non presente controlla email...");
                querySql = "SELECT email FROM Utenti WHERE email = ?";
                rit = dbManager.executeQuery(querySql, new Object[]{email});
                if (rit.size() > 0) {
                    trace("Ha trovato un utente gia' presente con la stessa email");
                    SFSErrorData errData = new SFSErrorData(SFSErrorCode.LOGIN_BAD_USERNAME);
                    errData.addParameter(email);
                    connection.close();
                    throw new SFSLoginException("Email utente gia' presente (" + email + ")", errData);
                }
                trace("Utente ed email non presenti procede con insert...");
                //in questo caso viene restituito l'IdUtente (ID della tabella)
                querySql = "INSERT INTO Utenti(email, password, username) values (?, ?, ?)";
                long id = (long) dbManager.executeInsert(querySql, new Object[]{email, password, soprannome});
                
                trace("Aggiunto utente con ID: " + id);
                sessione.setProperty("idDB", id);

            } else {
                stmt = connection.prepareStatement("SELECT idUtente, password FROM Utenti WHERE username=?");
                stmt.setString(1, soprannome);

                res = stmt.executeQuery();
                trace("eseguita query");
                //Se non c'e' manco un record, l'utente NON esiste
                if (!res.next()) {
                    trace("NON ha trovato nulla in tabella");
                    SFSErrorData errData = new SFSErrorData(SFSErrorCode.LOGIN_BAD_USERNAME);
                    errData.addParameter(soprannome);
                    connection.close();
                    throw new SFSLoginException("Username non presente (" + soprannome + ")", errData);
                }
                //se l'utente e' presente, ne recupera la sua pwd dal DB e la confronta con quella passata (pwd criptate a priori)
                String pwdDB = res.getString("password");
                trace("pwd su DB: " + pwdDB);
                if (!Objects.equals(pwdDB, password)) {
                    trace("le pwd non coincidono");
                    SFSErrorData data = new SFSErrorData(SFSErrorCode.LOGIN_BAD_PASSWORD);
                    data.addParameter(soprannome);
                    connection.close();
                    throw new SFSLoginException("Login fallito per l'utente: " + soprannome, data);
                } else {
                    trace("sono nell'else");
                }
                int idDB = res.getInt("idUtente");
                trace("Login: tutto liscio!");
                sessione.setProperty("idBD", idDB);
                res.close();
                stmt.close();
            }
            connection.close();
            //avvisare il client che e' andato tutto ok

        } catch (SQLException e) {
            try {
                trace("Errore SQL: " + e.getMessage());
                connection.close();
                SFSErrorData data = new SFSErrorData(SFSErrorCode.LOGIN_BAD_USERNAME);
                data.addParameter(soprannome);
                throw new SFSLoginException("Errore accesso al database: " + soprannome, data);
            } catch (SQLException ex) {
                // TODO Auto-generated catch block
                ex.getMessage();
            }
        }
    }
}
