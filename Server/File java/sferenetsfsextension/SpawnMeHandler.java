/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package sferenetsfsextension;

import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.db.IDBManager;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSArray;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSArray;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.RoomVariable;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.Dictionary;
import java.util.Hashtable;

/**
 *
 * @author Ninfea
 */
public class SpawnMeHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {

        int userDaAvvisare = isfso.getInt("usIn");
        String scena = isfso.getUtfString("scena");

        //String nomeSpawnPoint=isfso.getUtfString("nSpawn");
        SfereNetSfsExtension ext = (SfereNetSfsExtension) this.getParentExtension();
        Mondo mondo = ext.world;
        Player player = new Player();
       
        try {
            IDBManager dbManager = null;
            Connection connection = null;
            dbManager = getParentExtension().getParentZone().getDBManager();
            connection = dbManager.getConnection();
            String querySql = "SELECT * FROM PersonaggiUtente WHERE nomePersonaggio=? ";
            ISFSArray rit = dbManager.executeQuery(querySql, new Object[]{isfso.getUtfString("nome")});
            if (rit.size() > 0) {
                player.nome = isfso.getUtfString("nome");
                player.scena = isfso.getUtfString("scena");
                player.classe = isfso.getUtfString("classe");

                ISFSObject obj = rit.getSFSObject(0);
                player.vita = obj.getDouble("vitaAttuale");
                player.vitaMax = obj.getDouble("vitaMassima");
                player.modello = obj.getUtfString("nomeModello");
                player.attacco = obj.getDouble("attaccoBase");
                player.difesa = obj.getDouble("difesaBase");
                player.mana = obj.getDouble("manaAttuale");
                player.manaMax = obj.getDouble("manaMassimo");
                player.livello = obj.getDouble("livelloAttuale");
                player.xp = obj.getDouble("xpAttuale");
                player.xpMax = obj.getDouble("xpAttuale");
                player.giocabile = true;
                player.vivo = true;
                player.numeroUccisioni = 0;

                if (mondo.dizionarioUtentiPlayer.get(user) == null) {

                    player.numeroSpawn = mondo.listaSpawnPointLiberi.get(0);
                    mondo.listaSpawnPointLiberi.remove(0);
                    mondo.dizionarioUtentiPlayer.put(user, player);
                    mondo.utenteOwner = user.getLastJoinedRoom().getOwner();

                }

                ISFSObject objOut = new SFSObject();
                objOut.putUtfString("model", mondo.dizionarioUtentiPlayer.get(user).modello);
                objOut.putUtfString("nome", mondo.dizionarioUtentiPlayer.get(user).nome);

                objOut.putInt("ut", user.getId());
                objOut.putInt("nSpawn", mondo.dizionarioUtentiPlayer.get(user).numeroSpawn);
                if (userDaAvvisare == -1)//avviso tutti quelli dentro la stanza
                {
                    send("spawnMe", objOut, user.getLastJoinedRoom().getUserList());
                } else//avviso solo quello specificato cioè quello appena entrato
                {
                    send("spawnMe", objOut, user.getLastJoinedRoom().getUserById(userDaAvvisare));
                }

            } else {
                trace("personaggio non trovato nella tabella PersonaggiUtente: nomePersonaggio" + isfso.getUtfString("nome"));
                send("noPers", new SFSObject(), user);
            }
            connection.close();
        } catch (SQLException ex) {
            trace("errore, non sono riuscito a fare la select in PersonaggiUtente" + ex.getMessage());
        }

    }

}
