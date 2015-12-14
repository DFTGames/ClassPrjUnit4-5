using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class EditorPercorsi : EditorWindow
{

    public PercorsiClass percorsi;
    public GameData personaggi;

    public int contaPercorso;

    private Color OriginalBg = GUI.backgroundColor;
    private Color OriginalCont = GUI.contentColor;
    private Color OriginalColor = GUI.color;
    private const string STR_PercorsoConfig2 = "PercorsoConfigurazione";  //Path memorizzazione del Percorso
    private const string STR_DatabaseDiGioco2 = "/dataBasePercorso.asset";

    private static bool preferenzePercorsiCaricate = false;
    private static string pathPercorsi;

    public bool caricaMatricePercorsi;

    private string[] nomePercorsi;
    private int[] indexPopUp;


    private int indice = 1;


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
        EditorWindow.GetWindow<EditorPercorsi>("Editor PErcorsi");
    }


    private void OnEnable()
    {
        if (EditorPrefs.HasKey(STR_PercorsoConfig2))
        {
            pathPercorsi = EditorPrefs.GetString(STR_PercorsoConfig2);
            percorsi = AssetDatabase.LoadAssetAtPath<PercorsiClass>(pathPercorsi + STR_DatabaseDiGioco2);
        }

    }

    private void OnGUI()
    {
        if (personaggi == null)
        {
            personaggi = GameObject.FindGameObjectWithTag("GruppoPercorsi").GetComponent<RecuperoAsset>().gameData;
        }

        if (personaggi == null)
        {
            EditorGUILayout.HelpBox("DataBase Diplomazia Mancante nel GameObject GruppoPercorsi", MessageType.Error);
            EditorGUILayout.Separator();
           return ;
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

            if (GUILayout.Button("Gestisci Percorsi"))
                indice = 2;

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
            EditorGUILayout.HelpBox("DataBase Mancante", MessageType.Error);
            GUILayout.EndHorizontal();
        }
    }

    private void resetta()
    {
        percorsi.nomePercorsi.Clear();  
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
        if (GUILayout.Button("Resetta", GUILayout.Width(100f)))
        {
            resetta();
            EditorUtility.SetDirty(percorsi);
            AssetDatabase.SaveAssets();
        }
        EditorGUILayout.Separator();
        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
        GUILayout.Label("Classe Percorsi (Premere + per aggiungere percorso)", stileEtichetta);


        if (GUILayout.Button(" + ", GUILayout.Width(30)))
        {
            string tmp = "nome percorso";
            percorsi.nomePercorsi.Add(tmp);
            EditorUtility.SetDirty(percorsi);
            AssetDatabase.SaveAssets();
            caricaMatricePercorsi = false;
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < percorsi.nomePercorsi.Count; i++)
        {
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUILayout.Label("ID   " + i.ToString());
            string tmp = EditorGUILayout.TextField(percorsi.nomePercorsi[i]);
            //non ha messo gestione UNDo con la fretta...
            if (tmp != percorsi.nomePercorsi[i])
            {
                percorsi.nomePercorsi[i] = tmp;
                EditorUtility.SetDirty(percorsi);
                AssetDatabase.SaveAssets();
            }
            if (GUILayout.Button(" - ", GUILayout.Width(30)))  //mi permette di cancellare le righe
            {
                percorsi.nomePercorsi.RemoveAt(i);
                EditorUtility.SetDirty(percorsi);
                AssetDatabase.SaveAssets();
                caricaMatricePercorsi = false;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    private void GestisciPercorsi()
    {
    
        if (personaggi == null || percorsi == null) return;
        if (percorsi.nomePercorsi.Count < 1) return;

        if (!caricaMatricePercorsi) CaricaMatrice();
        if (personaggi != null && percorsi != null && nomePercorsi[0] != string.Empty && nomePercorsi[0] != null)  //Paranoia Luc_Code
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
         
            for (int i = 0; i < personaggi.tagEssere.Length; i++)
            {             
                if (personaggi.tagEssere[i] != "Player")
                {           
                    GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                    EditorGUILayout.LabelField(personaggi.tagEssere[i], stileEtichetta2, GUILayout.Width(130));
                    indexPopUp[i] = EditorGUILayout.Popup(indexPopUp[i], nomePercorsi);                 
                    GUILayout.EndHorizontal();                 
                }
            }
           GUILayout.EndVertical();

        }
    }

    void CaricaMatrice()
    {
        nomePercorsi = new string[personaggi.tagEssere.Length];
        indexPopUp = new int[nomePercorsi.Length];
        for (int i = 0; i < indexPopUp.Length; i++)
            indexPopUp[i] = 0;
        nomePercorsi = percorsi.nomePercorsi.ToArray();
        caricaMatricePercorsi = true;

    }

}
