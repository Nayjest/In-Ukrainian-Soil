using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RndScale : MonoBehaviour
{
    public Vector2 ScaleRange = new Vector2(1,1);
    void Start()
    {
        transform.localScale = Random.Range(ScaleRange.x, ScaleRange.y) * Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
