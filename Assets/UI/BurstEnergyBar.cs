using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstEnergyBar : MonoBehaviour
{
    void Update()
    {
        transform.localScale = new Vector3(Player.Inst.BurstEnergyPercent, 1, 1);
    }
}
