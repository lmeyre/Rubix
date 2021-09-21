using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotator : MonoBehaviour
{
    float cubeTurningSpeed = 140;
    Quaternion target;
    bool isRotating = false;
    MoveManager moveManager;


    void Awake()
    {
        moveManager = FindObjectOfType<MoveManager>();
        target = transform.rotation;
    }

    void Update()
    {
        RotateViewAngle();//For visibility of differents side
        RotateFaces();//For rotating part of the cube
    }

    void RotateFaces()
    {
        if (moveManager.isMoving)
            return;
        bool revert = false;
        if (Input.GetKey(KeyCode.LeftShift))
            revert = true;
        if (Input.GetKeyDown(KeyCode.U))
        {
            int move = (revert == true ? Tools.GetInverseMove(0) : 0);
            moveManager.Move(move);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            int move = (revert == true ? Tools.GetInverseMove(3) : 3);
            moveManager.Move(move);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            int move = (revert == true ? Tools.GetInverseMove(6) : 6);
            moveManager.Move(move);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            int move = (revert == true ? Tools.GetInverseMove(9) : 9);
            moveManager.Move(move);
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            int move = (revert == true ? Tools.GetInverseMove(12) : 12);
            moveManager.Move(move);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            int move = (revert == true ? Tools.GetInverseMove(15) : 15);
            moveManager.Move(move);
        }
    }

    void RotateViewAngle()
    {
        FollowTarget();
        if (!isRotating)
            CubeRotation();
    }

    void FollowTarget()
    {
        if (transform.rotation != target)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, cubeTurningSpeed * Time.deltaTime);
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