using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class CubeSolver : MonoBehaviour
{
    [HideInInspector] public int[] cubeState;

    SolverHelper helper;

    void Awake()
    {
        helper = new SolverHelper();
        cubeState = new int[Constants.FULL_STATE_SIZE];
    }

    public List<int> Solve()
    {
        List<int> winningMoves = new List<int>();
        for (int phase = 1; phase < 5; phase++)
        {
            int[] currSubState = helper.GetSubState(cubeState, phase);
            int[] solvedSubState = helper.GetSubState(Constants.SOLVED_STATE, phase);
            if (currSubState.SequenceEqual(solvedSubState))
                continue;
            List<int> currentPhaseMoves = DoubleSearch(currSubState, solvedSubState, phase);
            foreach (int currMove in currentPhaseMoves)
            {
                cubeState = MoveState(currMove, cubeState);
                winningMoves.Add(currMove);
            }
        }
        return winningMoves;
    }

    List<int> DoubleSearch(int[] currSubState, int[] solvedSubState, int phase)
    {
        S_data tmpData;
        Queue<int[]> doubleWay = new Queue<int[]>();
        Dictionary<int[], S_data> data = new Dictionary<int[], S_data>(new SurrogateEqualityKey());

        doubleWay.Enqueue(cubeState);
        doubleWay.Enqueue(Constants.SOLVED_STATE);

        tmpData = new S_data();
        tmpData.way = E_Way.Forward;
        data[currSubState] = tmpData;
        tmpData = new S_data();
        tmpData.way = E_Way.Backward;
        data[solvedSubState] = tmpData;
        while (true)
        {
            int[] oldState = doubleWay.Dequeue();
            //We needed to find a compatible key that we could re create from oldstate
            //That wasnt the old state or pc would explode
            int[] oldKey = helper.GetSubState(oldState, phase);
            E_Way lastDirection = data[oldKey].way;

            for (int move = 0; move < 18; move++)
            {
                if ((Constants.PHASES_MOVES[phase] & (1 << move)) > 0)
                {
                    int[] newState = MoveState(move, oldState);
                    int[] newKey = helper.GetSubState(newState, phase);
                    E_Way newDirection = E_Way.None;
                    if (data.ContainsKey(newKey))
                        newDirection = data[newKey].way;
                    if (newDirection != 0 && newDirection != lastDirection)
                    {
                        if (lastDirection == E_Way.Backward)
                        {
                            int[] tempKey = newKey;
                            newKey = oldKey;
                            oldKey = tempKey;
                            move = Tools.GetInverseMove(move);
                        }
                        List<int> currentPhaseMoves = new List<int>();
                        currentPhaseMoves.Insert(0, move);
                        while (!oldKey.SequenceEqual(currSubState))
                        {
                            currentPhaseMoves.Insert(0, data[oldKey].lastMove);
                            oldKey = data[oldKey].parentNode;
                        }
                        while (!newKey.SequenceEqual(solvedSubState))
                        {
                            currentPhaseMoves.Insert(currentPhaseMoves.Count, Tools.GetInverseMove(data[newKey].lastMove));
                            newKey = data[newKey].parentNode;
                        }
                        return currentPhaseMoves;
                    }
                    else if (newDirection == 0)
                    {
                        doubleWay.Enqueue(newState);

                        tmpData = new S_data();
                        tmpData.way = lastDirection;
                        tmpData.lastMove = move;
                        tmpData.parentNode = oldKey;
                        data.Add(newKey, tmpData);
                    }
                }
            }
        }
    }

    public int[] MoveState(int move, int[] origin)
    {
        int[] newState = new int[Constants.FULL_STATE_SIZE];
        int[] oldState = new int[Constants.FULL_STATE_SIZE];
        int sideToRotate = move / 3;
        int rotations = move % 3;
        rotations++;//"0" mean 1 turn, up to 2 meaning 3
        origin.CopyTo(newState, 0);

        while (rotations-- > 0)
        {
            newState.CopyTo(oldState, 0);
            for (int i = 0; i < 8; i++)
            {
                int oldCubieValue = Constants.CUBIES_POSITIONS[sideToRotate, i];
                int newCubieValue = Constants.CUBIES_POSITIONS[sideToRotate, (i & 3) == 3 ? i - 3 : i + 1];
                int orientationDelta = (i < 4) ? Convert.ToInt32(sideToRotate > 1 && sideToRotate < 4) :
                    (sideToRotate < 2) ? 0 : 2 - (i & 1);
                newState[oldCubieValue] = oldState[newCubieValue];
                newState[oldCubieValue + 20] = oldState[newCubieValue + 20] + orientationDelta;
                //If we are on the last rotation, we set back the value to a correct rotation (0-2 for edges / 0-3 for corners)
                //Because we might have rotated like 6 time, making it like 2 rotation (1)
                if (rotations == 0)
                {
                    int angleRotation = (i > 3 ? 1 : 0);
                    newState[oldCubieValue + 20] %= 2 + angleRotation;
                }
            }
        }
        return newState;
    }
}