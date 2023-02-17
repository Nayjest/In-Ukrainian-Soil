using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractEffect : MonoBehaviour
{
    [SerializeField]
    protected bool Active = false;

    [SerializeField]
    protected bool Repeat = false;

    public virtual float GetDuration()
    {
        throw new System.NotImplementedException();
    }

    public UnityEvent OnEffectEnd;

    protected void FinishEffect()
    {
        if (!Repeat) Active = false;
        if (OnEffectEnd != null)
        {
            OnEffectEnd.Invoke();
        }
    }

    public virtual void Activate()
    {
        Active = true;
    }

    public void DestroySelf()
    {
        GameObject.Destroy(gameObject);
    }
}
