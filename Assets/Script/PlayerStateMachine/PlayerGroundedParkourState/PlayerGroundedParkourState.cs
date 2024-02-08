using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStateMachine
{
    [Serializable]
    public class PlayerGroundedParkourState : BaseState<PlayerStateMachine.EState>
    {
        PlayerStateMachine player;
        public PlayerGroundedParkourState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }

        public override void EnterState()
        {
            player.anim.SetFloat(PlayerStateMachine.HORIZONTAL_VELOCITY_PERCENTAGE, 0f);
            player.StartCoroutine(PerformParkourAction(player.groundedState.currentParkourAction));
        }
        private IEnumerator PerformParkourAction(ParkourDefaultAction parkourAction)
        {
            player.anim.applyRootMotion = true;
            player.anim.CrossFade(parkourAction.action.animName, 0.05f);
            yield return null;
            AnimatorTransitionInfo animationTransition = player.anim.GetAnimatorTransitionInfo(0);
            AnimatorStateInfo animationState = player.anim.GetNextAnimatorStateInfo(0);
            if (!animationState.IsName(parkourAction.action.animName))
                Debug.Log("Parkour animation name not match player animation name");

            yield return new WaitForSeconds(animationState.length - animationTransition.duration);
            player.anim.applyRootMotion = false;
            TransitionToState(PlayerStateMachine.EState.Idle);
        }
    }
}