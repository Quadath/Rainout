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

    private float yDelta;

    private bool perp = false;
    

    public void Init(Vector3 m, Vector3 t, float speed, Vector3 launcher)
    {
        midpoint = m;
        target = t;
        this.speed = speed;
        startAngle = transform.localEulerAngles.z;
        startDistance = Vector3.Distance(new Vector3(launcher.x, 0, launcher.z), new Vector3(target.x, 0, target.z));
        yDelta = Mathf.Sqrt(Mathf.Pow(target.y - transform.position.y, 2));
    }

    private void Update()
    {
        var cp = transform.position;
        float distance = 0;

        float t = 0;
        Vector3 targetRotation;
        
        // if (midpoint != Vector3.zero)
        // {
        //     targetAngle = 85;
        //     distance = Vector3.Distance(cp, midpoint);
        //     t = Mathf.Pow((startDistance - distance) / (startDistance) + 0.05f, 3);
        //     targetRotation = new Vector3(0, 0, Mathf.Lerp(startAngle, targetAngle, t));
        //     transform.Translate(-Vector3.up * (speed * (1.25f - t) * Time.deltaTime));
        // }
        // else
        // {
        //     float horizontalDiff = Vector3.Distance(new Vector3(cp.x, 0, cp.z), new Vector3(target.x, 0, target.z));
        //     distance = Vector3.Distance(cp, target);
        //     targetAngle = Mathf.Atan2(midpoint.y - cp.y, horizontalDiff) * Mathf.Rad2Deg + 90;
        //     t = (float)Math.Pow((startDistance - distance) / (startDistance), 1f / 3f);
        //     Debug.Log($"start: {startDistance}, distance: {distance}");
        //     targetRotation = new Vector3(0, 0, Mathf.Lerp(startAngle, targetAngle, t));
        //     
        //     if (distance < 2f) Destroy(gameObject);
        // }

        distance = Vector3.Distance(new Vector3(cp.x, 0, cp.z), new Vector3(target.x, 0, target.z));
        t = (startDistance - distance) / (startDistance) + 0.1f;
        float targetAngle = Mathf.Atan2(target.y - midpoint.y, distance) * Mathf.Rad2Deg + 90f;
        
        targetRotation = new Vector3(0, 0, Mathf.Lerp(startAngle, Mathf.Lerp(startAngle, targetAngle, t), t));
        

        Debug.Log($"startAngle: {startDistance}, targetAngle: {distance}, t: {t}");
        transform.localEulerAngles = targetRotation;
        transform.Translate(-Vector3.up * (speed * (2f - t) * Time.deltaTime));

        // if (t >= 1)
        // {
        //     midpoint = Vector3.zero;
        //     startAngle = 90;
        //     startDistance = Vector3.Distance(cp, target);
        // }

        if (Vector3.Distance(cp, target) < 2)
        {
            var rb = GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.AddForce(-transform.up * speed, ForceMode.Impulse);
            Destroy(this);
        }

        Debug.DrawRay(cp, transform.up, Color.yellow);
    }
}
