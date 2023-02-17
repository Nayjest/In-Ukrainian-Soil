using Curve;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TubularMeshGen : MonoBehaviour
{

	public MeshFilter mf;
    public Mesh mesh;
	public CurveTester ct;

    [SerializeField] public int lengthSegments = 20;
    [SerializeField] public float radius = 0.1f;
    [SerializeField] public int radialSegments = 6;
    private object closed;

    private void Reset()
    {
        mf = GetComponent<MeshFilter>();
		ct = GetComponent<CurveTester>();
	}

    
    private void Start()
    {
		//mesh = new Mesh();
		mf.sharedMesh = mesh = new Mesh();
		MakeMesh();

    }
	[Button]
	public void Rebuild()
    {		
		MakeMesh();
    }


	void MakeMesh()
    {
		MeshGenerator.Build(mesh, ct.Build(), lengthSegments, radius, radialSegments, true);
	}
}
