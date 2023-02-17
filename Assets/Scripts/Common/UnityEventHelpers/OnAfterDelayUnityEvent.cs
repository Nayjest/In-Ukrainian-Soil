using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnAfterDelayUnityEvent : MonoBehaviour
{
    public float Delay;
    public UnityEvent Event;
    void Start()
    {
        Invoke("Trigger", Delay);
    }

    // Update is called once per frame
    void Trigger()
    {
        Event.Invoke();
    }
}
