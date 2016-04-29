using System;
using UnityEditor;
using UnityEngine;


namespace DFTGames.Tools.EditorTools
{
    public class GameDataEditor : EditorWindow
    {

        public const int NON_ESISTE = -1;
        public GameData gameData;
     //   public const string STR_PercorsoConfig = "PercorsoConfigurazione";
     //   public const string STR_DatabaseDiGioco = "/dataBaseDiGioco.asset";

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
                percorso = EditorPrefs.GetString(Statici.STR_PercorsoConfig);
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
                    EditorPrefs.SetString(Statici.STR_PercorsoConfig, percorso);
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
            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig))
            {
                percorso = EditorPrefs.GetString(Statici.STR_PercorsoConfig);
                gameData = AssetDatabase.LoadAssetAtPath<GameData>(percorso + Statici.STR_DatabaseDiGioco);
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
                    gameData = CreaDatabase();
                }

                EditorGUILayout.HelpBox("DataBase Mancante", MessageType.Error);
                GUILayout.EndHorizontal();
            }
        }

        public static  GameData CreaDatabase()
        {
            string tmpStr = "Assets";
            GameData gameData = null;

            if (percorso == null || percorso == string.Empty)
            {
                string tmpPercosro = EditorUtility.OpenFolderPanel("Percorso per Database", tmpStr, "");
                if (tmpPercosro != string.Empty)
                {
                    percorso = "Assets" + tmpPercosro.Substring(Application.dataPath.Length);
                    EditorPrefs.SetString(Statici.STR_PercorsoConfig, percorso);
                }
            }
            if (percorso != string.Empty)
            {
                gameData = ScriptableObject.CreateInstance<GameData>();
                AssetDatabase.CreateAsset(gameData, percorso + Statici.STR_DatabaseDiGioco);
                AssetDatabase.Refresh();
                ProjectWindowUtil.ShowCreatedAsset(gameData);
            }
            resettaParametri(gameData);
            return gameData;
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
                resettaParametri(gameData);
                EditorUtility.SetDirty(gameData);
                AssetDatabase.SaveAssets();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUILayout.Label(new GUIContent("Matrice Amicizie"), stileEtichetta, GUILayout.Width(140));

            //codice necessario in caso di aggiunta o rimozione di un tag:
            if (gameData.classiEssere.Length != Enum.GetValues(typeof(classiPersonaggi)).Length)
            {
                int vecchio = gameData.classiEssere.Length;
                int differenzaLunghezze = Enum.GetValues(typeof(classiPersonaggi)).Length - gameData.classiEssere.Length;
                Array.Resize<string>(ref gameData.classiEssere, Enum.GetValues(typeof(classiPersonaggi)).Length);
                Array.Resize<classiAmicizie>(ref gameData.matriceAmicizie, Enum.GetValues(typeof(classiPersonaggi)).Length);

                for (int i = 0; i < gameData.classiEssere.Length; i++)
                {
                    if (gameData.matriceAmicizie[i] == null)
                        gameData.matriceAmicizie[i] = new classiAmicizie();


                    Array.Resize<Amicizie>(ref gameData.matriceAmicizie[i].elementoAmicizia, Enum.GetValues(typeof(classiPersonaggi)).Length);
                }

                if (differenzaLunghezze > 0)
                {
                    Array tmpClassi = Enum.GetValues(typeof(classiPersonaggi));
                    for (int i = vecchio; i < tmpClassi.Length; i++)
                    {
                        gameData.classiEssere[i] = tmpClassi.GetValue(i).ToString();                      
                        for (int j = 0; j < tmpClassi.Length; j++)
                        {
                            gameData.matriceAmicizie[i].elementoAmicizia[j] = Amicizie.Neutro;
                            EditorUtility.SetDirty(gameData);
                            AssetDatabase.SaveAssets();
                        }
                    }
                }
            }

            //codice necessario per l'aggiornamento dei dati in caso qualcosa venga modificato
            for (int i = 0; i < gameData.classiEssere.Length; i++)
            {             
                 EditorGUILayout.LabelField(gameData.classiEssere[gameData.classiEssere.Length - i-1], stileEtichetta2, GUILayout.Width(140));
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginVertical((EditorStyles.objectFieldThumb));
            for (int i = 0; i < gameData.classiEssere.Length; i++)
            {

                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                EditorGUILayout.LabelField(gameData.classiEssere[i], stileEtichetta2, GUILayout.Width(140));
     
                for (int j = 0; j < (gameData.classiEssere.Length - i); j++)
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


        private static void resettaParametri(GameData gameData)
        {
            Array tmpClassi = Enum.GetValues(typeof(classiPersonaggi));

            for (int r = 0; r < tmpClassi.Length; r++)
            {    
                gameData.classiEssere[r] = tmpClassi.GetValue(r).ToString(); 
            }
            for (int r = 0; r < Enum.GetValues(typeof(classiPersonaggi)).Length; r++)
            {
                for (int c = 0; c < Enum.GetValues(typeof(classiPersonaggi)).Length; c++)
                {                  
                    gameData.matriceAmicizie[r].elementoAmicizia[c] = Amicizie.Neutro;
                }
            }
        }
    }
}