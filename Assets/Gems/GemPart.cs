using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
public class GemPart : MonoBehaviour
{

    public float ExpForce = 10f;
    public Vector2 TargetAttraction = new Vector2(5, 10);
    public float Duration = 3f;
    public Vector2 PivotRotAngleSpeed = new Vector2(5, 10);
    public Vector2 MeshRotAngleSpeed = new Vector2(10, 20);
    public float MaxOffset;


    protected Vector3 StartExpForce;
    protected Vector3 ActualExpForce;
    
    
    protected float ActualTargetAttraction = 0;    


    public Transform Target;
    protected Transform M => transform.GetChild(0);


    
    protected Vector3 MeshRotation;
    protected Vector3 PivotRotation;
    protected Vector3 PivotOffset;

    private Coroutine c;
    private float attractionModifier;
    protected float Offset;
    protected float mOffset;

    void Start()
    {
        ActualExpForce = StartExpForce = Random.onUnitSphere * (0.5f + Random.value) * ExpForce;
        ActualTargetAttraction = TargetAttraction.x;
        MeshRotation = Random.onUnitSphere * MeshRotAngleSpeed.Random();
        PivotRotation = Random.onUnitSphere * PivotRotAngleSpeed.Random();
        attractionModifier = Random.Range(0.95f, 1.05f);
        mOffset = Random.value * MaxOffset;
    }

    // Update is called once per frame
    void Update()
    {
        var dt = Time.deltaTime;
        if (Target == null) return;
        if (c == null) {
            c = StartCoroutine(Animation());
        }
        var diff = (Target.position - transform.position);
        if (diff.magnitude < 1)
        {
            StopAllCoroutines();
            GameObject.Destroy(gameObject);
            return;
        }
        var force = (ActualExpForce + diff.normalized * ActualTargetAttraction * attractionModifier) * dt;        
        //var force = ActualExpForce * dt;        
        transform.position += force;
        transform.Rotate(PivotRotation * dt, Space.World);
        M.Rotate(MeshRotation * dt);
        M.transform.localPosition = StartExpForce.normalized * Offset;

    }

    IEnumerator Animation()
    {        
        yield return StartCoroutine(Coroutines.AnimateFrames(Duration, (t) =>
        {
            Offset = (0.5f - Mathf.Abs(t - 0.5f)) * mOffset;
            ActualExpForce = Vector3.Lerp(StartExpForce, Vector3.zero, t);
            ActualTargetAttraction = Mathf.Lerp(TargetAttraction.x, TargetAttraction.y, t);
            float startFade = 0.2f;
            if (t > startFade)
            {
                var tt = (t - startFade)/(1 - startFade);
                transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, tt);
            }
        }));                
    }
}
