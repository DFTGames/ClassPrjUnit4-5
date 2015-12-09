using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using DFTGames.Tools.EditorTools;

[CustomEditor(typeof(GeneraPercorso))]
public class EditorGeneraPercorso : Editor
{
    RaycastHit hit;
    GeneraPercorso me;

    void OnEnable()
    {
        me = (GeneraPercorso)target;
    }

    private void OnSceneGUI()
    {
        for (int i = 1; i < me.transform.childCount; i++)
        {
            me.transform.GetChild(i - 1).name = " Nodo" + (i ); //queste 2 linee fanno in modo che se cambio ordine dei nodi la prox volta che seleziono
            me.transform.GetChild(i).name = " Nodo" + (i +1);  // il percorso lui mi riordina i nomi correttamente
            // Modifica Per Cambiare colori della texture
            Texture2D text= (Texture2D)ResourceHelper.Icon2;
            Color tmpColor = me.colore;
            for (int y = 0; y < text.height; y++)
            {
                for (int x = 0; x < text.width; x++)
                {
                    text.SetPixel(x, y, tmpColor);

                }
            }
            text.Apply();
            Handles.DrawAAPolyLine(text, HandleUtility.GetHandleSize(me.transform.GetChild(i - 1).position), me.transform.GetChild(i - 1).position, me.transform.GetChild(i).position);
            //fine Modifica
            //Originale==>   Handles.DrawAAPolyLine((Texture2D)ResourceHelper.Icon2, HandleUtility.GetHandleSize(me.transform.GetChild(i - 1).position), me.transform.GetChild(i-1).position,me.transform.GetChild(i).position);
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
                me.Percorso.Add(nuovo.transform);
                var utility = typeof(EditorGUIUtility);
                var impostaIcona = utility.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
                impostaIcona.Invoke(null, new object[] { nuovo, ResourceHelper.Icon1 });
                e.Use();

            }



        }
    }

}
