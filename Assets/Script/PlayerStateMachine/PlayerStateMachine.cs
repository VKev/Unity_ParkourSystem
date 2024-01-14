using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlayerStateMachine
{
    public class PlayerStateMachine : StateManager<PlayerStateMachine.EState>
    {
        public enum EState
        {
            Grounded, Jump,
            Idle, Run,
            Slide,
        }

        [HideInInspector] public Animator anim;
        [HideInInspector] public Rigidbody rigid;
        [HideInInspector] public Camera mainCamera;

        [Space(10)]
        [Header("HORIZONTAL MOVEMENT")]
        [SerializeField] private float moveSpeed = 230f;
        [SerializeField] private float rotationSpeed = 10f;
        [Space(10)]
        [SerializeField] private float acceleration = 2f;
        [SerializeField] private float deceleration = 2f;
        [SerializeField] private float walkRunSpeedRatio = 0.5f;
        [Space(10)]
        [SerializeField] private float maxSlopeAngle = 50f;
        [SerializeField] private float maxStairHeight = 0.25f;
        [SerializeField] private float maxRoughSurfaceHeight = 0.05f;
        public float MoveSpeed { get { return moveSpeed; } }
        public float RotationSpeed { get { return rotationSpeed; } }
        public float Acceleration { get { return acceleration; } }
        public float Deceleration { get { return deceleration; } }
        public float WalkRunSpeedRatio { get { return walkRunSpeedRatio; } }
        public float MaxSlopeAngle { get { return maxSlopeAngle; } }
        public float MaxStairHeight { get { return maxStairHeight; } }
        public float MaxRoughSurfaceHeight { get { return maxRoughSurfaceHeight; } }

        [HideInInspector] public float horizontalVelocityPercentage;
        [HideInInspector] public float velocityPercentageThreshold;

        [HideInInspector] public Vector2 walkInput;

        [HideInInspector] public bool isSlope;

        public const string VERTICAL_HEIGHT_PERCENTAGE = "Vertical_ClampHeight";
        public const string HORIZONTAL_VELOCITY_PERCENTAGE = "Horizontal_ClampVelocity";

        public void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();

            mainCamera = Camera.main;

        }

        public void Start()
        {
            States[EState.Grounded] = new PlayerGroundedState(EState.Grounded, this, 0);
            States[EState.Jump] = new PlayerJumpState(EState.Jump, this, 0);

            States[EState.Run] = new PlayerRunState(EState.Run, this, 1);
            States[EState.Idle] = new PlayerIdleState(EState.Idle, this, 1);

            States[EState.Slide] = new PlayerSlideState(EState.Slide, this, 2);

            CurrentState = States[EState.Grounded];
            CurrentState.EnterStates(CurrentState);
        }

        public void Update()
        {
            CurrentState.UpdateStates(CurrentState);

            Debug.Log(CurrentState.GetAllCurrentStatesToString(CurrentState));
        }
        public void FixedUpdate()
        {
            CurrentState.FixedUpdateStates(CurrentState);
        }
    }
}
