using System;
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
    SerializedProperty stringsProperty;
    private int indicePag = 1;

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
        if (GUILayout.Button("+", GUILayout.Width(100f)))
        {
            string tmp =  "Classe_"+ _caratteristichePersonaggio.classePersonaggio.Count;
            _caratteristichePersonaggio.classePersonaggio.Add(tmp);
            EditorUtility.SetDirty(_caratteristichePersonaggio);
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("Resetta", GUILayout.Width(100f)))
        {
            resettaParametri();
            EditorUtility.SetDirty(_caratteristichePersonaggio);
            AssetDatabase.SaveAssets();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
        for (int i = 0; i < _caratteristichePersonaggio.classePersonaggio.Count; i++)
        {
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            if (GUILayout.Button("-", GUILayout.Width(100f)))
            {
                _caratteristichePersonaggio.classePersonaggio.RemoveAt(i);
            }
            GUILayout.EndHorizontal();

            GUILayout.Label(new GUIContent("Razza_" + i.ToString()), stileEtichetta, GUILayout.Width(130));
            for (int r = 0; r < Enum.GetValues(typeof(TipidiMaga)).Length; r++)
            {
                EditorGUILayout.LabelField(_caratteristichePersonaggio.schieraTipiR[r], stileEtichetta2, GUILayout.Width(130));
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                //
                for (int c = 0; c < Enum.GetValues(typeof(Proprieta)).Length; c++)
                {
                    EditorGUILayout.LabelField(_caratteristichePersonaggio.schieraProprietaC[c], stileEtichetta2, GUILayout.Width(130));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);


                EditorGUI.BeginChangeCheck();
                serializedObject = new SerializedObject(_caratteristichePersonaggio);
                stringsProperty = serializedObject.FindProperty("character");
                EditorGUILayout.PropertyField(stringsProperty, new GUIContent(""), true, GUILayout.Width(130));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }


                //Ok 

                //QUESTO è un altra cosa
                float tmpV = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[1].Vita, GUILayout.Width(130));
                if (tmpV != _caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[1].Vita)
                {
                    _caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[1].Vita = tmpV;
                    Undo.RecordObject(_caratteristichePersonaggio, "Vita");
                }

                float tmpM = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[2].Mana, GUILayout.Width(130));
                if (tmpM != _caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[2].Mana)
                {
                    _caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[2].Mana = tmpM;
                    Undo.RecordObject(_caratteristichePersonaggio, "Mana");
                }

                int tmpL = EditorGUILayout.IntField(_caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[3].Livello, GUILayout.Width(130));
                if (tmpL != _caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[3].Livello)
                {
                    _caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[3].Livello = tmpL;
                    Undo.RecordObject(_caratteristichePersonaggio, "Livello");
                }

                float tmpXp = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[4].Xp, GUILayout.Width(130));
                if (tmpXp != _caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[4].Xp)
                {
                    _caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[4].Xp = tmpXp;
                    Undo.RecordObject(_caratteristichePersonaggio, "Xp");
                }

                float tmpA = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[5].Attacco, GUILayout.Width(130));
                if (tmpA!= _caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[5].Attacco)
                {
                    _caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[5].Attacco = tmpA;
                    Undo.RecordObject(_caratteristichePersonaggio, "Attacco");
                }
                float tmpD = EditorGUILayout.FloatField(_caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[6].difesa, GUILayout.Width(130));
                if (tmpD != _caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[6].difesa)
                {
                    _caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[6].difesa = tmpD;
                    Undo.RecordObject(_caratteristichePersonaggio, "Difesa");
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
        if (GUI.changed )
        {
            EditorUtility.SetDirty(_caratteristichePersonaggio);
            AssetDatabase.SaveAssets();
        }
        //if (caratteristichePersonaggio.tagEssere.Length != UnityEditorInternal.InternalEditorUtility.tags.Length - 5)
        //{
        //    int vecchio = caratteristichePersonaggio.tagEssere.Length;
        //    int differenzaLunghezze = UnityEditorInternal.InternalEditorUtility.tags.Length - 5 - caratteristichePersonaggio.tagEssere.Length;
        //    Array.Resize<string>(ref caratteristichePersonaggio.tagEssere, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
        //    Array.Resize<valoriProprieta>(ref caratteristichePersonaggio.matriceAmicizie, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);

        //    for (int i = 0; i < caratteristichePersonaggio.tagEssere.Length; i++)
        //    {
        //        if (caratteristichePersonaggio.matriceAmicizie[i] == null)
        //            caratteristichePersonaggio.matriceAmicizie[i] = new valoriProprieta();

        //        Array.Resize<Amicizie>(ref caratteristichePersonaggio.matriceAmicizie[i].elementoAmicizia, UnityEditorInternal.InternalEditorUtility.tags.Length - 5);
        //    }

        //    if (differenzaLunghezze > 0)
        //    {
        //        for (int i = vecchio; i < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; i++)
        //        {
        //            caratteristichePersonaggio.tagEssere[i] = UnityEditorInternal.InternalEditorUtility.tags[i + 5];

        //            for (int j = 0; j < UnityEditorInternal.InternalEditorUtility.tags.Length - 5; j++)
        //            {
        //                caratteristichePersonaggio.matriceAmicizie[i].elementoAmicizia[j] = Amicizie.Neutro;
        //                EditorUtility.SetDirty(caratteristichePersonaggio);
        //                AssetDatabase.SaveAssets();
        //            }
        //        }
        //    }
        //}
        GUILayout.EndVertical();

    }


    private void resettaParametri()
    {
        for (int r = 0; r < Enum.GetValues(typeof(TipidiMaga)).Length; r++)
        {

            for (int c = 0; c < Enum.GetValues(typeof(Proprieta)).Length; c++)
            {
                //_caratteristichePersonaggio.matriceProprieta[r] = new valoriPropieta();
                //_caratteristichePersonaggio.matriceProprieta[r].elementoProprieta[c] = new ClasseValorPersonaggio();
                _caratteristichePersonaggio.schieraTipiR[r] = Enum.GetName(typeof(TipidiMaga), r);
                _caratteristichePersonaggio.schieraProprietaC[c] = Enum.GetName(typeof(Proprieta), c);
            }
        }

    }


}
