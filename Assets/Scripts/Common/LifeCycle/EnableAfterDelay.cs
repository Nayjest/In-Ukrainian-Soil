using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnableAfterDelay : MonoBehaviour
{
    public List<GameObject> Targets;
    public float Delay;

    private bool hasNoTargets => Targets == null || Targets.Count == 0;
    
    [ShowIf("hasNoTargets")]
    public bool UseChildren = false;

    [ShowIf("UseChildren")]
    public bool DisableChildrenAtStart = false;
    void Start()
    {
        
        if (hasNoTargets && UseChildren)
        {
            Targets = new List<GameObject>();
            foreach (Transform child in transform)
            {
                Targets.Add(child.gameObject);
                if (DisableChildrenAtStart) child.gameObject.SetActive(false);
            }
        }
        Invoke("EnableAll", Delay);
    }

    // Update is called once per frame
    void EnableAll()
    {
        foreach (var o in Targets) o.SetActive(true);
    }
}
