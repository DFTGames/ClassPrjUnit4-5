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
import java.sql.Timestamp;
import java.util.Calendar;

/**
 *
 * @author Ninfea
 */
public class RichiestaAggiuntaNuovoPgHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {
        String nomeClasseScelta = isfso.getUtfString("classe");
        String nomePersonaggio = isfso.getUtfString("nome");
        byte sesso = isfso.getByte("sesso");
        IDBManager dbManager = null;
        Connection connection = null;
        boolean personaggioInserito = false;
        try {

            dbManager = getParentExtension().getParentZone().getDBManager();
            connection = dbManager.getConnection();
            connection.setAutoCommit(false);
            int idDB = user.getVariable("dbid").getIntValue();
            String querySql = null;
            if (sesso == 0) {
                querySql = "SELECT idPersonaggio, ClassiPersonaggi_idClassiPersonaggi, modelloM, nome, vitaMassima,vitaAttuale,"
                        + "manaMassimo,manaAttuale,livelloPartenza, xpPartenza, attaccoBase, difesaBase FROM VistaPersonaggiValidiGiocabili "
                        + "WHERE nome = ?";
            } else {
                querySql = "SELECT idPersonaggio,ClassiPersonaggi_idClassiPersonaggi, modelloF, nome, vitaMassima,vitaAttuale,"
                        + "manaMassimo,manaAttuale,livelloPartenza, xpPartenza, attaccoBase, difesaBase FROM VistaPersonaggiValidiGiocabili "
                        + "WHERE nome = ?";
            }

            ISFSArray rit = dbManager.executeQuery(querySql, new Object[]{nomeClasseScelta});

            if (rit.size() > 0) {
                ISFSObject obj = rit.getSFSObject(0);
                int idPersonaggio = obj.getInt("idPersonaggio");
                int idClasse = obj.getInt("ClassiPersonaggi_idClassiPersonaggi");
                String nomeModello = null;
                if (sesso == 0) {
                    nomeModello = obj.getUtfString("modelloM");
                } else {
                    nomeModello = obj.getUtfString("modelloF");
                }
                double vitaMassima = obj.getDouble("vitaMassima");
                double vitaAttuale = obj.getDouble("vitaAttuale");
                double manaMassimo = obj.getDouble("manaMassimo");
                double manaAttuale = obj.getDouble("manaAttuale");
                double livelloPartenza = obj.getDouble("livelloPartenza");
                double xpPartenza = obj.getDouble("xpPartenza");
                double attaccoBase = obj.getDouble("attaccoBase");
                double difesaBase = obj.getDouble("difesaBase");

                //inserisco il nuovo personaggio nella tabella remota PersonaggiUtente
                querySql = "INSERT INTO PersonaggiUtente(nomePersonaggio, Utenti_idUtente, Personaggi_idPersonaggio, nomeModello, eliminato,"
                        + "vitaMassima, vitaAttuale, manaMassimo, manaAttuale, livelloAttuale, xpAttuale, attaccoBase, difesaBase, nomeScena, checkPoint) "
                        + "VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

                dbManager.executeInsert(querySql, new Object[]{nomePersonaggio, idDB, idPersonaggio, nomeModello, 0,
                    vitaMassima, vitaAttuale, manaMassimo, manaAttuale, livelloPartenza, xpPartenza, attaccoBase, difesaBase, "Isola", "start"});

                trace("inseritA  una riga in PErsonaggiUtente.");

                querySql = "SELECT ClassiPersonaggi_idClasse, ClassiPersonaggi2_idClasse, relazione FROM VistaDiplomaziaValidi"
                        + " WHERE ClassiPersonaggi_idClasse=? or ClassiPersonaggi2_idClasse=?";

                ISFSArray rit2 = dbManager.executeQuery(querySql, new Object[]{idClasse, idClasse});
                for (int i = 0; i < rit2.size(); i++) {
                    ISFSObject obj2 = rit2.getSFSObject(i);
                    int classe1 = obj2.getInt("ClassiPersonaggi_idClasse");
                    int classe2 = obj2.getInt("ClassiPersonaggi2_idClasse");
                    int relazione = obj2.getInt("relazione");
                    //inserisco i dati in DiplomaziaPersonaggio relativi al nuovo personaggio
                    querySql = "INSERT INTO DiplomaziaPersonaggio(PersonaggiUtente_nomePersonaggio, PersonaggiUtente_Utenti_idUtente, Diplomazia_ClassiPersonaggi_idClasse, "
                            + "Diplomazia_ClassiPersonaggi2_idClasse, eliminato, relazione) VALUES(?,?,?,?,?,?)";
                    dbManager.executeInsert(querySql, new Object[]{nomePersonaggio, idDB, classe1, classe2, 0, relazione});

                }
                connection.commit();
                personaggioInserito = true;

                trace("nuovo personaggio inserito correttamente in remoto");

            }

        } catch (SQLException ex) {
            trace("errore, non sono riuscito a inserire il nuovo personaggio" + ex.getMessage());
            try {
                connection.rollback();

            } catch (SQLException ex2) {
                trace("errore:non sono riuscito a fare il rollback durante l'inserimento del nuovo Personaggio");
            }
            personaggioInserito = false;

        } finally {
            try {
                connection.close();
            } catch (SQLException ex3) {
                trace("non sono riuscito a chiudere la connessione al database remoto durante l'inserimento del nuovo personaggio");
            }
            SFSObject objOut = new SFSObject();
            objOut.putBool("pIns", personaggioInserito);
            send("insP", objOut, user);
        }
    }

}
