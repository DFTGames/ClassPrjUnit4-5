using System;
using UnityEditor;
using UnityEngine;


namespace DFTGames.Tools.EditorTools
{
    public class GameDataEditor : EditorWindow
    {
        public GameData gameData;
        public const string STR_PercorsoConfig = "PercorsoConfigurazione";
        public const string STR_DatabaseDiGioco = "/dataBaseDiGioco.asset";

        private Color OriginalBg = GUI.backgroundColor;
        private Color OriginalCont = GUI.contentColor;
        private Color OriginalColor = GUI.color;
      
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
                percorso = EditorPrefs.GetString(STR_PercorsoConfig);
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
                    EditorPrefs.SetString(STR_PercorsoConfig, percorso);
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
            if (EditorPrefs.HasKey(STR_PercorsoConfig))
            {
                percorso = EditorPrefs.GetString(STR_PercorsoConfig);
                gameData = AssetDatabase.LoadAssetAtPath<GameData>(percorso + STR_DatabaseDiGioco);
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
                //Inizializzazione e set valori default=-1 index percorso
                if (gameData.indexPercorsi == null)
                {
                    gameData.indexPercorsi = new int[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];
                    for (int i = 0; i < gameData.tagEssere.Length; i++)
                        gameData.indexPercorsi[i] = -1;
                }
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
                            EditorPrefs.SetString(STR_PercorsoConfig, percorso);
                        }
                    }
                    if (percorso != string.Empty)
                    {
                        gameData = ScriptableObject.CreateInstance<GameData>();
                        AssetDatabase.CreateAsset(gameData, percorso + STR_DatabaseDiGioco);
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
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUIStyle stileEtichetta = new GUIStyle(GUI.skin.GetStyle("Label"));
            stileEtichetta.alignment = TextAnchor.MiddleCenter;
            stileEtichetta.fontStyle = FontStyle.Bold;
            stileEtichetta.fontSize = 14;
            GUIStyle stileEtichetta2 = new GUIStyle(GUI.skin.GetStyle("Label"));
            stileEtichetta2.alignment = TextAnchor.MiddleLeft;
            stileEtichetta2.fontStyle = FontStyle.Bold;
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
            GUILayout.Label(new GUIContent("Matrice Amicizie"), stileEtichetta, GUILayout.Width(140));

            //codice necessario in caso di aggiunta o rimozione di un tag:
            if (gameData.tagEssere.Length != UnityEditorInternal.InternalEditorUtility.tags.Length - 5)
            {
                int vecchio = gameData.tagEssere.Length;
                int differenzaLunghezze = UnityEditorInternal.InternalEditorUtility.tags.Length - 5 - gameData.tagEssere.Length;
                Array.Resize<string>(ref gameData.tagEssere, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
                Array.Resize<classiAmicizie>(ref gameData.matriceAmicizie, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);

                for (int i = 0; i < gameData.tagEssere.Length; i++)
                {
                    if (gameData.matriceAmicizie[i] == null)
                        gameData.matriceAmicizie[i] = new classiAmicizie();


                    Array.Resize<Amicizie>(ref gameData.matriceAmicizie[i].elementoAmicizia, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
                }

                if (differenzaLunghezze > 0)
                {
                    for (int i = vecchio; i < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; i++)
                    {
                        gameData.tagEssere[i] = UnityEditorInternal.InternalEditorUtility.tags[i + 5];
                        gameData.indexPercorsi[i] = -1; //provvisorio finche si usano i tag
                        for (int j = 0; j < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; j++)
                        {
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
                 EditorGUILayout.LabelField(gameData.tagEssere[gameData.tagEssere.Length - i-1], stileEtichetta2, GUILayout.Width(140));
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginVertical((EditorStyles.objectFieldThumb));
            for (int i = 0; i < gameData.tagEssere.Length; i++)
            {

                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                EditorGUILayout.LabelField(gameData.tagEssere[i], stileEtichetta2, GUILayout.Width(140));
     
                for (int j = 0; j < (gameData.tagEssere.Length - i); j++)
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
                    //Qui se clicchiamo il bottone deve assegnare un icona differente in base all'indice "di click"
                    if (GUILayout.Button(new GUIContent(tmp), GUIStyle.none, GUILayout.Width(140), GUILayout.Height(80)))
                    {
                        //valore dell'Enum usato come indice per l'icona
                        int numIcona = (int)gameData.matriceAmicizie[i].elementoAmicizia[j];
                        //Debug.Log("Letto da matrice: " + gameData.matriceAmicizie[i].elementoAmicizia[j] + " = a " + numIcona);
                        numIcona++;
                        numIcona = numIcona <= 3 ? numIcona : 1;
                        gameData.matriceAmicizie[i].elementoAmicizia[j] = (Amicizie)numIcona;
                      //  gameData.matriceAmicizie[j].elementoAmicizia[i] = (Amicizie)numIcona;
                        EditorUtility.SetDirty(gameData);
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
                    gameData.matriceAmicizie[r].elementoAmicizia[c] = Amicizie.Neutro;
                }
            }
        }
    }
}