using Curve;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubTail : MonoBehaviour
{

    public MeshFilter mf;
    public Mesh mesh;
    public CurveTester ct;
    public RootController Controller;
    public TubularMeshGen mg;    

    private void Reset()
    {
        mf = GetComponent<MeshFilter>();
        ct = GetComponent<CurveTester>();
    }


    private void Start()
    {
        
        mf.sharedMesh = mesh = new Mesh();
        MakeMesh();

    }

    public void MakeMesh()
    {
        MeshGenerator.Build(mesh, ct.Build(), mg.lengthSegments, mg.radius, mg.radialSegments, false);
    }
    public void AddPoint(Vector3 point)
    {
        ct.Points.Add(point);
        var qty = ct.Points.Count;
        if (qty >= Controller.MaxNodes)
        {
            
            var s = new GameObject();
            s.layer = gameObject.layer;
            var mf2 = s.AddComponent<MeshFilter>();
            mf2.sharedMesh = mf.sharedMesh;
            var mr2 = s.AddComponent<MeshRenderer>();
            mr2.sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;            
            
            //var s = GameObject.Instantiate(gameObject);
            var d = s.AddComponent<DestroyAfterLifeTime>();
            d.Lifetime = 30;
            mf.sharedMesh = mesh = new Mesh();
            
            var p1 = ct.Points[qty - 3];
            var p2 = ct.Points[qty - 2];
            var p3 = ct.Points[qty - 1];

            ct.Points.Clear();
            ct.Points.Add(p1);
            ct.Points.Add(p2);
            ct.Points.Add(p3);

        }
        MakeMesh();
        //Tubular.Tubular.Build()
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
