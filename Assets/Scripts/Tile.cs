using UnityEngine;

public class Tile
{
    private GameObject building;
    private Vector3Int pos;

    private Block block;

    public Tile(Vector3Int pos, Block block)
    {
        this.pos = pos;
        this.block = block;
    }

    public bool IsSolid()
    {
        bool result = block.type != Constants.Blocks.Air;

        return result;
    }
}
