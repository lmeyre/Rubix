using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static int GetInverseMove(int move)
    {
        return move + 2 - 2 * (move % 3);
    }
}
