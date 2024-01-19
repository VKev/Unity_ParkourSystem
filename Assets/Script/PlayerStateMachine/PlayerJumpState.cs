using System;
using System.Collections;
using UnityEngine;

namespace PlayerStateMachine
{
    [Serializable]
    public class PlayerJumpState : BaseState<PlayerStateMachine.EState>
    {
        PlayerStateMachine player;
        public PlayerJumpState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }
        public override void EnterState()
        {


        }
        public override void CheckTransition()
        {
            if (player.rigid.velocity.y <= 0f)
            {
                TransitionToState(PlayerStateMachine.EState.Fall);
            }
        }
        public override void UpdateState()
        {
            CheckTransition();
        }
    }
}