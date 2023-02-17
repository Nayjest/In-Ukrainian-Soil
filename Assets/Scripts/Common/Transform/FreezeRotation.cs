using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRotation : MonoBehaviour
{
    public Vector3 Eulers;
    private Quaternion Rotatin;

    private void Start()
    {  
            Rotatin = Quaternion.Euler(Eulers);
    }
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Rotatin;
    }
}
