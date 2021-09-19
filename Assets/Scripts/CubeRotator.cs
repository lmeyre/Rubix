using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotator : MonoBehaviour
{
    [SerializeField] float speed = 200f;
    Quaternion target;
    bool isRotating = false;

    void Awake()
    {
        target = transform.rotation;
    }

    void Update()
    {
        FollowTarget();
        if (!isRotating)
            CubeRotation();
    }

    void FollowTarget()
    {
        if (transform.rotation != target)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, speed * Time.deltaTime);
            isRotating = true;
        }
        else
            isRotating = false;
    }

    void CubeRotation()
    {
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            target = Quaternion.Euler(0, 90, 0) * transform.rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            target = Quaternion.Euler(0, -90, 0) * transform.rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            target = Quaternion.Euler(-90, 0, 0) * transform.rotation;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            target = Quaternion.Euler(90, 0, 0) * transform.rotation;
        }
    }
}