using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AlsoMath 
{
    public static int DirectionToIndex(int d)
    {
        return d >= 2 ? d - 2 : d + 2;
    }

}
