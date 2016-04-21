using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor.SceneManagement;

namespace DFTGames.Tools.EditorTools
{

    enum DoveSono
    {
        Modifica,
        Gestione
    }

    public class EditorPercorsiClass : EditorWindow
    {

        public caratteristichePersonaggioV2 personaggi;
        public Percorsi percorsi;

        public int contaPercorso;
        private DoveSono scelta = DoveSono.Modifica;

        private const int Default = -1;
        private Color OriginalBg = GUI.backgroundColor;
        private Color OriginalCont = GUI.contentColor;
        private Color OriginalColor = GUI.color;

        private static bool preferenzePercorsiCaricate = false;
        private static string pathPercorsi;

        private bool isDirty = false;
        private bool controlloPercorsiNulli = false;
        private bool controlloPersonaggiNulli = false;

        [PreferenceItem("Percorsi")]
        private static void preferenzeDiGameGUI()
        {
            if (!preferenzePercorsiCaricate)
            {
                pathPercorsi = EditorPrefs.GetString(Statici.STR_PercorsoConfig2);
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
                    EditorPrefs.SetString(Statici.STR_PercorsoConfig2, pathPercorsi);
                }
            }
            GUILayout.Label(pathPercorsi);
            GUILayout.EndHorizontal();
        }


        [MenuItem("Window/ToolsGame/Configurazione Percorsi %&P")]
        private static void Init()
        {
            EditorWindow.GetWindow<EditorPercorsiClass>("Editor PercorsiClass");
        }

