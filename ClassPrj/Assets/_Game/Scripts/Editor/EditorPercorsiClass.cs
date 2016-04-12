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
        public PercorsiClass.Percorso percorso;

        public int contaPercorso;
        private DoveSono scelta = DoveSono.Gestione;

        private Color OriginalBg = GUI.backgroundColor;
        private Color OriginalCont = GUI.contentColor;
        private Color OriginalColor = GUI.color;

        private static bool preferenzePercorsiCaricate = false;
        private static string pathPercorsi;



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

            if (percorsi != null) percorsi.cambiatoAlmenoUnaScena = false;   //QUA C'E UN PROBLEMA..ME LO FA MA NON ALLA PRIMA CREAZIONE...PARTE DALLA SECONDA..CHIEDERE A PINO
        }

        void OnDisable()            //controlla la lista percorsi con i percorsi e se non c'e assegnazione mette indexpercorso del oggetto a default(NON_ESISTE) 
        {
            var scenaCorrente = EditorSceneManager.GetActiveScene();
            if (scenaCorrente.isDirty)  //chiedo se la scena corrente e' a dirty
            {
                bool scelta = EditorUtility.DisplayDialog("Save", "Salvi La Scena? ", " Ok ", "Cancel");
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
                    percorsi.ControlloIndexPercorsi();
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
            if (Enum.GetNames(typeof(classiPersonaggi)).Length == 0)

            {

                EditorGUILayout.HelpBox("DataBaseDiGioco Mancante nel GameObject GruppoPercorsi", MessageType.Error);
                EditorGUILayout.Separator();
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);

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

                if (percorsi.per.Count > 0)
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
                    //  resettaPercorsi();
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
            if (percorsi.per.Count > 0)

                if (GUILayout.Button("Resetta", GUILayout.Width(100f)))
                {
                    //    percorsi.resetta(gameData1); da  togliere....
                    percorsi.per.RemoveAll();
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
                string tmp = "nome percorso";

                percorso = new PercorsiClass.Percorso();
                percorso.nomePercorsi = tmp;
                percorsi.per.Add(percorso);


                EditorUtility.SetDirty(percorsi);
                AssetDatabase.SaveAssets();

            }
            GUILayout.EndHorizontal();

            if (percorsi.per.Count > 0)
            {
                for (int i = 0; i < percorsi.per.Count; i++)

                {
                    GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                    string tmp = EditorGUILayout.TextField(percorsi.per[i].nomePercorsi);
                    if (tmp != percorsi.per[i].nomePercorsi)
                    {
                        if (percorsi.per.ControlloNomiPercorso(tmp)) //problema che gia alla prima lettera mi fa il controllo..MODIFICARLO..(come non so..)
                        {
                            percorsi.per[i].nomePercorsi = tmp;
                            EditorUtility.SetDirty(percorsi);
                            AssetDatabase.SaveAssets();
                        }
                    }
                    if (GUILayout.Button(" - ", GUILayout.Width(30)))  //mi permette di cancellare le righe
                    {
                        percorsi.per.RemoveAt(i);
                        EditorUtility.SetDirty(percorsi);
                        AssetDatabase.SaveAssets();
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
            }
        }

        private void GestisciPercorsi()
        {

            if (percorsi == null) return;

            if (percorsi.per.Count == 0) return;
       
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
            GUILayout.Label(" (Premere + per aggiungere personaggi al percorso)", stileEtichetta2);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            GUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            bool setDirtyPersonaggi = false;

            for (int i = 0; i < percorsi.per.Count; i++)
            {
                int countPer = percorsi.per[i].classi.Count;
                if (countPer == 0) countPer++;
                  for (int c = 0; c < countPer; c++)
             
                {
                    GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);                   
                 if (c==0)  EditorGUILayout.LabelField(percorsi.per[i].nomePercorsi, stileEtichetta2, GUILayout.Width(130));
                    else EditorGUILayout.LabelField(string.Empty, stileEtichetta2, GUILayout.Width(130));
                   
                    int index = -1;
                    if (percorsi.per[i].classi.Count > 0)                 
                        index = Array.IndexOf((int[])Enum.GetValues(typeof(classiPersonaggi)), (int)percorsi.per[i].classi[c]);  //trova l'indice nella matrice che corrisponde al valore del campo indexPercorsi nella matrice diplomazia
                                                                                                                                                                                                                                   //    percorsi.elencaPercorsi
                    int index2 = index;
        
                    index = EditorGUILayout.Popup(index, (string[])Enum.GetNames(typeof(classiPersonaggi)));
                    if (GUILayout.Button(" + ", GUILayout.Width(30)))
                    {
                        percorsi.per[i].classi.Add(classiPersonaggi.goblin);
                        EditorUtility.SetDirty(percorsi);
                        AssetDatabase.SaveAssets();
                    }
                    if (index != index2)   //se e' stato fatto modifica
                    {
                        if (percorsi.per[i].classi.Count == 0)
                           percorsi.per[i].classi.Add((classiPersonaggi)index);
                        percorsi.per[i].classi[c] = (classiPersonaggi)index;
                        setDirtyPersonaggi = true;
                    }
                    GUILayout.EndHorizontal();
                }
            }  //  (ciclo for)
           GUILayout.EndVertical();
            
            if (setDirtyPersonaggi)
            {
                AssetDatabase.SaveAssets();
            }

        }
    }
}