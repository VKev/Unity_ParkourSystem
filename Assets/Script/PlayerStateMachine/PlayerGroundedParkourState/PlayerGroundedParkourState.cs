using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            player.StartCoroutine(PerformParkourAction(player.groundedState.currentParkourAction));
        }
        private IEnumerator PerformParkourAction(ParkourDefaultAction parkourAction)
        {
            player.rootState.legsProcedural.ResetWeight();
            player.anim.applyRootMotion = true;
            float transitionDurationMultiplier = 1f;
            if (player.anim.GetFloat(PlayerStateMachine.HORIZONTAL_VELOCITY_PERCENTAGE) < 0.5f)
                transitionDurationMultiplier = player.anim.GetFloat(PlayerStateMachine.HORIZONTAL_VELOCITY_PERCENTAGE) < 0.1f ? 0.1f : player.anim.GetFloat(PlayerStateMachine.HORIZONTAL_VELOCITY_PERCENTAGE);

            player.anim.CrossFade(parkourAction.action.animName, parkourAction.action.transitionDuration * transitionDurationMultiplier);

            yield return null;
            AnimatorTransitionInfo animationTransition = player.anim.GetAnimatorTransitionInfo(0);
            AnimatorStateInfo animationState = player.anim.GetNextAnimatorStateInfo(0);

            if (!animationState.IsName(parkourAction.action.animName))
                Debug.Log("Parkour animation name not match player animation name");
            yield return new WaitForSeconds(animationState.length - animationTransition.duration);

            player.anim.applyRootMotion = false;
            player.anim.SetFloat(PlayerStateMachine.HORIZONTAL_VELOCITY_PERCENTAGE, 0f);
            TransitionToState(PlayerStateMachine.EState.Idle);
        }


    }
}