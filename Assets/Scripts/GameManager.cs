using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;

public class GameManager : MonoBehaviour
{
    public bool visualizing;

    [SerializeField] CubeSolver solvingAlgo;
    [SerializeField] CubeRotator rotator;
    [SerializeField] MoveManager moveManager;
    [SerializeField] GameObject cube;
    [SerializeField] GameObject colliderHolder;

    Stopwatch timer;
    List<int> movesSequence;
    bool cascading;

    void Awake()
    {
        timer = new Stopwatch();
        movesSequence = new List<int>();
    }

    void Start()
    {
        moveManager.animationEvent.AddListener(TriggerNextMove);
        Reset();
    }

    public void Reset()
    {
        if (cascading)
            return;
        Constants.SOLVED_STATE.CopyTo(solvingAlgo.cubeState, 0);
        rotator.Reset();
    }

    public void Solve()
    {
        if (cascading)
            return;
        if (visualizing)
        {
            cube.SetActive(true);
            colliderHolder.SetActive(true);
            timer.Start();
            movesSequence = solvingAlgo.Solve();
            timer.Stop();
            UnityEngine.Debug.Log("Duration (With graphics) : " + timer.Elapsed.TotalSeconds);
            UnityEngine.Debug.Log("Moves Count : " + movesSequence.Count);
            string sequence = GetStringSequence(movesSequence);
            UnityEngine.Debug.Log("Sequence : " + sequence);
            TriggerNextMove();
        }
        else
        {
            cube.SetActive(false);
            colliderHolder.SetActive(false);
            timer.Start();
            solvingAlgo.Solve();
            timer.Stop();
            UnityEngine.Debug.Log("Duration : " + timer.Elapsed.TotalSeconds);
        }
    }

    //Affect visual AND logic
    public void ApplyInput()
    {
        if (cascading)
            return;
        movesSequence.Clear();
        string[] parts = ArgumentEmulator.ARGUMENT.Split(' ');
        foreach (string s in parts)
        {
            int move = Tools.TranslateInputToMove(s);
            if (move == -42)
            {
                UnityEngine.Debug.LogError("Wrong input : |" + s + "|");
                return;
            }
            solvingAlgo.cubeState = solvingAlgo.MoveState(move, solvingAlgo.cubeState);
            movesSequence.Add(move);
        }
        TriggerNextMove();
    }

    //Only visual
    void TriggerNextMove()
    {
        cascading = true;
        if (movesSequence.Count == 0)
        {
            cascading = false;
            return;
        }
        moveManager.Move(movesSequence[0], 300);
        movesSequence.RemoveAt(0);
    }

    string GetStringSequence(List<int> moves)
    {
        string str = "";
        foreach (int i in moves)
        {
            str += (i.ToString() + " ");
        }
        return str;
    }
}
