using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleDebugHelper : MonoBehaviour
{

    [Button]
    void Set0()
    {
        Time.timeScale = 0;
    }

    [Button]
    void Set1()
    {
        Time.timeScale = 0.01f;
    }

    [Button]
    void Set10()
    {
        Time.timeScale = 0.1f;
    }


    [Button]
    void Set30()
    {
        Time.timeScale = 0.3f;
    }

    [Button]
    void Set50()
    {
        Time.timeScale = 0.5f;
    }

    [Button]
    void Set65()
    {
        Time.timeScale = 0.65f;
    }

    [Button]
    void Set80()
    {
        Time.timeScale = 0.8f;
    }

    [Button]
    void Set100()
    {
        Time.timeScale = 1f;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
