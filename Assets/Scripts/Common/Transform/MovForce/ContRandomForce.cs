using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class ContRandomForce : AbstractForceController
{
    public float MaxVal = 1;    
    
    // Update is called once per frame
    void Update()
    {
        Force += new Vector3(Random.Range(-1.0f,1.0f), Random.Range(-1.0f,1.0f), 0) * MaxVal * Time.deltaTime;
    }
}
