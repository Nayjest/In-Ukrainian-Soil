using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePowerBar : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(Player.Inst.LifePower, 1, 1);
    }
}
