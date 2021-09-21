using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;

public class CubeStateX : MonoBehaviour
{
    public bool visualizing;

    [SerializeField] CubeSolver solvingAlgo;
    [SerializeField] GameObject cube;

    Stopwatch timer;

    void Awake()
    {
        timer = new Stopwatch();
    }

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        Constants.SOLVED_STATE.CopyTo(solvingAlgo.cubeState, 0);
    }

    public void Solve()
    {
        if (visualizing)
        {
            cube.gameObject.SetActive(true);
            List<int> moves = solvingAlgo.Solve();
            //Apply move visualy
        }
        else
        {
            cube.gameObject.SetActive(false);
            timer.Start();
            solvingAlgo.Solve();
            timer.Stop();
            double elasped = timer.Elapsed.TotalSeconds;
            UnityEngine.Debug.Log("Duration : " + elasped);
        }
    }
}
