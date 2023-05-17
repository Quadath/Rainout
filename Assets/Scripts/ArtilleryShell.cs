using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryShell : MonoBehaviour
{
    private Vector3 midpoint, target;
    private float speed = 0;

    private Vector3 startAngle;
    private float startDistance;

    private float yDelta;

    private bool perp = false;
    

    public void Init(Vector3 m, Vector3 t, float speed, Vector3 launcher)
    {
        midpoint = m;
        target = t;
        this.speed = speed;
        startAngle = transform.localEulerAngles;
        startDistance = Vector3.Distance(new Vector3(launcher.x, 0, launcher.z), new Vector3(target.x, 0, target.z));
        yDelta = Mathf.Sqrt(Mathf.Pow(target.y - transform.position.y, 2));
        
        Destroy(gameObject, 10);
    }

    private void Update()
    {
        var cp = transform.position;
        float distance = 0;

        float t = 0;
        Vector3 targetRotation;
        
        Vector3 diff = target - cp;
        diff.Normalize();
        float rot_y = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg - 90;
        
        distance = Vector3.Distance(new Vector3(cp.x, 0, cp.z), new Vector3(target.x, 0, target.z));
        t = (startDistance - distance) / (startDistance) + 0.1f;
        float targetAngle = Mathf.Atan2(target.y - cp.y, distance) * Mathf.Rad2Deg + 90f;
        
        targetRotation = new Vector3(0, rot_y, Mathf.Lerp(startAngle.z, Mathf.Lerp(startAngle.z, targetAngle, t), t));
        

        Debug.Log($"startAngle: {startDistance}, targetAngle: {distance}, t: {t}");
        transform.localEulerAngles = targetRotation;
        transform.Translate(-Vector3.up * (speed *  (Mathf.Sqrt((float)Math.Pow(t - 0.5, 2)) + 0.5f)) * Time.deltaTime);

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