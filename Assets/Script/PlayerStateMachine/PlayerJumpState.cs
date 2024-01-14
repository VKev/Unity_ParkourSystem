using System.Collections;
using UnityEngine;

namespace PlayerStateMachine
{
    public class PlayerJumpState : BaseState<PlayerStateMachine.EState>
    {
        public PlayerJumpState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
        }
        public override void EnterState()
        {


        }
        public override void UpdateState()
        {

        }
    }
}