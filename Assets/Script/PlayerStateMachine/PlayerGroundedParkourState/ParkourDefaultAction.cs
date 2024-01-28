using System;
using UnityEngine;

namespace PlayerStateMachine
{
    [Serializable]
    public class ParkourDefaultAction
    {
        public ParkourActionSO action;
        protected PlayerStateMachine player;
        public void SetPlayerStateMachineData(PlayerStateMachine context)
        {
            player = context;
        }
        public virtual void UpdateState() { }
        public virtual void EnterState() { }
    }
}