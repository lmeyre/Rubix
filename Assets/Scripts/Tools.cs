using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static int GetInverseMove(int move)
    {
        return move + 2 - 2 * (move % 3);
    }

    public static int TranslateInputToMove(string input)
    {
        switch (input)
        {
            case "U":
                return 0;
            case "U2":
                return 1;
            case "U'":
                return 2;
            case "D":
                return 3;
            case "D2":
                return 4;
            case "D'":
                return 5;
            case "F":
                return 6;
            case "F2":
                return 7;
            case "F'":
                return 8;
            case "B":
                return 9;
            case "B2":
                return 10;
            case "B'":
                return 11;
            case "L":
                return 12;
            case "L2":
                return 13;
            case "L'":
                return 14;
            case "R":
                return 15;
            case "R2":
                return 16;
            case "R'":
                return 17;
            default:
                return -42;
        }
    }
}
