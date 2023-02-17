using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemFractureTest : MonoBehaviour
{
    public GameObject GemPrefab;
    public Transform Target;
    [Button]
    void ReInit()
    {
        var o  =GameObject.Instantiate(GemPrefab);
        o.GetComponent<GemFracture>().Target = Target;
        o.GetComponent<GemFracture>().OnDestroyed.AddListener(ReInit);
    }

    private void Start()
    {
        ReInit();
    }
}
