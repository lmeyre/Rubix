using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SolverHelper
{
    public int GetInverseMove(int move)
    {
        return move + 2 - 2 * (move % 3);
    }

    //en fait ce qui est bien c'est que comme on l'appelle en boucle quand on test les move
    //quand on essai de bien mettre les edge par exemple on copie que cette info et on manipule des donnees plus faible en boucle, en ignorant le reste
    public int[] GetSubState(int[] state, int phase)
    {
        int[] subState;

        switch (phase)
        {
            // Phase 1: Edge orientations
            case 1:
                subState = new int[Constants.EDGES_COUNT];
                Array.Copy(state, 20, subState, 0, Constants.EDGES_COUNT); // on copie juste la partie qui parle de l'edge orientation
                return subState;
            // Phase 2: Corner orientations, E slice edges
            case 2:
                subState = new int[Constants.CORNERS_COUNT];
                Array.Copy(state, 31, subState, 0, Constants.CORNERS_COUNT); // copie juste la partie du goal / current state qui parle de l'orientation des corner
                for (int e = 0; e < 12; e++)
                    subState[0] |= (state[e] / 8) << e; // A COMPRENDRE, ou ptet pas, on peut dire que c'etait dans la formule de maths et voila
                return subState;
            // Phase 3: Edge slices M and S, corner tetrads, overall parity

            // ajout : 
            // The end goal of phase 3 is to get every square on all sides of the cube either correct or the opposite color. 
            // All squares on the Red side should be Red or Orange, all squares on the Blue side should be Blue or Green, etc. 
            // This can be done using only F, B, F^2, B^2, L^2, R^2, U^2, and D^2 moves, and can be done in 13 or fewer moves.
            case 3:
                subState = new int[3] { 0, 0, 0 };
                for (int e = 0; e < 12; e++)//Edges
                    subState[0] |= ((state[e] > 7) ? 2 : (state[e] & 1)) << (2 * e);
                for (int c = 12; c < 20; c++)//Corners
                    subState[1] |= ((state[c] - 12) & 5) << (3 * c);
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
}
