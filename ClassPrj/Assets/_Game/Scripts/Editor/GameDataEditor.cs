using System;
using UnityEditor;
using UnityEngine;
namespace DFTGames.Tools.EditorTools
{
    public class GameDataEditor : EditorWindow
    {
        public GameData gameData;

        private int indice = 1;
        private int indiceButton = 0;
        private Color OriginalBg = GUI.backgroundColor;
        private Color OriginalCont = GUI.contentColor;
        private Color OriginalColor = GUI.color;
        private const string STR_PercorsoConfig2 = "PercorsoConfigurazione";
        private const string STR_DatabaseDiGioco2 = "/dataBaseDiGioco.asset";
        private static bool preferenzeCaricate = false;
        private static string percorso;
        private Vector2 posizioneScroll;
        private Texture icon;
        Texture icon1 = EditorGUIUtility.LoadRequired(string.Format("{0}/icon1.png", ResourceHelper.DFTGamesFolderPath)) as Texture;
        Texture icon2 = EditorGUIUtility.LoadRequired(string.Format("{0}/icon2.png", ResourceHelper.DFTGamesFolderPath)) as Texture;
        Texture icon3 = EditorGUIUtility.LoadRequired(string.Format("{0}/icon3.png", ResourceHelper.DFTGamesFolderPath)) as Texture;

        [PreferenceItem("Alleanze")]
        private static void preferenzeDiGameGUI()
        {
            if (!preferenzeCaricate)
            {
                percorso = EditorPrefs.GetString(STR_PercorsoConfig2);
                preferenzeCaricate = true;
            }
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            if (GUILayout.Button("...", GUILayout.Width(30)))
            {
                string tmpStr = "Assets";
                string tmpPercosro = EditorUtility.OpenFolderPanel("Percorso del Database", tmpStr, "");
                if (tmpPercosro != string.Empty)
                {
                    percorso = "Assets" + tmpPercosro.Substring(Application.dataPath.Length);
                    EditorPrefs.SetString(STR_PercorsoConfig2, percorso);
                }
            }
            GUILayout.Label(percorso);
            GUILayout.EndHorizontal();
        }
        [MenuItem("Window/ToolsGame/Configurazione Diplomazia %&D")]
        private static void Init()
        {
            EditorWindow.GetWindow<GameDataEditor>("Editor Alleanze");
        }
        private void OnEnable()
        {
            if (EditorPrefs.HasKey(STR_PercorsoConfig2))
            {
                percorso = EditorPrefs.GetString(STR_PercorsoConfig2);
                gameData = AssetDatabase.LoadAssetAtPath<GameData>(percorso + STR_DatabaseDiGioco2);
            }
        }

