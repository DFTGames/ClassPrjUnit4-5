using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using DFTGames.Tools.EditorTools;
using System.Collections.Generic;
using System;

namespace DFTGames.Tools.EditorTools
{
    [CustomEditor(typeof(GestorePercorso))]
    public class EditorGestorePercorso : Editor

    {

        public const int NON_ESISTE = -1;  //messo dal Prof...cosi' si capisce meglio

        RaycastHit hit;
        GestorePercorso me;
        public string[] percorsiNomi;

        private List<string> percorsiDisponibili = new List<string>();
        private List<int> indexDisponibili = new List<int>();

        private List<int> tmpIndexLiberi;
        private List<string> tmpPercorsiLiberi;

        private Percorsi percorsi;
        private Transform transformOggettoSelezionato = null;

        void OnEnable()
        {

            me = (GestorePercorso)target;

            if (EditorPrefs.HasKey(Statici.STR_PercorsoConfig2))
            {
                string pathPercorsi = EditorPrefs.GetString(Statici.STR_PercorsoConfig2);
                percorsi = AssetDatabase.LoadAssetAtPath<Percorsi>(pathPercorsi + Statici.STR_DatabaseDiGioco2);

            }

        }

        private void OnSceneGUI()
        {
            //MODIFICATO USANDO GLI ITWEEN (Si trovano nel GestorePercorso )

            for (int i = 1; i < me.transform.childCount; i++)
            {
                me.transform.GetChild(i - 1).name = " Nodo" + (i); //queste 2 linee fanno in modo che se cambio ordine dei nodi la prox volta che seleziono
                me.transform.GetChild(i).name = " Nodo" + (i + 1);  // il percorso lui mi riordina i nomi correttamente  

            }
            Event e = Event.current;
            Vector2 mousePos = e.mousePosition;
            if (e.button == 0 && e.type == EventType.MouseDown && e.alt && e.control)
            {

                if (Physics.Raycast(HandleUtility.GUIPointToWorldRay(mousePos), out hit))
                {
                    GameObject nuovo = new GameObject("Nuovo Nodo");
                    nuovo.transform.position = hit.point;
                    nuovo.transform.parent = me.transform;
                    var utility = typeof(EditorGUIUtility);
                    var impostaIcona = utility.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
                    impostaIcona.Invoke(null, new object[] { nuovo, ResourceHelper.Icon1 });
                    e.Use();
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                    //AGGIUNTO MarkSceneDirty (si ringrazia Armando della dftStudent che ha fornito la classe) PER OVVIARE AL BUG DI UNITY DOVE UN PERCORSO CREATO VIA SCRIPT NON ME LO VEDE DA SALVARE QUANDO SI CAMBIA SCENA
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (percorsi == null)
            {
                EditorGUILayout.HelpBox("DataBaseDiGioco Mancante nel GameObject GruppoPercorsi", MessageType.Error);
                EditorGUILayout.Separator();
                return;
            }

            EditorGUILayout.Separator();
            GUIStyle stileEtichetta2 = new GUIStyle(GUI.skin.GetStyle("Label"));
            stileEtichetta2.alignment = TextAnchor.MiddleLeft;
            stileEtichetta2.fontStyle = FontStyle.Italic;
            stileEtichetta2.fontSize = 11;
            EditorGUILayout.LabelField("IndexPercorso  " + me.IndexPercorso.ToString(), stileEtichetta2, GUILayout.Width(130));

            if (Selection.activeTransform != transformOggettoSelezionato)
            {

                transformOggettoSelezionato = Selection.activeTransform;
                percorsiDisponibili = percorsi.elencaPercorsi();
                indexDisponibili = percorsi.elencaIdxPercorsi();

                tmpIndexLiberi = new List<int>(indexDisponibili);  //creata lista indexDisponibili
                tmpPercorsiLiberi = new List<string>(percorsiDisponibili); //ceata lista Nomi percorsi Disponibili


                if (percorsiDisponibili.Count < 1)
                {
                    EditorGUILayout.HelpBox(" Lista dei Percorsi Vuota....Inserirli in Windows ToolGame", MessageType.Error);
                    EditorGUILayout.Separator();
                    return;
                }

                GameObject tmpObj = GameObject.Find("PadrePercorso");

                if (GameObject.Find("PadrePercorso") == null)
                {
                    EditorGUILayout.HelpBox(" GameObject GruppoPercorsi Mancante", MessageType.Error);
                    EditorGUILayout.Separator();
                    return;
                }

                else   //carica nella lista gli index dei percorsi utilizzati     
                {
                    for (int i = 0; i < tmpObj.transform.childCount; i++)
                    {
                        int indexDaTogliere = tmpObj.transform.GetChild(i).GetComponent<GestorePercorso>().IndexPercorso;

                        if ((indexDaTogliere > NON_ESISTE && indexDaTogliere != me.IndexPercorso) && tmpIndexLiberi.Contains(indexDaTogliere))
                        {
                            int tmp = tmpIndexLiberi.IndexOf(indexDaTogliere);
                            tmpIndexLiberi.Remove(indexDaTogliere); //Debug.Log("Sto togliendio index " + indexDaTogliere);
                            tmpPercorsiLiberi.RemoveAt(tmp); //Debug.Log("Sto togliendio percorso " + tmp);

                        }
                    }

                }

            }

            if (tmpPercorsiLiberi.Count < 1)
            {
                EditorGUILayout.HelpBox(" Lista dei Percorsi Finita....Inserirli in Windows ToolGame", MessageType.Error);
                EditorGUILayout.Separator();
                me.IndexPercorso = NON_ESISTE; // me.gameObject.name = "Percorso";  
                me.name = "PERCORSO ERRATO";
                return;
            }
            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            EditorGUILayout.LabelField("IMPOSTAZIONE GESTORE PERCORSO");

            me.offsetSpostaOggetto = EditorGUILayout.Slider(new GUIContent("OffsetSollevaOggetto", "Di quando l'oggetto va sollevato dalla superficie"), me.offsetSpostaOggetto, 0, 2);
            me.colore = EditorGUILayout.ColorField(new GUIContent("Colore Percorso", "Imposta il colore dei nodi dei percorsi"), me.colore);


            int index = tmpIndexLiberi.IndexOf(me.IndexPercorso);
            int index2 = index;
            index = EditorGUILayout.Popup("Percorsi Disponibili", index, tmpPercorsiLiberi.ToArray()); //assegna index selezionato nella lista dei Liberi

            if (index > NON_ESISTE && (index != index2 || tmpPercorsiLiberi[index] != me.gameObject.name))
            {
                me.IndexPercorso = tmpIndexLiberi[index];
                me.gameObject.name = tmpPercorsiLiberi[index];

            }
            EditorGUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            }

        }

    }
}

