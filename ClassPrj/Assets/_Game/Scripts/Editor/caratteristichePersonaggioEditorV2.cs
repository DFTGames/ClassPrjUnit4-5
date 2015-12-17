using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class caratteristichePersonaggioEditorV2 : EditorWindow
{
    public caratteristichePersonaggioV2 _caratteristichePersonaggioV2;
    private Color OriginalBg = GUI.backgroundColor;
    private Color OriginalCont = GUI.contentColor;
    private Color OriginalColor = GUI.color;
    private const string STR_PercorsoConfig = "PercorsoConfigurazione";
    private const string STR_DatabaseDiGioco = "/dataBasePersonaggioV2.asset";
    private static bool preferenzeCaricate = false;
    private static string percorso;
   
    private Vector2 posizioneScroll;
    private SerializedObject serializedObject;
    List<SerializedProperty> stringsProperty_M = new List<SerializedProperty>();
    List<SerializedProperty> stringsProperty_F = new List<SerializedProperty>();
    



    private int indicePag = 1;
    int contatore = 0;

    [PreferenceItem("PersonaggiV2")]
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

    [MenuItem("Window/ToolsGame/Configurazione Proprieta V2 %&M")]
    private static void Init()
    {
        EditorWindow.GetWindow<caratteristichePersonaggioEditorV2>("Editor Proprieta");
    }

    private void OnEnable()
    {
        if (EditorPrefs.HasKey(STR_PercorsoConfig))
        {
            percorso = EditorPrefs.GetString(STR_PercorsoConfig);
            _caratteristichePersonaggioV2 = AssetDatabase.LoadAssetAtPath<caratteristichePersonaggioV2>(percorso + STR_DatabaseDiGioco);
          

        }
    }

    private void OnGUI()
    {
        if (_caratteristichePersonaggioV2 != null)
        {

            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUILayout.Label("Editor by DFT Students", GUI.skin.GetStyle("Label"));
            GUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            GestisciProprieta();
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
                    _caratteristichePersonaggioV2 = ScriptableObject.CreateInstance<caratteristichePersonaggioV2>();
                _caratteristichePersonaggioV2.classePersonaggio = new System.Collections.Generic.List<String>();
              

                AssetDatabase.CreateAsset(_caratteristichePersonaggioV2, percorso + STR_DatabaseDiGioco);
                AssetDatabase.Refresh();
                ProjectWindowUtil.ShowCreatedAsset(_caratteristichePersonaggioV2);
                resettaParametri();
            }
            EditorGUILayout.HelpBox("DataBase Mancante", MessageType.Error);
            GUILayout.EndHorizontal();
        }
    }

    private void GestisciProprieta()
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

        GUILayout.Label("Gestione Personaggi V2.0", stileEtichetta);
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);

        if (_caratteristichePersonaggioV2.matriceProprieta == null)
            _caratteristichePersonaggioV2.matriceProprieta = new List<ClasseValorPersonaggioV2>();
     
        if (GUILayout.Button("+", GUILayout.Width(100f)))
        {
            
            string tmp = "nessuna classe";
            _caratteristichePersonaggioV2.classePersonaggio.Add(tmp);
            ClasseValorPersonaggioV2 tmpC = new ClasseValorPersonaggioV2();
            ClasseValorPersonaggioV2 tmpF= new ClasseValorPersonaggioV2();
     

            _caratteristichePersonaggioV2.matriceProprieta.Add(tmpC);
          
           
            EditorUtility.SetDirty(_caratteristichePersonaggioV2);
            AssetDatabase.SaveAssets();

            contatore++;
        }
        if (GUILayout.Button("Resetta", GUILayout.Width(100f)))
        {
            resettaParametri();
            EditorUtility.SetDirty(_caratteristichePersonaggioV2);
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
        
       
        List<string> tmpclassePersonaggio = new List<string>();
        for (int k = 0; k < _caratteristichePersonaggioV2.classePersonaggio.Count; k++)
            tmpclassePersonaggio.Add(_caratteristichePersonaggioV2.classePersonaggio[k]);

        List<ClasseValorPersonaggioV2> tmpMatrice = new List<ClasseValorPersonaggioV2>();
        for (int w = 0; w < _caratteristichePersonaggioV2.matriceProprieta.Count; w++)
            tmpMatrice.Add(_caratteristichePersonaggioV2.matriceProprieta[w]);

      


        for (int i = 0; i < _caratteristichePersonaggioV2.classePersonaggio.Count; i++)
        {

            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            if (GUILayout.Button("-", GUILayout.Width(100f)))
            {
                tmpclassePersonaggio.Remove(_caratteristichePersonaggioV2.classePersonaggio[i]);
                tmpMatrice.Remove(_caratteristichePersonaggioV2.matriceProprieta[i]);
               
                stringsProperty_M.RemoveAt(i);
                stringsProperty_F.RemoveAt(i);
            }
            if(_caratteristichePersonaggioV2.classePersonaggio[i] == "nessuna classe")
                 GUILayout.Label(new GUIContent(_caratteristichePersonaggioV2.classePersonaggio[i]), stileEtichetta, GUILayout.Width(150));
            else 
                 GUILayout.Label(new GUIContent(_caratteristichePersonaggioV2.classePersonaggio[i]), stileEtichetta, GUILayout.Width(150));
           


            GUILayout.EndHorizontal();

       
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                
                for (int c = 0; c < Enum.GetValues(typeof(ProprietaPersonaggio)).Length; c++)

                {
                _caratteristichePersonaggioV2.schieraProprietaC[c] = Enum.GetName(typeof(ProprietaPersonaggio), c);
                EditorGUILayout.LabelField(_caratteristichePersonaggioV2.schieraProprietaC[c], stileEtichetta2, GUILayout.Width(130));
                }
              
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                EditorGUI.BeginChangeCheck();
                serializedObject = new SerializedObject(_caratteristichePersonaggioV2);
                stringsProperty_M.Add(serializedObject.FindProperty("character_M"));
                EditorGUILayout.PropertyField(stringsProperty_M[i], new GUIContent(""), true, GUILayout.Width(130));
                if (EditorGUI.EndChangeCheck())
                {
				  
						serializedObject.ApplyModifiedProperties();
                      
						
                        tmpMatrice[i].nomeM = stringsProperty_M[i].objectReferenceValue.name;
                    Undo.RecordObject(_caratteristichePersonaggioV2, "NomeM");
						EditorUtility.SetDirty(_caratteristichePersonaggioV2);
						AssetDatabase.SaveAssets();
                 

                }

            EditorGUI.BeginChangeCheck();
            serializedObject = new SerializedObject(_caratteristichePersonaggioV2);
            stringsProperty_F.Add(serializedObject.FindProperty("character_F"));
            EditorGUILayout.PropertyField(stringsProperty_F[i], new GUIContent(""), true, GUILayout.Width(130));
            if (EditorGUI.EndChangeCheck())
            {

                serializedObject.ApplyModifiedProperties();



                tmpMatrice[i].nomeF = stringsProperty_F[i].objectReferenceValue.name;

                Undo.RecordObject(_caratteristichePersonaggioV2, "NomeF");
                EditorUtility.SetDirty(_caratteristichePersonaggioV2);
                AssetDatabase.SaveAssets();


            }


            string tmpC = EditorGUILayout.TextField(_caratteristichePersonaggioV2.matriceProprieta[i].classe, GUILayout.Width(130));
            if (tmpC.Trim().ToLower().Replace(" ","") != _caratteristichePersonaggioV2.matriceProprieta[i].classe.Trim().ToLower().Replace(" ", ""))
            {
                if (!_caratteristichePersonaggioV2.classePersonaggio.Contains(tmpC.Trim().ToLower().Replace(" ", "")))
                {
                    _caratteristichePersonaggioV2.matriceProprieta[i].classe = tmpC.Trim().ToLower().Replace(" ", "");
                    tmpclassePersonaggio[i] = _caratteristichePersonaggioV2.matriceProprieta[i].classe;
                    Undo.RecordObject(_caratteristichePersonaggioV2, "Classe");
                }
                else
                {
                    tmpclassePersonaggio[i] = "errore";
                    _caratteristichePersonaggioV2.matriceProprieta[i].classe = "errore";
                    EditorUtility.DisplayDialog("Errore:", "Razza Esistente", "ok");
                    
                }
            }

            float tmpV = EditorGUILayout.FloatField(_caratteristichePersonaggioV2.matriceProprieta[i].Vita, GUILayout.Width(130));
                if (tmpV != _caratteristichePersonaggioV2.matriceProprieta[i].Vita)
                {
                    _caratteristichePersonaggioV2.matriceProprieta[i].Vita = tmpV;
                    Undo.RecordObject(_caratteristichePersonaggioV2, "Vita");
                }

                float tmpM = EditorGUILayout.FloatField(_caratteristichePersonaggioV2.matriceProprieta[i].Mana, GUILayout.Width(130));
                if (tmpM != _caratteristichePersonaggioV2.matriceProprieta[i].Mana)
                {
                    _caratteristichePersonaggioV2.matriceProprieta[i].Mana = tmpM;
                    Undo.RecordObject(_caratteristichePersonaggioV2, "Mana");
                }

                int tmpL = EditorGUILayout.IntField(_caratteristichePersonaggioV2.matriceProprieta[i].Livello, GUILayout.Width(130));
                if (tmpL != _caratteristichePersonaggioV2.matriceProprieta[i].Livello)
                {
                    _caratteristichePersonaggioV2.matriceProprieta[i].Livello = tmpL;
                    Undo.RecordObject(_caratteristichePersonaggioV2, "Livello");
                }

                float tmpXp = EditorGUILayout.FloatField(_caratteristichePersonaggioV2.matriceProprieta[i].Xp, GUILayout.Width(130));
                if (tmpXp != _caratteristichePersonaggioV2.matriceProprieta[i].Xp)
                {
                    _caratteristichePersonaggioV2.matriceProprieta[i].Xp = tmpXp;
                    Undo.RecordObject(_caratteristichePersonaggioV2, "Xp");
                }

                float tmpA = EditorGUILayout.FloatField(_caratteristichePersonaggioV2.matriceProprieta[i].Attacco, GUILayout.Width(130));
                if (tmpA != _caratteristichePersonaggioV2.matriceProprieta[i].Attacco)
                {
                    _caratteristichePersonaggioV2.matriceProprieta[i].Attacco = tmpA;
                    Undo.RecordObject(_caratteristichePersonaggioV2, "Attacco");
                }
                float tmpD = EditorGUILayout.FloatField(_caratteristichePersonaggioV2.matriceProprieta[i].difesa, GUILayout.Width(130));
                if (tmpD != _caratteristichePersonaggioV2.matriceProprieta[i].difesa)
                {
                    _caratteristichePersonaggioV2.matriceProprieta[i].difesa = tmpD;
                    Undo.RecordObject(_caratteristichePersonaggioV2, "Difesa");
                }
    
                GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            EditorGUILayout.LabelField(_caratteristichePersonaggioV2.matriceProprieta[i].nomeM, stileEtichetta2, GUILayout.Width(130));
            EditorGUILayout.LabelField(_caratteristichePersonaggioV2.matriceProprieta[i].nomeF, stileEtichetta2, GUILayout.Width(130));
            GUILayout.EndHorizontal();
          
        }
        
        _caratteristichePersonaggioV2.classePersonaggio = tmpclassePersonaggio;
        _caratteristichePersonaggioV2.matriceProprieta = tmpMatrice;
        
        GUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(_caratteristichePersonaggioV2);
            AssetDatabase.SaveAssets();
        }

        GUILayout.EndVertical();
        
    }

    private void resettaParametri()
    {
      
        for (int i = 0; i < _caratteristichePersonaggioV2.classePersonaggio.Count; i++)
            _caratteristichePersonaggioV2.classePersonaggio[i] = "nessuna classe";

        for (int r = 0; r < _caratteristichePersonaggioV2.classePersonaggio.Count; r++)
        {
            _caratteristichePersonaggioV2.matriceProprieta[r].classe = "nessuna classe";
            _caratteristichePersonaggioV2.matriceProprieta[r].nomeM = "nessun nome";
            _caratteristichePersonaggioV2.matriceProprieta[r].nomeF = "nessun nome";
            _caratteristichePersonaggioV2.matriceProprieta[r].Attacco = 0;
            _caratteristichePersonaggioV2.matriceProprieta[r].difesa = 0;
            _caratteristichePersonaggioV2.matriceProprieta[r].Livello = 0;
            _caratteristichePersonaggioV2.matriceProprieta[r].Mana = 0;
            _caratteristichePersonaggioV2.matriceProprieta[r].Vita = 0;
            _caratteristichePersonaggioV2.matriceProprieta[r].Xp = 0;

           

            for (int c = 0; c < Enum.GetValues(typeof(ProprietaPersonaggio)).Length; c++)
            {
                _caratteristichePersonaggioV2.schieraProprietaC[c] = Enum.GetName(typeof(ProprietaPersonaggio), c);              
            }
            AssetDatabase.Refresh();
        }
        EditorUtility.SetDirty(_caratteristichePersonaggioV2);
        AssetDatabase.SaveAssets();

    }

   
    

}
