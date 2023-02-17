using Curve;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootController : MonoBehaviour
{
    public CurveTester ct;
    public Transform Player;
    public TubularMeshGen mg;
    public TubTail Tail;

    public int PointsQty => ct.Points.Count;
    //public Vector3 Pos => Player.position;
    public Vector3 Pos => ForwardPos;
    public Vector3 ForwardPos => Player.position + Player.transform.right * 0.5f;


    public int MaxNodes = 20;
    public int MinNodes = 10;

    [ShowNativeProperty]
    public int NodesQty => ct.Points.Count;
    public float PointDist = 1f;

    bool IsVisible(Vector3 point)
    {
        var camera = Camera.main;
        Vector3 viewportPoint = camera.WorldToViewportPoint(point);
        return (new Rect(0, 0, 1, 1)).Contains(viewportPoint);
        //return (viewportPoint.z > 0(new Rect(0, 0, 1, 1)).Contains(viewportPoint));
    }

    
    private Vector3 prev => ct.Points[ct.Points.Count - 2];

    void DeleteInvisible()
    {
        if (PointsQty <= MinNodes) return;
        for(int i = PointsQty-MinNodes; i>1;i--)
        {
            if (!IsVisible(ct.Points[i]))
            {                
                ct.Points.RemoveRange(0, i - 1);
                return;
            }
        }       
    }
    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(prev, Pos) > PointDist)
        {
            ct.AddPoint(Pos);            
            
            if (ct.Points.Count > MaxNodes)
            {
                if (Tail) Tail.AddPoint(ct.Points[2]);
                ct.Points.RemoveAt(0);
            }
            
        } else
        {
            ct.Points[ct.Points.Count - 1] = Pos;
        }
        //DeleteInvisible();
        mg.Rebuild();
        

    }
};
