using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static VkevLibrary;

namespace PlayerStateMachine
{
    [Serializable]
    public class PlayerRunState : BaseState<PlayerStateMachine.EState>
    {
        PlayerStateMachine player;
        Coroutine accelerateCoroutine;
        Coroutine decelerateCoroutine;

        [HideInInspector] public float horizontalVelocityPercentage { get; private set; }
        [HideInInspector] public float velocityPercentageThreshold { get; private set; }

        [HideInInspector] public Vector2 walkInput;

        public PlayerRunState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }
        public override void EnterState()
        {
            InputController.WalkAction.AddPerformed(WalkActionPerform);
            InputController.WalkAction.AddCanceled(WalkActionCanceled);
            InputController.RunAction.AddPerformed(RunActionPerformed);
            InputController.RunAction.AddCanceled(RunActionCanceled);

            velocityPercentageThreshold = player.groundedState.WalkRunSpeedRatio;
            if (InputController.RunAction.isPressed)
                velocityPercentageThreshold = 1f;

            accelerateCoroutine = player.StartCoroutine(Accelerate(velocityPercentageThreshold));

            player.rigid.linearVelocity = new Vector3(0, player.rigid.linearVelocity.y, 0);
        }
        public override void ExitState()
        {
            InputController.WalkAction.RemovePerformed(WalkActionPerform);
            InputController.WalkAction.RemoveCanceled(WalkActionCanceled);
            InputController.RunAction.RemovePerformed(RunActionPerformed);
            InputController.RunAction.RemoveCanceled(RunActionCanceled);

            horizontalVelocityPercentage = 0f;

            StopCoroutine(accelerateCoroutine, player);
            StopCoroutine(decelerateCoroutine, player);
        }
        public override void UpdateState()
        {
            OnRotation();
            player.anim.SetFloat(PlayerStateMachine.HORIZONTAL_VELOCITY_PERCENTAGE, horizontalVelocityPercentage);
        }
        public override void FixedUpdateState()
        {
            OnMove();
        }
        private void OnRotation()
        {
            Vector3 moveDir;
            moveDir = new Vector3(walkInput.x, 0, walkInput.y);

            Vector3 rotationDir = Quaternion.AngleAxis(player.mainCamera.transform.rotation.eulerAngles.y, Vector3.up) * moveDir;
            Vector3 lookDir = Vector3.RotateTowards(player.transform.forward, rotationDir, Time.deltaTime * player.groundedState.RotationSpeed, 0.0f);
            player.transform.rotation = Quaternion.LookRotation(lookDir);
        }
        private void OnMove()
        {
            Vector2 slowRunInput_Lerped = Vector2.Lerp(Vector2.zero, walkInput, horizontalVelocityPercentage);

            Vector3 moveDir;
            moveDir = new Vector3(slowRunInput_Lerped.x, 0, slowRunInput_Lerped.y);
            moveDir = new Vector3(moveDir.x, player.rigid.linearVelocity.y, moveDir.z);
            moveDir = Quaternion.AngleAxis(player.mainCamera.transform.eulerAngles.y, Vector3.up) * moveDir * player.groundedState.MoveSpeed * Time.fixedDeltaTime;

            player.rigid.linearVelocity = new Vector3(moveDir.x, player.rigid.linearVelocity.y, moveDir.z);
        }

        private void WalkActionPerform(InputAction.CallbackContext context)
        {
            StopCoroutine(decelerateCoroutine, player);
            StopCoroutine(accelerateCoroutine, player);
            accelerateCoroutine = player.StartCoroutine(Accelerate(velocityPercentageThreshold));
        }
        private void WalkActionCanceled(InputAction.CallbackContext context)
        {
            StopCoroutine(decelerateCoroutine, player);
            StopCoroutine(accelerateCoroutine, player);

            decelerateCoroutine = player.StartCoroutine(Decelerate(0f));
        }
        private void RunActionPerformed(InputAction.CallbackContext context)
        {
            StopCoroutine(accelerateCoroutine, player);

            velocityPercentageThreshold = 1f;
            accelerateCoroutine = player.StartCoroutine(Accelerate(velocityPercentageThreshold));
        }
        private void RunActionCanceled(InputAction.CallbackContext context)
        {
            StopCoroutine(accelerateCoroutine, player);

            velocityPercentageThreshold = player.groundedState.WalkRunSpeedRatio;

            if (InputController.WalkAction.isPressed && horizontalVelocityPercentage >= player.groundedState.WalkRunSpeedRatio)
            {
                decelerateCoroutine = player.StartCoroutine(Decelerate(velocityPercentageThreshold));
            }
            else if (horizontalVelocityPercentage < player.groundedState.WalkRunSpeedRatio)
            {
                accelerateCoroutine = player.StartCoroutine(Accelerate(velocityPercentageThreshold));
            }
        }
        public IEnumerator Accelerate(float maxAcceleratePercentage)
        {
            while (InputController.WalkAction.isPressed)
            {
                walkInput = InputController.WalkAction.action.ReadValue<Vector2>();
                horizontalVelocityPercentage += Time.deltaTime * player.groundedState.Acceleration;


                if (horizontalVelocityPercentage >= maxAcceleratePercentage)
                {
                    horizontalVelocityPercentage = maxAcceleratePercentage;
                    break;
                }
                yield return null;
            }
            if (horizontalVelocityPercentage <= 0)
            {
                TransitionToState(PlayerStateMachine.EState.Idle);
            }

        }
        public IEnumerator Decelerate(float minDeceleratePercentage)
        {
            while (true)
            {
                horizontalVelocityPercentage -= Time.deltaTime * player.groundedState.Deceleration;

                if (horizontalVelocityPercentage < minDeceleratePercentage)
                {
                    horizontalVelocityPercentage = minDeceleratePercentage;
                    break;
                }
                yield return null;
            }
            if (horizontalVelocityPercentage <= 0)
            {
                TransitionToState(PlayerStateMachine.EState.Idle);
            }
        }
    }
}