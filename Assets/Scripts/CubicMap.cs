using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class CubicMap : MonoBehaviour
{
    private int xSize = 128;
    private int ySize = 128;
    private int zSize = 128;

    private Mesh mesh;

    private bool[,,] tiles;
    
    private Vector3[] vertices;
    private Color[] colors;
    private int[] triangles;

    private int[,] heights;

    private int vert, tris;

    private enum Planes
    {
        X,
        Y,
        Z
    }

    private void Start()
    {
        mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
        

        tiles = new bool[xSize, ySize, zSize];
;        
        for(int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < xSize; z++)
                {
                    if (x is > 2 and < 5)
                    {
                        if (y is > 2 and < 5)
                        {
                            // if (z is > 2 and < 5) tiles[x, y, z] = true;
                        }
                    }
                }
            }
        }
        CreateShape();
        UpdateMesh();
    }


    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (ySize + 1) * (zSize + 1)];
        triangles = new int[xSize * zSize * ySize * 6];
        colors = new Color[(xSize + 1) * (ySize + 1) * (zSize + 1)];

        heights = new int[xSize, zSize];

        int v = 0;
        for (int y = 0; y <= ySize; y++)
        {
            for (int z = 0; z <= xSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    vertices[v] = new Vector3(x, y, z);
                    colors[v] = Color.Lerp(Color.black, new Color(0.97f, 0.79f, 0.5f), (y - 10) / 9.5f);
                    v++;
                }
            }
        }

        for (int y = 0; y < ySize; y++)
        {
            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    if (y == 0)
                        heights[x, z] = (int) (Mathf.PerlinNoise(x * .04f, z * .04f) * 15f + 10);
                    if (y < heights[x, z]) tiles[x, y, z] = true;
                }
            }
        }

        for (int y = 0; y < ySize; y++)
        {
            UpdateMesh();
            for (int z = 0; z < xSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    ProcessTile(new Vector3Int(x, y, z));
                }
            }
        }
        
        // StartCoroutine(processing());
    }

    IEnumerator processing()
    {
        int x = 0;
        int y = 0;
        int z = 0;
        while (true)
        {
            ProcessTile(new Vector3Int(x, y, z));
            // Debug.Log("x: " + x + " y: " + y + " z: " + z);
            
            x++;
            if (x == xSize)
            {
                x = 0;
                z++;
            }

            if (z == zSize)
            {
                z = 0;
                y++;
            }

            if (y == ySize)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
    void ProcessTile(Vector3Int pos)
    {
        if (!tiles[pos.x, pos.y, pos.z]) return;
        
        if (pos.x == 0)
        {
            CreateFace(pos, Planes.X, false);
        }
        if (pos.y == 0)
        {
            CreateFace(pos, Planes.Y, false);
        }
        if (pos.z == 0)
        {
            CreateFace(pos, Planes.Z, false);
        }
        if (pos.x < xSize - 1 && !tiles[pos.x + 1, pos.y, pos.z]) //FORWARD
            CreateFace(pos + Vector3Int.right, Planes.X, true);
        if (pos.x > 0 && !tiles[pos.x - 1, pos.y, pos.z]) //BACK
            CreateFace(pos, Planes.X, false);
        if (pos.y < ySize - 1 && !tiles[pos.x, pos.y + 1, pos.z]) //UP
            CreateFace(pos + Vector3Int.up, Planes.Y, true);
        if (pos.y > 0 && !tiles[pos.x, pos.y - 1, pos.z]) //DOWN
            CreateFace(pos, Planes.Y, false);
        if(pos.z < zSize - 1 && !tiles[pos.x, pos.y, pos.z + 1]) //RIGHT
            CreateFace(pos + Vector3Int.forward, Planes.Z, true);
        if (pos.z > 0 && !tiles[pos.x, pos.y, pos.z - 1]) //LEFT
            CreateFace(pos, Planes.Z, false);
        
        if (pos.x == xSize - 1) //END X
            CreateFace(pos + Vector3Int.right, Planes.X, true);
        if (pos.y == ySize - 1) //END Y
            CreateFace(pos + Vector3Int.up, Planes.Y, true);
        if (pos.z == zSize - 1) //END Z
            CreateFace(pos + Vector3Int.forward, Planes.Z, true);
    }
    void CreateFace(Vector3Int p, Planes plane, bool inside)
    {
        int corner;
        switch (plane)
        {
            case Planes.X:
            {
                corner = p.y * (xSize + 1) * (zSize + 1) + p.z * (xSize + 1) + p.x;
                DefineTriangles(new []
                {
                    corner,
                    corner + xSize + 1,
                    corner + (xSize + 1) * (zSize + 1) + (xSize + 1),
                    corner + (xSize + 1) * (zSize + 1) + (xSize + 1),
                    corner + (xSize + 1) * (zSize + 1),
                    corner
                }, inside);
            } break;
            case Planes.Y:
            {
                corner = p.y * (xSize + 1) * (zSize + 1) + p.z * (xSize + 1) + p.x;
                DefineTriangles(new []
                {
                    corner,
                    corner + xSize + 2,
                    corner + xSize + 1,
                    corner,
                    corner + 1,
                    corner + xSize + 2
                }, inside);
            } break;
            case Planes.Z:
            {
                corner = p.y * (xSize + 1) * (zSize + 1) + p.z * (xSize + 1) + p.x;
                DefineTriangles(new []
                {
                    corner,
                    corner + (xSize + 1) * (zSize + 1),
                    corner + (xSize + 1) * (zSize + 1) + 1,
                    corner,
                    corner + (xSize + 1) * (zSize + 1) + 1,
                    corner + 1
                }, inside);
            } break;
        }
    }

    private void DefineTriangles(int[] tri, bool inside)
    {
        triangles[inside ? tris + 2 : tris] = tri[0];
        triangles[tris + 1] = tri[1];
        triangles[inside ? tris : tris + 2] = tri[2];
        triangles[inside ? tris + 5 : tris + 3] = tri[3];
        triangles[tris + 4] = tri[4];
        triangles[inside ? tris + 3 : tris + 5] = tri[5];
        
        tris += 6;
    }
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        // GetComponent<MeshCollider>().sharedMesh = mesh;
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        foreach (var vert in vertices)
        {
            Gizmos.DrawSphere(vert, 0.1f);
        }
    }
}
