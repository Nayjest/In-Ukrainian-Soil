using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

[ExecuteInEditMode]
public class PolarMovement : MonoBehaviour
{
    public enum CenterType
    {
        Vector3,
        Transform
    }
    public CenterType TargetType;

    public bool FreezeDecart = false;

    public bool TargetIsTransform => TargetType.Equals(CenterType.Transform);
    public bool TargetIsVector => TargetType.Equals(CenterType.Vector3);

    [ShowIf("TargetIsTransform")]
    [Required]
    public Transform Center;

    [ShowIf("TargetIsVector")]
    public Vector3 CenterPos = Vector3.zero;

    public Vector3 ActualCenterPos => TargetIsTransform ? (Center == null ? Vector3.zero : Center.position) : CenterPos;

    [Tooltip("Degrees per second")]
    public float AngleSpeed;

    [Tooltip("World units per second, > 0 --> increase radius")]
    public float RadiusSpeed;

    [Tooltip("World units per second speed limit for polar movement")]
    public float SpeedLimit = 0;

    public bool RandomizeAngle = false;

    private float a;
    [ShowNativeProperty]
    public float Angle
    {
        get { return float.IsNaN(a) ? 0 : a; }
        set
        {
            a = value;
            DecartFromPolar();
        }
    }

    private float r;
    [ShowNativeProperty]
    public float Radius
    {
        get { return r; }
        set {
            r = value;
            DecartFromPolar();
        }
    }

    public Vector2 PolarCoords
    {
        get { return new Vector2(a, r); }
        set
        {
            a = value.x;
            r = value.y;
            DecartFromPolar();
        }
    }


    public void SetCenter(MonoBehaviour target)
    {
        SetCenter(target.transform);
    }
    public void SetCenter(GameObject target)
    {
        SetCenter(target.transform);
    }

    public void SetCenter(Transform target)
    {
        TargetType = CenterType.Transform;
        Center = target;
    }

    public void SetCenter(Vector3 target)
    {
        TargetType = CenterType.Vector3;
        CenterPos = target;
    }

    public void PolarFromDecart()
    {
        var pos = transform.position;
        var cpos = ActualCenterPos;
        Vector2 diff = pos - cpos;        
        var ndiff = diff.normalized;
        a = Mathf.Atan2(ndiff.y, ndiff.x);        
        r = diff.magnitude;
    }

    void DecartFromPolar(float DistanceLimit = 0)
    {        
        Vector3 npos = ActualCenterPos + new Vector3(
               Mathf.Cos(Angle) * r,
               Mathf.Sin(Angle) * r,
               0
       );
        npos.z = transform.position.z;        
        if (DistanceLimit != 0)
        {
            var mov = npos - transform.position;
            mov = Vector2.ClampMagnitude(mov, SpeedLimit);
            npos = transform.position + mov;
        }
        // @todo fix it, NaN appears not here
        if (float.IsNaN(npos.x) || float.IsNaN(npos.y) || float.IsNaN(npos.z)) return;
        transform.position = npos;
    }


    void ApplyPolarMovement(bool UseSpeedLimit = false)
    {
        a += AngleSpeed * Mathf.Deg2Rad * Time.deltaTime;
        r += RadiusSpeed * Time.deltaTime;
        if (r < 0) r = 0;
        DecartFromPolar(UseSpeedLimit ? SpeedLimit * Time.deltaTime : 0);
    }

    void Update()
    {
        if (!FreezeDecart) PolarFromDecart();
        if (!Application.isPlaying) return;
        ApplyPolarMovement(true);
    }

    private void Start()
    {
        PolarFromDecart();
        if (RandomizeAngle) DoRandomizeAngle();
    }

    [Button]
    private void DoRandomizeAngle()
    {
        PolarFromDecart();
        Angle = Random.Range(0f, 360f);
    }
}
