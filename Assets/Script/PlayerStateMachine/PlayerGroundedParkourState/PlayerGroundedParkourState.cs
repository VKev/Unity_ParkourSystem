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

        private bool isParkour;
        private float obstacleHeight;

        private Coroutine parkourActionPerformCoroutine;
        [SerializeField] private List<ParkourDefaultAction> parkourActions = new List<ParkourDefaultAction>();

        public PlayerGroundedParkourState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }

        public override void EnterState()
        {
            isParkour = false;
            obstacleHeight = player.rootState.DetectedObstacleHeight;
            if (parkourActions.Count == 0)
            {
                Debug.LogWarning("No Parkour action available!");
                return;
            }
            Debug.Log(obstacleHeight);
            foreach (ParkourDefaultAction parkourAction in parkourActions)
            {
                if (parkourAction.action.CanParkour(obstacleHeight))
                {
                    isParkour = true;
                    VkevLibrary.StopCoroutine(parkourActionPerformCoroutine, player);
                    parkourActionPerformCoroutine = player.StartCoroutine(PerformParkourAction(parkourAction));
                    break;
                }
            }
            if (!isParkour)
                TransitionToState(PlayerStateMachine.EState.Idle);
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