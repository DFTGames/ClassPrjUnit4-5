using UnityEngine;
using System.Collections;

public class TriggerCheckPoint : MonoBehaviour {

	void OnTriggerEnter(Collider coll)
    {
        if(coll.CompareTag("Player"))
        GameManager.MemorizzaCheckPoint(transform.name);
    }
}
