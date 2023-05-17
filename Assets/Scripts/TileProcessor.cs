using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class TileProcessor : MonoBehaviour
{
    private int xSize = 64;
    private int ySize = 64;
    private int zSize = 64;

    private Mesh mesh;

    private Tile[,,] tiles;
    
    private Vector3[] vertices;
    private Color[] colors;
    private int[] triangles;

    private int[,] heights;

    private int vert, tris;
    private Chunk[,,] chunks;
    private Vector3Int chunkSize = new(16, 16, 16);

    public GameObject chunkPrefab;

    private enum Planes
    {
        X,
        Y,
        Z
    }

    private void Start()
    {
        tiles = new Tile[xSize, ySize, zSize];
        heights = new int[xSize, zSize];
        for (int y = 0; y < ySize; y++)
        {
            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    if (y == 0)
                        heights[x, z] = (int) (Mathf.PerlinNoise(x * .02f, z * .02f) * 35 + 10);
                        // heights[x,z] = 8;
                    if (y < heights[x, z])
                        tiles[x, y, z] = new Tile(new Vector3Int(x, y, z),
                            new Block(Constants.Blocks.Sandstone, 900));
                    else 
                        tiles[x, y, z] = new Tile(new Vector3Int(x, y, z),
                            new Block(Constants.Blocks.Air, 0));
                }
            }
        }

        chunks = new Chunk[xSize / chunkSize.x, ySize / chunkSize.y,zSize / chunkSize.z];
        for (int y = 0; y < chunks.GetLength(1); y++)
        {
            for (int z = 0; z < chunks.GetLength(2); z++)
            {
                for (int x = 0; x < chunks.GetLength(0); x++)
                {
                    Chunk ch = Instantiate(chunkPrefab, transform).GetComponent<Chunk>();
                    ch.InitChunk(new Vector3Int(x, y, z), chunkSize, new Vector3Int(xSize, ySize, zSize));
                    chunks[x, y, z] = ch;
                }
            }
        }
    }
    public Vector3Int CastClick(Vector3 clickPoint, Vector3 normal, string operation)
    {
        // Debug.Log("clicked: " + clickPoint + "   normal: " + normal);
        normal *= -1;
        Vector3Int targetedBlock = new Vector3Int(
            (int) Math.Floor(normal.x > 0 ? clickPoint.x : clickPoint.x + normal.x),
            (int) Math.Floor(normal.y > 0 ? clickPoint.y : clickPoint.y + normal.y),
            (int) Math.Floor(normal.z > 0 ? clickPoint.z : clickPoint.z + normal.z)
        );
        // Debug.Log("target: " + targetedBlock);
        
        if (operation == "destroy") DestroyBlock(targetedBlock);
        if (operation == "place") PlaceBlock(new Vector3Int(targetedBlock.x - (int)normal.x, targetedBlock.y - (int)normal.y, targetedBlock.z - (int)normal.z));
        
        // Instantiate(cube, Vector3.one * 0.5f + targetedBlock, Quaternion.identity);
        return targetedBlock;
    }

    public Tile[,,] GetAllTiles()
    {
        return tiles;
    }

    public Chunk GetChunkByNumber(Vector3Int number)
    {
        if (number.x > chunks.GetLength(0) || number.x < 0) Debug.LogError("GetChunkByNumber - out of range. X");
        if (number.y > chunks.GetLength(1) || number.y < 0) Debug.LogError("GetChunkByNumber - out of range. Y");
        if (number.z > chunks.GetLength(2) || number.z < 0) Debug.LogError("GetChunkByNumber - out of range. Z");

        return chunks[number.x, number.y, number.z];
    }

    public void PlaceBlock(Vector3Int pos)
    {
        if (pos.x >= xSize || pos.y >= ySize || pos.z >= zSize)
        {
            Debug.LogWarning("Cannot place block outside a world.");
            return;
        }

        if (pos.x < 0 || pos.y < 0 || pos.z < 0)
        {
            Debug.LogWarning("Cannot place block outside a world.");
            return;
        }
        
        tiles[pos.x, pos.y, pos.z].PutBlock(new Block(Constants.Blocks.Sandstone, 900));
        // Debug.Log("X: " + (int)Math.Floor((float)pos.x / chunkSize.x) + " Y: " + (int)Math.Floor((float)pos.y / chunkSize.y) + " Z: " + (int)Math.Floor((float)pos.z / chunkSize.z));
        chunks[(int)Math.Floor((float)pos.x / chunkSize.x), (int)Math.Floor((float)pos.y / chunkSize.y), (int)Math.Floor((float)pos.z / chunkSize.z)].StateChange();
    }

    public void DestroyBlock(Vector3Int pos)
    {
        tiles[pos.x, pos.y, pos.z].Destroy();
        // Debug.Log("X: " + (int)Math.Floor((float)pos.x / chunkSize.x) + " Y: " + (int)Math.Floor((float)pos.y / chunkSize.y) + " Z: " + (int)Math.Floor((float)pos.z / chunkSize.z));
        chunks[(int)Math.Floor((float)pos.x / chunkSize.x), (int)Math.Floor((float)pos.y / chunkSize.y), (int)Math.Floor((float)pos.z / chunkSize.z)].StateChange();
    }
    private void OnDrawGizmos()
    {
        // for (int y = 0; y < ySize; y++)
        // {
        //     for (int z = 0; z < xSize; z++)
        //     {
        //         for (int x = 0; x < xSize; x++)
        //         {
        //             if (tiles[x, y, z].IsSolid())
        //             Gizmos.DrawWireSphere(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), 0.1f);
        //         }
        //     }
        // }
    }
}
