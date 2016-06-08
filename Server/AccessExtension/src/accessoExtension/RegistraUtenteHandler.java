/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package accessoExtension;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.db.IDBManager;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.exceptions.SFSErrorCode;
import com.smartfoxserver.v2.exceptions.SFSErrorData;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

/**
 *
 * @author Pierluigi
 */
public class RegistraUtenteHandler extends BaseClientRequestHandler {

    private final String CMD_REGISTRA = "regUt";
    private final String STR_SUCCESSO = "successo";
    private final String STR_MESSAGGIO_ERRORE = "messaggioErrore";
    private final String STR_ID_UTENTE = "idUtente";

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
        trace("in regUtente");
        String soprannome = (String) isfso.getUtfString("soprannome");
        String pwd = (String) isfso.getUtfString("password");
        String email = (String) isfso.getUtfString("email");
        
        int idUtenteIns = 0;
        boolean successo = false;
        String messaggioErrore = "";
        
        
        trace("soprannome ricevuto: " + soprannome);
        trace("Pwd ricevuta: " + pwd);
        trace("email ricevuta: " + email);
        //Recupera il DB Manager 
        IDBManager dbManager = getParentExtension().getParentZone().getDBManager();
        Connection connection = null;
        
        try {
            connection = dbManager.getConnection();

            //controlla se il soprannome (username) e' gia' presente
            if (SoprannomeGiaPresente(connection, soprannome)) {
                trace("Il soprannome e' gia' resente");
                SFSErrorData errData = new SFSErrorData(SFSErrorCode.GENERIC_ERROR);
                errData.addParameter(soprannome);
                connection.close();
                ISFSObject sfsoSopr = new SFSObject();
                sfsoSopr.putBool(STR_SUCCESSO, successo);
                messaggioErrore = "Il soprannome richiesto (" + soprannome + " e' gia' in uso, si prega di sceglierne un altro e riprovare.";
                sfsoSopr.putUtfString(STR_MESSAGGIO_ERRORE, messaggioErrore);
                send(CMD_REGISTRA, sfsoSopr, user);
            }

            //Controlla se la email e' gia' presente
            if (EmailGiaPresente(connection, email)) {
                trace("La email e' gia presente");
                SFSErrorData errData = new SFSErrorData(SFSErrorCode.GENERIC_ERROR);
                errData.addParameter(email);
                connection.close();
                ISFSObject sfsoEmail = new SFSObject();
                sfsoEmail.putBool(STR_SUCCESSO, successo);
                messaggioErrore = "La email inserita (" + email + " e' gia' presente in archivio, si prega di usarne un'altra o effettuare il login.";
                sfsoEmail.putUtfString(STR_MESSAGGIO_ERRORE, messaggioErrore);
                send(CMD_REGISTRA, sfsoEmail, user);
            }

            //Se fin qui nessun problema allora procede con la registrazione dell'utente
            trace("Procedo alla registrazione dei dati utente");
            PreparedStatement stmt = connection.prepareStatement("INSERT INTO Utenti \n"
                    + "(`emailUtente`,\n"
                    + "`password`,\n"
                    + "`soprannomeUtente`)\n"
                    + "VALUES\n"
                    + "(?, ?, ?);");
            stmt.setString(1, email);
            stmt.setString(2, pwd);
            stmt.setString(3, soprannome);

            stmt.executeQuery();
            stmt.close();

            //recupera l'ID assegnato per restituirlo al Client
            stmt = connection.prepareStatement("SELECT idUtenti FROM Utenti WHERE soprannomeUtente = ?");
            stmt.setString(1, soprannome);
            ResultSet res = stmt.executeQuery();
            if (res.next()) {
                trace("trovato un record dopo insert");
                idUtenteIns = res.getInt("idUtenti");
            }
            res.close();
            stmt.close();
            connection.close();

            trace("Tutto liscio!");
            successo = true;
            
        } catch (SQLException e) {
            try {
                trace("Errore registrazione utente: " + e.getMessage());
                connection.close();
                successo = false;
                messaggioErrore = "Errore registrazione utente\n" + e.getMessage();
            } catch (SQLException ex) {
                // TODO Auto-generated catch block
                ex.getMessage();
            }
        }

        //Invia messaggio al client riportando l'IdUtente inserito
        ISFSObject sfso = new SFSObject();
        sfso.putBool(STR_SUCCESSO, successo);
        sfso.putUtfString(STR_MESSAGGIO_ERRORE, messaggioErrore);
        sfso.putInt(STR_ID_UTENTE, idUtenteIns);
        send(CMD_REGISTRA, sfso, user);
    }

    /**
     * Veirifica se il soprannome e' gia' presente nel DB Utenti
     *
     * @param conn La Connessione al Db da usare
     * @param soprannome Il soprannome da verificare se gia' presente
     * @return True se il soprannome e' gia' presente, False altrimenti
     */
    private boolean SoprannomeGiaPresente(Connection conn, String soprannome) {
        boolean ret = true; //In caso di errore restituiamo true cosi da non fare procedere la registrazione 
        try {
            PreparedStatement stmt = conn.prepareStatement("SELECT soprannomeUtente FROM Utenti WHERE soprannomeUtente = ?");
            stmt.setString(1, soprannome);

            ResultSet res = stmt.executeQuery();
            stmt.close();
            trace("eseguita query controllo soprannome");
            //Se non c'e' manco un record, il soprannome NON esiste
            if (!res.next()) {
                trace("il soprannome non e' presente");
                ret = false;
            }
            res.close();
        } catch (SQLException ex) {
            trace("si e' verificato un errore nel controllo soprannomeGiaPresente" + ex.getMessage());
        }
        return ret;
    }

    /**
     * Verifica se la email e' gia' presente nel DB Utenti
     *
     * @param conn la connessione al DB da usare
     * @param email La email da verificare se gia' presente
     * @return True se la email e' gia' presente, false altrimenti
     */
    private boolean EmailGiaPresente(Connection conn, String email) {
        boolean ret = true; //In caso di errore restituiamo true cosi da non fare procedere la registrazione 
        try {
            PreparedStatement stmt = conn.prepareStatement("SELECT soprannomeUtente FROM Utenti WHERE emailUtente = ?");
            stmt.setString(1, email);

            ResultSet res = stmt.executeQuery();
            stmt.close();
            trace("eseguita query controllo email");
            //Se non c'e' manco un record, la email NON esiste
            if (!res.next()) {
                trace("la email non e' presente");
                ret = false;
            }
            res.close();
        } catch (SQLException ex) {
            trace("si e' verificato un errore nel controllo emailGiaPresente" + ex.getMessage());
        }
        return ret;
    }
}
