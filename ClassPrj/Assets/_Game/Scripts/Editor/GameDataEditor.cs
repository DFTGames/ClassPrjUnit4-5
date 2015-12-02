using System;
using UnityEditor;
using UnityEngine;


namespace DFTGames.Tools.EditorTools
{
    public class GameDataEditor : EditorWindow
    {
        public GameData gameData;

        private Color OriginalBg = GUI.backgroundColor;
        private Color OriginalCont = GUI.contentColor;
        private Color OriginalColor = GUI.color;
        private const string STR_PercorsoConfig2 = "PercorsoConfigurazione";
        private const string STR_DatabaseDiGioco2 = "/dataBaseDiGioco.asset";
        private static bool preferenzeCaricate = false;
        private static string percorso;
        private Vector2 posizioneScroll;

        Texture icon1 = EditorGUIUtility.LoadRequired(string.Format("{0}/icon1.png", ResourceHelper.DFTGamesFolderPath)) as Texture;    //Amico
        Texture icon2 = EditorGUIUtility.LoadRequired(string.Format("{0}/icon2.png", ResourceHelper.DFTGamesFolderPath)) as Texture;    //Nemico
        Texture icon3 = EditorGUIUtility.LoadRequired(string.Format("{0}/icon3.png", ResourceHelper.DFTGamesFolderPath)) as Texture;    //Neutro



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
            posizioneScroll = EditorGUILayout.BeginScrollView(posizioneScroll,false, false);

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
            EditorGUILayout.EndScrollView();
        }




        private void GestisciDiplomazia()
        {
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUIStyle stileEtichetta = new GUIStyle(GUI.skin.GetStyle("Label"));
            stileEtichetta.alignment = TextAnchor.MiddleCenter;
            stileEtichetta.fontStyle = FontStyle.Bold;
            // stileEtichetta.normal.textColor = Color.black;
            stileEtichetta.fontSize = 14;
            GUIStyle stileEtichetta2 = new GUIStyle(GUI.skin.GetStyle("Label"));
            stileEtichetta2.alignment = TextAnchor.MiddleLeft;
            stileEtichetta2.fontStyle = FontStyle.Bold;
            // stileEtichetta2.normal.textColor = Color.black;
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
                Array.Resize<classeAmicizie>(ref gameData.matriceAmicizie, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);

                for (int i = 0; i < gameData.matriceBottoni.Length; i++)
                {
                    if (gameData.matriceBottoni[i] == null)
                        gameData.matriceBottoni[i] = new classeBottoni();
                    if (gameData.matriceAmicizie[i] == null)
                        gameData.matriceAmicizie[i] = new classeAmicizie();

                    Array.Resize<bool>(ref gameData.matriceBottoni[i].elementoBottoni, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
                    Array.Resize<Amicizie>(ref gameData.matriceAmicizie[i].elementoAmicizia, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
                }

                if (differenzaLunghezze > 0)
                {
                    for (int i = vecchio; i < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; i++)
                    {
                        gameData.tagEssere[i] = UnityEditorInternal.InternalEditorUtility.tags[i + 5];

                        for (int j = 0; j < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; j++)
                        {
                            gameData.matriceBottoni[i] = new classeBottoni();
                            gameData.matriceBottoni[i].elementoBottoni[j] = false;
                            gameData.matriceAmicizie[i].elementoAmicizia[j] = Amicizie.Neutro;
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
                    //Qui recupera la texture da mettere al bottone che si sta mostrando
                    Texture tmp = new Texture();
                    switch (gameData.matriceAmicizie[i].elementoAmicizia[j])
                    {
                        case Amicizie.Neutro:
                            tmp = icon3;
                            break;
                        case Amicizie.Alleato:
                            tmp = icon1;
                            break;
                        case Amicizie.Nemico:
                            tmp = icon2;
                            break;
                        default:
                            tmp = icon3;    
                            break;
                    }
                    //qui mostra il bottone con la texture recuperata in base al valore dell'Enum della matriceAmicizie
                    gameData.matriceBottoni[i].elementoBottoni[j] = GUILayout.Button(new GUIContent(tmp), GUIStyle.none, GUILayout.Width(110), GUILayout.Height(80));
                    //Qui se clicchiamo il bottone deve assegnare un icona differente in base all'indice "di click"
                    if (gameData.matriceBottoni[i].elementoBottoni[j])
                    {
                        //valore dell'Enum usato come indice per l'icona
                        int numIcona = (int)gameData.matriceAmicizie[i].elementoAmicizia[j];
                        //Debug.Log("Letto da matrice: " + gameData.matriceAmicizie[i].elementoAmicizia[j] + " = a " + numIcona);
                        numIcona++;
                        numIcona = numIcona <= 3 ? numIcona : 1;
                        gameData.matriceAmicizie[i].elementoAmicizia[j] = (Amicizie)numIcona;
                        gameData.matriceAmicizie[j].elementoAmicizia[i] = (Amicizie)numIcona;
                        AssetDatabase.SaveAssets();
                        //Debug.Log("cliccato bottone: " + i + "," + j + " - ValEnum: " + (Amicizie)numIcona);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
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
                    gameData.matriceBottoni[r] = new classeBottoni();
                    gameData.matriceBottoni[r].elementoBottoni[c] = false;
                    gameData.matriceAmicizie[r].elementoAmicizia[c] = Amicizie.Neutro;
                }
            }
        }
    }
}