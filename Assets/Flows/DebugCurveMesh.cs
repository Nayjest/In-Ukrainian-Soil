using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[DefaultExecutionOrder(10000000)]
public class DebugCurveMesh : MonoBehaviour
{
    [SerializeField]
    private DynamicCurve _curveSrc;
    public float Width = 1f;
    
    void CreateMesh()
    {        
            var filter = GetComponent<MeshFilter>();
            filter.sharedMesh = Tubular.Tubular.Build(_curveSrc.Curve(), 10000, Width, 8, false);        
    }

   

    private void OnEnable()
    {        
        _curveSrc.RegisterOnCurveChanged(CreateMesh);        
    }

    private void OnDisable()
    {
        _curveSrc.UnregisterOnCurveChanged(CreateMesh);
    }

    private void Start()
    {
        CreateMesh();
    }
}
