using System;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using UnityEngine;
using Sfs2X.Entities;
using System.Collections.Generic;

public class ScenaInizialeNetwork : MonoBehaviour
{
    private static ScenaInizialeNetwork me;
    private const string EXTENSION_CLASS_SINGLEPLAYER = "SinglePlayerExtension.SinglePlayerExtension";
    private const string EXTENSION_ID_SINGLEPLAYER = "singlePlayer";

   

    public static void RichiestaCreazionePersonaggio(string classePersonaggio, byte sesso, string nomePersonaggio)
    {
        SFSObject objOut = new SFSObject();
        objOut.PutUtfString("classe", classePersonaggio);
        objOut.PutByte("sesso", sesso);
        objOut.PutUtfString("nome", nomePersonaggio);
        Statici.sfs.Send(new ExtensionRequest(Statici.CMD_INSERISCI_NUOVO_PERSONAGGIO, objOut));
    }

    public static void VaiAlleStanze(string nomeScena, string scritta)
    {
        Statici.sfs.RemoveAllEventListeners();
        ManagerIniziale.CaricaScena(nomeScena, scritta);
    }

    internal static void CancellaPersonaggio()
    {
        SFSObject objOut = new SFSObject();
        objOut.PutUtfString("nome", Statici.nomePersonaggio);
        Statici.sfs.Send(new ExtensionRequest(Statici.CMD_CANCELLA_PERSONAGGIO, objOut));
    }

    internal static void ControllaSeNomeEsiste(string v)
    {
        SFSObject objOut = new SFSObject();
        objOut.PutUtfString("nome", v);
        Statici.sfs.Send(new ExtensionRequest(Statici.CMD_RICHIESTA_SE_NOME_ESISTE, objOut));
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        Statici.sfs.RemoveAllEventListeners();
        ManagerIniziale.CaricaScena("ScenaZero", "Connection lost");
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        ISFSObject sfsObjIn = (SFSObject)evt.Params["params"];
        string cmd = (string)evt.Params["cmd"];
        switch (cmd)
        {
            case (Statici.CMD_RICHIESTA_SE_NOME_ESISTE):
                bool nomeEsiste = sfsObjIn.GetBool("nE");
                if (nomeEsiste)
                    ManagerIniziale.SollevaErroreScenaInizialeCreazionePg("Esiste Gia Un Personaggio con questo nome");
                else
                    ManagerIniziale.InserimentoNuovoPersonaggio();

                break;

            case (Statici.CMD_INSERISCI_NUOVO_PERSONAGGIO):
                bool personaggioInseritoInRemoto = sfsObjIn.GetBool("pIns");
                if (personaggioInseritoInRemoto)
                {
                    Debug.Log("Personaggio inserito in remoto");
                    if (Statici.AggiornaPersonaggiUtente(Statici.arrayPersNewPersUt) && Statici.AggiornaDiplomaziaPersonaggio(Statici.arrayPersNewPersDipPers))
                    {
                        Debug.Log("Personaggio e diplomazia per nuovo personaggio inseriti in locale");
                        if (!Statici.multigiocatoreOn)//SOLO SINGLEPLAYER
                        {
                            Statici.valoriPersonaggioScelto.NomeScena = "Isola";                            
                            ScenaInizialeNetwork.CreaStanzaSinglePlayer();
                        }                           
                        else//solo multiplayer
                            VaiAlleStanze("ScenaStanze", "The Lobby");
                    }
                }
                else
                    ManagerIniziale.SollevaErroreScenaInizialeCreazionePg("Errore di inserimento del nuovo personaggio sul database remoto");
                break;

            case (Statici.CMD_CANCELLA_PERSONAGGIO):
                bool personaggioEliminatoInRemoto = sfsObjIn.GetBool("pE");
                if (personaggioEliminatoInRemoto)
                {
                    Debug.Log("Personaggio eliminato in remoto");
                    if (Statici.EliminatoPersonaggioDaDbLocale(Statici.nomePersonaggio))
                    {
                        ManagerIniziale.AggiornaElencoPersonaggiEsistenti();
                        Debug.Log("Personaggio eliminato in locale");
                    }
                }
                else
                    ManagerIniziale.SollevaErroreScenaInizialeCreazionePg("Errore di inserimento del nuovo personaggio sul database remoto");
                break;
            case (Statici.CMD_REGISTRA_SINGLEPLAYER_START):                
                Statici.listaIdPersonaggiAI.AddRange(sfsObjIn.GetIntArray("idClassiAI"));
                VaiAlleStanze(Statici.valoriPersonaggioScelto.NomeScena, Statici.valoriPersonaggioScelto.NomeScena);
                break;
        }
    }

    public static void CreaStanzaSinglePlayer()
    {
        RoomSettings settings = new RoomSettings(Statici.sfs.MySelf.Name);
        settings.GroupId = "games";
        settings.IsGame = true;
        settings.MaxUsers = 1;
        settings.MaxSpectators = 0;
        SFSRoomVariable roomVariableStart = new SFSRoomVariable("gameStarted", false);
        roomVariableStart.IsPersistent = true;
        settings.Variables.Add(roomVariableStart);
        settings.Extension = new RoomExtension(EXTENSION_ID_SINGLEPLAYER, EXTENSION_CLASS_SINGLEPLAYER);
        Statici.sfs.Send(new CreateRoomRequest(settings, true, Statici.sfs.LastJoinedRoom));
    }

    // Use this for initialization
    private void Start()
    {
        me = this;

        Application.runInBackground = true;
        if (!SmartFoxConnection.NonNulla)
        {
            ManagerIniziale.CaricaScena("ScenaZero", "You're not connected to the server.");
            return;
        }

        //        Statici.sfs = SmartFoxConnection.Connection;
        Statici.sfs.ThreadSafeMode = true;
        Statici.sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        Statici.sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
        Statici.sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        Statici.sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);       
        Statici.sfs.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError);
    }
    private void OnRoomCreationError(BaseEvent evt)
    {
        
        Debug.LogError("impossibile creare la stanza perchè: " + (string)evt.Params["errorMessage"]);
    }

    private void OnRoomJoinError(BaseEvent evt)
    {
        string ragioneErrore = (string)evt.Params["errorMessage"];
        Debug.LogError( "Impossibile entrare nella stanza per questo motivo :" + ragioneErrore);
    }

    //solo per singleplayer
    private void OnRoomJoin(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
        if (!Statici.multigiocatoreOn && room.IsGame)
            RegistraDatiSulServerSinglePlayer();
          
    }

    private void RegistraDatiSulServerSinglePlayer()
    {
        SFSObject objOut = new SFSObject();
        objOut.PutUtfString("nome", Statici.valoriPersonaggioScelto.NomePersonaggio);        
        Statici.sfs.Send(new ExtensionRequest(Statici.CMD_REGISTRA_SINGLEPLAYER_START, objOut, Statici.sfs.LastJoinedRoom));
    }


    // Update is called once per frame
    private void Update()
    {
        if (Statici.sfs != null)
            Statici.sfs.ProcessEvents();
    }
}