using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayHit : MonoBehaviour
{
     void Update()
        {
            // Debug.DrawLine(transform.position, transform.position + Camera.main.ScreenPointToRay(Input.mousePosition).direction * 10, Color.blue);
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
                // Debug.Log(hit.point);
                // Debug.Log(hit.normal);
                if (Input.GetKeyUp(KeyCode.Mouse0))
                    FindObjectOfType<TileProcessor>().CastClick(hit.point, hit.normal);
            }
        }
}
