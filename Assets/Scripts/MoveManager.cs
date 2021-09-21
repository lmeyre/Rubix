using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public bool isMoving;

    float movingSpeed = 100;

    [SerializeField] ColliderDetection up;
    [SerializeField] ColliderDetection down;
    [SerializeField] ColliderDetection front;
    [SerializeField] ColliderDetection back;
    [SerializeField] ColliderDetection left;
    [SerializeField] ColliderDetection right;

    [SerializeField] Transform rotaterHolder;
    [SerializeField] Transform cube;

    public void Move(int move)
    {
        if (isMoving == true)
            Debug.LogError("PROBLEM");
        isMoving = true;
        foreach (Transform child in rotaterHolder)
            child.SetParent(cube);
        switch (move)
        {
            case 0:
                RotateFace(up, -90);
                break;
            default:
                break;
        }
    }

    void RotateFace(ColliderDetection face, float degree)
    {
        foreach (GameObject cubies in face.GetCubies())
        {
            cubies.transform.SetParent(rotaterHolder);
        }
        StartCoroutine(Rotation(degree));
    }

    IEnumerator Rotation(float degree)
    {
        while (degree != 0)
        {
            if (degree > 0)
                rotaterHolder.Rotate();
            yield return null;
        }
        isMoving = false;
    }
}
