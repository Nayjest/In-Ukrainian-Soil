using DinoFracture;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gem : MonoBehaviour
{
    public FractureGeometry Fract;
    
    public GameObject GemPartPrefab;

    public UnityEvent OnDestroyed;

    private bool activated = false;
    public bool NoBlur = false;

    private void Start()
    {
        if (Fract == null) Fract = GetComponentInChildren<FractureGeometry>();
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (!activated && other.gameObject.tag == "Player")
        {
            activated = true;
            other.gameObject.GetComponent<Player>().ConsumeLifePowerRegen();
            if (Fract)
            {
                var r = GetComponentInParent<MaterialShowcaseRotationScript>();
                if (r) r.enabled = false;
                Fract.Fracture().SetCallbackObject(this);
                if (NoBlur) Fract.gameObject.layer = LayerMask.NameToLayer("noPost");
            } else
            {
                CallOnDestroyed();
            }
        }
    }

    private void OnFracture(OnFractureEventArgs args)
    {
        if (args.IsValid)
        {
            Explode(args.FracturePiecesRootObject, args.OriginalMeshBounds, args.OriginalObject.transform.localScale);
        }
    }

    private void Explode(GameObject root, Bounds bounds, Vector3 scale)
    {
        
        var l = new List<Transform>();
        foreach (Transform t in root.transform) l.Add(t);
        float dur = 0;
        foreach (var t in l)
        {
            var o = GameObject.Instantiate(GemPartPrefab, root.transform);
            o.transform.position = t.position;
            t.SetParent(o.transform);
            var pt = o.GetComponent<GemPart>();
            pt.Target = Player.Inst.transform;
            dur = pt.Duration;
        }
        Invoke("CallOnDestroyed", dur);
    }

    void CallOnDestroyed()
    {
        OnDestroyed.Invoke();
        StopAllCoroutines();
        GameObject.Destroy(gameObject);
    }


}
