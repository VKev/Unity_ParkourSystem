using System;
using System.Collections;
using UnityEngine;

namespace PlayerStateMachine
{
    [Serializable]
    public class PlayerRootState : BaseState<PlayerStateMachine.EState>
    {
        PlayerStateMachine player;

        [Header("FLOATING CAPSULE")]
        [SerializeField] private float maxFloatingHeight;
        public float FloatingHeight { get; private set; }



        private float distanceToGround;
        private RaycastHit groundHit;
        [SerializeField] private LayerMask groundLayers;
        public float DistanceToGround { get { return distanceToGround; } }
        public LayerMask GroundLayers { get { return groundLayers; } }
        public RaycastHit GroundHit { get { return groundHit; } }
        public bool IsGrounded { get; private set; }



        public Vector3 ColliderTopPoint { get; private set; }
        public Vector3 ColliderCenterPoint { get; private set; }
        public Vector3 ColliderBottomPoint { get; private set; }
        public Vector3 OriginPoint { get; private set; }

        public PlayerRootState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }

        public override void EnterState()
        {
            CurrentSubState.EnterState();

            FloatingHeight = maxFloatingHeight;
        }

        public override void UpdateState()
        {
            UpdateContinuousVariable();
        }
        public override void FixedUpdateState()
        {
            GroundSphereCast();
        }

        private void GroundSphereCast()
        {
            Physics.SphereCast(
                ColliderBottomPoint,
                0.01f, Vector3.down,
                out groundHit,
                Mathf.Infinity,
                groundLayers);
        }
        private void UpdateContinuousVariable()
        {
            ColliderCenterPoint = player.transform.TransformPoint(player.mainCollider.center);
            ColliderBottomPoint = ColliderCenterPoint - 0.5f * player.ColliderHeight * Vector3.up;
            ColliderTopPoint = ColliderCenterPoint + 0.5f * player.ColliderHeight * Vector3.up;

            distanceToGround = ColliderBottomPoint.y - groundHit.point.y;
            OriginPoint = ColliderBottomPoint - maxFloatingHeight * Vector3.up;

            if (distanceToGround <= ((CurrentSubState?.StateKey == PlayerStateMachine.EState.Grounded) ?
                                    maxFloatingHeight + player.groundedState.MaxStairHeight : maxFloatingHeight))
                IsGrounded = true;
            else
                IsGrounded = false;
        }
    }
}