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
    private string razza_m = string.Empty;
    private string razza_f = string.Empty;
    private Vector2 posizioneScroll;
    private SerializedObject serializedObject;
    List<SerializedProperty> stringsProperty_M = new List<SerializedProperty>();
    List<SerializedProperty> stringsProperty_F = new List<SerializedProperty>();
    private List<String> classePersonaggio = new List<string>();
    private List<String> listaRazzeM = new List<string>();
    private List<String> listaRazzeF = new List<string>();


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
            if (_caratteristichePersonaggio != null && _caratteristichePersonaggio.matriceProprietaM != null)
            {
                for (int i = 0; i < _caratteristichePersonaggio.matriceProprietaM.Count; i++)
                {
                    if (classePersonaggio == null)
                        classePersonaggio = new List<string>();
                    if (listaRazzeM == null)
                        listaRazzeM = new List<string>();

                    classePersonaggio.Add(_caratteristichePersonaggio.matriceProprietaM[i].razza);
                    listaRazzeM.Add(_caratteristichePersonaggio.matriceProprietaM[i].razza);
                }
            }
            if (_caratteristichePersonaggio != null && _caratteristichePersonaggio.matriceProprietaF != null)
            {
                for (int i = 0; i < _caratteristichePersonaggio.matriceProprietaF.Count; i++)
                {

                    if (listaRazzeF == null)
                        listaRazzeF = new List<string>();

                    listaRazzeF.Add(_caratteristichePersonaggio.matriceProprietaF[i].razza);
                }
            }

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
                classePersonaggio = new System.Collections.Generic.List<String>();


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

            string tmp = "Classe_" + classePersonaggio.Count;
            classePersonaggio.Add(tmp);
            ClasseValorPersonaggio tmpC = new ClasseValorPersonaggio();
            ClasseValorPersonaggio tmpF = new ClasseValorPersonaggio();
            _caratteristichePersonaggio.matriceProprietaM.Add(tmpC);
            _caratteristichePersonaggio.matriceProprietaF.Add(tmpF);
            listaRazzeM.Add(string.Empty);
            listaRazzeF.Add(string.Empty);
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


        List<string> tmpClassePersonaggio = new List<string>();
        for (int k = 0; k < classePersonaggio.Count; k++)
            tmpClassePersonaggio.Add(classePersonaggio[k]);

        List<ClasseValorPersonaggio> tmpMatrice = new List<ClasseValorPersonaggio>();
        for (int w = 0; w < _caratteristichePersonaggio.matriceProprietaM.Count; w++)
            tmpMatrice.Add(_caratteristichePersonaggio.matriceProprietaM[w]);

        List<ClasseValorPersonaggio> tmpMatrice_F = new List<ClasseValorPersonaggio>();
        for (int w = 0; w < _caratteristichePersonaggio.matriceProprietaF.Count; w++)
            tmpMatrice_F.Add(_caratteristichePersonaggio.matriceProprietaF[w]);


        List<string> tmplistaM = new List<string>();
        for (int k = 0; k < listaRazzeM.Count; k++)
            tmplistaM.Add(listaRazzeM[k]);

        List<string> tmplistaF = new List<string>();
        for (int k = 0; k < listaRazzeF.Count; k++)
            tmplistaF.Add(listaRazzeF[k]);

        for (int i = 0; i < classePersonaggio.Count; i++)
        {

            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            if (GUILayout.Button("-", GUILayout.Width(100f)))
            {
                tmpClassePersonaggio.Remove(classePersonaggio[i]);
                tmpMatrice.Remove(_caratteristichePersonaggio.matriceProprietaM[i]);
                tmpMatrice_F.Remove(_caratteristichePersonaggio.matriceProprietaF[i]);
                tmplistaF.Remove(listaRazzeF[i]);
                tmplistaM.Remove(listaRazzeM[i]);
                stringsProperty_M.RemoveAt(i);
                stringsProperty_F.RemoveAt(i);
            }
            if (_caratteristichePersonaggio.matriceProprietaM[i].nome == "nessun nome" && _caratteristichePersonaggio.matriceProprietaF[i].nome == "nessun nome")
                GUILayout.Label(new GUIContent("Razza_" + i.ToString()), stileEtichetta, GUILayout.Width(150));
            else
            {
                if (!listaRazzeF[i].Equals(string.Empty) && listaRazzeM[i].Equals(string.Empty))
                {
                    GUILayout.Label(new GUIContent("Razza_" + listaRazzeF[i]), stileEtichetta, GUILayout.Width(150));

                    tmpMatrice_F[i].razza = listaRazzeF[i];

                }
                else if (listaRazzeF[i].Equals(string.Empty) && !listaRazzeM[i].Equals(string.Empty))
                {
                    GUILayout.Label(new GUIContent("Razza_" + listaRazzeM[i]), stileEtichetta, GUILayout.Width(150));

                    tmpMatrice[i].razza = listaRazzeM[i];

                }
                else if (!listaRazzeF[i].Equals(string.Empty) && !listaRazzeM[i].Equals(string.Empty) && listaRazzeF[i] == listaRazzeM[i])
                {
                    GUILayout.Label(new GUIContent("Razza_" + listaRazzeM[i]), stileEtichetta, GUILayout.Width(150));

                    tmpMatrice_F[i].razza = listaRazzeF[i];
                    tmpMatrice[i].razza = listaRazzeM[i];

                }
                else if (!listaRazzeF[i].Equals(string.Empty) && !listaRazzeM[i].Equals(string.Empty) && listaRazzeF[i] != listaRazzeM[i])
                {
                    GUILayout.Label(new GUIContent("Razza_" + "Attenzione! M e F di razze diverse"), stileEtichetta, GUILayout.Width(400));

                    tmpMatrice_F[i].razza = listaRazzeF[i];
                    tmpMatrice[i].razza = listaRazzeM[i];
                }
            }
            GUILayout.EndHorizontal();



            EditorGUILayout.LabelField(_caratteristichePersonaggio.Maschio, stileEtichetta2, GUILayout.Width(130));
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);

            for (int c = 0; c < Enum.GetValues(typeof(Proprieta)).Length; c++)

            {
                _caratteristichePersonaggio.schieraProprietaC[c] = Enum.GetName(typeof(Proprieta), c);
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
                razza_m = stringsProperty_M[i].objectReferenceValue.name.Substring(0, stringsProperty_M[i].objectReferenceValue.name.Length - 2);
                string msgErr_m = string.Empty;
                if (controlloPersonaggi(razza_m, (stringsProperty_M[i].objectReferenceValue == null) ? string.Empty : stringsProperty_M[i].objectReferenceValue.name,
                                      (_caratteristichePersonaggio.matriceProprietaF[i].nome == "nessun nome") ? string.Empty : _caratteristichePersonaggio.matriceProprietaF[i].nome,
                                        out msgErr_m, listaRazzeM))
                {
                    serializedObject.ApplyModifiedProperties();
                    tmpClassePersonaggio[i] = razza_m;
                    tmplistaM[i] = razza_m;
                    tmpMatrice[i].nome = stringsProperty_M[i].objectReferenceValue.name;
                    tmpMatrice[i].razza = razza_m;
                    Undo.RecordObject(_caratteristichePersonaggio, "Nome");
                    EditorUtility.SetDirty(_caratteristichePersonaggio);
                    AssetDatabase.SaveAssets();

                }
                else
                {
                    if (!string.IsNullOrEmpty(msgErr_m))
                        EditorUtility.DisplayDialog("Errore Editor", msgErr_m, "Ok");
                }

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


            EditorGUILayout.LabelField(_caratteristichePersonaggio.Femmine, stileEtichetta2, GUILayout.Width(130));

            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            EditorGUI.BeginChangeCheck();
            serializedObject = new SerializedObject(_caratteristichePersonaggio);
            stringsProperty_F.Add(serializedObject.FindProperty("character_F"));
            EditorGUILayout.PropertyField(stringsProperty_F[i], new GUIContent(""), true, GUILayout.Width(130));
            if (EditorGUI.EndChangeCheck())
            {
                razza_f = stringsProperty_F[i].objectReferenceValue.name.Substring(0, stringsProperty_F[i].objectReferenceValue.name.Length - 2);
                string msgErr_f = string.Empty;

                if (controlloPersonaggi(razza_f, (_caratteristichePersonaggio.matriceProprietaM[i].nome == "nessun nome") ? string.Empty : _caratteristichePersonaggio.matriceProprietaM[i].nome,
                                        stringsProperty_F[i].objectReferenceValue == null ? string.Empty : stringsProperty_F[i].objectReferenceValue.name,
                                        out msgErr_f, listaRazzeF))
                {
                    if (!string.IsNullOrEmpty(msgErr_f))
                        EditorUtility.DisplayDialog("Errore Editor", msgErr_f, "Ok");
                    serializedObject.ApplyModifiedProperties();
                    tmpClassePersonaggio[i] = razza_f;
                    tmplistaF[i] = razza_f;
                    tmpMatrice_F[i].nome = stringsProperty_F[i].objectReferenceValue.name;
                    tmpMatrice_F[i].razza = razza_f;
                    Undo.RecordObject(_caratteristichePersonaggio, "Nome");
                    EditorUtility.SetDirty(_caratteristichePersonaggio);
                    AssetDatabase.SaveAssets();

                }
                else
                {
                    if (!string.IsNullOrEmpty(msgErr_f))
                        EditorUtility.DisplayDialog("Errore Editor", msgErr_f, "Ok");
                }
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

        classePersonaggio = tmpClassePersonaggio;
        _caratteristichePersonaggio.matriceProprietaM = tmpMatrice;
        _caratteristichePersonaggio.matriceProprietaF = tmpMatrice_F;
        listaRazzeM = tmplistaM;
        listaRazzeF = tmplistaF;
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
        for (int i = 0; i < listaRazzeM.Count; i++)
            listaRazzeM[i] = string.Empty;

        for (int i = 0; i < listaRazzeF.Count; i++)
            listaRazzeF[i] = string.Empty;

        for (int i = 0; i < classePersonaggio.Count; i++)
            classePersonaggio[i] = "Classe_" + i;

        for (int r = 0; r < classePersonaggio.Count; r++)
        {
            _caratteristichePersonaggio.matriceProprietaM[r].razza = string.Empty;
            _caratteristichePersonaggio.matriceProprietaM[r].nome = "nessun nome";
            _caratteristichePersonaggio.matriceProprietaM[r].Attacco = 0;
            _caratteristichePersonaggio.matriceProprietaM[r].difesa = 0;
            _caratteristichePersonaggio.matriceProprietaM[r].Livello = 0;
            _caratteristichePersonaggio.matriceProprietaM[r].Mana = 0;
            _caratteristichePersonaggio.matriceProprietaM[r].Vita = 0;
            _caratteristichePersonaggio.matriceProprietaM[r].Xp = 0;

            _caratteristichePersonaggio.matriceProprietaF[r].razza = string.Empty;
            _caratteristichePersonaggio.matriceProprietaF[r].nome = "nessun nome";
            _caratteristichePersonaggio.matriceProprietaF[r].Attacco = 0;
            _caratteristichePersonaggio.matriceProprietaF[r].difesa = 0;
            _caratteristichePersonaggio.matriceProprietaF[r].Livello = 0;
            _caratteristichePersonaggio.matriceProprietaF[r].Mana = 0;
            _caratteristichePersonaggio.matriceProprietaF[r].Vita = 0;
            _caratteristichePersonaggio.matriceProprietaF[r].Xp = 0;

            for (int c = 0; c < Enum.GetValues(typeof(Proprieta)).Length; c++)
                _caratteristichePersonaggio.schieraProprietaC[c] = Enum.GetName(typeof(Proprieta), c);

            AssetDatabase.Refresh();
        }
        EditorUtility.SetDirty(_caratteristichePersonaggio);
        AssetDatabase.SaveAssets();

    }

    bool controlloPersonaggi(string razza, string nomeM, string nomeF, out string msg, List<string> listaRazza)
    {
        msg = string.Empty;

        if (listaRazza.Contains(razza))
        {
            msg = "Razza gia' esistente";
            return false;
        }
        else
        {
            if (nomeM.Equals(string.Empty) && !nomeF.Equals(string.Empty) || !nomeM.Equals(string.Empty) && nomeF.Equals(string.Empty))
                return true;

            else if (!nomeM.Equals(string.Empty) && !nomeF.Equals(string.Empty))
            {
                string tmpRM = nomeM.Substring(0, nomeM.Length - 2);
                string tmpRF = nomeF.Substring(0, nomeF.Length - 2);
                if (tmpRM.Equals(tmpRF))
                    return true;

                else
                {
                    msg = "Errore : M e F di Razze diverse";
                    return true;
                }
            }
            else
            {
                msg = "Assegnare un Game Object";
                return false;
            }
        }
    }

}
