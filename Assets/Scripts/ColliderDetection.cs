using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetection : MonoBehaviour
{
    private List<GameObject> colliders = new List<GameObject>();
    public List<GameObject> GetCubies() { return colliders; }

    private void OnTriggerEnter(Collider other)
    {
        if (!colliders.Contains(other.gameObject)) { colliders.Add(other.gameObject); }
    }

    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other.gameObject);
    }
}
