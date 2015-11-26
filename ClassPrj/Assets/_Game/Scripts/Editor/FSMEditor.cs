using UnityEngine;
using System.Collections;
using UnityEditor;

namespace DFTGames.Tools.EditorTools
{
    [CustomEditor(typeof(FSM))]
    public class FSMeditor : Editor
    {
        private FSM vista;
        private FSM fsm_;
        private bool isDirty = false;
        private float tmpDist;
        private float tmpArc;
        private float tmpAmp;
        private float tmpVel;
        private float tmpDistArco;
        private float tmpDistSpada;
        private static Color backColor = Color.cyan;
        private static Color labelColor = Color.yellow;

        public override void OnInspectorGUI()
        {
            EditorGUI.indentLevel = 0;
            vista = (FSM)target;
            EditorGUILayout.Separator();
            Commons.SetColors(backColor, labelColor);
            Commons.DrawTexture(ResourceHelper.LogoFSM);
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            EditorGUILayout.LabelField("IMPOSTAZIONE ARCO E DISTANZA VISIVA");
            bool tmpBool = EditorGUILayout.Toggle(new GUIContent("Vis. Handle Vista", "Visualizza nella scena gli Handle della Vista"), vista.visualizzaHandleVista);
            if (tmpBool != vista.visualizzaHandleVista)
            {
                vista.visualizzaHandleAttacco = false;  //PER ORA FATTO PROVVISORIO..QUANDO CI SARAN PIU' ARMI SI FA UNA COSA DIVERSA
                isDirty = true;
                Undo.RecordObject(vista, "Dimensione Handle Vista");
                vista.visualizzaHandleVista = tmpBool;
            }

            tmpDist = EditorGUILayout.Slider(new GUIContent("Distanza Visibilita", "Quanto ci vedo senza occhiali"),
                vista.quantoCiVedoSenzaOcchiali, 0.1f, 100f);
            if (tmpDist != vista.quantoCiVedoSenzaOcchiali)
            {
                isDirty = true;
                Undo.RecordObject(vista, "Distanza Visibilita");
                vista.quantoCiVedoSenzaOcchiali = tmpDist;
            }

            tmpArc = EditorGUILayout.Slider(new GUIContent("Dimensione Arco", "Dimensione Angolo arco"), vista.alphaGrad, 5f, 175f);
            if (tmpArc != vista.alphaGrad)
            {
                isDirty = true;
                Undo.RecordObject(vista, "Dimensioni Arco");
                vista.alphaGrad = tmpArc;
            }
           
            float tmpHanD1 = EditorGUILayout.Slider(new GUIContent("Dimensione Handle Vista", "Imposta la dimensione degli handle Vista in questo pannello"),
               vista.dimensioneHandleVista, 0.1f, 15f);
            if (tmpHanD1 != vista.dimensioneHandleVista)
            {
                isDirty = true;
                Undo.RecordObject(vista, "Distanza Visibilita");
                vista.dimensioneHandleVista = tmpHanD1;
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("IMPOSTAZIONE AMPIEZZA E VELOCITA VISIVA");
            tmpAmp = EditorGUILayout.Slider(new GUIContent("Ampiezza Percezione Vista", "Dimensione Ampiezza"), vista.ampiezza, 0f, 5f);
            if (tmpAmp != vista.ampiezza)
            {
                isDirty = true;
                Undo.RecordObject(vista, "Dimensioni Ampiezza");
                vista.ampiezza = tmpAmp;
            }

            tmpVel = EditorGUILayout.Slider(new GUIContent("Velocita oscillazione Vista", "Velocita Oscillazione"), vista.velocitaOscillazioneVista, 0f, 5f);
            if (tmpVel != vista.velocitaOscillazioneVista)
            {
                isDirty = true;
                Undo.RecordObject(vista, "Vel Oscil Vista");
                vista.velocitaOscillazioneVista = tmpVel;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("IMPOSTAZIONE DISTANZA ARCO E SPADA");
            bool tmpBol = EditorGUILayout.Toggle(new GUIContent("Vis. Handle Distanza Attacco", "Visualizza nella scena gli Handle della Distanza Attacco"), vista.visualizzaHandleAttacco);
            if (tmpBol != vista.visualizzaHandleAttacco)
            {
                vista.visualizzaHandleVista = false;  //PER ORA FATTO PROVVISORIO..QUANDO CI SARAN PIU' ARMI SI FA UNA COSA DIVERSA
                isDirty = true;
                Undo.RecordObject(vista, "Visual Handle Dist Attacco");
                vista.visualizzaHandleAttacco = tmpBol;
            }

            tmpDistArco = EditorGUILayout.Slider(new GUIContent("Arco Distanza Attacco", "Distanza Di Attacco del Arco Goblin"), vista.distanzaAttaccoGoblinArco, 0.1f, 30f);
            if (tmpDistArco != vista.distanzaAttaccoGoblinArco)
            {
                isDirty = true;
                Undo.RecordObject(vista, "Arc Dist Attacco");
                vista.distanzaAttaccoGoblinArco = tmpDistArco;
            }

            tmpDistSpada = EditorGUILayout.Slider(new GUIContent("Spada Distanza Attacco", "Distanza Di Attacco del Spada Goblin"), vista.distanzaAttaccoGoblinSpada, 0.1f, 30f);
            if (tmpDistSpada != vista.distanzaAttaccoGoblinSpada)
            {
                isDirty = true;
                Undo.RecordObject(vista, "Spada Dist Attacco");
                vista.distanzaAttaccoGoblinSpada = tmpDistSpada;
            }
            float tmpHanD2 = EditorGUILayout.Slider(new GUIContent("Dimensione Handle DistArmi", "Imposta la dimensione degli handle Distanza armi in questo pannello"),
              vista.dimensioneHandleDistArmi, 1f, 15f);
            if (tmpHanD2 != vista.dimensioneHandleDistArmi)
            {
                isDirty = true;
                Undo.RecordObject(vista, "Dimensione Handle Distanza Armi");
                vista.dimensioneHandleDistArmi = tmpHanD2;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();

            if (GUI.changed || isDirty)
            {
                EditorUtility.SetDirty(vista);
            }
        }
        public void OnSceneGUI()
        {

            fsm_ = (FSM)target;
            if (fsm_.visualizzaHandleVista)
            {
                Handles.color = new Color(0.8f, 0.2f, 0.0f, 1f);

                fsm_.quantoCiVedoSenzaOcchiali = Mathf.Clamp(Handles.ScaleValueHandle(fsm_.quantoCiVedoSenzaOcchiali,
                    fsm_.transform.position + (fsm_.transform.forward * fsm_.quantoCiVedoSenzaOcchiali),
                      fsm_.transform.rotation, fsm_.dimensioneHandleVista, Handles.CubeCap,
                HandleUtility.GetHandleSize(fsm_.transform.position + (fsm_.transform.forward * fsm_.quantoCiVedoSenzaOcchiali))),
                0.1f, 100f);

                if (tmpDist != fsm_.quantoCiVedoSenzaOcchiali)
                {
                    isDirty = true;
                    Undo.RecordObject(fsm_, " Distanza Visiva");
                }

                fsm_.alphaGrad = Mathf.Clamp(Handles.ScaleValueHandle(fsm_.alphaGrad,
                            fsm_.transform.position + (fsm_.transform.forward * fsm_.quantoCiVedoSenzaOcchiali * 1.25f),
                            fsm_.transform.rotation, fsm_.dimensioneHandleVista,
                            Handles.ConeCap, HandleUtility.GetHandleSize
                            (fsm_.transform.position + (fsm_.transform.forward * fsm_.alphaGrad))), 5f, 175f);

                if (tmpArc != fsm_.alphaGrad)
                {
                    isDirty = true;
                    Undo.RecordObject(fsm_, " Ampieza Arco");
                }

                Vector3 DistanzaNegativa = Quaternion.Euler(0f, -fsm_.alphaGrad / 2f, 0f) * fsm_.transform.forward;

                Handles.color = new Color(0.2f, 0.8f, 0.4f, 0.2f);
                Handles.DrawSolidDisc(fsm_.transform.position, Vector3.up, fsm_.quantoCiVedoSenzaOcchiali);
                Handles.color = new Color(1f, 0.2f, 0.4f, 0.2f);
                Handles.DrawSolidArc(fsm_.transform.position, Vector3.up, DistanzaNegativa, fsm_.alphaGrad,
                    fsm_.quantoCiVedoSenzaOcchiali * 1.25f);
            }
            if (fsm_.visualizzaHandleAttacco)
            {
                fsm_.distanzaAttaccoGoblinArco = Mathf.Clamp(Handles.ScaleValueHandle(fsm_.distanzaAttaccoGoblinArco,
                fsm_.transform.position + (fsm_.transform.forward * fsm_.distanzaAttaccoGoblinArco),
                  fsm_.transform.rotation, fsm_.dimensioneHandleDistArmi, Handles.ConeCap,
            HandleUtility.GetHandleSize(fsm_.transform.position + (fsm_.transform.forward * fsm_.distanzaAttaccoGoblinArco))),
            0.1f, 30f);

                if (tmpDistArco != fsm_.distanzaAttaccoGoblinArco)
                {
                    isDirty = true;
                    Undo.RecordObject(fsm_, " Arc Dist Attacco");
                }

                Handles.color = Color.red;
                Handles.DrawLine(fsm_.transform.position, new Vector3(fsm_.transform.position.x, fsm_.transform.position.y, fsm_.transform.position.z + fsm_.distanzaAttaccoGoblinArco));
                fsm_.distanzaAttaccoGoblinSpada = Mathf.Clamp(Handles.ScaleValueHandle(fsm_.distanzaAttaccoGoblinSpada,
                   fsm_.transform.position + (fsm_.transform.forward * fsm_.distanzaAttaccoGoblinSpada),
                     fsm_.transform.rotation, fsm_.dimensioneHandleDistArmi, Handles.ConeCap,
               HandleUtility.GetHandleSize(fsm_.transform.position + (fsm_.transform.forward * fsm_.distanzaAttaccoGoblinSpada))),
               0.1f, 30f);

                if (tmpDistArco != fsm_.distanzaAttaccoGoblinSpada)
                {
                    isDirty = true;
                    Undo.RecordObject(fsm_, " Spada Dist Attacco");
                }

                Handles.color = Color.blue;
                Handles.DrawLine(fsm_.transform.position, new Vector3(fsm_.transform.position.x, fsm_.transform.position.y, fsm_.transform.position.z + fsm_.distanzaAttaccoGoblinSpada));
            }

            if (GUI.changed || isDirty)
                EditorUtility.SetDirty(fsm_);
        }
        
 
  
    }

}
