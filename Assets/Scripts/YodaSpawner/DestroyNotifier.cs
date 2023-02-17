using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyNotifier : MonoBehaviour
{
    public System.Action DestroyAction;

    private void OnDestroy()
    {
        if (DestroyAction != null) DestroyAction.Invoke();
    }
}
