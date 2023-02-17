using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Vector3 Movement = Vector3.zero;
    public bool Randomize;
    [ShowIf("Randomize")]
    public Vector3 RndMin;

    [ShowIf("Randomize")]
    public Vector3 RndMax;
    public bool WroldSpace = true;
    // Update is called once per frame
    void Update()
    {
        if (WroldSpace)
        {
            transform.position = transform.position + Movement * Time.deltaTime;
        } else
        {
            transform.Translate(Time.deltaTime * Movement, Space.Self);
        }
    }

   

    void Awake()
    {
        if (Randomize)
        {
            Movement = new Vector3(
                Random.Range(RndMin.x, RndMax.x),
                Random.Range(RndMin.y, RndMax.y),
                Random.Range(RndMin.z, RndMax.z)
            );
        }
    }
}
