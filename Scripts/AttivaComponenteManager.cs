using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AttivaComponenteManager : MonoBehaviour {

  
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Application.loadedLevel != 0)
        {
            
          gameObject.GetComponent<GameManager>().enabled = true;
            this.enabled = false;
        }
	}
}
