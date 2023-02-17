using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class RndMovDir : MonoBehaviour
{
    public Vector2 MinMaxForce = new Vector2(1, 5);

    public Vector2 MinMaxAngle = new Vector2(0,360);

    public bool AddTransformAngle = false;

    void Start()
    {        
        var a = Mathf.Deg2Rad * (Random.Range(MinMaxAngle.x, MinMaxAngle.y) + (AddTransformAngle ? transform.rotation.eulerAngles.z : 0));
        var force = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * MinMaxForce.RandomRange();
        GetComponent<MovSpeed>().Speed = force;
    }
}
