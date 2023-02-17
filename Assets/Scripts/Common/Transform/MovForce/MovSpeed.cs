using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovSpeed : MonoBehaviour
{
    [FormerlySerializedAs("Force")]
    public Vector3 Speed = Vector3.zero;    
    public float DampingSpeed = 2.5f;    

    // Update is called once per frame
    void Update()
    {
        transform.position += Speed * Time.deltaTime;
        Speed *= Mathf.Lerp(1, 0, Time.deltaTime * DampingSpeed);
    }
}
