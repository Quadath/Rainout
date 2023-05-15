using UnityEngine;

public class Tile
{
    public bool busy = false;

    public readonly float height;
    public GameObject building;

    public Tile(bool busy, float height)
    {
        this.busy = busy;
        this.height = height;
    }
}
