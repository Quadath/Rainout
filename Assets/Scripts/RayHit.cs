using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayHit : MonoBehaviour
{
     void Update()
        {
            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    
                if (Physics.Raycast(ray, out hit))
                {
                    // Find the line from the gun to the point that was clicked.
                    Vector3 incomingVec = hit.point - transform.position;
    
                    // Use the point's normal to calculate the reflection vector.
                    Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);
    
                    // Draw lines to show the incoming "beam" and the reflection.
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    Debug.DrawRay(hit.point, reflectVec, Color.green);

                    if (hit.collider.GetComponent<TileProcessor>())
                    {
                        hit.collider.GetComponent<TileProcessor>().Cube(hit.point);
                    }
                }
            }
            RaycastHit hihglight;
            Ray light = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(light, out hihglight))
            {
                if (hihglight.collider.GetComponent<TileProcessor>())
                {
                    hihglight.collider.GetComponent<TileProcessor>().HighlightTile(hihglight.point);
                    if (Input.GetMouseButton(2))
                    {
                        hihglight.collider.GetComponent<TileProcessor>().ReallyCube(hihglight.point);
                    }
                }
            }
        }
}
