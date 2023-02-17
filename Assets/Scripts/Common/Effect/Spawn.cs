using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : AbstractEffect
{
    public GameObject Prefab;
    public override void Activate()
    {
        base.Activate();
        GameObject.Instantiate(Prefab, transform.position, transform.rotation);
        FinishEffect();
    }
}