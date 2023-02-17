using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MvLerpToTarget : MonoBehaviour
{
    public Transform Target;
    public float LerpSpeed = 1.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null) return;
        transform.position = Vector3.Lerp(transform.position, Target.position, Time.deltaTime * LerpSpeed);
    }
}
