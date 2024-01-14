using System.Collections;
using UnityEngine;

namespace PlayerStateMachine
{
    public class PlayerGroundedState : BaseState<PlayerStateMachine.EState>
    {
        PlayerStateMachine player;
        public PlayerGroundedState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }
        public override void EnterState()
        {
            if (CurrentSubState == null)
                SetSubState(PlayerStateMachine.EState.Idle);

            player.rigid.velocity = new Vector3(player.rigid.velocity.x, 0f, player.rigid.velocity.z);
            if (CurrentSubState.StateKey == PlayerStateMachine.EState.Idle)
                player.rigid.velocity = Vector3.zero;
            player.rigid.useGravity = false;

        }
        public override void UpdateState()
        {
            player.anim.SetFloat(PlayerStateMachine.HORIZONTAL_VELOCITY_PERCENTAGE, player.horizontalVelocityPercentage);
        }
    }
}