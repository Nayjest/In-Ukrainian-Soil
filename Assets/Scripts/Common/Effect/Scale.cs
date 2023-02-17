using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : AbstractEffect
{
    public float ScalePerSecond = 1.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Update()
    {
        if (!Active)
        {
            return;
        }
        transform.localScale *= Mathf.Lerp(1, ScalePerSecond, Time.deltaTime);
    }
}
