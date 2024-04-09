using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerStateMachine
{
    [Serializable]
    public class PlayerIdleState : BaseState<PlayerStateMachine.EState>
    {
        PlayerStateMachine player;
        public PlayerIdleState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }
        private void WalkActionPerform(InputAction.CallbackContext context)
        {
            TransitionToState(PlayerStateMachine.EState.Run);
        }
        public override void EnterState()
        {
            InputController.WalkAction.AddPerformed(WalkActionPerform);
            if (InputController.WalkAction.isPressed)
                player.StartCoroutine(wait1frame_transitionToRunState());

            if (CurrentSuperState?.StateKey == PlayerStateMachine.EState.Grounded)
                player.rigid.linearVelocity = Vector3.zero;
        }

        IEnumerator wait1frame_transitionToRunState()
        {
            yield return null;
            TransitionToState(PlayerStateMachine.EState.Run);
        }

        public override void UpdateState()
        {
            player.anim.SetFloat(PlayerStateMachine.HORIZONTAL_VELOCITY_PERCENTAGE, player.runState.horizontalVelocityPercentage);
        }

        public override void ExitState()
        {
            InputController.WalkAction.RemovePerformed(WalkActionPerform);
        }
    }
}