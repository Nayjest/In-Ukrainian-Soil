using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fix1stFrameAnimatorLag : MonoBehaviour
{
    public Animator Animator;

    private void Reset()
    {
        Animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        Animator.Update(Time.deltaTime);
    }
}
