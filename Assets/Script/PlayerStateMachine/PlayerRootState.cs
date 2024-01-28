using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerStateMachine
{
    [Serializable]
    public class PlayerRootState : BaseState<PlayerStateMachine.EState>
    {
        PlayerStateMachine player;

        [Header("FLOATING CAPSULE")]
        [SerializeField] private Vector3 obstacleDetectionOffset = new Vector3(0, 0, 1f);
        [SerializeField] private Vector3 obstacleHalfExtend = new Vector3(0.15f, 0.01f, 0.2f);
        [SerializeField] private float maxFloatingHeight;


        [SerializeField] private LayerMask groundLayers;
        public LayerMask GroundLayers { get { return groundLayers; } }


        private float distanceToGround;
        public float DistanceToGround { get { return distanceToGround; } }

        private RaycastHit groundHit;
        public RaycastHit GroundHit { get { return groundHit; } }



        public Vector3 ColliderTopPoint { get; private set; }
        public Vector3 ColliderCenterPoint { get; private set; }
        public Vector3 ColliderBottomPoint { get; private set; }
        public Vector3 OriginPoint { get; private set; }

        public bool IsGrounded { get; private set; }
        public bool IsObstacle { get; private set; }
        public float FloatingHeight { get; private set; }
        public float DetectedObstacleHeight { get; private set; }

        public PlayerRootState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }

        public override void EnterState()
        {
            CurrentSubState.EnterState();

            FloatingHeight = maxFloatingHeight;
            obstacleDetectionOffset.z *= player.ColliderRadius;
            obstacleDetectionOffset.z += obstacleHalfExtend.z;
        }

        public override void UpdateState()
        {
            UpdateContinuousVariable();
        }
        public override void FixedUpdateState()
        {
            GroundSphereCast();
            ObstacleDetection();
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

        private void ObstacleDetection()
        {
            if (IsObstacle = Physics.BoxCast(
                ColliderTopPoint + player.transform.forward * obstacleDetectionOffset.z
                + new Vector3(obstacleDetectionOffset.x, obstacleDetectionOffset.y, 0f),
                obstacleHalfExtend, Vector3.down,
                out RaycastHit hitInfo, player.transform.rotation,
                player.ColliderHeight + maxFloatingHeight - player.groundedState.MaxStairHeight, GroundLayers))
            {
                DetectedObstacleHeight = hitInfo.point.y - OriginPoint.y;
                Debug.DrawLine(hitInfo.point, hitInfo.point + Vector3.up * (ColliderTopPoint.y - hitInfo.point.y), Color.red);
            }


        }

        public override void OnDrawGizmos()
        {
            Gizmos.matrix = player.transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(player.transform.InverseTransformPoint(
                                ColliderTopPoint
                                + player.transform.forward * obstacleDetectionOffset.z
                                + new Vector3(obstacleDetectionOffset.x, obstacleDetectionOffset.y, 0f)),
                                2 * obstacleHalfExtend);
        }
    }
}