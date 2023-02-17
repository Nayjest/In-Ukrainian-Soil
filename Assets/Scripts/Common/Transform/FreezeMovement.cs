using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeMovement : MonoBehaviour
{
    public Vector3 Pos;
    public bool FreezeX = false;
    public bool FreezeY = false;
    public bool FreezeZ = false;
    
    // Update is called once per frame
    void LateUpdate()
    {        
        transform.position = new Vector3(
            FreezeX ? Pos.x : transform.position.x,
            FreezeY ? Pos.y : transform.position.y,
            FreezeZ ? Pos.z : transform.position.z
        );
    }
}
