using UnityEngine;
using System.Collections;

public class CambioLivello : MonoBehaviour {

 public string  livello=""; 

	void OnTriggerEnter ()
	{
		Application.LoadLevel(livello);
	}
}
