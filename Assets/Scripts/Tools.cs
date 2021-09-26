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

    public static string TranslateMoveToInput(List<int> moves)
    {
        string str = "";
        foreach (int i in moves)
        {
            switch (i)
            {
                case 0:
                    str += "U ";
                    break;
                case 1:
                    str += "U2 ";
                    break;
                case 2:
                    str += "U' ";
                    break;
                case 3:
                    str += "D ";
                    break;
                case 4:
                    str += "D2 ";
                    break;
                case 5:
                    str += "D' ";
                    break;
                case 6:
                    str += "F ";
                    break;
                case 7:
                    str += "F2 ";
                    break;
                case 8:
                    str += "F' ";
                    break;
                case 9:
                    str += "B ";
                    break;
                case 10:
                    str += "B2 ";
                    break;
                case 11:
                    str += "B' ";
                    break;
                case 12:
                    str += "L ";
                    break;
                case 13:
                    str += "L2 ";
                    break;
                case 14:
                    str += "L' ";
                    break;
                case 15:
                    str += "R ";
                    break;
                case 16:
                    str += "R2 ";
                    break;
                case 17:
                    str += "R' ";
                    break;
                default:
                    break;
            }
        }
        return str;
    }
}
