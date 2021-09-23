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
        // Debug.Log("Initial CubeState :");
        // foreach (int i in cubeState)
        //     Debug.Log(i);
        for (int phase = 1; phase < 5; phase++)
        {
            int[] currSubState = helper.GetSubState(cubeState, phase);
            int[] solvedSubState = helper.GetSubState(Constants.SOLVED_STATE, phase);
            if (currSubState.SequenceEqual(solvedSubState))
            {
                //Debug.Log("EQUAL");
                continue;
            }
            LinkedList<int> currentPhaseMoves = DoubleSearch(currSubState, solvedSubState, phase);
            // Debug.Log("Phase " + phase + " moves :");
            // foreach (int i in currentPhaseMoves)
            //     Debug.Log(i);
            foreach (int currMove in currentPhaseMoves)
            {
                cubeState = MoveState(currMove, cubeState);
                winningMoves.Add(currMove);
            }
        }
        return winningMoves;
    }

    LinkedList<int> DoubleSearch(int[] currSubState, int[] solvedSubState, int phase)
    {
        //la clef est un substate, car on mettant des fullstate, ca fait exploser le pc, en mettant des hashcode ca marche pas car 2 substate identique sont different via ID
        S_data tmpData;
        Queue<int[]> doubleWay = new Queue<int[]>();
        Dictionary<int[], S_data> data = new Dictionary<int[], S_data>(new SurrogateEqualityKey());

        doubleWay.Enqueue(cubeState);
        doubleWay.Enqueue(Constants.SOLVED_STATE);

        tmpData = new S_data();
        tmpData.way = E_Way.Forward;
        data[currSubState] = tmpData;
        // Debug.Log("Curr1 :");
        // foreach (int i in currSubState)
        //     Debug.Log(i);
        tmpData = new S_data();
        tmpData.way = E_Way.Backward;
        data[solvedSubState] = tmpData;
        //int debug = 0;
        //Debug.Log("Curr and goal length -> " + currSubState.Length + " " + solvedSubState.Length);
        while (true)
        {
            // debug++;
            // if (debug == 10000)
            // {
            //     Debug.Log("BLOQUEEEEEEEEEEEEEE");
            //     return null;
            // }
            int[] oldState = doubleWay.Dequeue();
            //We needed to find a compatible key that we could re create from oldstate
            //That wasnt the old state or pc would explode
            int[] oldKey = helper.GetSubState(oldState, phase);
            // Debug.Log("Curr2 :");
            // for (int i = 0; i < oldKey.Length; i++)
            //     if (oldKey[i] != currSubState[i])
            //         Debug.Log("old = " + oldKey[i] + " and curr = " + currSubState[i] + " AT INDEX " + i); ;
            //Debug.Log("At first loop only, first is curr, length of it after it again :" + oldKey.Length);
            E_Way lastDirection = data[oldKey].way;

            for (int move = 0; move < 18; move++) // 18 moves car ya 6 faces, qu'on peut faire aller d'un ou 2 cran clockwise ou counter, donc 6 x 4, mais faire avancer de 2 cran en clockwise ou counter clockwise revient au meme donc 6x3
            {
                if ((Constants.PHASES_MOVES[phase] & (1 << move)) > 0)
                {                               // ci dessus, si on est au move 2, ben on decale le "1" de 2 bit, comme ca il est en deuxieme (ou troisieme a voir) position, donc genre 0001 0000 veut dire qu'on a le 5eme move c'est tres simple
                    // generate a new state from the old state
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
                        LinkedList<int> currentPhaseMoves = new LinkedList<int>(); // virer la linked list ? a reflechir
                        currentPhaseMoves.AddFirst(move);
                        while (!oldKey.SequenceEqual(currSubState))
                        {
                            currentPhaseMoves.AddFirst(data[oldKey].lastMove);
                            oldKey = data[oldKey].parentNode;
                        }
                        while (!newKey.SequenceEqual(solvedSubState))
                        {
                            currentPhaseMoves.AddLast(Tools.GetInverseMove(data[newKey].lastMove));
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
        //New / old to have a tmp buffer to not erase a value while we turn
        int[] newState = new int[Constants.FULL_STATE_SIZE];
        int[] oldState = new int[Constants.FULL_STATE_SIZE];
        int sideToRotate = move / 3;
        int rotations = move % 3;
        rotations++;//"0" mean 1 turn, up to 2 meaning 3
        origin.CopyTo(newState, 0);

        while (rotations-- > 0)
        {
            newState.CopyTo(oldState, 0);
            for (int i = 0; i < 8; i++)// un move influe 8 cubie d'une face (pas le centre)
            {
                // il a mit d'abord les edge en 0 1 2 3 puis les corner en 0 1 2 3 comme ca dans la boucle on separe les corner des edge, corner a i = 4
                //int isCorner = Convert.ToInt32(i > 3);
                int oldCubieValue = Constants.CUBIES_POSITIONS[sideToRotate, i];// + isCorner * 12;// la il trouve le corner, et jcrois qu'il rajoute 12 si corner (iscorner = 1), pour que le premier corner vaille 12, et pas 0, pour le differentier du premier edge (0)
                // Debug.Log(oldCubieValue);                                      //car on traite le tableau qui est au meme format que GOAL state, donc 0-11 edge, puis a partir de 12 les corner commencent
                // if (isCorner > 1)
                //     Debug.LogError("notre analyze est pas bonne!");
                //ici on remplace par le i + 1, sauf pour 3 et 7 ou c'est i -3, vu que ca fait le tour du carre de 4
                int newCubieValue = Constants.CUBIES_POSITIONS[sideToRotate, (i & 3) == 3 ? i - 3 : i + 1];// + isCorner * 12; ;
                int orientationDelta = (i < 4) ? Convert.ToInt32(sideToRotate > 1 && sideToRotate < 4) :
                    (sideToRotate < 2) ? 0 : 2 - (i & 1); // pour capter quel orientation change, pas besoin de comprendre de ouf forcement, ya 2 maniere pour corner ou edge
                newState[oldCubieValue] = oldState[newCubieValue];
                //Debug.Log("old = " + oldState[oldCubieValue] + " and new = " + newState[oldCubieValue]);
                //au dessus on remplace la position
                //ci dessous l'orientation
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