        private void OnGUI()
        {
            if (gameData != null)
            {
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                GUILayout.Label("Editor by DFT Students", GUI.skin.GetStyle("Label"));
                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                GestisciDiplomazia();
            }
            else
            {
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                if (GUILayout.Button("Crea il DataBase"))
                {
                    string tmpStr = "Assets";
                    if (percorso == null || percorso == string.Empty)
                    {
                        string tmpPercosro = EditorUtility.OpenFolderPanel("Percorso per Database", tmpStr, "");
                        if (tmpPercosro != string.Empty)
                        {
                            percorso = "Assets" + tmpPercosro.Substring(Application.dataPath.Length);
                            EditorPrefs.SetString(STR_PercorsoConfig2, percorso);
                        }
                    }
                    if (percorso != string.Empty)
                    {
                        gameData = ScriptableObject.CreateInstance<GameData>();
                        AssetDatabase.CreateAsset(gameData, percorso + STR_DatabaseDiGioco2);
                        AssetDatabase.Refresh();
                        ProjectWindowUtil.ShowCreatedAsset(gameData);
                    }
                    resettaParametri();
                }
                EditorGUILayout.HelpBox("DataBase Mancante", MessageType.Error);
                GUILayout.EndHorizontal();
            }
        }
        private void GestisciDiplomazia()
        {
            posizioneScroll = GUILayout.BeginScrollView(posizioneScroll, false, false);
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUIStyle stileEtichetta = new GUIStyle(GUI.skin.GetStyle("Label"));
            stileEtichetta.alignment = TextAnchor.MiddleCenter;
            stileEtichetta.fontStyle = FontStyle.Bold;
            stileEtichetta.normal.textColor = Color.black;
            stileEtichetta.fontSize = 14;
            GUIStyle stileEtichetta2 = new GUIStyle(GUI.skin.GetStyle("Label"));
            stileEtichetta2.alignment = TextAnchor.MiddleLeft;
            stileEtichetta2.fontStyle = FontStyle.Bold;
            stileEtichetta2.normal.textColor = Color.black;
            stileEtichetta2.fontSize = 11;
            GUILayout.Label("Gestione Diplomazia", stileEtichetta);
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            if (GUILayout.Button("Resetta", GUILayout.Width(100f)))
            {
                resettaParametri();
                EditorUtility.SetDirty(gameData);
                AssetDatabase.SaveAssets();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUILayout.Label(new GUIContent("Matrice Amicizie"), stileEtichetta, GUILayout.Width(130));
            //codice necessario in caso di aggiunta o rimozione di un tag:
            if (gameData.tagEssere.Length != UnityEditorInternal.InternalEditorUtility.tags.Length - 5)
            {
                int vecchio = gameData.tagEssere.Length;
                int differenzaLunghezze = UnityEditorInternal.InternalEditorUtility.tags.Length - 5 - gameData.tagEssere.Length;
                Array.Resize<string>(ref gameData.tagEssere, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
                Array.Resize<classeBottoni>(ref gameData.matriceBottoni, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
                for (int i = 0; i < gameData.matriceBottoni.Length; i++)
                {
                    if (gameData.matriceBottoni[i] == null)
                        gameData.matriceBottoni[i] = new classeBottoni();

                    Array.Resize<bool>(ref gameData.matriceBottoni[i].elementoBottoni, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
                }
                Array.Resize<classeTexture>(ref gameData.matriceIcone, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
                for (int i = 0; i < gameData.matriceIcone.Length; i++)
                {
                    if (gameData.matriceIcone[i] == null)
                        gameData.matriceIcone[i] = new classeTexture();

                    Array.Resize<Texture>(ref gameData.matriceIcone[i].iconeBottoni, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
                }
                if (differenzaLunghezze > 0)
                {
                    for (int i = vecchio; i < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; i++)
                    {
                        gameData.tagEssere[i] = UnityEditorInternal.InternalEditorUtility.tags[i + 5];

                        for (int j = 0; j < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; j++)
                        {
                            gameData.matriceBottoni[i].elementoBottoni[j] = new bool();
                            gameData.matriceIcone[i].iconeBottoni[j] = new Texture();
                            gameData.matriceIcone[i].iconeBottoni[j] = icon3;
                            gameData.matriceIcone[j].iconeBottoni[i] = icon3;
                            EditorUtility.SetDirty(gameData);
                            AssetDatabase.SaveAssets();
                        }
                    }
                }
            }
            //codice necessario per l'aggiornamento dei dati in caso qualcosa venga modificato
            for (int i = 0; i < gameData.tagEssere.Length; i++)
            {
                EditorGUILayout.LabelField(gameData.tagEssere[i], stileEtichetta2, GUILayout.Width(130));
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginVertical((EditorStyles.objectFieldThumb));
            for (int i = 0; i < gameData.tagEssere.Length; i++)
            {
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                EditorGUILayout.LabelField(gameData.tagEssere[i], stileEtichetta2, GUILayout.Width(130));
                for (int j = 0; j < gameData.tagEssere.Length; j++)
                {
                    gameData.matriceBottoni[i].elementoBottoni[j] = GUILayout.Button(new GUIContent(gameData.matriceIcone[i].iconeBottoni[j]), GUIStyle.none, GUILayout.Width(110), GUILayout.Height(80));
                    if (gameData.matriceBottoni[i].elementoBottoni[j])
                    {
                        int numIcona = int.Parse(gameData.matriceIcone[i].iconeBottoni[j].name.Substring(4));
                        numIcona++;
                        numIcona = numIcona <= 3 ? numIcona : 1;
                        switch (numIcona)
                        {
                            case 1:
                                icon = icon1;
                                break;
                            case 2:
                                icon = icon2;
                                break;
                            case 3:
                                icon = icon3;
                                break;
                            default:
                                icon = icon3;
                                break;
                        }
                        gameData.matriceIcone[i].iconeBottoni[j] = icon;
                        gameData.matriceIcone[j].iconeBottoni[i] = icon;
                        AssetDatabase.SaveAssets();
                        Debug.Log("cliccato bottone: " + i + "," + j + "\tIcona: " + icon.name);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }
        private void resettaParametri()
        {
            for (int r = 0; r < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; r++)
            {
                gameData.tagEssere[r] = UnityEditorInternal.InternalEditorUtility.tags[r + 5];
            }
            for (int r = 0; r < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; r++)
            {
                for (int c = 0; c < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; c++)
                {
                    gameData.matriceIcone[r].iconeBottoni[c] = icon3;
                }
            }
        }
    }
}