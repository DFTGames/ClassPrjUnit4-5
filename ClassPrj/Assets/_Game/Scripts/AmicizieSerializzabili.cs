using UnityEngine;
using System.Collections;

[System.Serializable]
public class AmicizieSerializzabili {

    public string[] tipoEssere = new string[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];

    public classiAmicizie[] matriceAmicizie = new classiAmicizie[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];

    public string[] tagEssere = new string[UnityEditorInternal.InternalEditorUtility.tags.Length - 5]; //Aggiunto Tag cosi' ho gia riferimento per trovarlo

    public int[] indexPercorsi = new int[UnityEditorInternal.InternalEditorUtility.tags.Length - 5];  //Aggiunto index per memorizzazione percorso

}
