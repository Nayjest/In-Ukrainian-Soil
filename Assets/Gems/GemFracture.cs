using DinoFracture;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;

public class GemFracture : MonoBehaviour
{
    public UnityEvent OnDestroyed;

    /// <summary>
    /// List of explosions to trigger
    /// </summary>
    [UnityEngine.Tooltip("List of explosions to trigger")]
    public FractureGeometry Gem;

    public Transform Target;


    public GameObject GemPartPrefab;

    private bool exploded = false;
    /*
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (exploded) return;
            exploded = true;
            Gem.Fracture().SetCallbackObject(this);
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        
    }

    /// <summary>
    /// Automatically called by FractureEngine when fracturing is complete
    /// </summary>
    /// <param name="args"></param>
    private void OnFracture(OnFractureEventArgs args)
    {
        if (args.IsValid)
        {            
            Explode(args.FracturePiecesRootObject, args.OriginalMeshBounds, args.OriginalObject.transform.localScale);
        }
    }

    private void Explode(GameObject root, Bounds bounds, Vector3 scale)
    {
        exploded = true;
        var l = new List<Transform>();
        foreach (Transform t in root.transform) l.Add(t);
        foreach(var t in l)
        {            
            var o = GameObject.Instantiate(GemPartPrefab, root.transform);
            o.transform.position = t.position;
            t.SetParent(o.transform);
            o.GetComponent<GemPart>().Target = Target;
        }
        Invoke("CallOnDestroyed", 2);
    }

    void CallOnDestroyed()
    {

        OnDestroyed.Invoke();
    }
}

