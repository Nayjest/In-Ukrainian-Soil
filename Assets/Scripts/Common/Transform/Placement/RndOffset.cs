using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RndOffset : MonoBehaviour
{
    public Vector3 Min = -Vector3.one;
    public Vector3 Max = Vector3.one;

    // Start is called before the first frame update
    void Start()
    {
        transform.position += new Vector3(
            Random.Range(Min.x, Max.x),
            Random.Range(Min.y, Max.y),
            Random.Range(Min.z, Max.z)
        );
    }
}
