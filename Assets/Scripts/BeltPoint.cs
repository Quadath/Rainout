using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BeltPoint: MonoBehaviour
{
    public int index;
    public Vector3 pos;
    public bool busy;
    public Transform item;
}
