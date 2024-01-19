using UnityEngine;


namespace PlayerStateMachine
{
    public class PlayerInAirState : BaseState<PlayerStateMachine.EState>
    {
        PlayerStateMachine player;
        public PlayerInAirState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }

        public override void EnterState()
        {
            player.rigid.useGravity = true;
        }
        public override void ExitState()
        {
            player.rigid.useGravity = false;
        }

        public override void CheckTransition()
        {
            if (player.rootState.IsGrounded)
            {
                TransitionToState(PlayerStateMachine.EState.Grounded);
            }
        }
        public override void UpdateState()
        {
            CheckTransition();
        }
    }
}