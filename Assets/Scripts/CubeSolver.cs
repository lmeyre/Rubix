using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class CubeSolver : MonoBehaviour
{
    public int[] cubeState;

    public List<int> Solve()
    {
        List<int> winningMoves = new List<int>();

        for (int phase = 1; phase < 5; phase++)
        {
            int[] currSubState = this.GetSubState(this.cubeState, phase, true);
            int[] goalSubState = this.GetSubState(Constants.SOLVED_STATE, phase);
            if (currSubState.SequenceEqual(goalSubState))
                continue;
            LinkedList<int> currentPhaseMoves = DoubleSearch(currSubState, goalSubState, phase);
            foreach (int currMove in currentPhaseMoves)
            {
                cubeState = ApplyMove(currMove, cubeState);
                winningMoves.Add(currMove);
            }
        }
        return winningMoves;
    }

    LinkedList<int> DoubleSearch(int[] currSubState, int[] goalSubState, int phase)
    {
        //la clef est un substate, car on mettant des fullstate, ca fait exploser le pc, en mettant des hashcode ca marche pas car 2 substate identique sont different via ID
        S_data tmpData;
        Queue<int[]> doubleWay = new Queue<int[]>();
        Dictionary<int[], S_data> data = new Dictionary<int[], S_data>();

        doubleWay.Enqueue(cubeState);
        doubleWay.Enqueue(Constants.SOLVED_STATE);

        tmpData = new S_data();
        tmpData.way = E_Way.Forward;
        data[currSubState] = tmpData;
        tmpData = new S_data();
        tmpData.way = E_Way.Backward;
        data[goalSubState] = tmpData;
        while (true)
        {
            int[] oldState = doubleWay.Dequeue();
            int[] oldID = this.GetSubState(oldState, phase);
            E_Way lastDirection = data[oldID].way;

            for (int move = 0; move < 18; move++) // 18 moves car ya 6 faces, qu'on peut faire aller d'un ou 2 cran clockwise ou counter, donc 6 x 4, mais faire avancer de 2 cran en clockwise ou counter clockwise revient au meme donc 6x3
            {
                if ((Constants.PHASES_MOVES[phase] & (1 << move)) > 0)
                {                               // ci dessus, si on est au move 2, ben on decale le "1" de 2 bit, comme ca il est en deuxieme (ou troisieme a voir) position, donc genre 0001 0000 veut dire qu'on a le 5eme move c'est tres simple
                    // generate a new state from the old state
                    int[] newState = this.ApplyMove(move, oldState);
                    int[] newID = this.GetSubState(newState, phase);
                    E_Way newDirection = E_Way.None;
                    if (data.ContainsKey(newID))
                        newDirection = data[newID].way;

                    if (newDirection != 0 && newDirection != lastDirection)
                    {
                        if (lastDirection == E_Way.Backward)
                        {
                            int[] tempID = newID;
                            newID = oldID;
                            oldID = tempID;
                            move = this.GetInverseMove(move);
                        }
                        LinkedList<int> currentPhaseMoves = new LinkedList<int>(); // virer la linked list ? a reflechir
                        currentPhaseMoves.AddFirst(move);
                        while (!oldID.SequenceEqual(currSubState))
                        {
                            currentPhaseMoves.AddFirst(data[oldID].lastMove);
                            oldID = data[oldID].parentNode;
                        }
                        while (!newID.SequenceEqual(goalSubState))
                        {
                            currentPhaseMoves.AddLast(this.GetInverseMove(data[newID].lastMove));
                            newID = data[newID].parentNode;
                        }
                        return currentPhaseMoves;
                    }
                    else if (newDirection == 0)
                    {
                        doubleWay.Enqueue(newState);

                        tmpData = new S_data();
                        tmpData.way = lastDirection;
                        tmpData.lastMove = move;
                        tmpData.parentNode = oldID;
                        data.Add(newID, tmpData);
                    }
                }
            }
        }
    }

    /* Move		int	-> inverse int
	 * U		0	-> 2
	 * U2		1	-> 1
	 * U'		2	-> 0
	 * D		3	-> 5
	 * D2		4	-> 4
	 * D'		5	-> 3
	 * F		6	-> 8
	 * F2		7	-> 7
	 * F'		8	-> 6
	 * B		9	-> 11
	 * B2		10	-> 10
	 * B'		11	-> 9
	 * L		12	-> 14
	 * L2		13	-> 13
	 * L'		14	-> 12
	 * R		15	-> 17
	 * R2		16	-> 16
	 * R'		17	-> 15
	 */
    int GetInverseMove(int move)
    {
        return move + 2 - 2 * (move % 3);
    }

    /* Get a subset of the current cube state representation relevant for the current phase */
    //en fait ce qui est bien c'est que comme on l'appelle en boucle quand on test les move
    //quand on essai de bien mettre les edge par exemple on copie que cette info et on manipule des donnees plus faible en boucle, en ignorant le reste
    int[] GetSubState(int[] state, int phase, bool test = false)
    {
        int[] subState;

        switch (phase)
        {
            // Phase 1: Edge orientations
            case 1:
                subState = new int[12];
                Array.Copy(state, 20, subState, 0, 12); // on copie juste la partie qui parle de l'edge orientation
                return subState;
            // Phase 2: Corner orientations, E slice edges
            case 2:
                subState = new int[8];
                Array.Copy(state, 31, subState, 0, 8); // copie juste la partie du goal / current state qui parle de l'orientation des corner
                if (test)
                {
                    Debug.Log("base array ;");
                    foreach (int val in subState)
                        Debug.Log(val);
                    Debug.Log("final array ;");
                }
                for (int e = 0; e < 12; e++)
                    subState[0] |= (state[e] / 8) << e; // A COMPRENDRE, ou ptet pas, on peut dire que c'etait dans la formule de maths et voila
                if (test)
                    foreach (int val in subState)
                        Debug.Log(val);
                return subState;
            // Phase 3: Edge slices M and S, corner tetrads, overall parity

            // ajout : 
            // The end goal of phase 3 is to get every square on all sides of the cube either correct or the opposite color. 
            // All squares on the Red side should be Red or Orange, all squares on the Blue side should be Blue or Green, etc. 
            // This can be done using only F, B, F^2, B^2, L^2, R^2, U^2, and D^2 moves, and can be done in 13 or fewer moves.
            case 3:
                subState = new int[3] { 0, 0, 0 };
                for (int e = 0; e < 12; e++)
                    subState[0] |= ((state[e] > 7) ? 2 : (state[e] & 1)) << (2 * e);
                for (int c = 0; c < 8; c++)
                    subState[1] |= ((state[c + 12] - 12) & 5) << (3 * c);
                for (int i = 12; i < 20; i++)
                    for (int j = i + 1; j < 20; j++)
                        subState[2] ^= Convert.ToInt32(state[i] > state[j]);
                return subState;
            // Phase 4: Everything
            //The fourth phase is to get the cube totally solved. 
            //This can be done with only U^2, D^2, L^2, R^2, F^2, and B^2 moves, with a maximum of 15 of them.
            case 4:
                subState = state;
                return subState;
            default:
                return null;
        }
    }

    //ptet refaire cette methode nous meme
    int[] ApplyMove(int move, int[] state)
    {
        int turns = move % 3 + 1;
        int face = move / 3;
        state = (int[])state.Clone();

        while (turns-- > 0)
        {
            int[] oldState = (int[])state.Clone();
            for (int i = 0; i < 8; i++)// un move influe 8 cubie d'une face (pas le centre)
            {
                // il a mit d'abord les edge en 0 1 2 3 puis les corner en 0 1 2 3 comme ca dans la boucle on separe les corner des edge, corner a i = 5
                int isCorner = Convert.ToInt32(i > 3);
                Debug.Log(i + " -> " + isCorner);
                int target = Constants.CUBIES_POSITIONS[face, i] + isCorner * 12;// la il trouve le corner, et jcrois qu'il rajoute 12 si corner (iscorner = 1), pour que le premier corner vaille 12, et pas 0, pour le differentier du premier edge (0)
                Debug.Log(target);                                      //car on traite le tableau qui est au meme format que GOAL state, donc 0-11 edge, puis a partir de 12 les corner commencent
                if (isCorner > 1)
                    Debug.LogError("notre analyze est pas bonne!");
                //ici on remplace par le i + 1, sauf pour 3 et 7 ou c'est i -3, vu que ca fait le tour du carre de 4
                int killer = Constants.CUBIES_POSITIONS[face, (i & 3) == 3 ? i - 3 : i + 1] + isCorner * 12; ;
                int orientationDelta = (i < 4) ? Convert.ToInt32(face > 1 && face < 4) :
                    (face < 2) ? 0 : 2 - (i & 1); // pour capter quel orientation change, pas besoin de comprendre de ouf forcement, ya 2 maniere pour corner ou edge
                state[target] = oldState[killer];
                //au dessus on remplace la position
                //ci dessous l'orientation
                state[target + 20] = oldState[killer + 20] + orientationDelta;
                if (turns == 0)// si on est sur la derniere rotation du move
                    state[target + 20] %= 2 + isCorner;//on remet les valeurs, genre ca va entre 0 et 2, a chque rotation on ajoute de la rotation, si on se retrouve a 4, c'est comme si on etait a 2, on le fait a la fin
            }
        }
        return state;
    }
}
