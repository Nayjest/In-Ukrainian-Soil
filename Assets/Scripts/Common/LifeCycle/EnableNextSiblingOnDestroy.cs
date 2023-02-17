using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableNextSiblingOnDestroy : MonoBehaviour
{    
    void OnDestroy()
    {
        int i = transform.GetSiblingIndex();
        if (transform.parent.childCount > i+1)
        {
            transform.parent.GetChild(i + 1).gameObject.SetActive(true);
        }
    }
}
