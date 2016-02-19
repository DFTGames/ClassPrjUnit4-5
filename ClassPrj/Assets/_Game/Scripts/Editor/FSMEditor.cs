using UnityEngine;
using System.Collections;
using UnityEditor;

/*
Descrizione della classe "FSMEditor" nel Gdd

Questa classe viene messa dentro la cartella editor in quanto viene eseguita non in 
modalita run-time
Mi disegna una maschera nel inspector con dentro 

- Impostazione Arco e Distanza Visiva 
    Mi permette di  impostare l'arco e il campo di azione della vista 
    Presente un flag per selezionare o deselezionare la rappresentazione nella scena
- Impostazione ampiezza e velocita visiva
    Mi permette di  impostare i dati relativi alla sensibilita della vista nel tempo 
    Abbiamo impotizzato un andamento sinusoidale per rendere piu' simile al comportamento
    effettivo di tale senso.
- Impostazione distanza arco e spada
    Si imposta la distanza di attaco della spada e arco (per ora solo questi 2), cioe' e' la
    distanza sotto la quale i colpi arrecono danno all'avversario
    Presente un flag per selezionare o deselezionare la rappresentazione nella scena

Ciascuna voce e' predisposta per fare l'Undo e Redo in Edit

DICHIARAZIONI nello script  : Utilizza la referenza alla classe FSM
*/

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
            EditorGUILayout.LabelField("Indice Percorso   " + vista.IndexPercorso); //Lo devo abbellire...Ci metto il nome del percorso a cui si riferisce
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            EditorGUILayout.LabelField("IMPOSTAZIONE VISTA");
            bool tmpBool = EditorGUILayout.Toggle(new GUIContent("Vis. Area Vista", "Visualizza nella scena gli Handle della Vista"), vista.visualizzaHandleVista);
            if (tmpBool != vista.visualizzaHandleVista)
            {
                vista.visualizzaHandleAttacco = false;  //PER ORA FATTO PROVVISORIO..QUANDO CI SARAN PIU' ARMI SI FA UNA COSA DIVERSA
                isDirty = true;
                Undo.RecordObject(vista, "Vis. Handle Vista");
                vista.visualizzaHandleVista = tmpBool;
            }

       
                bool tmpBoolTipoAttacco = EditorGUILayout.Toggle(new GUIContent("Attacco da Vicino", "Attiva l'attacco da vicino o da lontano. N.B. Vietato cambiarlo a runtime"), vista.attaccoDaVicino);
                if (tmpBoolTipoAttacco != vista.attaccoDaVicino)
                {

                    isDirty = true;
                    Undo.RecordObject(vista, "Attacco da Vicino");
                    vista.attaccoDaVicino = tmpBoolTipoAttacco;
                    if (tmpBoolTipoAttacco)
                        vista.distanzaAttacco = vista.distanzaAttaccoGoblinPugno;
                    else
                        vista.distanzaAttacco = vista.distanzaAttaccoGoblinArco;
                    
                }
            

            if (vista.visualizzaHandleVista)
            {
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
                    Undo.RecordObject(vista, "Distanza Handle Vista");
                    vista.dimensioneHandleVista = tmpHanD1;
                }                          
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("IMPOSTAZIONE PERCEZIONE VISIVA");
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

            EditorGUILayout.LabelField("IMPOSTAZIONE ARMI");
            bool tmpBol = EditorGUILayout.Toggle(new GUIContent("Vis. Area Attacco", "Visualizza nella scena gli Handle della Distanza Attacco"), vista.visualizzaHandleAttacco);
            if (tmpBol != vista.visualizzaHandleAttacco)
            {
                vista.visualizzaHandleVista = false;  //PER ORA FATTO PROVVISORIO..QUANDO CI SARAN PIU' ARMI SI FA UNA COSA DIVERSA
                isDirty = true;
                Undo.RecordObject(vista, "Vis. Handle Area Attacco");
                vista.visualizzaHandleAttacco = tmpBol;
            }
            if (vista.visualizzaHandleAttacco)
            {
                tmpDistArco = EditorGUILayout.Slider(new GUIContent("Arco Dist. Attacco", "Distanza Di Attacco del Arco Goblin"), vista.distanzaAttaccoGoblinArco, 0.1f, 30f);
                if (tmpDistArco != vista.distanzaAttaccoGoblinArco)
                {
                    isDirty = true;
                    Undo.RecordObject(vista, "Arc Dist Attacco");
                    vista.distanzaAttaccoGoblinArco = tmpDistArco;
                }

                tmpDistSpada = EditorGUILayout.Slider(new GUIContent("Spada Dist. Attacco", "Distanza Di Attacco del Spada Goblin"), vista.distanzaAttaccoGoblinPugno, 0.1f, 30f);
                if (tmpDistSpada != vista.distanzaAttaccoGoblinPugno)
                {
                    isDirty = true;
                    Undo.RecordObject(vista, "Spada Dist Attacco");
                    vista.distanzaAttaccoGoblinPugno = tmpDistSpada;
                }
                float tmpHanD2 = EditorGUILayout.Slider(new GUIContent("Dimensione Handle DistArmi", "Imposta la dimensione degli handle Distanza armi in questo pannello"),
                  vista.dimensioneHandleDistArmi, 1f, 15f);
                if (tmpHanD2 != vista.dimensioneHandleDistArmi)
                {
                    isDirty = true;
                    Undo.RecordObject(vista, "Dimensione Handle Distanza Armi");
                    vista.dimensioneHandleDistArmi = tmpHanD2;
                }
                
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            if (GUI.changed || isDirty)
            {
                EditorUtility.SetDirty(vista);
            }
            EditorGUILayout.EndVertical();

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

                Handles.color = new Color(0.8f,0.3f,0.5f,0.2f); 
                Handles.DrawSolidDisc(fsm_.transform.position, Vector3.up, fsm_.distanzaAttaccoGoblinArco);

                fsm_.distanzaAttaccoGoblinPugno = Mathf.Clamp(Handles.ScaleValueHandle(fsm_.distanzaAttaccoGoblinPugno,
                   fsm_.transform.position + (fsm_.transform.forward * fsm_.distanzaAttaccoGoblinPugno),
                     fsm_.transform.rotation, fsm_.dimensioneHandleDistArmi, Handles.ConeCap,
               HandleUtility.GetHandleSize(fsm_.transform.position + (fsm_.transform.forward * fsm_.distanzaAttaccoGoblinPugno))),
               0.1f, 30f);

                if (tmpDistArco != fsm_.distanzaAttaccoGoblinPugno)
                {
                    isDirty = true;
                    Undo.RecordObject(fsm_, " Spada Dist Attacco");
                }

                Handles.color = new Color(1,0.92f,0.016f,0.2f);
                Handles.DrawSolidDisc(fsm_.transform.position, Vector3.up, fsm_.distanzaAttaccoGoblinPugno);
               
            }

            if (GUI.changed || isDirty)
                EditorUtility.SetDirty(fsm_);
        }
        
 
  
    }

}
