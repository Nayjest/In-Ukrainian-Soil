using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWithParent : MonoBehaviour
{
    private void OnDestroy()
    {
        GameObject.Destroy(transform.parent.gameObject);
    }
}
