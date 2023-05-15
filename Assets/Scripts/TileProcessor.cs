using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TileProcessor : MonoBehaviour
{
    private readonly int xSize = 75;
    private readonly int zSize = 75;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    private Tile[,] tiles;


    public GameObject cube;

    public GameObject exactlyCube;

    private int direction;
    private GameObject cursor;


    private void Start()
    {
        tiles = new Tile[xSize, zSize];
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateMap();

        // StartCoroutine(spawn());
    }

    private IEnumerator spawn()
    {
        var xx = 1;
        while (xx < 65)
        {
            CreateBelt(0, xx, 10, cube);
            CreateBelt(2, xx, 12, cube);
            CreateBelt(1, 30, xx, cube);
            CreateBelt(3, 25, xx, cube);
            xx++;
            yield return new WaitForEndOfFrame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            direction++;
            if (direction > 3) direction = 0;
        }
    }

    public void Cube(Vector3 hit)
    {
        var x = (int) (hit.x + 0.5f);
        var y = (int) (hit.y + 0.5f);
        var z = (int) (hit.z + 0.5f);
        if (tiles[x, z].busy) return;

        var belt = CreateBelt(direction, x, z, cube);
        tiles[x, z].busy = true;

        {
            // Vector3[] quad = GetTileVertices(x, z);
            // Instantiate(cube, new Vector3(x, tiles[x, z].height + 0.5f, z), 
            //     Quaternion.Euler(length(0, x, 1), 90, 0));
            //
            // Debug.DrawLine(quad[0], quad[1], Color.white);
            // Debug.DrawLine(quad[0], quad[2], Color.white);
            // Debug.DrawLine(quad[1], quad[3], Color.white);
            // Debug.DrawLine(quad[2], quad[3], Color.white);
            // tiles[x, z].busy = true;
            // length(0, x, z);
        }
    }

    public void ReallyCube(Vector3 hit)
    {
        var x = (int) (hit.x + 0.5f);
        var y = (int) (hit.y + 0.5f);
        var z = (int) (hit.z + 0.5f);
        if (tiles[x, z].busy) return;

        Instantiate(exactlyCube, new Vector3(x, tiles[x, z].height + 0.5f, z), Quaternion.identity);
    }

    public void HighlightTile(Vector3 hit)
    {
        var x = (int) (hit.x + 0.5f);
        var y = (int) (hit.y + 0.5f);
        var z = (int) (hit.z + 0.5f);

        var quad = GetTileVertices(x, z);

        for (var i = 0; i < 4; i++) quad[i] += new Vector3(0, 0.1f, 0);

        var m = new Mesh();

        if (cursor == null)
        {
            cursor = new GameObject();
            cursor.AddComponent<MeshRenderer>();
            cursor.AddComponent<MeshFilter>();
        }

        m.vertices = quad;
        m.triangles = new[] {2, 1, 0, 1, 2, 3};
        cursor.GetComponent<MeshFilter>().mesh = m;
    }

    #region TILES

    private void GenerateMap()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        colors = new Color[(xSize + 1) * (zSize + 1)];
        tiles = new Tile[xSize, zSize];

        for (int i = 0, z = 0; z <= zSize; z++)
        for (var x = 0; x <= xSize; x++)
        {
            var y = Mathf.PerlinNoise(x * .03f, z * .03f) * 15;
            vertices[i] = new Vector3(x - 0.5f, y, z - 0.5f);
            colors[i] = Color.Lerp(Color.white, Color.black, y / 15);
            i++;
        }

        triangles = new int[xSize * zSize * 6];
        var vert = 0;
        var tris = 0;
        for (var z = 0; z < zSize; z++)
        {
            for (var x = 0; x < xSize; x++)
            {
                var height = CalculateTileHeight(GetTileVertices(x, z));
                tiles[x, z] = new Tile(false, height);

                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                vert++;
                tris += 6;
            }

            vert++;
        }

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        mesh.RecalculateNormals();
    }

    private Vector3[] GetTileVertices(int x, int z)
    {
        return new[]
        {
            vertices[(xSize + 1) * z + x],
            vertices[(xSize + 1) * z + x + 1],
            vertices[(xSize + 1) * (z + 1) + x],
            vertices[(xSize + 1) * (z + 1) + x + 1]
        };
    }

    private float CalculateTileHeight(Vector3[] vert)
    {
        return vert.Select(v => v.y).Prepend(0).Max();
    }

    public Tile GetTile(int x, int z)
    {
        return tiles[x, z];
    }

    #endregion

    #region TransportBelt

    private GameObject CreateBelt(int dir, int x, int z, GameObject prefab)
    {
        if (tiles[x, z].busy) return null;
        tiles[x, z].busy = true;
        float len = 1;
        float diff = 0;

        var position = Vector3.zero;
        var scale = Vector3.one;

        float a = 0;
        float b = 0;

        var points = CalculateLinkPoints(dir, x, z);
        a = points.x;
        b = points.y;

        len = Vector2.Distance(new Vector2(0, a), new Vector2(1, b));
        diff = a - b;
        position = new Vector3(x, (a + b) / 2 + 0.5f, z);
        scale = new Vector3(len, 1, 1);
        var angle = -Mathf.Asin(diff / len) * Mathf.Rad2Deg;

        var belt = Instantiate(prefab, position, Quaternion.Euler(0, 90 * dir, angle));
        belt.transform.localScale = scale;
        belt.name = "TransportBelt";
        belt.GetComponent<TransportBelt>().Redirect(dir);

        tiles[x, z].building = belt;

        var neighbors = GetNearBuildings(x, z);
        foreach (var neighbor in neighbors)
            if (neighbor && neighbor.name == "TransportBelt")
                RecalculateBeltRot(neighbor);
        return belt;
    }

    private void RecalculateBeltRot(GameObject belt)
    {
        var beltPos = belt.transform.position;

        var x = (int) beltPos.x;
        var z = (int) beltPos.z;

        float len = 1;
        float diff = 0;

        var position = Vector3.zero;
        var scale = Vector3.one;

        float a = 0;
        float b = 0;

        var dir = belt.GetComponent<TransportBelt>().direction;

        var points = CalculateLinkPoints(dir, x, z);
        a = points.x;
        b = points.y;

        len = Vector2.Distance(new Vector2(0, a), new Vector2(1, b));
        diff = a - b;
        position = new Vector3(x, (a + b) / 2 + 0.5f, z);
        scale = new Vector3(len, 1, 1);
        var angle = -Mathf.Asin(diff / len) * Mathf.Rad2Deg;

        belt.transform.position = position;
        belt.transform.localEulerAngles = new Vector3(0, 90 * dir, angle);
        belt.transform.localScale = scale;
    }

    private Vector2 CalculateLinkPoints(int dir, int x, int z)
    {
        var result = Vector2.zero;
        GameObject[] buildings =
        {
            tiles[x + 1, z].building,
            tiles[x, z - 1].building,
            tiles[x - 1, z].building,
            tiles[x, z + 1].building
        };

        int x1 = 0, x2 = 0, z1 = 0, z2 = 0;

        switch (dir)
        {
            case 0:
            {
                x1 = -1;
                x2 = 1;
            }
                break;
            case 1:
            {
                z1 = 1;
                z2 = -1;
            }
                break;
            case 2:
            {
                x1 = 1;
                x2 = -1;
            }
                break;
            case 3:
            {
                z1 = -1;
                z2 = 1;
            }
                break;
        }

        if (IsBeltLinkable(buildings[dir >= 2 ? dir - 2 : dir + 2]))
            result.x = (tiles[x, z].height + tiles[x + x1, z + z1].height) / 2;
        else
            result.x = tiles[x + x1, z + z1].height;

        if (IsBeltLinkable(buildings[dir]))
            result.y = (tiles[x, z].height + tiles[x + x2, z + z2].height) / 2;
        else
            result.y = tiles[x + x2, z + z2].height;
        return result;
    }

    private static bool IsBeltLinkable(GameObject building)
    {
        string[] names = {"TransportBelt"};

        if (!building) return false;

        return names.Contains(building.name);
    }

    #endregion

    private GameObject[] GetNearBuildings(int x, int z)
    {
        return new[]
        {
            tiles[x + 1, z].building,
            tiles[x, z - 1].building,
            tiles[x - 1, z].building,
            tiles[x, z + 1].building
        };
    }
    // private void OnDrawGizmos()
    //  {
    //      if (tiles == null)
    //      {
    //          return;
    //      }
    //
    //      for (int x = 0; x < xSize; x++)
    //      {
    //          for (int z = 0; z < zSize; z++)
    //          {
    //              Gizmos.DrawCube(new Vector3(x, tiles[x, z].height, z), new Vector3(1, 0.1f, 1));
    //          }
    //      }
    //  }
}
