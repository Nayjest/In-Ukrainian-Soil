using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickTo : MonoBehaviour
{
    public Transform Target;
    public void Update()
    {
        transform.position = Target.position;
        //transform.rotation = Target.rotation;
        //transform.localScale = Target.localScale;        
    }
}
