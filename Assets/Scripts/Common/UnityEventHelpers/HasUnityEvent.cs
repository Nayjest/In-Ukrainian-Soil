using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HasUnityEvent : MonoBehaviour
{
    public UnityEvent Event;
    
    public void TriggerEvent()
    {
        Event.Invoke();
    }
}
