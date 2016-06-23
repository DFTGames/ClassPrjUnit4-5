/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package accessoExtension;

import com.smartfoxserver.v2.db.IDBManager;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import java.sql.Connection;
import java.sql.SQLException;

/**
 *
 * @author Ninfea
 */
public class RichiestaEliminazionePersonaggioHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
        String nomePersonaggioDaEliminare = isfso.getUtfString("nome");
        IDBManager dbManager = null;
        Connection connection = null;
        boolean personaggioEliminato = false;
        try {
            dbManager = getParentExtension().getParentZone().getDBManager();
            connection = dbManager.getConnection();
            connection.setAutoCommit(false);
            String querySql = null;
            querySql = "UPDATE PersonaggiUtente SET eliminato=? WHERE nomePersonaggio=?";
            dbManager.executeUpdate(querySql, new Object[]{1, nomePersonaggioDaEliminare});
            querySql = "UPDATE DiplomaziaPersonaggio SET eliminato=? WHERE PersonaggiUtente_nomePersonaggio=?";
            dbManager.executeUpdate(querySql, new Object[]{1, nomePersonaggioDaEliminare});

            connection.commit();
            personaggioEliminato = true;
            trace("personaggio eliminato correttamente da db remoto");

        } catch (SQLException ex) {
            trace("errore, non sono riuscito a eliminare il personaggio dal db remoto" + ex.getMessage());
            try {
                connection.rollback();

            } catch (SQLException ex2) {
                trace("errore:non sono riuscito a fare il rollback durante l'eliminazione del Personaggio");
            }
            personaggioEliminato = false;

        } finally {
            try {
                connection.close();
            } catch (SQLException ex3) {
                trace("non sono riuscito a chiudere la connessione al database remoto durante l'eliminazione del personaggio");
            }
            SFSObject objOut = new SFSObject();
            objOut.putBool("pE", personaggioEliminato);
            send("delP", objOut, user);
        }
    }

}
