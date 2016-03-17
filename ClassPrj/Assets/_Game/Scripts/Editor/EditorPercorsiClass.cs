using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor.SceneManagement;


namespace DFTGames.Tools.EditorTools
{

    enum DoveSono
    {
        Modifica,
        Gestione
    }

    public class EditorPercorsiClass : EditorWindow
    {

        public PercorsiClass percorsi;
        public caratteristichePersonaggioV2 personaggi;

        public int contaPercorso;
        private DoveSono scelta = DoveSono.Gestione;

        private Color OriginalBg = GUI.backgroundColor;
        private Color OriginalCont = GUI.contentColor;
        private Color OriginalColor = GUI.color;

        private static bool preferenzePercorsiCaricate = false;
        private static string pathPercorsi;

        string[] array;   //array provvisory usati per costruire la logica del popUp
        int[] ar;         //array provvisory usati per costruire la logica del popUp
        bool caricaArray = true;

        [PreferenceItem("Percorsi")]
        private static void preferenzeDiGameGUI()
        {
            if (!preferenzePercorsiCaricate)
            {
                pathPercorsi = EditorPrefs.GetString(Statici.STR_PercorsoConfig2);
                preferenzePercorsiCaricate = true;
            }
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string tmpStr = "Assets";
                string tmpPercosro = EditorUtility.OpenFolderPanel("Percorso del Database", tmpStr, "");
                if (tmpPercosro != string.Empty)
                {
                    pathPercorsi = "Assets" + tmpPercosro.Substring(Application.dataPath.Length);
                    EditorPrefs.SetString(Statici.STR_PercorsoConfig2, pathPercorsi);
                }
            }
            GUILayout.Label(pathPercorsi);
            GUILayout.EndHorizontal();
        }


        [MenuItem("Window/ToolsGame/Configurazione Percorsi %&P")]
        private static void Init()
        {
            EditorWindow.GetWindow<EditorPercorsiClass>("Editor PercorsiClass");
        }

