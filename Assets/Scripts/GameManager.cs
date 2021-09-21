using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;

public class GameManager : MonoBehaviour
{
    public bool visualizing;

    [SerializeField] CubeSolver solvingAlgo;
    [SerializeField] GameObject cube;
    [SerializeField] GameObject colliderHolder;

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

    [ContextMenu("Solve")]
    public void Solve()
    {
        if (visualizing)
        {
            cube.SetActive(true);
            colliderHolder.SetActive(true);
            List<int> moves = solvingAlgo.Solve();
            //Apply move visualy
        }
        else
        {
            cube.SetActive(false);
            colliderHolder.SetActive(false);
            timer.Start();
            solvingAlgo.Solve();
            timer.Stop();
            double elasped = timer.Elapsed.TotalSeconds;
            UnityEngine.Debug.Log("Duration : " + elasped);
        }
    }
}
