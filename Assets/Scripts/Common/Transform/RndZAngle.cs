using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RndZAngle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Random.Range(0, 360));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
