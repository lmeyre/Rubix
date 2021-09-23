using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SolverHelper
{
    public int[] GetSubState(int[] state, int phase)
    {
        int[] subState;

        switch (phase)
        {
            case 1:
                subState = new int[Constants.EDGES_COUNT];
                Array.Copy(state, 20, subState, 0, Constants.EDGES_COUNT);
                return subState;
            case 2:
                subState = new int[Constants.CORNERS_COUNT];
                Array.Copy(state, 31, subState, 0, Constants.CORNERS_COUNT);
                for (int e = 0; e < 12; e++)
                    subState[0] |= (state[e] / 8) << e;
                return subState;
            // The end goal of phase 3 is to get every square on all sides of the cube either correct or the opposite color. 
            // All squares on the Red side should be Red or Orange, all squares on the Blue side should be Blue or Green, etc. 
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
            case 4:
                subState = state;
                return subState;
            default:
                return null;
        }
    }
}

public class SurrogateEqualityKey : IEqualityComparer<int[]>
{
    public bool Equals(int[] x, int[] y)
    {
        return x.SequenceEqual(y);
    }

    public int GetHashCode(int[] obj)
    {
        //Set value above the max value we can find in the array, multiply it by more than the max value we can find
        int val = 30;
        for (int i = 0; i < obj.Length; i++)
        {
            unchecked { val = val * 25 + obj[i]; }
        }
        return val;
    }
}
