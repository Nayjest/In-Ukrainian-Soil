using Curve;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;


[System.Serializable]
public struct BoidsData
{
    public Vector3 position;
    public Vector3 velocity;
    public float size;
}
[DefaultExecutionOrder(1000000)]
public class ParticleBoids : MonoBehaviour
{
    public Transform Observer;

    [SerializeField]
    protected float observerScareDist = 20;

    [SerializeField]
    private int TEX_RESOLUTION = 100;
    
    [ShowNativeProperty]
    private int PARTICLE_COUNT => TEX_RESOLUTION * TEX_RESOLUTION;
    

    [Range(0, 3f)]
    public float speedFac = 0.5f;
    [Range(0, 1)] public float separationFac = 0.5f;
    [Range(0, 1)] public float alignmentFac = 0.5f;
    [Range(0, 1)] public float cohesionFac = 0.5f;
    //[Range(0, 1f)] 
    public float attractorFac = 0.005f;
    [Range(0, 1)] public float randomFac = 0.005f;

    public float curveDirSpeedFac = 0.001f;

    [Range(0, 0.9999f)]
    public float dump = 0.01f;

    [Range(0, 0.9999f)]
    public float inertia = 0.9f;

    //[Range(0, 1)] 
    public float tunnelRadius = 1f;

    public float interactRange = 1;

    private bool initialized = false;

    [SerializeField] protected VisualEffect graph;
    [SerializeField] protected RenderTexture particlePosTex;
    [SerializeField] protected ComputeShader particleCS;

    protected const string ENCODE_POSITION = "EncodePosition";
    protected const string PARTICLE_BOIDS_COMPUTE = "ParticleBoidsCompute";
    protected const string PARTICLE_MOVE = "ParticleMove";
    protected const string MOVE_FROM_TAIL = "MoveFromTail";
    

    protected int encodePosKernelIndex;
    protected int particleAttractKernelIndex;
    protected int particleMoveKernelIndex;
    protected int moveFromTailIndex;
    
    protected ComputeBuffer particleBuffer;
    protected ComputeBuffer curveBuffer;

    public DynamicCurve Curve;
    // Start is called before the first frame update
    
    [ShowNonSerializedField]
    private int curveBufferSize;

    [Button]
    public void Init()
    {
        if (initialized) return;
        initialized = true;
        InitializeCompute();
        
        curveBufferSize = Curve.Points().Count * 4;

        curveBuffer = new ComputeBuffer(curveBufferSize, Marshal.SizeOf(typeof(Vector3)));
        curveBuffer.SetData(Points());
        ClearPath();
        InitializeParticles();
        Curve.RegisterOnCurveChanged(PointsChanged);
        graph.SetFloat("ObserverScareDist", observerScareDist);
    }
    protected void Start()
    {
        Init();
    }


    private void PointsChanged()
    {        
        var points = Points();
        curveBuffer.SetData(points);
        // @todo remove
        DispatchMoveFromTail();
        SetConstants();
        ClearPath();
        
    }

    void ClearPath()
    {
        var pts = Points();
        USoil.Quadrant q = null;
        foreach (var p in pts)
        {
            if (q== null || !q.ContainsPoint(p))
            {
                q = USoil.Space.Inst.GetQuadrant(p);
                if (q == null) continue;
            }
            foreach(var r in q.Root.GetComponentsInChildren<Rock>())
            {
                if ((r.transform.position - p).magnitude < (r.transform.localScale.x + 15f))
                {
                    r.gameObject.SetActive(false);
                }
            }
            /*
            foreach(Transform t in q.Root.transform)
            {
                if (t.gameObject.name == "MapGenerator(Clone)")
                {
                    foreach(Transform r in t)
                    {
                        if ((r.position - p).magnitude<100)
                        {
                            r.gameObject.SetActive(false);
                        }
                    }
                }
            }
            */
            
        }
    }

