using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public int xSize, zSize;

    private Tile[,] tiles;

    public GameObject tilePrefab;
    private void Start()
    {
        tiles = new Tile[xSize, zSize];
        for (int x = 0; x < xSize; x++)
        {
            for (int z = 0; z < zSize; z++)
            {
                tiles[x, z] = Instantiate(tilePrefab, new Vector3(x, 0, z), Quaternion.Euler(90, 0, 0))
                    .GetComponent<Tile>();
            }
        }
    }
}
