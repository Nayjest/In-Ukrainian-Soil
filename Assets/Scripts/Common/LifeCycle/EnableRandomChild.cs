using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRandomChild : MonoBehaviour
{
    // Start is called before the first frame updat
    // e    
    public bool DestroyWithChild = true;
    public float StartDelay = 0f;
    public bool DisableAfterExecution = false;
    public bool DisableOnEnable = false;
    public bool UseEnableInsteadOfStart = false;
    public RandomUtils.Seed Seed = RandomUtils.Seed.None;
    void Start()
    {        
        if (!UseEnableInsteadOfStart) Invoke("DoEnableRandomChild", StartDelay);
    }

    private void OnEnable()
    {        
        if (DisableOnEnable) foreach (Transform c in transform) c.gameObject.SetActive(false);
        if (UseEnableInsteadOfStart) Invoke("DoEnableRandomChild", StartDelay);
    }
    public void DoEnableRandomChild()
    {

        int qty = transform.childCount;        
        var index = RandomUtils.Range(0, qty, Seed);
        var o = transform.GetChild(index).gameObject;
        o.SetActive(true);
        if (DestroyWithChild) o.AddComponent<HasOnDestroyAction>().OnDestroyAction += SelfDestroy;
        if (DisableAfterExecution) this.enabled = false;
    }

    public void SelfDestroy()
    {
        GameObject.Destroy(gameObject);
    }
}