    protected void DispatchMoveFromTail()
    {
        if (!initialized) return;
        //set texture and buffer
        particleCS.SetBuffer(moveFromTailIndex, "_ParticleBuffer", particleBuffer);
        curveBuffer.SetData(Points());
        particleCS.SetBuffer(moveFromTailIndex, "_CurveBuffer", curveBuffer);
        //particleCS.SetTexture(moveFromTailIndex, "_ParticlePositions", particlePosTex);
        particleCS.Dispatch(moveFromTailIndex, (int)(PARTICLE_COUNT / 64), 1, 1);
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!initialized) return;
        
        
        //VisualizePositions();
        SetBufferToGraph();
        
        
    }

    protected void FixedUpdate()
    {
        if (!initialized) return;
        SetConstants();
        DispatchEncodePos();
        DispatchParticleAttract();
        DispatchParticleMove();
    }

    protected BoidsData CreateParticle()
    {
        BoidsData p = new BoidsData();

        var rndPoint = Curve.curve.GetPointAt(Random.value);
        p.position = rndPoint + Random.insideUnitSphere * tunnelRadius;
        //random inside the bounds

        //p.position = (Random.insideUnitSphere);

        // p.position = (Random.insideUnitSphere * 2 - Vector3.one);
        // p.position.x *= bounds.x/2;
        // p.position.y *= bounds.y/2;
        // p.position.z *= bounds.z/2;
        // p.position += this.transform.position; //add gameobject position

        //random direction
        p.velocity = (Random.insideUnitSphere * 2 - Vector3.one)*0.1f;
        
        //random size
        p.size = Random.value * 0.9f + 0.1f;
        
        return p;
    }
    
    protected RenderTexture CreateRT(int resolution, FilterMode filterMode)
    {
        RenderTexture texture = new RenderTexture(resolution,resolution,1, RenderTextureFormat.ARGBFloat);

        texture.name = "Particles";
        texture.enableRandomWrite = true;
        texture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
        texture.volumeDepth = 1;
        texture.filterMode = filterMode;
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.autoGenerateMips = false;
        texture.useMipMap = false;
        texture.Create();

        return texture;

    }

    protected void InitializeCompute()
    {
        //find the function in compute shader
        encodePosKernelIndex = particleCS.FindKernel(ENCODE_POSITION);
        particleAttractKernelIndex = particleCS.FindKernel(PARTICLE_BOIDS_COMPUTE);
        particleMoveKernelIndex = particleCS.FindKernel(PARTICLE_MOVE);
        moveFromTailIndex = particleCS.FindKernel(MOVE_FROM_TAIL);
        
        //initiliaze the texture
        particlePosTex = CreateRT(TEX_RESOLUTION, FilterMode.Point);
        
    }

    public Vector3[] Points()
    {
        int qty = curveBufferSize;
        Vector3[] p = new Vector3[qty];
        for (int i = 0; i < qty; i++)
        {
            p[i] = Curve.curve.GetPointAt(i / (float)qty);
        }
        return p;
        //return Curve.Points.ToArray();
    }

    protected void InitializeParticles()
    {        
        //initialize the buffer
        particleBuffer = new ComputeBuffer(PARTICLE_COUNT, Marshal.SizeOf(typeof(BoidsData)));                

        //temp array to initialize particles
        BoidsData[] arr = new BoidsData[PARTICLE_COUNT];
        
        //fill that array with particle
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = CreateParticle();
            
        }
        
        //put that array into a compute buffer
        particleBuffer.SetData(arr);
    }

    protected void SetConstants()
    {
        particleCS.SetInt("_Resolution", TEX_RESOLUTION);
        //particleCS.SetVector("_Bounds", bounds);
        //particleCS.SetVector("_BoundsPosition", this.transform.position);
        particleCS.SetFloat("_SpeedFac", speedFac);
        particleCS.SetFloat("_SeparationFac", separationFac);
        particleCS.SetFloat("_AlignmentFac", alignmentFac);
        particleCS.SetFloat("_CohesionFac", cohesionFac);
        particleCS.SetFloat("_AttractorFac", attractorFac);
        particleCS.SetFloat("_RandomFac", randomFac);
        //particleCS.SetVector("_LinePoint1", LinePoint1.position);
        //particleCS.SetVector("_LinePoint2", LinePoint2.position);
        particleCS.SetFloat("_TunnelRadius", tunnelRadius);
        particleCS.SetFloat("_CurveDirSpeedFac", curveDirSpeedFac);        
        particleCS.SetInt("_CurvePointCount", Points().Length);
        particleCS.SetFloat("_Dump", dump);
        particleCS.SetFloat("_Inertia", inertia);
        particleCS.SetFloat("_InteractRange", interactRange);
        particleCS.SetVector("_ObserverPos", Observer.position);
        particleCS.SetFloat("_ObserverScareDist", observerScareDist);



    }

    protected void DispatchEncodePos()
    {
        //set texture and buffer
        particleCS.SetTexture(encodePosKernelIndex,"_ParticlePositions", particlePosTex);
        particleCS.SetBuffer(encodePosKernelIndex,"_ParticleBuffer", particleBuffer);        

        //call the function in the compute shader
        particleCS.Dispatch(encodePosKernelIndex, 16,16,1);
    }

    protected void DispatchParticleAttract()
    {
        //set texture and buffer
        particleCS.SetBuffer(particleAttractKernelIndex,"_ParticleBuffer", particleBuffer);
        // todo may be removed
        //curveBuffer.SetData(Points());
        particleCS.SetBuffer(particleAttractKernelIndex, "_CurveBuffer", curveBuffer);

        //call the function in the compute shader
        particleCS.Dispatch(particleAttractKernelIndex, (int)(PARTICLE_COUNT / 64) ,1,1);
    }

    
    protected void DispatchParticleMove()
    {
        //set texture and buffer
        particleCS.SetBuffer(particleMoveKernelIndex,"_ParticleBuffer", particleBuffer);
        
        //call the function in the compute shader
        particleCS.Dispatch(particleMoveKernelIndex, (int)(PARTICLE_COUNT / 64) ,1,1);
    }   

    protected void SetBufferToGraph()
    {
        graph.SetTexture("PositionsBufferTex", particlePosTex);
        graph.SetVector3("ObserverPos", Observer.transform.position);        
    }
    
    protected void DebugParticleData()
    {
        BoidsData[] particleArr = new BoidsData[PARTICLE_COUNT];
        particleBuffer.GetData(particleArr);

        for (int i = 0; i < 15; i++)
        {
            int index = Random.Range(0, particleArr.Length);
            Vector3 position = particleArr[i].position;
            Vector3 direction = particleArr[i].velocity;
            float size = particleArr[i].size;
            Debug.LogError($"Agent #{index}: Position {position}, Vel {direction}");
        }
            
    }

    protected void OnDestroy()
    {
        //flush out the textures
        if (particlePosTex != null)
        {
            particlePosTex.Release();
        }
        
        //flush out the buffer on the GPU
        if (particleBuffer != null)
        {
            particleBuffer.Release();
        }
        Curve.UnregisterOnCurveChanged(PointsChanged);
    }
}
