using Curve;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DynamicCurve : MonoBehaviour, CurveSourceInterface
{

    System.Action onPointsChange = ()=> { }; 

    [SerializeField] protected List<Vector3> points;
    public List<Vector3> Points()
    {
        if (points == null || points.Count <= 0)
        {
            points = new List<Vector3>() {
                    Vector3.zero,
                    Vector3.up,
                    Vector3.right
                };
        }
        return points;
    }

    public IEnumerator MoveCurve()
    {        
        while (true)
        {
            var c = points.Count;
            var a = points[c - 2];
            var b = points[c - 1];
            var dir = (b - a).normalized + 
                Random.insideUnitSphere;
            dir *= 200f;
            points.Add(dir + b);
            points.RemoveAt(0);
            
            onPointsChange?.Invoke();
            yield return new WaitForSeconds(2);
        }
    }

    public Curve.Curve curve;
    public Curve.Curve Curve()
    {
        if (curve == null) CreateCurve();
        return curve;
    }
    public void CreateCurve()
    {
        curve = new CatmullRomCurve(Points(), false);
    }

    [SerializeField] protected float unit = 0.1f;
    [SerializeField] protected bool point = true, tangent = true, frame = false;       
    protected List<FrenetFrame> frames;

    private void Start()
    {
        //StartCoroutine(MoveCurve());
    }
    void OnEnable()
    {
        Points();
        Curve();
        //onPointsChange += CreateCurve;
    }

    private void OnDisable()
    {
        //onPointsChange -= CreateCurve;
    }

    public void SetPoints(List<Vector3> points)
    {
        this.points = points;
        CreateCurve();
        onPointsChange.Invoke();
    }
    public void AddPoint(Vector3 p)
    {
        points.Add(p);
        CreateCurve();
        onPointsChange.Invoke();
    }


    void OnDrawGizmos()
    {
        if (curve == null)
        {
            CreateCurve();
        }
        DrawGizmos();
    }

    void DrawGizmos()
    {
        const float delta = 0.01f;
        float dunit = unit * 2f;
        var count = Mathf.FloorToInt(1f / delta);

        if (frames == null)
        {
            frames = curve.ComputeFrenetFrames(count, false);
        }

        Gizmos.matrix = transform.localToWorldMatrix;
        for (int i = 0; i < count; i++)
        {
            var t = i * delta;
            var p = curve.GetPointAt(t);

            if (point)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(p, unit);
            }

            if (tangent)
            {
                var t1 = curve.GetTangentAt(t);
                var n1 = (t1 + Vector3.one) * 0.5f;
                Gizmos.color = new Color(n1.x, n1.y, n1.z);
                Gizmos.DrawLine(p, p + t1 * dunit);
            }

#if UNITY_EDITOR
            Handles.matrix = transform.localToWorldMatrix;
            Handles.Label(p, i.ToString());
#endif

            if (frame)
            {
                var fr = frames[i];

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(p, p + fr.Tangent * dunit);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(p, p + fr.Normal * dunit);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(p, p + fr.Binormal * dunit);
            }
        }
    }

    public void RegisterOnCurveChanged(System.Action Handler)
    {
        onPointsChange += Handler;
    }
    public void UnregisterOnCurveChanged(System.Action Handler)
    {
        onPointsChange -= Handler;
    }
}
