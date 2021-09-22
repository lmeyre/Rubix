using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetection : MonoBehaviour
{
    private List<GameObject> colliders = new List<GameObject>();
    public List<GameObject> GetCubies() { return colliders; }

    void OnCollisionEnter(Collision collision)
    {
        if (!colliders.Contains(collision.gameObject)) { colliders.Add(collision.gameObject); }
    }

    void OnCollisionExit(Collision other)
    {
        colliders.Remove(other.gameObject);
    }
}
