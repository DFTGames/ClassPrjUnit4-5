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
        public GameData gameData1;

        public int contaPercorso;
        private DoveSono scelta = DoveSono.Gestione;

        private Color OriginalBg = GUI.backgroundColor;
        private Color OriginalCont = GUI.contentColor;
        private Color OriginalColor = GUI.color;
        //public const string STR_PercorsoConfig2 = "PercorsoConfigurazione";  //Path memorizzazione del Percorso
        //public const string STR_DatabaseDiGioco2 = "/dataBasePercorso.asset";

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

            /*
            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig))
            {
                string percorso = EditorPrefs.GetString(Statici.STR_PercorsoConfig);
                gameData1 = AssetDatabase.LoadAssetAtPath<GameData>(percorso + Statici.STR_DatabaseDiGioco);
            }
            */
            if (percorsi.per!=null) percorsi.cambiatoAlmenoUnaScena = false; 
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

                    string tmpScene = "Assets/_Game/Scene/"+ sceneName + ".unity";
                    UnityEngine.SceneManagement.Scene tmpscenee = EditorSceneManager.OpenScene(tmpScene,OpenSceneMode.Single);
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
            if (gameData1 == null)
            {

                EditorGUILayout.HelpBox("DataBaseDiGioco Mancante nel GameObject GruppoPercorsi", MessageType.Error);
                EditorGUILayout.Separator();

                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                if (GUILayout.Button("Crea il DataBase"))              
                    gameData1 = GameDataEditor.CreaDatabase();
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
                  //  percorsi.indexPercorsi.Clear(); da togliere
                  //  percorsi.nomePercorsi.Clear(); da togliere
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
                //    int indexx = percorsi.trovaIndexLibero(percorsi.indexPercorsi);

                //    percorsi.indexPercorsi.Add(indexx);
                //    percorsi.nomePercorsi.Add(tmp);
                //    percorsi.ordinaClasseListaDouble(ref percorsi.indexPercorsi, ref percorsi.nomePercorsi);

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
                  //  GUILayout.Label("ID   " + percorsi.indexPercorsi[i]);
                    string tmp = EditorGUILayout.TextField(percorsi.per[i].nomePercorsi);

                    if (tmp != percorsi.per[i].nomePercorsi)
                    {
                        percorsi.per[i].nomePercorsi = tmp;
                        EditorUtility.SetDirty(percorsi);
                        AssetDatabase.SaveAssets();
                    }
                    if (GUILayout.Button(" - ", GUILayout.Width(30)))  //mi permette di cancellare le righe
                    {    
                 //       percorsi.ResettaIndexGameData1(percorsi.indexPercorsi[i],gameData1);
                 //       percorsi.indexPercorsi.RemoveAt(i);
                        percorsi.per.RemoveAt(i);
                        EditorUtility.SetDirty(percorsi);
                        EditorUtility.SetDirty(gameData1);
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
        //    if (gameData1 == null || percorsi == null) return;
            if (percorsi.per.Count == 0) return;

          
         //   if (gameData1 != null && percorsi != null && percorsi.nomePercorsi[0] != string.Empty && percorsi.nomePercorsi[0] != null)  //Paranoia Luc_Code
          // {         
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

           /*
                for (int i = 0; i < gameData1.classiEssere.Length; i++)    
                {
                 //   if (gameData1.classiEssere[i] != "Player")
                 //   {
                        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                        EditorGUILayout.LabelField(gameData1.classiEssere[i], stileEtichetta2, GUILayout.Width(130));

                        int index = Array.IndexOf(percorsi.indexPercorsi.ToArray(), gameData1.indexPercorsi[i]);  //trova l'indice nella matrice che corrisponde al valore del campo indexPercorsi nella matrice diplomazia
                        int index2 = index;

                        index = EditorGUILayout.Popup(index, percorsi.nomePercorsi.ToArray()); //assegna index selezionato della matrice

                        if (index != index2)   //se e' stato fatto modifica
                        {
                            gameData1.indexPercorsi[i] = percorsi.indexPercorsi[index];
                            setDirtyPersonaggi = true;
                        }
                        GUILayout.EndHorizontal();
                  //  }
                }  
                GUILayout.EndVertical();
                if (setDirtyPersonaggi)
                {
                    EditorUtility.SetDirty(gameData1);
                    AssetDatabase.SaveAssets();
                }
                */
             //   *****************************************

            for (int i =0; i < percorsi.per.Count; i++)
            {
         
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                EditorGUILayout.LabelField(Enum.GetName(typeof(classiPersonaggi), 0), stileEtichetta2, GUILayout.Width(130));

                int index = Array.IndexOf((string[])Enum.GetNames(typeof(classiPersonaggi)), percorsi.per[i].classi[0]);  //trova l'indice nella matrice che corrisponde al valore del campo indexPercorsi nella matrice diplomazia
                int index2 = index;


                index = EditorGUILayout.Popup(index, (string[])Enum.GetNames(typeof(classiPersonaggi))); //assegna index selezionato della matrice

                if (index != index2)   //se e' stato fatto modifica
                {
                    // gameData1.indexPercorsi[i] = percorsi.indexPercorsi[index];
                    percorsi.per[i].classi[0] = (classiPersonaggi)Enum.GetValues(typeof(classiPersonaggi)).GetValue(index);

            setDirtyPersonaggi = true;
                }
                GUILayout.EndHorizontal();
                //  }
            }
            GUILayout.EndVertical();
            if (setDirtyPersonaggi)
            {
                EditorUtility.SetDirty(gameData1);
                AssetDatabase.SaveAssets();
            }

            //******************************************

           // }
        }      
    }
}