        private void OnEnable()
        {
            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig2))
            {
                pathPercorsi = EditorPrefs.GetString(Statici.STR_PercorsoConfig2);
                percorsi = AssetDatabase.LoadAssetAtPath<PercorsiClass>(pathPercorsi + Statici.STR_DatabaseDiGioco2);
            }

            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig))
            {
                string percorso = EditorPrefs.GetString(Statici.STR_PercorsoConfig);
                personaggi = AssetDatabase.LoadAssetAtPath<caratteristichePersonaggioV2>(percorso + Statici.STR_DatabaseDiGioco3);
            }
            percorsi.cambiatoAlmenoUnaScena = false;

        }

        void OnDisable()            //controlla la lista percorsi con i percorsi e se non c'e assegnazione mette indexpercorso del oggetto a default(NON_ESISTE) 
        {
            var scenaCorrente = EditorSceneManager.GetActiveScene();
            if (scenaCorrente.isDirty)  //chiedo se la scena corrente e' a dirty
            {
                bool scelta = EditorUtility.DisplayDialog("Save", "Salvi La Scena? ", " Ok ", "No");
                if (scelta)
                    EditorSceneManager.SaveScene(scenaCorrente);
            }

            if (!percorsi.cambiatoAlmenoUnaScena) return;

            var sceneName2 = Path.GetFileNameWithoutExtension(scenaCorrente.path);

            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)  //MI FA IL CAMBIAMENTO IN TUTTE LE SCENE DEL GIOCO
            {
                var scene = EditorBuildSettings.scenes[i];

                if (scene.enabled)
                {
                    percorsi.scenaDaSalvare = false;
                    var sceneName = Path.GetFileNameWithoutExtension(scene.path);

                    string tmpScene = "Assets/_Game/Scene/" + sceneName + ".unity";
                    UnityEngine.SceneManagement.Scene tmpscenee = EditorSceneManager.OpenScene(tmpScene, OpenSceneMode.Single);
                    //       percorsi.ControlloIndexPercorsi(); 14 MARZO 2016...DA RIMETTERE
                    if (percorsi.scenaDaSalvare)
                    {
                        EditorSceneManager.MarkSceneDirty(tmpscenee);  //imposto Scena a dirty...
                        EditorSceneManager.SaveScene(tmpscenee);
                    }
                    EditorSceneManager.CloseScene(tmpscenee, true);
                }
            }

            string tmpScenaCorrente = "Assets/_Game/Scene/" + sceneName2 + ".unity";  //mi ricarica la scena iniziale
            EditorSceneManager.OpenScene(tmpScenaCorrente, OpenSceneMode.Single);

        }


        private void OnGUI()
        {
            if (personaggi == null)
            {

                EditorGUILayout.HelpBox("DataBasePersonaggi Mancante ", MessageType.Error);
                EditorGUILayout.Separator();

                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                if (GUILayout.Button("Crea il DataBase"))
                    personaggi = caratteristichePersonaggioEditorV2.CreaDatabase();
                EditorGUILayout.HelpBox("DataBase Mancante", MessageType.Error);
                GUILayout.EndHorizontal();


                return;
            }

            if (percorsi != null)
            {

                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                GUILayout.Label("Editor by DFT Students", GUI.skin.GetStyle("Label"));
                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                if (GUILayout.Button("Inserisci/Modifca Percorsi"))
                    scelta = DoveSono.Modifica;

                if (percorsi.percorsi.Count > 0)
                    if (GUILayout.Button("Gestisci Percorsi"))
                        scelta = DoveSono.Gestione;

                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                switch (scelta)
                {
                    case DoveSono.Modifica:
                        InserisciModificaPercorsi();
                        break;
                    case DoveSono.Gestione:
                        GestisciPercorsi();
                        break;
                }
            }
            else  //  if (percorsi != null)
            {
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                if (GUILayout.Button("Crea il DataBase Percorsi"))
                {
                    string tmpStr = "Assets";
                    if (pathPercorsi == null || pathPercorsi == string.Empty)
                    {
                        string tmpPercosro = EditorUtility.OpenFolderPanel("Percorso per Database", tmpStr, "");
                        if (tmpPercosro != string.Empty)
                        {
                            pathPercorsi = "Assets" + tmpPercosro.Substring(Application.dataPath.Length);
                            EditorPrefs.SetString(Statici.STR_PercorsoConfig2, pathPercorsi);
                        }
                    }
                    if (pathPercorsi != string.Empty)
                    {
                        percorsi = ScriptableObject.CreateInstance<PercorsiClass>();
                        AssetDatabase.CreateAsset(percorsi, pathPercorsi + Statici.STR_DatabaseDiGioco2);
                        AssetDatabase.Refresh();
                        ProjectWindowUtil.ShowCreatedAsset(percorsi);
                    }
                }
                EditorGUILayout.HelpBox("DataBasePercorsi Mancante", MessageType.Error);
                GUILayout.EndHorizontal();
            }
        }



        private void InserisciModificaPercorsi()
        {
            GUIStyle stileEtichetta = new GUIStyle(GUI.skin.GetStyle("Label"));
            stileEtichetta.alignment = TextAnchor.MiddleCenter;
            stileEtichetta.fontStyle = FontStyle.Bold;
            stileEtichetta.fontSize = 14;
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUILayout.Label("Inserisci/Modifica Nome Percorsi", stileEtichetta);
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(EditorStyles.objectFieldThumb);

            if (percorsi.percorsi.Count > 0)

                if (GUILayout.Button("Resetta", GUILayout.Width(100f)))
                {
                    percorsi.resetta();
                    EditorUtility.SetDirty(percorsi);
                    AssetDatabase.SaveAssets();
                }
            EditorGUILayout.Separator();
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            stileEtichetta.alignment = TextAnchor.MiddleLeft;
            stileEtichetta.fontStyle = FontStyle.Bold;
            stileEtichetta.fontSize = 12;
            GUILayout.Label("Classe Percorsi (Premere + per aggiungere percorso)", stileEtichetta);


            if (GUILayout.Button(" + ", GUILayout.Width(30)))
            {

                PercorsiClass.GruppoPercorsi tmpp = new PercorsiClass.GruppoPercorsi();
                tmpp.nomePercorsi = "nome percorso";
                tmpp.indice = percorsi.trovaIndexLibero();
                percorsi.percorsi.Add(tmpp);
                EditorUtility.SetDirty(percorsi);
                AssetDatabase.SaveAssets();

                caricaArray = true;

            }
            GUILayout.EndHorizontal();

            if (percorsi.percorsi.Count > 0)
            {
                for (int i = 0; i < percorsi.percorsi.Count; i++)

                {
                    GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                    string tmp = EditorGUILayout.TextField(percorsi.percorsi[i].nomePercorsi.ToString());

                    if (tmp != percorsi.percorsi[i].nomePercorsi)
                    {
                        percorsi.percorsi[i].nomePercorsi = tmp;
                        EditorUtility.SetDirty(percorsi);
                        AssetDatabase.SaveAssets();
                    }
                    if (GUILayout.Button(" - ", GUILayout.Width(30)))  //mi permette di cancellare le righe
                    {
                        //  percorsi.ResettaIndice(percorsi.n)
                        percorsi.ResettaIndicePercorso(percorsi.percorsi[i].indice);
                        //    percorsi.indexPercorsi.RemoveAt(i);
                        percorsi.percorsi.RemoveAt(i);
                        EditorUtility.SetDirty(percorsi);
                        //   EditorUtility.SetDirty(gameData1);
                        AssetDatabase.SaveAssets();
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
            }
        }

        private void GestisciPercorsi()
        {

            if (personaggi == null || percorsi == null) return;
            if (percorsi.percorsi.Count < 1) return;

            //if personaggi != null && percorsi != null &&  percorsi.road != null
            if (percorsi.percorsi[0].nomePercorsi != string.Empty)  //Paranoia Luc_Code
            {
                GUIStyle stileEtichetta = new GUIStyle(GUI.skin.GetStyle("Label"));
                stileEtichetta.alignment = TextAnchor.MiddleCenter;
                stileEtichetta.fontStyle = FontStyle.Bold;
                stileEtichetta.fontSize = 14;
                GUIStyle stileEtichetta2 = new GUIStyle(GUI.skin.GetStyle("Label"));
                stileEtichetta2.alignment = TextAnchor.MiddleLeft;
                stileEtichetta2.fontStyle = FontStyle.Bold;
                stileEtichetta2.fontSize = 11;
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                GUILayout.Label("Gestione Percorsi", stileEtichetta);
                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();

                GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                EditorGUILayout.Separator();
                GUILayout.Label(new GUIContent("Assegnazione Percorsi "), stileEtichetta, GUILayout.Width(200));
                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                bool setDirtyPersonaggi = false;

                //controllo lista road...se non presente l'alimento
                if (percorsi.road == null) percorsi.road = new List<PercorsiClass.Road>();

                for (int i = 0; i < personaggi.matriceProprieta.Count; i++)
                {
                    if (!personaggi.matriceProprieta[i].giocabile)
                    {
                        bool stoppete = false;
                        for (int gi = 0; gi < percorsi.road.Count; gi++)
                            if (percorsi.road[gi].nomeClassi.Equals(personaggi.matriceProprieta[i].classe.ToString()))
                            {
                                stoppete = true;
                                break;
                            }

                        if (!stoppete)
                        {
                            PercorsiClass.Road tmR = new PercorsiClass.Road();
                            tmR.nomeClassi = personaggi.matriceProprieta[i].classe.ToString();
                            tmR.indice = -1;
                            percorsi.road.Add(tmR);
                        }
                    }


                }
                for (int i = 0; i < percorsi.road.Count; i++)
                {
                    GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                    EditorGUILayout.LabelField(percorsi.road[i].nomeClassi, stileEtichetta2, GUILayout.Width(130));


                    if (caricaArray)
                        CaricaArray();

                    int index = Array.IndexOf(ar, percorsi.road[i].indice);

                    int index2 = index;

                    index = EditorGUILayout.Popup(index, array);

                    if (index != index2)   //se e' stato fatto modifica
                    {
                        percorsi.road[i].indice = ar[index];
                        setDirtyPersonaggi = true;
                        caricaArray = !caricaArray;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                if (setDirtyPersonaggi)
                {
                    EditorUtility.SetDirty(percorsi);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private void CaricaArray()   //EditorGuiLayout.Popup vuole un array di strighe...e ho dovuto fare questo 
        {
            array = new string[percorsi.percorsi.Count];
            ar = new int[percorsi.percorsi.Count];
            for (int fr = 0; fr < percorsi.percorsi.Count; fr++)
            {
                array[fr] = percorsi.percorsi[fr].nomePercorsi;
                ar[fr] = percorsi.percorsi[fr].indice;
            }
            caricaArray = !caricaArray;
        }
    }

}