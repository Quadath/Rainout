using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportBelt : MonoBehaviour
{
    public int direction;

    public BeltPoint[] left;
    public BeltPoint[] right;

    private TileProcessor processor;

    // private Vector3[,] points = new Vector3[,] {new Vector3()}

    private void Start()
    {
        processor = FindObjectOfType<TileProcessor>();
        GameObject c = GameObject.Find("I");
        if (c)
        {
            c.name = " ";
            Push(true, 0, c);
        }
    }
    public void Redirect(int direction)
    {
        this.direction = direction;
    }

    public void Push(bool leftSide, int point, GameObject item)
    {
        if (leftSide)
        {
            if (point >= left.Length)
            {
                int xn = 0, zn = 0;
        
                switch (direction)
                {
                    case 0: { xn = 1; } break;
                    case 1: { zn = -1; } break;
                    case 2: { xn = -1; } break;
                    case 3: { zn = 1; } break;
                }
                
                Vector2 pos = new Vector2(transform.position.x, transform.position.z);
                Tile tile = processor.GetTile((int) pos.x + xn, (int) pos.y + zn);
                if (tile.building)
                {
                    tile.building.GetComponent<TransportBelt>().Push(leftSide, 0, item);
                }
                return;
            }
            if (left[point].item) return;
            left[point].item = item.transform;
            item.transform.localEulerAngles = transform.localEulerAngles;        
            
            StartCoroutine(MoveItem(left[point]));
        }
    }

    public IEnumerator MoveItem(BeltPoint point)
    {
        Transform item = point.item;
        Vector3 target = point.transform.position;
        while (true)
        {
            if (item.position != target)
            {
                item.position = Vector3.MoveTowards(item.position, target, Time.deltaTime);
                if (Vector3.Distance(item.position, target) < 0.02f)
                {
                    Push(true, point.index + 1, item.gameObject);
                    point.item = null;
                    break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
    
    

    private void OnDrawGizmos()
    {
        if (left.Length > 0)
        {
            foreach (var beltPoint in left)
            {
                Gizmos.DrawWireCube(beltPoint.transform.position, Vector3.one * .2f);
            }
        }
        if (right.Length > 0)
        {
            foreach (var beltPoint in right)
            {
                Gizmos.DrawWireCube(beltPoint.transform.position, Vector3.one * .2f);
            }
        }
    }
}
