using UnityEngine;

public class Artillery : MonoBehaviour
{
    public Transform target;

    public float shellSpeed = 100;

    public GameObject shellPrefab;

    public Transform shootElement;

    private Vector3 midpoint;

    private float shootCooldown = 0;
    
    void Update()
    {
        Debug.DrawRay(transform.position, transform.right * 5);
        shootCooldown -= Time.deltaTime;
        
            var cp = transform.position;
            var tp = target.position;
            Vector3 diff = tp - cp;
            diff.Normalize();
            float rot_y = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg - 90;
            
            float horizontalDiff = Vector3.Distance(new Vector3(cp.x, 0, cp.z), new Vector3(tp.x, 0, tp.z));
            
            midpoint = new Vector3((cp.x + tp.x) / 2, tp.y > cp.y ? tp.y + 5: cp.y + 5, (cp.z + tp.z) / 2);
            float toMidpoint = Vector3.Distance(new Vector3(cp.x, 0, cp.z), new Vector3(midpoint.x, 0, midpoint.z));
            float rot_z = Mathf.Atan2(midpoint.y - cp.y, toMidpoint) * Mathf.Rad2Deg;
            rot_z = rot_z + (90 - rot_z) / 2;
            
            Vector3 angles = new(0, rot_y, rot_z);

            transform.localEulerAngles = angles;
            if (shootCooldown <= 0)
            {
                Instantiate(shellPrefab, shootElement.transform.position, Quaternion.Euler(0, rot_y, rot_z + 90))
                    .GetComponent<ArtilleryShell>().Init(midpoint, tp, shellSpeed, transform.position);
                shootCooldown = 1 + Random.Range(-0.1f, 0.1f);
            }
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(midpoint, Vector3.one);
    }
}
