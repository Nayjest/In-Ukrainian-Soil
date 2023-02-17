using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnDestroyUnityEvent : MonoBehaviour
{
    public UnityEvent OnDestroyEvent;

    private void OnDestroy()
    {
        OnDestroyEvent.Invoke();
    }
}
