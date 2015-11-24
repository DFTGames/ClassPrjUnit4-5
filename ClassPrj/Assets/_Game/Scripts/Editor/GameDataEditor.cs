using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class GameDataEditor : EditorWindow
{

    public GameData gameData;
    private int indiceLista;
    
    private Color OriginalBg = GUI.backgroundColor;
    private Color OriginalCont = GUI.contentColor;
    private Color OriginalColor = GUI.color;

    private const string STR_PercorsoConfig = "PercorsoConfigurazione";
    private const string STR_DatabaseDiGioco = "/dataBaseDiGioco.asset";

    private static bool preferenzeCaricate = false;
    private static string percorso;

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
        }
        else
        {
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            if (GUILayout.Button("Crea il DataBase"))
            {
                EditorPrefs.DeleteKey(STR_PercorsoConfig);
                string tmpStr = "Assets";
                if (percorso == null || percorso == string.Empty  )
                {
                    string tmpPercosro = EditorUtility.OpenFolderPanel("Percorso per Database", tmpStr, "");
                    if (tmpPercosro != string.Empty)
                    {
                        percorso = "Assets" + tmpPercosro.Substring(Application.dataPath.Length);
                        EditorPrefs.SetString(STR_PercorsoConfig, percorso);
                    }
                    if (percorso != string.Empty)
                    {
                        gameData = ScriptableObject.CreateInstance<GameData>();
                        AssetDatabase.CreateAsset(gameData,percorso + STR_DatabaseDiGioco);
                        AssetDatabase.Refresh();
                        ProjectWindowUtil.ShowCreatedAsset(gameData);
                    }
                    EditorGUILayout.HelpBox("DataBase Mancante", MessageType.Error);
                    GUILayout.EndHorizontal();

                }
            }
        }

    }

    private void GestisciDiplomazia()
    {
        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        GUIStyle stileEtichetta = new GUIStyle(GUI.skin.GetStyle("Label"));
        stileEtichetta.alignment = TextAnchor.MiddleCenter;
        stileEtichetta.fontStyle = FontStyle.Bold;
        stileEtichetta.fontSize = 14;
        GUILayout.Label("Gestione Diplomazia", stileEtichetta);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        if(gameData.listaClasse.Count <=0)
        {
            if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(30)))
            {
                classeSogg tipoSog = new classeSogg();
                gameData.listaClasse.Add(tipoSog);
            }
        }
        else
        {
           if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(30)))
            {
                for (indiceLista = 0; indiceLista < gameData.listaClasse.Count; indiceLista++)
                {
                    gameData.listaClasse.RemoveAt(indiceLista);
                }
            }
            else if (GUILayout.Button("Save", GUILayout.Width(60), GUILayout.Height(30)))
            {
                    EditorUtility.SetDirty(gameData);
                    AssetDatabase.SaveAssets();
            }
        }
     
        GUILayout.EndHorizontal();
       
        for (indiceLista =0 ; indiceLista < gameData.listaClasse.Count; indiceLista++)
        {
            
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            for (int x = 1; x <= Enum.GetValues(typeof(GameData.TipiDiEsseriA)).Length; x++)
            {
                GUILayout.Label("" , stileEtichetta);
                GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                for (int y = 1; y <= Enum.GetValues(typeof(GameData.TipiDiEsseriA)).Length; y++)
                {
                    GameData.Amicizie tmpEsseriA = new GameData.Amicizie();
                    tmpEsseriA = (GameData.Amicizie)EditorGUILayout.EnumPopup(new GUIContent("" ), gameData.listaClasse[indiceLista].Amicizie[y, x]);
                    if (tmpEsseriA != gameData.listaClasse[indiceLista].Amicizie[y, x])
                    {
                        gameData.listaClasse[indiceLista].Amicizie[y, x] = tmpEsseriA;
                        EditorUtility.SetDirty(gameData);
                        AssetDatabase.SaveAssets();
                    }
                }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
    }
}
