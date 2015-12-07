using UnityEngine;
using System.Collections;

public class ObjectCollision : MonoBehaviour
{

    void OnCollisionEnter(Collision collision)
    {
        // Only release the object if hits the plane
        if (collision.contacts[0].otherCollider.gameObject.name != "Plane")
            return;
        // release this object
        SendMessage("Dismiss");
    }
}