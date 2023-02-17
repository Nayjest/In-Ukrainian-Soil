using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncNumber : StateMachineBehaviour
{
    public string ParamName;
    public int Step = 1;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger(ParamName, animator.GetInteger(ParamName) + Step);
    }

}
