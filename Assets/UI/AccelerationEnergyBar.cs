using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationEnergyBar : MonoBehaviour
{
   

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(Player.Inst.AccelerationEnergyPercent, 1, 1);
    }
}
