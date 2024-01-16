using System;
using System.Collections;
using UnityEngine;

namespace PlayerStateMachine
{
    [Serializable]
    public class PlayerSlideState : BaseState<PlayerStateMachine.EState>
    {
        public PlayerSlideState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
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