/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package SinglePlayerExtension;

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
import java.util.ArrayList;
import java.util.Dictionary;
import java.util.Hashtable;
import java.util.List;
import java.util.Random;

/**
 *
 * @author Ninfea
 */
public class RegistraDatiInizialiHandler extends BaseClientRequestHandler {

    @Override
    public void handleClientRequest(User user, ISFSObject isfso) {

        SinglePlayerExtension ext = (SinglePlayerExtension) this.getParentExtension();
        Mondo mondo = ext.world;
        EssereAIeNon player = new EssereAIeNon();

        try {
            IDBManager dbManager = null;
            Connection connection = null;
            dbManager = getParentExtension().getParentZone().getDBManager();
            connection = dbManager.getConnection();
            //cerco il personaggio nella tabella PersonaggiUtente per ricavarne i valori
            String querySql = "SELECT * FROM PersonaggiUtente WHERE nomePersonaggio=? ";
            ISFSArray rit = dbManager.executeQuery(querySql, new Object[]{isfso.getUtfString("nome")});
            if (rit.size() > 0) {
                player.nome = isfso.getUtfString("nome");

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
                player.scena = obj.getUtfString("nomeScena");
                player.checkPoint = obj.getUtfString("checkPoint");
                player.giocabile = true;
                player.vivo = true;
                player.numeroUccisioni = 0;
                player.idClasse = obj.getInt("Personaggi_idPersonaggio");

                //cerco il nome della classe in base all'idClasse nella tabella Personaggi e se lo trovo assegno tutti i valori al dizionario
                querySql = "SELECT nome FROM Personaggi WHERE ClassiPersonaggi_idClassiPersonaggi=? ";
                ISFSArray rit2 = dbManager.executeQuery(querySql, new Object[]{player.idClasse});
                if (rit2.size() > 0) {
                    ISFSObject obj2 = rit2.getSFSObject(0);
                    player.classe = obj2.getUtfString("nome");
                    //assegno i valori del giocatore a datiGiocatore e mi servono in altre parti del codice li prendo da lì.
                    mondo.datiGiocatore = player;                    
                    mondo.utenteOwner = user.getLastJoinedRoom().getOwner();

                    //recupero tutte le classi non giocabili come il goblin
                    querySql = "SELECT * FROM VistaPersonaggiValidiNonGiocabili";
                    ISFSArray rit3 = dbManager.executeQuery(querySql, new Object[]{});
                    int numeroSpawnLiberi = mondo.NUMERO_MAX_AI;
                    List<Integer> listaAIClassi=new ArrayList();
                    if (rit3.size() > 0) {
                        for (int i = 0; i < rit3.size(); i++) {
                            ISFSObject obj3 = rit3.getSFSObject(i);
                            EssereAIeNon AI = new EssereAIeNon();
                            AI.idClasse = obj3.getInt("ClassiPersonaggi_idClassiPersonaggi");
                            AI.modello = obj3.getUtfString("modelloM");
                            AI.nome = obj3.getUtfString("nome");
                            AI.classe = obj3.getUtfString("nome");
                            AI.vitaMax = obj3.getDouble("vitaMassima");
                            AI.vita = obj3.getDouble("vitaAttuale");
                            AI.manaMax = obj3.getDouble("manaMassimo");
                            AI.mana = obj3.getDouble("manaAttuale");
                            AI.livello = obj3.getDouble("livelloPartenza");
                            AI.xpMax = obj3.getDouble("xpPartenza");
                            AI.xp = obj3.getDouble("xpPartenza");
                            AI.attacco = obj3.getDouble("attaccoBase");
                            AI.difesa = obj3.getDouble("difesaBase");

                            if (numeroSpawnLiberi > 0) {
                                Random rn = new Random();
                                int numeroAIdiQuestaClasse = rn.nextInt(numeroSpawnLiberi + 1);
                                if (numeroAIdiQuestaClasse > 0) {
                                    int start = mondo.NUMERO_MAX_AI - numeroSpawnLiberi;
                                    int end = start + numeroAIdiQuestaClasse;
                                    for (int j = start; j < end; j++) {
                                        mondo.dizionarioAI.put(j, AI);
                                       listaAIClassi.add(AI.idClasse);
                                    }
                                    
                                    numeroSpawnLiberi -= numeroAIdiQuestaClasse;
                                    if (numeroSpawnLiberi <= 0) {
                                        break;
                                    }
                                }
                            }

                        }

                    }
                    SFSObject objOut=new SFSObject();                    
                    objOut.putIntArray("idClassiAI", listaAIClassi);
                    
                    send("reg", objOut, user);
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
