using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using NaughtyAttributes;

public class LookAt2D : MonoBehaviour
{
    public Transform Target;
    public float zOffset;
    public bool UseMaxRotationSpeed = false;

    [ShowIf("UseMaxRotationSpeed")]
    public float MaxRotationSpeed = 180;
    public void LookAtTarget()
    {
        if (Target == null) return;
        Vector2 diff = Target.position - transform.position;        
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        if (UseMaxRotationSpeed == false)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z + zOffset);
        } else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, rot_z + zOffset), Time.deltaTime * MaxRotationSpeed);
        }
    }
    // Update is called once per frame
    void Update()
    {
        LookAtTarget();
    }
}
