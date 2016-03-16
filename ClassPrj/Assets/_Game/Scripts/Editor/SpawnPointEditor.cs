using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SpawnPoint))]

public class SpawnPointEditor : Editor {

    private SpawnPoint spPoint;
    private int tmpQuantita;
    private float tmpTempo;
    private float tmpRaggio;
    private bool isDirty = false;
    private SerializedObject serializedObject;
    //aoidjoi
    public override void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 0;
        spPoint = (SpawnPoint)target;

        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
        GUIStyle stileLabel = new GUIStyle(GUI.skin.GetStyle("Label"));
        stileLabel.alignment = TextAnchor.MiddleCenter;
        stileLabel.fontStyle = FontStyle.Bold;
        stileLabel.fontSize = 14;
        GUILayout.Label(new GUIContent("SPAWN POINT "),stileLabel);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);

        EditorGUI.BeginChangeCheck();
        serializedObject = new SerializedObject(spPoint);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tipi"), new GUIContent(""), true, GUILayout.Width(130));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            Undo.RecordObject(spPoint, "Classe Tipo");
            EditorUtility.SetDirty(spPoint);
        }

        EditorGUI.BeginChangeCheck();
        serializedObject = new SerializedObject(spPoint);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sesso"), new GUIContent(""), true, GUILayout.Width(130));
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            Undo.RecordObject(spPoint, "Classe Tipo");
            EditorUtility.SetDirty(spPoint);
        }
        //classiPersonaggi tmpTipi = (classiPersonaggi)EditorGUILayout.EnumPopup("Classe Tipo :", spPoint.tipi,GUILayout.Width(130));
        //if(tmpTipi != spPoint.tipi)
        //{
        //    spPoint.tipi = tmpTipi;
        //    Undo.RecordObject(spPoint, "Classe");
        //}
        tmpQuantita = EditorGUILayout.IntField(new GUIContent("Quantità", "Numero di Oggetti da Istanziare"),(int)Mathf.Clamp(spPoint.quantita,0,int.MaxValue));
        if(tmpQuantita != spPoint.quantita)
        {
            isDirty = true;
            Undo.RecordObject(spPoint, "Numero Oggetti");
            spPoint.quantita = tmpQuantita;
        }
        tmpTempo = EditorGUILayout.FloatField(new GUIContent("Tempo", "Tempo da uno spawn a l'altro"), Mathf.Clamp(spPoint.tempo, 0, float.MaxValue));
        if (tmpTempo != spPoint.tempo)
        {
            isDirty = true;
            Undo.RecordObject(spPoint, "Tempo Spawn");
            spPoint.tempo = tmpTempo;
        }
        tmpRaggio = EditorGUILayout.FloatField(new GUIContent("Raggio", "Area d'aazione dello Spawn"), Mathf.Clamp(spPoint.raggio, 0, float.MaxValue));
        if (tmpRaggio != spPoint.raggio)
        {
            isDirty = true;
            Undo.RecordObject(spPoint, "Raggio di Spawn");
            spPoint.raggio = tmpRaggio;
        }
        if(GUI.changed || isDirty)
        {
            EditorUtility.SetDirty(spPoint);
        }
        EditorGUILayout.EndVertical();
    }

    void OnSceneGUI()
    {
        
    }

}
