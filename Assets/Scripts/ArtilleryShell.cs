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

    private float flightDistance;
    

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
        float distance = 0;

        float targetAngle;
        float t = 0;
        Vector3 targetRotation;
        
        if (midpoint != Vector3.zero)
        {
            targetAngle = 85;
            distance = Vector3.Distance(cp, midpoint);
            t = Mathf.Pow((startDistance - distance) / (startDistance) + 0.05f, 3);
            targetRotation = new Vector3(0, 0, Mathf.Lerp(startAngle, targetAngle, t));
            transform.Translate(-Vector3.up * (speed * (1.25f - t) * Time.deltaTime));
        }
        else
        {
            float horizontalDiff = Vector3.Distance(new Vector3(cp.x, 0, cp.z), new Vector3(target.x, 0, target.z));
            distance = Vector3.Distance(cp, target);
            targetAngle = Mathf.Atan2(midpoint.y - cp.y, horizontalDiff) * Mathf.Rad2Deg + 90;
            t = (float)Math.Pow((startDistance - distance) / (startDistance), 1f / 3f);
            Debug.Log($"start: {startDistance}, distance: {distance}");
            targetRotation = new Vector3(0, 0, Mathf.Lerp(startAngle, targetAngle, t));
            transform.Translate(-Vector3.up * (speed * t * Time.deltaTime));
            
            if (distance < 2f) Destroy(gameObject);
        }


        Debug.Log($"startAngle: {startAngle}, targetAngle: {targetAngle}, t: {t}");
        transform.localEulerAngles = targetRotation;

        if (t >= 1)
        {
            midpoint = Vector3.zero;
            startAngle = 90;
            startDistance = Vector3.Distance(cp, target);
        }

        Debug.DrawRay(cp, transform.up, Color.yellow);
        
        // if (distance < 1) Destroy(this);
    }
}
