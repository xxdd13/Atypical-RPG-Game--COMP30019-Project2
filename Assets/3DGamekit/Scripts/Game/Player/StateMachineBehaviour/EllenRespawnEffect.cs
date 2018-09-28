using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proj2
{
    public class EllenRespawnEffect : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<PlayerController>().Respawn();
        }
    } 
}