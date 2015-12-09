using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
[System.Serializable]
public class button 
{
    public string Nome;
    public Image img;
    public Text nome;
    public Text Livello;
    public Text Luogo;

}

public class DinamicButton : MonoBehaviour {

    public List<button> parametriButton = new List<button>();
    public List<GameObject> listaSalvataggi = new List<GameObject>();
    public GameObject bottonePref;
    private int i = 0;

    void Start ()
    {
     }
	void Update ()
    {

        if(i < listaSalvataggi.Count)
        {
            for (i = 0; i < listaSalvataggi.Count; i++)
            {
                GameObject tmpButton = Instantiate(bottonePref) as GameObject;
                
            }
        }
    }
}
