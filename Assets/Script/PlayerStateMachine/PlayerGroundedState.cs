using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerStateMachine
{
    [Serializable]
    public class PlayerGroundedState : BaseState<PlayerStateMachine.EState>
    {

        PlayerStateMachine player;

        #region HORIZONTAL MOVEMENT
        [Header("HORIZONTAL MOVEMENT")]
        [SerializeField] private float moveSpeed = 500f;
        [SerializeField] private float rotationSpeed = 10f;

        [Space(10)]
        [SerializeField] private float acceleration = 2f;
        [SerializeField] private float deceleration = 2f;
        [SerializeField] private float walkRunSpeedRatio = 0.5f;

        [Space(10)]
        [SerializeField] private float maxSlopeAngle = 50f;
        [SerializeField] private float maxStairHeight = 0.4f;
        [SerializeField] private float maxRoughSurfaceHeight = 0.05f;
        public float MoveSpeed { get { return moveSpeed; } }
        public float RotationSpeed { get { return rotationSpeed; } }
        public float Acceleration { get { return acceleration; } }
        public float Deceleration { get { return deceleration; } }
        public float WalkRunSpeedRatio { get { return walkRunSpeedRatio; } }
        public float MaxSlopeAngle { get { return maxSlopeAngle; } }
        public float MaxStairHeight { get { return maxStairHeight; } }
        public float MaxRoughSurfaceHeight { get { return maxRoughSurfaceHeight; } }
        #endregion

        #region PARKOUR
        [Header("PARKOUR MOVEMENT")]
        [SerializeField] private List<ParkourDefaultAction> parkourActions = new List<ParkourDefaultAction>();
        public ParkourDefaultAction currentParkourAction { get; private set; }
        #endregion

        public PlayerGroundedState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }
        public override void EnterState()
        {
            InputController.JumpAction.AddPerformed(JumpActionPerform);

            player.rigid.linearVelocity = new Vector3(player.rigid.linearVelocity.x, 0f, player.rigid.linearVelocity.z);
            if (CurrentSubState.StateKey == PlayerStateMachine.EState.Idle)
                player.rigid.linearVelocity = Vector3.zero;

        }
        public override void ExitState()
        {
            InputController.JumpAction.RemovePerformed(JumpActionPerform);
        }
        public override void UpdateState()
        {
            HandleStairMovement();
            CheckTransition();
        }
        public override void CheckTransition()
        {
            if (!player.rootState.IsGrounded && CurrentSubState.StateKey != PlayerStateMachine.EState.GroundedParkour)
            {
                TransitionToState(PlayerStateMachine.EState.InAir);
            }
        }

        private float positionOffsetY_SmoothDamp;
        private void HandleStairMovement()
        {
            if (CurrentSubState.StateKey == PlayerStateMachine.EState.GroundedParkour)
                return;

            float changeDistance = Mathf.Abs(player.rootState.DistanceToGround - player.rootState.FloatingHeight);
            if (changeDistance < maxStairHeight && changeDistance > maxRoughSurfaceHeight
                && CurrentSubState.StateKey != PlayerStateMachine.EState.Slide)
            {
                float positionOffsetY = 0f;
                float floatingToDistance = player.rootState.FloatingHeight - player.rootState.DistanceToGround;

                positionOffsetY = Mathf.SmoothDamp(positionOffsetY, floatingToDistance,
                                                   ref positionOffsetY_SmoothDamp, Time.deltaTime * 3f);
                player.transform.position = player.transform.position + new Vector3(0, positionOffsetY, 0);

            }
        }

        private void JumpActionPerform(InputAction.CallbackContext context)
        {
            if (player.rootState.Horizontal_ObstacleRay.isHit && CurrentSubState?.StateKey != PlayerStateMachine.EState.GroundedParkour)
            {
                if (parkourActions.Count == 0)
                {
                    Debug.LogWarning("No Parkour action available!");
                    return;
                }
                foreach (ParkourDefaultAction parkourAction in parkourActions)
                {
                    float obstacleHeight = player.rootState.Vertical_ObstacleRay.hit.point.y - player.rootState.OriginPoint.y;

                    Plane plane = new Plane(player.transform.forward, player.transform.position);
                    float obstacleDistance = plane.GetDistanceToPoint(player.rootState.Vertical_ObstacleRay.hit.point);
                    Debug.Log(obstacleHeight + "," + obstacleDistance);
                    if (parkourAction.action.CanParkour(obstacleHeight, obstacleDistance, CurrentSubState.StateKey))
                    {
                        currentParkourAction = parkourAction;
                        CurrentSubState.TransitionToState(PlayerStateMachine.EState.GroundedParkour);
                        return;
                    }
                }

            }
        }
    }
}