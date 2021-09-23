using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveManager : MonoBehaviour
{
    [HideInInspector] public bool isMoving;
    [HideInInspector] public UnityEvent animationEvent;

    float movingSpeed = 300;

    [SerializeField] ColliderDetection up;
    [SerializeField] ColliderDetection down;
    [SerializeField] ColliderDetection front;
    [SerializeField] ColliderDetection back;
    [SerializeField] ColliderDetection left;
    [SerializeField] ColliderDetection right;

    [SerializeField] Transform upCubie, downCubie, frontCubie, backCubie, leftCubie, rightCubie; //Center cubies -> Rotator parents
    [SerializeField] Transform cube;

    void Awake()
    {
        animationEvent = new UnityEvent();
    }

    public void Move(int move, int speed = 300)
    {
        if (isMoving == true)
            Debug.LogError("PROBLEM");
        isMoving = true;
        movingSpeed = speed;
        switch (move)
        {
            //Up
            case 0:
                RotateFace(up, 90, Vector3.up, upCubie);
                break;
            case 1:
                RotateFace(up, 180, Vector3.up, upCubie);
                break;
            case 2:
                RotateFace(up, -90, Vector3.up, upCubie);
                break;
            //Down
            case 3:
                RotateFace(down, 90, -Vector3.up, downCubie);
                break;
            case 4:
                RotateFace(down, 180, -Vector3.up, downCubie);
                break;
            case 5:
                RotateFace(down, -90, -Vector3.up, downCubie);
                break;
            //Front
            case 6:
                RotateFace(front, 90, Vector3.right, frontCubie);
                break;
            case 7:
                RotateFace(front, 180, Vector3.right, frontCubie);
                break;
            case 8:
                RotateFace(front, -90, Vector3.right, frontCubie);
                break;
            //Back
            case 9:
                RotateFace(back, 90, Vector3.left, backCubie);
                break;
            case 10:
                RotateFace(back, 180, Vector3.left, backCubie);
                break;
            case 11:
                RotateFace(back, -90, Vector3.left, backCubie);
                break;
            //Left
            case 12:
                RotateFace(left, 90, -Vector3.forward, leftCubie);
                break;
            case 13:
                RotateFace(left, 180, -Vector3.forward, leftCubie);
                break;
            case 14:
                RotateFace(left, -90, -Vector3.forward, leftCubie);
                break;
            //Right
            case 15:
                RotateFace(right, 90, -Vector3.back, rightCubie);
                break;
            case 16:
                RotateFace(right, 180, -Vector3.back, rightCubie);
                break;
            case 17:
                RotateFace(right, -90, -Vector3.back, rightCubie);
                break;
            default:
                break;
        }
    }

    void RotateFace(ColliderDetection face, float angle, Vector3 axis, Transform rotater)
    {
        foreach (Transform child in rotater)
        {
            if (child.gameObject.name == "Cubie")//Small bruteforce for visual so it doesnt affect other graphical plane
                child.SetParent(cube);
        }
        foreach (GameObject cubies in face.GetCubies())
        {
            cubies.transform.SetParent(rotater);
        }
        StartCoroutine(Rotation(angle, axis, rotater));
    }

    [ContextMenu("TESTTT")]
    public void test()
    {
        RotateFace(up, -90, Vector3.up, upCubie);
    }

    IEnumerator Rotation(float angle, Vector3 axis, Transform rotater)
    {
        Quaternion origin = rotater.rotation;
        int way = (angle < 0 ? -1 : 1);
        float target = Mathf.Abs(angle);
        while (target > 0)
        {
            rotater.Rotate(axis * Time.deltaTime * movingSpeed * way, Space.Self);
            target -= Time.deltaTime * movingSpeed;
            yield return null;
        }
        rotater.rotation = origin;
        rotater.Rotate(axis * angle, Space.Self);
        isMoving = false;
        animationEvent?.Invoke();
    }
}
