using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerStateMachine
{
    public class PlayerIdleState : BaseState<PlayerStateMachine.EState>
    {
        private PlayerStateMachine player;
        public PlayerIdleState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }
        private void WalkActionPerform(InputAction.CallbackContext context)
        {
            TransitionToState(PlayerStateMachine.EState.Run);
        }
        private void RunActionPerform(InputAction.CallbackContext context)
        {
            player.velocityPercentageThreshold = 1f;
        }
        private void RunActionCancel(InputAction.CallbackContext context)
        {
            player.velocityPercentageThreshold = player.WalkRunSpeedRatio;
        }
        public override void EnterState()
        {
            InputController.WalkAction.AddPerformed(WalkActionPerform);
            InputController.RunAction.AddPerformed(RunActionPerform);
            InputController.RunAction.AddCanceled(RunActionCancel);

            player.velocityPercentageThreshold = player.WalkRunSpeedRatio;
            if (InputController.RunAction.isPressed)
                player.velocityPercentageThreshold = 1f;

            if (CurrentSuperState.StateKey == PlayerStateMachine.EState.Grounded)
                player.rigid.velocity = Vector3.zero;

        }

        public override void UpdateState()
        {
        }

        public override void ExitState()
        {
            InputController.WalkAction.RemovePerformed(WalkActionPerform);
            InputController.RunAction.RemovePerformed(RunActionPerform);
            InputController.RunAction.RemoveCanceled(RunActionCancel);
        }
    }
}