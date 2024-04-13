using System;
using UnityEngine;

namespace PlayerStateMachine
{
    [Serializable]
    public class PlayerRootState : BaseState<PlayerStateMachine.EState>
    {
        PlayerStateMachine player;

        [Header("FLOATING CAPSULE")]
        [SerializeField] private BoxcastProperties obstacleDetectionHorizontal = new BoxcastProperties(Vector3.zero, new Vector3(0.15f, 0.01f, 0.2f), 1f);
        [SerializeField] private float maxFloatingHeight;

        [Header("PROCEDURAL ANIMATION")]
        [SerializeField] public PlayerLegsProcedural legsProcedural = new PlayerLegsProcedural();


        [Header("LAYERS MASK")]
        [SerializeField] private LayerMask groundLayers;
        public LayerMask GroundLayers { get { return groundLayers; } }


        private float distanceToGround;
        public float DistanceToGround { get { return distanceToGround; } }

        public HitInfo GroundRay { get; private set; } = new HitInfo();
        public HitInfo Horizontal_ObstacleRay { get; private set; } = new HitInfo();
        public HitInfo Vertical_ObstacleRay { get; private set; } = new HitInfo();

        public Vector3 ColliderTopPoint { get; private set; }
        public Vector3 ColliderCenterPoint { get; private set; }
        public Vector3 ColliderBottomPoint { get; private set; }
        public Vector3 OriginPoint { get; private set; }

        public bool IsGrounded { get; private set; }
        public float FloatingHeight { get; private set; }

        public PlayerRootState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }

        public override void EnterState()
        {
            CurrentSubState.EnterState();

            FloatingHeight = maxFloatingHeight;
            legsProcedural.Init(player.anim, player.rootState.groundLayers);
        }

        public override void UpdateState()
        {
            UpdateContinuousVariable();

        }
        public override void FixedUpdateState()
        {
            GroundSphereCast();
            ObstacleDetection();
            legsProcedural.SphereCast();
        }

        public override void OnAnimationIK(int layerIndex)
        {
            legsProcedural.GetAnimationIK();
        }

        private void GroundSphereCast()
        {
            GroundRay.isHit = Physics.SphereCast(
                ColliderBottomPoint,
                0.01f, Vector3.down,
                out GroundRay.hit,
                Mathf.Infinity,
                groundLayers);
        }
        private void UpdateContinuousVariable()
        {
            ColliderCenterPoint = player.transform.TransformPoint(player.mainCollider.center);
            ColliderBottomPoint = ColliderCenterPoint - 0.5f * player.ColliderHeight * Vector3.up;
            ColliderTopPoint = ColliderCenterPoint + 0.5f * player.ColliderHeight * Vector3.up;

            distanceToGround = ColliderBottomPoint.y - GroundRay.hit.point.y;
            OriginPoint = ColliderBottomPoint - FloatingHeight * Vector3.up;

            if (distanceToGround <= ((CurrentSubState?.StateKey == PlayerStateMachine.EState.Grounded) ?
                                    FloatingHeight + player.groundedState.MaxStairHeight : FloatingHeight))
                IsGrounded = true;
            else
                IsGrounded = false;
        }

        private void ObstacleDetection()
        {
            Vector3 originUpper = OriginPoint + player.groundedState.MaxStairHeight * Vector3.up;
            Vector3 forwardOffset = +obstacleDetectionHorizontal.halfExtend.z * player.transform.forward;
            obstacleDetectionHorizontal.origin = originUpper + forwardOffset;

            if (Horizontal_ObstacleRay.isHit = Physics.BoxCast(
                obstacleDetectionHorizontal.origin,
                obstacleDetectionHorizontal.halfExtend,
                player.transform.forward,
                out Horizontal_ObstacleRay.hit,
                player.transform.rotation,
                obstacleDetectionHorizontal.distance,
                groundLayers))
            {
                Debug.DrawLine(Horizontal_ObstacleRay.hit.point, Horizontal_ObstacleRay.hit.point + (ColliderTopPoint - OriginPoint - player.groundedState.MaxStairHeight * Vector3.up), Color.red);

                float sphereCastDistance = ColliderTopPoint.y - originUpper.y;
                float radius = 0.01f;
                Vector3 origin = Horizontal_ObstacleRay.hit.point + new Vector3(0, sphereCastDistance, 0);
                if (Vertical_ObstacleRay.isHit = Physics.SphereCast(
                                origin,
                                radius,
                                Vector3.down,
                                out Vertical_ObstacleRay.hit,
                                sphereCastDistance,
                                groundLayers))
                {
                    Debug.DrawLine(Vertical_ObstacleRay.hit.point, Vertical_ObstacleRay.hit.point + Vector3.up * (ColliderTopPoint.y - Vertical_ObstacleRay.hit.point.y), Color.magenta);
                }
            }



        }

        public override void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Gizmos.matrix = player.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(player.transform.InverseTransformPoint(OriginPoint + player.groundedState.MaxStairHeight * Vector3.up + 0.5f * (obstacleDetectionHorizontal.distance + obstacleDetectionHorizontal.halfExtend.z) * player.transform.forward),
                                 new Vector3(2f * obstacleDetectionHorizontal.halfExtend.x, 2f * obstacleDetectionHorizontal.halfExtend.y, obstacleDetectionHorizontal.distance + obstacleDetectionHorizontal.halfExtend.z));


            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.green;
            Vector3 offset = player.transform.forward * 0.1f;
            Gizmos.DrawCube(legsProcedural.rightFoot.position + legsProcedural.rightFoot.SphereCastOffset - (1 / 2f) * Vector3.up * legsProcedural.rightFoot.sphereCastDistance + offset, new Vector3(0.02f, legsProcedural.rightFoot.sphereCastDistance, 0.02f));
            Gizmos.DrawCube(legsProcedural.leftFoot.position + legsProcedural.leftFoot.SphereCastOffset - (1 / 2f) * Vector3.up * legsProcedural.leftFoot.sphereCastDistance + offset, new Vector3(0.02f, legsProcedural.leftFoot.sphereCastDistance, 0.02f));
            Gizmos.DrawSphere(legsProcedural.rightFoot.groundRay.point, 0.1f);
            Gizmos.DrawSphere(legsProcedural.leftFoot.groundRay.point, 0.1f);
        }
    }

    [Serializable]
    public class BoxcastProperties
    {
        [HideInInspector] public Vector3 origin;
        public Vector3 halfExtend;
        public float distance;

        public BoxcastProperties(Vector3 origin, Vector3 halfExtend, float distance)
        {
            this.origin = origin;
            this.halfExtend = halfExtend;
            this.distance = distance;
        }
    }

    public class HitInfo
    {
        public bool isHit;
        public RaycastHit hit;
    }
}