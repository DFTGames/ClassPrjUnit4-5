using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class caratteristichePersonaggioEditor : EditorWindow
{
    public caratteristichePersonaggio _caratteristichePersonaggio;
    private Color OriginalBg = GUI.backgroundColor;
    private Color OriginalCont = GUI.contentColor;
    private Color OriginalColor = GUI.color;
    private const string STR_PercorsoConfig = "PercorsoConfigurazione";
    private const string STR_DatabaseDiGioco = "/dataBasePersonaggio.asset";
    private static bool preferenzeCaricate = false;
    private static string percorso;
    private Vector2 posizioneScroll;
    private SerializedObject serializedObject;
    List<SerializedProperty> stringsProperty_M = new List<SerializedProperty>();
    List<SerializedProperty> stringsProperty_F = new List<SerializedProperty>();

    private int indicePag = 1;
    int contatore = 0;

    [PreferenceItem("Personaggi")]
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

    [MenuItem("Window/ToolsGame/Configurazione Proprieta %&X")]
    private static void Init()
    {
        EditorWindow.GetWindow<caratteristichePersonaggioEditor>("Editor Proprieta");
    }

    private void OnEnable()
    {
        if (EditorPrefs.HasKey(STR_PercorsoConfig))
        {
            percorso = EditorPrefs.GetString(STR_PercorsoConfig);
            _caratteristichePersonaggio = AssetDatabase.LoadAssetAtPath<caratteristichePersonaggio>(percorso + STR_DatabaseDiGioco);
        }
    }

    private void OnGUI()
    {
        if (_caratteristichePersonaggio != null)
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
                    _caratteristichePersonaggio = ScriptableObject.CreateInstance<caratteristichePersonaggio>();
                _caratteristichePersonaggio.classePersonaggio = new System.Collections.Generic.List<String>();
              

                AssetDatabase.CreateAsset(_caratteristichePersonaggio, percorso + STR_DatabaseDiGioco);
                AssetDatabase.Refresh();
                ProjectWindowUtil.ShowCreatedAsset(_caratteristichePersonaggio);
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

        GUILayout.Label("Gestione Personaggi", stileEtichetta);
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
        GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);


        if (_caratteristichePersonaggio.matriceProprietaM == null)
            _caratteristichePersonaggio.matriceProprietaM = new List<ClasseValorPersonaggio>();
        if (_caratteristichePersonaggio.matriceProprietaF == null)
            _caratteristichePersonaggio.matriceProprietaF = new List<ClasseValorPersonaggio>();
        if (GUILayout.Button("+", GUILayout.Width(100f)))
        {
            
            string tmp = "Classe_" + _caratteristichePersonaggio.classePersonaggio.Count;
            _caratteristichePersonaggio.classePersonaggio.Add(tmp);
            ClasseValorPersonaggio tmpC = new ClasseValorPersonaggio();
            ClasseValorPersonaggio tmpF= new ClasseValorPersonaggio();

            _caratteristichePersonaggio.matriceProprietaM.Add(tmpC);
            _caratteristichePersonaggio.matriceProprietaF.Add(tmpF);
            EditorUtility.SetDirty(_caratteristichePersonaggio);
            AssetDatabase.SaveAssets();

            contatore++;
        }
        if (GUILayout.Button("Resetta", GUILayout.Width(100f)))
        {
            resettaParametri();
            EditorUtility.SetDirty(_caratteristichePersonaggio);
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
        
       
        List<string> tmpClassePersonaggio = new List<string>();
        for (int k = 0; k < _caratteristichePersonaggio.classePersonaggio.Count; k++)
            tmpClassePersonaggio.Add(_caratteristichePersonaggio.classePersonaggio[k]);

        List<ClasseValorPersonaggio> tmpMatrice = new List<ClasseValorPersonaggio>();
        for (int w = 0; w < _caratteristichePersonaggio.matriceProprietaM.Count; w++)
            tmpMatrice.Add(_caratteristichePersonaggio.matriceProprietaM[w]);

        List<ClasseValorPersonaggio> tmpMatrice_F = new List<ClasseValorPersonaggio>();
        for (int w = 0; w < _caratteristichePersonaggio.matriceProprietaF.Count; w++)
            tmpMatrice_F.Add(_caratteristichePersonaggio.matriceProprietaF[w]);
        
        for (int i = 0; i < _caratteristichePersonaggio.classePersonaggio.Count; i++)
        {

            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            if (GUILayout.Button("-", GUILayout.Width(100f)))
            {
                tmpClassePersonaggio.Remove(_caratteristichePersonaggio.classePersonaggio[i]);
                tmpMatrice.Remove(_caratteristichePersonaggio.matriceProprietaM[i]);
                tmpMatrice_F.Remove(_caratteristichePersonaggio.matriceProprietaF[i]);
                stringsProperty_M.RemoveAt(i);
                stringsProperty_F.RemoveAt(i);
            }
            GUILayout.Label(new GUIContent("Razza_" + i.ToString()), stileEtichetta, GUILayout.Width(130));
            GUILayout.EndHorizontal();

            //for (int r = 0; r < Enum.GetValues(typeof(TipidiMaga)).Length; r++)

            //{
   
                EditorGUILayout.LabelField(_caratteristichePersonaggio.Maschio, stileEtichetta2, GUILayout.Width(130));
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                
                for (int c = 0; c < Enum.GetValues(typeof(Proprieta)).Length; c++)

                {
                    EditorGUILayout.LabelField(_caratteristichePersonaggio.schieraProprietaC[c], stileEtichetta2, GUILayout.Width(130));
                }
              
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                EditorGUI.BeginChangeCheck();
                serializedObject = new SerializedObject(_caratteristichePersonaggio);
                stringsProperty_M.Add(serializedObject.FindProperty("character_M"));
                EditorGUILayout.PropertyField(stringsProperty_M[i], new GUIContent(""), true, GUILayout.Width(130));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();

                    _caratteristichePersonaggio.matriceProprietaM[i].nome = stringsProperty_M[i].objectReferenceValue.name;
                    Undo.RecordObject(_caratteristichePersonaggio, "Nome");
                    EditorUtility.SetDirty(_caratteristichePersonaggio);
                    AssetDatabase.SaveAssets();
                }

                float tmpV = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprietaM[i].Vita, GUILayout.Width(130));
                if (tmpV != _caratteristichePersonaggio.matriceProprietaM[i].Vita)
                {
                    _caratteristichePersonaggio.matriceProprietaM[i].Vita = tmpV;
                    Undo.RecordObject(_caratteristichePersonaggio, "Vita");
                }

                float tmpM = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprietaM[i].Mana, GUILayout.Width(130));
                if (tmpM != _caratteristichePersonaggio.matriceProprietaM[i].Mana)
                {
                    _caratteristichePersonaggio.matriceProprietaM[i].Mana = tmpM;
                    Undo.RecordObject(_caratteristichePersonaggio, "Mana");
                }

                int tmpL = EditorGUILayout.IntField(_caratteristichePersonaggio.matriceProprietaM[i].Livello, GUILayout.Width(130));
                if (tmpL != _caratteristichePersonaggio.matriceProprietaM[i].Livello)
                {
                    _caratteristichePersonaggio.matriceProprietaM[i].Livello = tmpL;
                    Undo.RecordObject(_caratteristichePersonaggio, "Livello");
                }

                float tmpXp = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprietaM[i].Xp, GUILayout.Width(130));
                if (tmpXp != _caratteristichePersonaggio.matriceProprietaM[i].Xp)
                {
                    _caratteristichePersonaggio.matriceProprietaM[i].Xp = tmpXp;
                    Undo.RecordObject(_caratteristichePersonaggio, "Xp");
                }

                float tmpA = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprietaM[i].Attacco, GUILayout.Width(130));
                if (tmpA != _caratteristichePersonaggio.matriceProprietaM[i].Attacco)
                {
                    _caratteristichePersonaggio.matriceProprietaM[i].Attacco = tmpA;
                    Undo.RecordObject(_caratteristichePersonaggio, "Attacco");
                }
                float tmpD = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprietaM[i].difesa, GUILayout.Width(130));
                if (tmpD != _caratteristichePersonaggio.matriceProprietaM[i].difesa)
                {
                    _caratteristichePersonaggio.matriceProprietaM[i].difesa = tmpD;
                    Undo.RecordObject(_caratteristichePersonaggio, "Difesa");
                }
    
                GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            EditorGUILayout.LabelField(_caratteristichePersonaggio.matriceProprietaM[i].nome, stileEtichetta2, GUILayout.Width(500));
            GUILayout.EndHorizontal();

            // INIZIO _F
            EditorGUILayout.LabelField(_caratteristichePersonaggio.Femmine, stileEtichetta2, GUILayout.Width(130));

            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            EditorGUI.BeginChangeCheck();
            serializedObject = new SerializedObject(_caratteristichePersonaggio);
            stringsProperty_F.Add(serializedObject.FindProperty("character_F"));
            EditorGUILayout.PropertyField(stringsProperty_F[i], new GUIContent(""), true, GUILayout.Width(130));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                _caratteristichePersonaggio.matriceProprietaF[i].nome = stringsProperty_F[i].objectReferenceValue.name;
                Undo.RecordObject(_caratteristichePersonaggio, "Nome");
                EditorUtility.SetDirty(_caratteristichePersonaggio);
                AssetDatabase.SaveAssets();
            }


            float tmpV_F = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprietaF[i].Vita, GUILayout.Width(130));
            if (tmpV_F != _caratteristichePersonaggio.matriceProprietaF[i].Vita)
            {
                _caratteristichePersonaggio.matriceProprietaF[i].Vita = tmpV_F;
                Undo.RecordObject(_caratteristichePersonaggio, "Vita");
            }

            float tmpM_F = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprietaF[i].Mana, GUILayout.Width(130));
            if (tmpM_F != _caratteristichePersonaggio.matriceProprietaF[i].Mana)
            {
                _caratteristichePersonaggio.matriceProprietaF[i].Mana = tmpM_F;
                Undo.RecordObject(_caratteristichePersonaggio, "Mana");
            }
           
            int tmpL_F = EditorGUILayout.IntField(_caratteristichePersonaggio.matriceProprietaF[i].Livello, GUILayout.Width(130));
            if (tmpL_F != _caratteristichePersonaggio.matriceProprietaF[i].Livello)
            {
                _caratteristichePersonaggio.matriceProprietaF[i].Livello = tmpL_F;
                Undo.RecordObject(_caratteristichePersonaggio, "Livello");
            }

            float tmpXp_F = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprietaF[i].Xp, GUILayout.Width(130));
            if (tmpXp_F != _caratteristichePersonaggio.matriceProprietaF[i].Xp)
            {
                _caratteristichePersonaggio.matriceProprietaF[i].Xp = tmpXp_F;
                Undo.RecordObject(_caratteristichePersonaggio, "Xp");
            }

            float tmpA_F = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprietaF[i].Attacco, GUILayout.Width(130));
            if (tmpA_F != _caratteristichePersonaggio.matriceProprietaF[i].Attacco)
            {
                _caratteristichePersonaggio.matriceProprietaF[i].Attacco = tmpA_F;
                Undo.RecordObject(_caratteristichePersonaggio, "Attacco");
            }
            float tmpD_F = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprietaF[i].difesa, GUILayout.Width(130));
            if (tmpD_F != _caratteristichePersonaggio.matriceProprietaF[i].difesa)
            {
                _caratteristichePersonaggio.matriceProprietaF[i].difesa = tmpD_F;
                Undo.RecordObject(_caratteristichePersonaggio, "Difesa");
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            EditorGUILayout.LabelField(_caratteristichePersonaggio.matriceProprietaF[i].nome, stileEtichetta2, GUILayout.Width(500));
            GUILayout.EndHorizontal();
            }

        _caratteristichePersonaggio.classePersonaggio = tmpClassePersonaggio;
        _caratteristichePersonaggio.matriceProprietaM = tmpMatrice;
        _caratteristichePersonaggio.matriceProprietaF = tmpMatrice_F;
        GUILayout.EndVertical();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(_caratteristichePersonaggio);
            AssetDatabase.SaveAssets();
        }

        GUILayout.EndVertical();
    }


    private void resettaParametri()
    {
        for (int r = 0; r < Enum.GetValues(typeof(TipidiMaga)).Length; r++)
        {
            for (int c = 0; c < Enum.GetValues(typeof(Proprieta)).Length; c++)
            {
                _caratteristichePersonaggio.matriceProprietaM[r].nome = "Nessun Nome";
                _caratteristichePersonaggio.matriceProprietaM[r].Attacco = 0;
                _caratteristichePersonaggio.matriceProprietaM[r].difesa = 0;
                _caratteristichePersonaggio.matriceProprietaM[r].Livello = 0;
                _caratteristichePersonaggio.matriceProprietaM[r].Mana = 0;
                _caratteristichePersonaggio.matriceProprietaM[r].Vita = 0;
                _caratteristichePersonaggio.matriceProprietaM[r].Xp = 0;

                _caratteristichePersonaggio.matriceProprietaF[r].nome = "Nessun Nome";
                _caratteristichePersonaggio.matriceProprietaF[r].Attacco = 0;
                _caratteristichePersonaggio.matriceProprietaF[r].difesa = 0;
                _caratteristichePersonaggio.matriceProprietaF[r].Livello = 0;
                _caratteristichePersonaggio.matriceProprietaF[r].Mana = 0;
                _caratteristichePersonaggio.matriceProprietaF[r].Vita = 0;
                _caratteristichePersonaggio.matriceProprietaF[r].Xp = 0;

                _caratteristichePersonaggio.schieraProprietaC[c] = Enum.GetName(typeof(Proprieta), c);
            }
        }

    }


}
