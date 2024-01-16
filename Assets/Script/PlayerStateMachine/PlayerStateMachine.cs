using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlayerStateMachine
{
    public class PlayerStateMachine : StateManager<PlayerStateMachine.EState>
    {
        public enum EState
        {
            Root,
            Grounded, Jump,
            Idle, Run,
            Slide,
        }

        [SerializeField] public PlayerRootState rootState;
        [SerializeField] public PlayerIdleState idleState;
        [SerializeField] public PlayerJumpState jumpState;
        [SerializeField] public PlayerGroundedState groundedState;
        [SerializeField] public PlayerRunState runState;
        [SerializeField] public PlayerSlideState slideState;
        public PlayerStateMachine()
        {
            rootState = new PlayerRootState(EState.Root, this, 0);

            groundedState = new PlayerGroundedState(EState.Grounded, this, 1);
            jumpState = new PlayerJumpState(EState.Jump, this, 1);

            runState = new PlayerRunState(EState.Run, this, 2);
            idleState = new PlayerIdleState(EState.Idle, this, 2);

            slideState = new PlayerSlideState(EState.Slide, this, 3);
        }

        [HideInInspector] public Animator anim;
        [HideInInspector] public Rigidbody rigid;
        [HideInInspector] public Camera mainCamera;
        [HideInInspector] public CapsuleCollider mainCollider;

        public float ColliderHeight { get; private set; }
        public float ColliderRadius { get; private set; }


        public const string VERTICAL_HEIGHT_PERCENTAGE = "Vertical_ClampHeight";
        public const string HORIZONTAL_VELOCITY_PERCENTAGE = "Horizontal_ClampVelocity";


        public void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();
            mainCollider = GetComponent<CapsuleCollider>();

            ColliderHeight = GetComponent<CapsuleCollider>().height * transform.lossyScale.y;
            ColliderRadius = GetComponent<CapsuleCollider>().radius * transform.lossyScale.x;

            mainCamera = Camera.main;
        }

        public void Start()
        {
            States[EState.Root] = rootState;

            States[EState.Grounded] = groundedState;
            States[EState.Jump] = jumpState;

            States[EState.Run] = runState;
            States[EState.Idle] = idleState;

            States[EState.Slide] = slideState;

            CurrentState = States[EState.Root];
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
