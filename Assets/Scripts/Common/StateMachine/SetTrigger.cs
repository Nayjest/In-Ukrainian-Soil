using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTrigger : StateMachineBehaviour
{
    public string TriggerName;
    public Animator CustomAnimator;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Animator a = CustomAnimator ?? animator;
        a.SetTrigger(TriggerName);
    }
}