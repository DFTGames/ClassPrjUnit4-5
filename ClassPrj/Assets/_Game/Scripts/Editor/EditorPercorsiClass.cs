using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DFTGames.Tools.EditorTools
{

    public class EditorPercorsiClass : EditorWindow
    {

        public PercorsiClass percorsi;
        public GameData gameData1;
        public string[] nomePercorsi;

        public int contaPercorso;

        private Color OriginalBg = GUI.backgroundColor;
        private Color OriginalCont = GUI.contentColor;
        private Color OriginalColor = GUI.color;
        private const string STR_PercorsoConfig2 = "PercorsoConfigurazione";  //Path memorizzazione del Percorso
        private const string STR_DatabaseDiGioco2 = "/dataBasePercorso.asset";

        private static bool preferenzePercorsiCaricate = false;
        private static string pathPercorsi;

        public bool caricaMatricePercorsi;
      
        private int indice = 1;
        private bool controlloGameObject = false;



        [PreferenceItem("Percorsi")]
        private static void preferenzeDiGameGUI()
        {
            if (!preferenzePercorsiCaricate)
            {
                pathPercorsi = EditorPrefs.GetString(STR_PercorsoConfig2);
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
                    EditorPrefs.SetString(STR_PercorsoConfig2, pathPercorsi);
                }
            }
            GUILayout.Label(pathPercorsi);
            GUILayout.EndHorizontal();
        }



        [MenuItem("Window/ToolsGame/Configurazione Percorsi %&P")]
        private static void Init()
        {
            EditorWindow.GetWindow<EditorPercorsiClass>("Editor PErcorsiClass");
        }


        private void OnEnable()
        {
            if (EditorPrefs.HasKey(STR_PercorsoConfig2))
            {
                pathPercorsi = EditorPrefs.GetString(STR_PercorsoConfig2);
                percorsi = AssetDatabase.LoadAssetAtPath<PercorsiClass>(pathPercorsi + STR_DatabaseDiGioco2);
            }


            if (EditorPrefs.HasKey(GameDataEditor.STR_PercorsoConfig))
            {
                string percorso = EditorPrefs.GetString(GameDataEditor.STR_PercorsoConfig);
                gameData1 = AssetDatabase.LoadAssetAtPath<GameData>(percorso + GameDataEditor.STR_DatabaseDiGioco);

            }

        }

        private void OnGUI()
        {


            if (gameData1 == null)
            {

                EditorGUILayout.HelpBox("DataBaseDiGioco Mancante nel GameObject GruppoPercorsi", MessageType.Error);
                EditorGUILayout.Separator();
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
                    indice = 1;

                if (percorsi.nomePercorsi.Count > 0)
                {
                    if (GUILayout.Button("Gestisci Percorsi"))
                        indice = 2;
                }

                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                switch (indice)
                {
                    case 1:
                        InserisciModificaPercorsi();
                        break;
                    case 2:
                        GestisciPercorsi();
                        break;
                }

            }

            else
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
                            EditorPrefs.SetString(STR_PercorsoConfig2, pathPercorsi);
                        }
                    }
                    if (pathPercorsi != string.Empty)
                    {
                        percorsi = ScriptableObject.CreateInstance<PercorsiClass>();
                        AssetDatabase.CreateAsset(percorsi, pathPercorsi + STR_DatabaseDiGioco2);
                        AssetDatabase.Refresh();
                        ProjectWindowUtil.ShowCreatedAsset(percorsi);
                        caricaMatricePercorsi = false;

                    }
                    //  resettaPercorsi();
                }
                EditorGUILayout.HelpBox("DataBasePercorsi Mancante", MessageType.Error);
                GUILayout.EndHorizontal();
            }
        }

        private void resetta()
        {
            percorsi.nomePercorsi.Clear();
            ResettaIndexGameData1(-1);
        }

        private void InserisciModificaPercorsi()
        {

            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUIStyle stileEtichetta = new GUIStyle(GUI.skin.GetStyle("Label"));
            stileEtichetta.alignment = TextAnchor.MiddleCenter;
            stileEtichetta.fontStyle = FontStyle.Bold;
            stileEtichetta.fontSize = 14;
            GUILayout.Label("Inserisci/Modifica Nome Percorsi", stileEtichetta);
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            stileEtichetta.alignment = TextAnchor.MiddleLeft;
            stileEtichetta.fontStyle = FontStyle.Bold;
            stileEtichetta.fontSize = 12;
            GUILayout.EndHorizontal();
            if (percorsi.nomePercorsi.Count > 0)
                if (GUILayout.Button("Resetta", GUILayout.Width(100f)))
                {
                    resetta();
                    ResettaIndexGameData1(-1);
                    EditorUtility.SetDirty(percorsi);
                    AssetDatabase.SaveAssets();
                }
            EditorGUILayout.Separator();
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUILayout.Label("Classe Percorsi (Premere + per aggiungere percorso)", stileEtichetta);


            if (GUILayout.Button(" + ", GUILayout.Width(30)))
            {
                string tmp = "nome percorso";

                int indexx;
                if (percorsi.nomePercorsi.Count == 0) indexx = 1;
                else
                    indexx = percorsi.nomePercorsi.Keys.Max() + 1;

                percorsi.nomePercorsi.Add(indexx, tmp);
                EditorUtility.SetDirty(percorsi);
                AssetDatabase.SaveAssets();
            }
            GUILayout.EndHorizontal();

            List<int> index = new List<int>(percorsi.nomePercorsi.Keys);

            foreach (int key in index)

            {
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                GUILayout.Label("ID   " + key);
                string tmp = EditorGUILayout.TextField(percorsi.nomePercorsi[key]);
                if (tmp != percorsi.nomePercorsi[key])
                {
                    percorsi.nomePercorsi[key] = tmp;
                    EditorUtility.SetDirty(percorsi);
                    AssetDatabase.SaveAssets();
                }
                if (GUILayout.Button(" - ", GUILayout.Width(30)))  //mi permette di cancellare le righe
                {
                    percorsi.nomePercorsi.Remove(key);
                    EditorUtility.SetDirty(percorsi);
                    AssetDatabase.SaveAssets();
                    ResettaIndexGameData1(key);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            caricaMatricePercorsi = false;
        }

        private void GestisciPercorsi()
        {

            if (gameData1 == null || percorsi == null) return;
            if (percorsi.nomePercorsi.Count < 1) return;

            if (!caricaMatricePercorsi) CaricaMatrice();
            if (gameData1 != null && percorsi != null && nomePercorsi[0] != string.Empty && nomePercorsi[0] != null)  //Paranoia Luc_Code
            {
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                GUIStyle stileEtichetta = new GUIStyle(GUI.skin.GetStyle("Label"));
                stileEtichetta.alignment = TextAnchor.MiddleCenter;
                stileEtichetta.fontStyle = FontStyle.Bold;
                stileEtichetta.fontSize = 14;
                GUIStyle stileEtichetta2 = new GUIStyle(GUI.skin.GetStyle("Label"));
                stileEtichetta2.alignment = TextAnchor.MiddleLeft;
                stileEtichetta2.fontStyle = FontStyle.Bold;
                stileEtichetta2.fontSize = 11;
                GUILayout.Label("Gestione Percorsi", stileEtichetta);
                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);

                GUILayout.EndHorizontal();
                GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                EditorGUILayout.Separator();
                GUILayout.Label(new GUIContent("Assegnazione Percorsi "), stileEtichetta, GUILayout.Width(200));
                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                bool setDirtyPersonaggi = false;

                for (int i = 0; i < gameData1.tagEssere.Length; i++)
                {
                    if (gameData1.tagEssere[i] != "Player")
                    {
                        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                        EditorGUILayout.LabelField(gameData1.tagEssere[i], stileEtichetta2, GUILayout.Width(130));
                        int index = gameData1.indexPercorsi[i];
                        gameData1.indexPercorsi[i] = EditorGUILayout.Popup(index, nomePercorsi);

                        if (index != gameData1.indexPercorsi[i]) setDirtyPersonaggi = true;
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
                if (setDirtyPersonaggi)
                {
                    EditorUtility.SetDirty(gameData1);
                    AssetDatabase.SaveAssets();
                }

            }
        }

        void CaricaMatrice()   //mi carica i percorsi nella matrice ..ho dovuto usarla perche  EditorGUILayout.Popup mi accetta una schiera di stringhe
        {

            if (percorsi.nomePercorsi.Count < 1) return;
            nomePercorsi = new string[percorsi.nomePercorsi.Count];
            //  nomePercorsi = percorsi.nomePercorsi.ToArray(); 
            nomePercorsi = percorsi.nomePercorsi.Values.ToArray();
            caricaMatricePercorsi = true;
        }

        void ResettaIndexGameData1(int key)  //mi resetta(default = -1) i valori del indexPercorso GameData
        {                                    // se key=-1 mi resetta tutti i valori..altrimenti solo il valore corrispondente
            if (gameData1.indexPercorsi.Length < 1) return;
            for (int i = 0; i < gameData1.indexPercorsi.Length; i++)
            {
                if ((key == -1) || (gameData1.indexPercorsi[i] == key))
                    gameData1.indexPercorsi[i] = -1;
            }


        }


    }

}