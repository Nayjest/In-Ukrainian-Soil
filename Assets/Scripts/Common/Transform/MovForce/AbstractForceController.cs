using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

[RequireComponent(typeof(MovSpeed))]
public abstract class AbstractForceController : MonoBehaviour
{
    protected MovSpeed force;

    public Vector3 Force
    {
        get {
            if (force == null) return Vector3.zero;
            return force.Speed; 
        }
        set {
            if (force == null) return;
            force.Speed = value;
        }
    }

    // Start is called before the first frame update
    protected void Start()
    {
        force = GetComponent<MovSpeed>();
        if (force == null)
        {
            Debug.LogError("Creating ForceController without MovSpeed Component");
            Destroy(gameObject);
        }
    }

}