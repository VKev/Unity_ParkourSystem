using UnityEngine;


namespace PlayerStateMachine
{
    public class PlayerFallState : BaseState<PlayerStateMachine.EState>
    {
        PlayerStateMachine player;
        public PlayerFallState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }
        public override void EnterState()
        {

        }
        public override void CheckTransition()
        {
        }
        public override void ExitState()
        {

        }

    }
}