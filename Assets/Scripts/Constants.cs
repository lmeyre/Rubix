using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static readonly int[,] CUBIES_POSITIONS =
    {
        {  0,  1,  2,  3,  12,  13,  14,  15 },
        {  4,  7,  6,  5,  16,  17,  18,  19 },
        {  0,  9,  4,  8,  12,  15,  17,  16 },
        {  2, 10,  6, 11,  14,  13,  19,  18 },
        {  3, 11,  7,  9,  15,  14,  18,  17 },
        {  1,  8,  5, 10,  13,  12,  16,  19 },
    };

    public static readonly int[] SOLVED_STATE =
    {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11,
        12, 13, 14, 15, 16, 17, 18, 19,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0
    };

    public static readonly int[] PHASES_MOVES = { 0, 262143, 259263, 74943, 74898 };

    public static readonly int EDGES_COUNT = 12;
    public static readonly int CORNERS_COUNT = 8;
    public static readonly int FULL_STATE_SIZE = 40;

    public static readonly string VALID_INPUT_1 = "UDFBLR";
    public static readonly string VALID_INPUT_2 = "2'";
}
