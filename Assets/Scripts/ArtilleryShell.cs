using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryShell : MonoBehaviour
{
    private Vector3 midpoint, target;
    private float speed = 0;

    private float startAngle;
    private float startDistance;
    

    public void Init(Vector3 m, Vector3 t, float speed)
    {
        midpoint = m;
        target = t;
        this.speed = speed;
        startAngle = transform.localEulerAngles.z;
        startDistance = Vector3.Distance(transform.position, midpoint);
        
    }

    private void Update()
    {
        var cp = transform.position;
        float distance = Vector3.Distance(cp, midpoint);

        float t = (startDistance - distance) / (startDistance) + 0.05f;
        if (t < 0) t = 0;

        float horizontalDiff = Vector3.Distance(new Vector3(cp.x, 0, cp.z), new Vector3(midpoint.x, 0, midpoint.z));

        float targetAngle = Mathf.Atan2(midpoint.y - cp.y, horizontalDiff) * Mathf.Rad2Deg + 90;
        targetAngle = 90;
        Debug.Log($"startAngle: {startAngle}, targetAngle: {targetAngle}, t: {Mathf.Pow(t, 4)}");
        transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(startAngle, targetAngle, Mathf.Pow(t, 4)));

        if (t >= 1)
        {
            
        }

        Debug.DrawRay(cp, transform.up, Color.yellow);
        
        transform.Translate(-Vector3.up * (speed * Time.deltaTime));
        // if (distance < 1) Destroy(this);
    }
}
