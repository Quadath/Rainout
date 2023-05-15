using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubicMap : MonoBehaviour
{
    private int xSize = 8;
    private int ySize = 16;
    private int zSize = 8;

    private Mesh mesh;

    private bool[,,] tiles;
    
    private Vector3[] vertices;
    private Color[] colors;
    private int[] triangles;

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
        GetComponent<MeshFilter>().mesh = mesh;
        

        tiles = new bool[xSize, ySize, zSize];
;        
        for(int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                for (int z = 0; z < xSize; z++)
                {
                    if (x == 0) tiles[x, y, z] = true;
                    if (y == 0) tiles[x, y, z] = true;
                    if (z == 0) tiles[x, y, z] = true;
                }
            }
        }
        CreateShape();
        UpdateMesh();
    }


    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (ySize + 1) * (zSize + 1)];
        colors = new Color[(xSize + 1) * (ySize + 1) * (zSize + 1)];
        triangles = new int[xSize * zSize * ySize * 6];

        int v = 0;
        for (int y = 0; y <= ySize; y++)
        {
            for (int z = 0; z <= xSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    vertices[v] = new Vector3(x, y, z);
                    v++;
                }
            }
        }

        StartCoroutine(processing());

        // int vert = 0;
        // int tris = 0;
        // for (int z = 0; z < zSize; z++)
        // {
        //     for (int x = 0; x < xSize; x++)
        //     {
        //         triangles[tris + 0] = vert + 0;
        //         triangles[tris + 1] = vert + xSize + 1;
        //         triangles[tris + 2] = vert + 1;
        //         triangles[tris + 3] = vert + 1;
        //         triangles[tris + 4] = vert + xSize + 1;
        //         triangles[tris + 5] = vert + xSize + 2;
        //         vert++;
        //         tris += 6;
        //     }
        //     vert++;
        // }
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

            yield return new WaitForSeconds(0.25f);
        }
    }
    void ProcessTile(Vector3Int pos)
    {
        if (!tiles[pos.x, pos.y, pos.z]) return;
        if (pos.x == 0)
        {
            CreateFace(pos, Planes.X, true);
        }
        if (pos.y == 0)
        {
            CreateFace(pos, Planes.Y, false);
        }

        if (pos.z == 0)
        {
            CreateFace(pos, Planes.Z, false);
        }
    }
    void CreateFace(Vector3Int p, Planes plane, bool inside)
    {
        int corner = 0;
        switch (plane)
        {
            case Planes.X:
            {
                corner = p.y * (xSize + 1) * (zSize + 1) + p.z * (xSize + 1);
                
                triangles[inside ? tris + 2 : tris] = corner;
                triangles[tris + 1] = corner + xSize + 1;
                triangles[inside? tris : tris + 2] = corner + (xSize + 1) * (zSize + 1) + (xSize + 1);
                triangles[inside ? tris + 5 : tris + 3] = corner + (xSize + 1) * (zSize + 1) + (xSize + 1);
                triangles[tris + 4] = corner + (xSize + 1) * (zSize + 1);
                triangles[inside? tris + 3 : tris + 5] = corner;

                tris += 6;
            } break;
            case Planes.Y:
            {
                corner = p.z * (xSize + 1) + p.x;
                
                triangles[tris] = corner;
                triangles[tris + 1] = corner + xSize + 1;
                triangles[tris + 2] = corner + xSize + 2;
                triangles[tris + 3] = corner + xSize + 2;
                triangles[tris + 4] = corner + 1;
                triangles[tris + 5] = corner;

                tris += 6;
            } break;
            case Planes.Z:
            {
                corner = p.y * (xSize + 1) * (zSize + 1) + p.x;
                
                triangles[tris] = corner;
                triangles[tris + 1] = corner + (xSize + 1) * (zSize + 1);
                triangles[tris + 2] = corner + (xSize + 1) * (zSize + 1) + 1;
                triangles[tris + 3] = corner;
                triangles[tris + 4] = corner + (xSize + 1) * (zSize + 1) + 1;
                triangles[tris + 5] = corner + 1;

                tris += 6;
            } break;
        }
        UpdateMesh();
    }
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        // mesh.colors = colors;
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
