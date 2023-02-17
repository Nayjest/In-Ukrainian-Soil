using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using NaughtyAttributes;
using UnityEngine.Serialization;

[DefaultExecutionOrder(0)]
public class MapGen3D : MonoBehaviour
{
    public GameObject Prefab;
    public Transform Root;
    public float Magnitude = 100;

    [FormerlySerializedAs("qty")]
    public float Qty;

    public bool UseScale = true;
    public Vector2 MinMaxScale = new Vector2(1, 10);
    public bool GenerateOnStart = false;
    public bool RegenerateOnNewQuadrant = false;
    Vector3 RandVector(float magnitude)
    {
        return new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * 2 * magnitude;
    }

    [Button]
    public void ReGenerate()
    {
        Clear();
        Generate();
    }

    public void ReActivate()
    {
        foreach(Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
    }
    public void Generate(int qty = 0)
    {
        if (qty == 0)
        {
            var low = Mathf.FloorToInt(Qty);
            qty = low;
            if (Random.value <= Qty - low) qty++;
        }
        for (int i = 0; i < qty; i++)
        {
            var q = Quaternion.Euler(RandVector(360));
            var o = GameObject.Instantiate(Prefab, RandVector(Magnitude) + Root.transform.position, q, Root);
            if (UseScale)  o.transform.localScale = Vector3.one * MinMaxScale.Random();
            o.SetActive(true);
        }
    }

    [Button]
    public void Clear()
    {
        foreach (Transform t in Root)
        {
            GameObject.DestroyImmediate(t.gameObject);
        }
    }

    private void Start()
    {        
        if (GenerateOnStart)
        {
            ReGenerate();
        }
    }
}
