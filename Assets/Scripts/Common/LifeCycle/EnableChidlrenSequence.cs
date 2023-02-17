using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableChidlrenSequence : MonoBehaviour
{
    public float StartDelay = 0;
    public bool DisableOnAwake = true;
    public bool RandomizeOrder = false;
    void Awake()
    {
        if (RandomizeOrder) foreach (Transform c in transform) c.SetSiblingIndex(Random.Range(0, transform.childCount));        
        if (DisableOnAwake) foreach (Transform c in transform) c.gameObject.SetActive(false);
        Invoke("Activate", StartDelay);        
    }

    // Update is called once per frame
    public void Activate()
    {
        if (transform.childCount == 0) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.AddComponent<EnableNextSiblingOnDestroy>();
        }
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