        private void OnEnable()
        {

            isDirty = false;
            controlloPercorsiNulli = false;
            controlloPersonaggiNulli = false;

            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig2))
            {
                pathPercorsi = EditorPrefs.GetString(Statici.STR_PercorsoConfig2);
                percorsi = AssetDatabase.LoadAssetAtPath<Percorsi>(pathPercorsi + Statici.STR_DatabaseDiGioco2);
            }


            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig3))
            {
                pathPercorsi = EditorPrefs.GetString(Statici.STR_PercorsoConfig3);
                personaggi = AssetDatabase.LoadAssetAtPath<caratteristichePersonaggioV2>(pathPercorsi + Statici.STR_DatabaseDiGioco3);
                
            }

            if (percorsi != null) percorsi.cambiatoAlmenoUnaScena = false;   //QUA C'E UN PROBLEMA..ME LO FA MA NON ALLA PRIMA CREAZIONE...PARTE DALLA SECONDA..CHIEDERE A PINO

        }

        void OnDisable()            //controlla la lista percorsi con i percorsi e se non c'e assegnazione mette indexpercorso del oggetto a default(NON_ESISTE) 
        {
            controlloPercorsiVuoti();
            controlloPercorsiVuoti();
            SalvaAsset();   //LO MESSO QUA INIZIO..PERRCHE SE LO METTO ALLA FINE NON MI FUNZIONA..
            var scenaCorrente = EditorSceneManager.GetActiveScene();
            if (scenaCorrente.isDirty)  //chiedo se la scena corrente e' a dirty
            {
                bool scelta = EditorUtility.DisplayDialog("Save", "Salvi La Scena? ", " Ok ", "Cancel");
                if (scelta)
                    EditorSceneManager.SaveScene(scenaCorrente);
            }

            if (!percorsi.cambiatoAlmenoUnaScena) return;

            var sceneName2 = Path.GetFileNameWithoutExtension(scenaCorrente.path);

            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)  //MI FA IL CAMBIAMENTO IN TUTTE LE SCENE DEL GIOCO
            {
                var scene = EditorBuildSettings.scenes[i];

                if (scene.enabled)
                {
                    percorsi.scenaDaSalvare = false;
                    var sceneName = Path.GetFileNameWithoutExtension(scene.path);

                    string tmpScene = "Assets/_Game/Scene/" + sceneName + ".unity";
                    UnityEngine.SceneManagement.Scene tmpscenee = EditorSceneManager.OpenScene(tmpScene, OpenSceneMode.Single);
                    percorsi.ControlloIndexPercorsi();
                    if (percorsi.scenaDaSalvare)
                    {
                        EditorSceneManager.MarkSceneDirty(tmpscenee);  //imposto Scena a dirty...
                        EditorSceneManager.SaveScene(tmpscenee);
                    }
                    EditorSceneManager.CloseScene(tmpscenee, true);
                }
            }

            string tmpScenaCorrente = "Assets/_Game/Scene/" + sceneName2 + ".unity";  //mi ricarica la scena iniziale
            EditorSceneManager.OpenScene(tmpScenaCorrente, OpenSceneMode.Single);
        }

        private void SalvaAsset()
        {
            if (!isDirty) return;
            EditorUtility.SetDirty(percorsi);
            AssetDatabase.SaveAssets();
            isDirty = false;
        }

        private void controlloPercorsiVuoti() //VIENE CHIAMATO SU ONDISABLE OPPURE QUANDO PASSA NEL ALTRO MENU
        {
            if (controlloPercorsiNulli)
            {
                percorsi.EliminaPercorsiVuoti();
                controlloPercorsiNulli = false;
              //  SalvaAsset();
            }
        }
        private void controlloPersonaggiVuoti() //VIENE CHIAMATO SU ONDISABLE OPPURE QUANDO PASSA NEL ALTRO MENU
        {
            if (controlloPersonaggiNulli)
            {
                percorsi.EliminaClassiVuote();
                controlloPersonaggiNulli = false;
              //  SalvaAsset();
            }
        }

        private void OnGUI()
        {
            if (percorsi != null)
            {
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                GUILayout.Label("Editor by DFT Students", GUI.skin.GetStyle("Label"));
                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                if (GUILayout.Button("Inserisci/Modifca Percorsi"))
                    scelta = DoveSono.Modifica;

                if (percorsi.Count > 0)
                    if (GUILayout.Button("Gestisci Percorsi"))
                        scelta = DoveSono.Gestione;

                GUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                switch (scelta)
                {
                    case DoveSono.Modifica:
                        InserisciModificaPercorsi();
                        break;
                    case DoveSono.Gestione:
                        GestisciPercorsi();
                        break;
                }
            }
            else  //  if (percorsi != null)
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
                            EditorPrefs.SetString(Statici.STR_PercorsoConfig2, pathPercorsi);
                        }
                    }
                    if (pathPercorsi != string.Empty)
                    {
                        percorsi = ScriptableObject.CreateInstance<Percorsi>();
                        AssetDatabase.CreateAsset(percorsi, pathPercorsi + Statici.STR_DatabaseDiGioco2);
                        AssetDatabase.Refresh();
                        ProjectWindowUtil.ShowCreatedAsset(percorsi);
                    }
                }
                EditorGUILayout.HelpBox("DataBasePercorsi Mancante", MessageType.Error);
                GUILayout.EndHorizontal();
            }
            SalvaAsset();
        }

        private void InserisciModificaPercorsi()
        {
            controlloPersonaggiVuoti();
            GUIStyle stileEtichetta = new GUIStyle(GUI.skin.GetStyle("Label"));
            stileEtichetta.alignment = TextAnchor.MiddleCenter;
            stileEtichetta.fontStyle = FontStyle.Bold;
            stileEtichetta.fontSize = 14;
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUILayout.Label("Inserisci/Modifica Nome Percorsi", stileEtichetta);
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            if (percorsi.Count > 0)

                if (GUILayout.Button("Resetta", GUILayout.Width(100f)))
                {
                    percorsi.Clear();
                    isDirty = true;
                }
            EditorGUILayout.Separator();
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            stileEtichetta.alignment = TextAnchor.MiddleLeft;
            stileEtichetta.fontStyle = FontStyle.Bold;
            stileEtichetta.fontSize = 12;
            GUILayout.Label("Classe Percorsi (Premere + per aggiungere percorso)", stileEtichetta);

            if (GUILayout.Button(" + ", GUILayout.Width(30)))
            {
                string tmp = Statici.tmpPercorsi;

                controlloPercorsiNulli = true;
                percorsi.Add(tmp);
            }
            GUILayout.EndHorizontal();

            if (percorsi.Count > 0)
            {
                for (int i = 0; i < percorsi.Count; i++)

                {
                    GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                    string tmp = EditorGUILayout.TextField(percorsi[i].nomePercorsi);
                    if (tmp != percorsi[i].nomePercorsi)
                    {
                        if (percorsi.ControlloNomiPercorso(tmp)) //problema che gia alla prima lettera mi fa il controllo..MODIFICARLO..(come non so..)
                        {
                            percorsi[i].nomePercorsi = tmp;
                            isDirty = true;
                        }
                    }
                    if (GUILayout.Button(" - ", GUILayout.Width(30)))  //mi permette di cancellare le righe
                    {
                        percorsi.RemoveAt(i);
                        isDirty = true;
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
            }
        }

        private void GestisciPercorsi()
        {
            if (percorsi == null) return;

            if (percorsi.Count == 0) return;

            if (personaggi == null)
            {
                EditorGUILayout.HelpBox("DataBase Caratteristiche Personaggi mancante.....Crearlo e alimentarlo", MessageType.Error);
                return;
            }

            if (personaggi.elencaClassiNonGiocabiliToString().Length == 0)
            {
                EditorGUILayout.HelpBox("Classi non Giocabili mancanti nel DataBase Caratteristiche Personaggi ", MessageType.Error);
                return;
            }
            controlloPercorsiVuoti();  //HO DECISO DI FARE CONTROLLO PERCORSI VUOTI ANCHE QUANDO VADO IN GESTISCIPERCORSI..
            GUIStyle stileEtichetta = new GUIStyle(GUI.skin.GetStyle("Label"));
            stileEtichetta.alignment = TextAnchor.MiddleCenter;
            stileEtichetta.fontStyle = FontStyle.Bold;
            stileEtichetta.fontSize = 14;
            GUIStyle stileEtichetta2 = new GUIStyle(GUI.skin.GetStyle("Label"));
            stileEtichetta2.alignment = TextAnchor.MiddleLeft;
            stileEtichetta2.fontStyle = FontStyle.Bold;
            stileEtichetta2.fontSize = 11;
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            GUILayout.Label("Gestione Percorsi", stileEtichetta);
            GUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            GUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
            EditorGUILayout.Separator();
            GUILayout.Label(new GUIContent("Assegnazione Percorsi "), stileEtichetta, GUILayout.Width(200));
            GUILayout.Label(" (Premere + per aggiungere personaggi al percorso)", stileEtichetta2);
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            GUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            bool setDirtyPersonaggi = false;
            for (int i = 0; i < percorsi.Count; i++)
            {
                EditorGUILayout.LabelField(percorsi[i].nomePercorsi, stileEtichetta2, GUILayout.Width(130));
                for (int c = 0; c < percorsi[i].classi.Count; c++)
                {
                    GUILayout.BeginHorizontal(EditorStyles.objectFieldThumb);
                    int index = -1;
                    index = Array.IndexOf(personaggi.elencaClassiNonGiocabiliToEnumArray(), percorsi[i].classi[c]);

                    int index2 = index;
                    EditorGUILayout.LabelField(string.Empty, stileEtichetta2, GUILayout.Width(130));
 
                    index = EditorGUILayout.Popup(index, personaggi.elencaClassiNonGiocabiliToString());

                    if ((c == percorsi[i].classi.Count - 1) && (GUILayout.Button(" + ", GUILayout.Width(30))))
                    {
                        percorsi[i].classi.Add(classiPersonaggi.indefinito);
                        controlloPersonaggiNulli = true;

                    }
                    if (c > 0 && GUILayout.Button(" - ", GUILayout.Width(30)))
                    {
                        percorsi.RemoveAtClass(i, c);

                        setDirtyPersonaggi = true;
                    }
                    if (index != index2)   //se e' stato fatto modifica
                    {
                        if (percorsi.trovaPersonaggiDaIndicePercorsi(i).Contains(personaggi.elencaClassiNonGiocabiliToEnumArray()[index]))
                        {
                            percorsi[i].classi[c] = classiPersonaggi.indefinito;
                            controlloPersonaggiNulli = true;
                            Debug.LogError("Ti si e' disconnesso il cervello per qualche secondo ?....hai scelto un personaggio gia usato ");
                        }
                        else
                        {

                            if (index == -1)
                                percorsi[i].classi[c] = classiPersonaggi.indefinito;
                            else percorsi[i].classi[c] = personaggi.elencaClassiNonGiocabiliToEnumArray()[index];
                        }

                        setDirtyPersonaggi = true; ;
                    }
                    GUILayout.EndHorizontal();
                }
            }  //  (ciclo for)

            GUILayout.EndVertical();

            if (setDirtyPersonaggi)  //mi salva i cambiamenti dentrl al GestisciPercorsi
            {
                EditorUtility.SetDirty(percorsi);
                AssetDatabase.SaveAssets();
            }
        }
    }
}