using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static VkevLibrary;

namespace PlayerStateMachine
{
    public class PlayerRunState : BaseState<PlayerStateMachine.EState>
    {
        PlayerStateMachine player;
        Coroutine accelerateCoroutine;
        Coroutine decelerateCoroutine;

        public PlayerRunState(PlayerStateMachine.EState key, PlayerStateMachine context, int level) : base(key, context, level)
        {
            player = context;
        }

        public override void UpdateState()
        {
            OnRotation();
        }
        public override void FixedUpdateState()
        {
            OnMove();
        }
        private void OnRotation()
        {
            Vector3 moveDir;
            moveDir = new Vector3(player.walkInput.x, 0, player.walkInput.y);

            Vector3 rotationDir = Quaternion.AngleAxis(player.mainCamera.transform.rotation.eulerAngles.y, Vector3.up) * moveDir;
            Vector3 lookDir = Vector3.RotateTowards(player.transform.forward, rotationDir, Time.deltaTime * player.RotationSpeed, 0.0f);
            player.transform.rotation = Quaternion.LookRotation(lookDir);
        }
        private void OnMove()
        {
            Vector2 slowRunInput_Lerped = Vector2.Lerp(Vector2.zero, player.walkInput, player.horizontalVelocityPercentage);

            Vector3 moveDir;
            moveDir = new Vector3(slowRunInput_Lerped.x, 0, slowRunInput_Lerped.y);
            moveDir = new Vector3(moveDir.x, player.rigid.velocity.y, moveDir.z);
            moveDir = Quaternion.AngleAxis(player.mainCamera.transform.eulerAngles.y, Vector3.up) * moveDir * player.MoveSpeed * Time.fixedDeltaTime;

            player.rigid.velocity = new Vector3(moveDir.x, player.rigid.velocity.y, moveDir.z);
        }

        public override void EnterState()
        {
            InputController.WalkAction.AddPerformed(WalkActionPerform);
            InputController.WalkAction.AddCanceled(WalkActionCanceled);
            InputController.RunAction.AddPerformed(RunActionPerformed);
            InputController.RunAction.AddCanceled(RunActionCanceled);
            accelerateCoroutine = player.StartCoroutine(Accelerate(player.velocityPercentageThreshold));

            player.rigid.velocity = new Vector3(0, player.rigid.velocity.y, 0);
        }
        public override void ExitState()
        {
            InputController.WalkAction.RemovePerformed(WalkActionPerform);
            InputController.WalkAction.RemoveCanceled(WalkActionCanceled);
            InputController.RunAction.RemovePerformed(RunActionPerformed);
            InputController.RunAction.RemoveCanceled(RunActionCanceled);

            StopCoroutine(accelerateCoroutine, player);
            StopCoroutine(decelerateCoroutine, player);
        }
        private void WalkActionPerform(InputAction.CallbackContext context)
        {
            StopCoroutine(decelerateCoroutine, player);
            StopCoroutine(accelerateCoroutine, player);
            accelerateCoroutine = player.StartCoroutine(Accelerate(player.velocityPercentageThreshold));
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

            player.velocityPercentageThreshold = 1f;
            accelerateCoroutine = player.StartCoroutine(Accelerate(player.velocityPercentageThreshold));
        }
        private void RunActionCanceled(InputAction.CallbackContext context)
        {
            StopCoroutine(accelerateCoroutine, player);

            player.velocityPercentageThreshold = player.WalkRunSpeedRatio;

            if (InputController.WalkAction.isPressed && player.horizontalVelocityPercentage >= player.WalkRunSpeedRatio)
            {
                decelerateCoroutine = player.StartCoroutine(Decelerate(player.velocityPercentageThreshold));
            }
            else if (player.horizontalVelocityPercentage < player.WalkRunSpeedRatio)
            {
                accelerateCoroutine = player.StartCoroutine(Accelerate(player.velocityPercentageThreshold));
            }
        }
        public IEnumerator Accelerate(float maxAcceleratePercentage)
        {
            while (InputController.WalkAction.isPressed)
            {
                player.walkInput = InputController.WalkAction.action.ReadValue<Vector2>();
                player.horizontalVelocityPercentage += Time.deltaTime * player.Acceleration;


                if (player.horizontalVelocityPercentage >= maxAcceleratePercentage)
                {
                    player.horizontalVelocityPercentage = maxAcceleratePercentage;
                    break;
                }
                yield return null;
            }
            if (player.horizontalVelocityPercentage <= 0)
            {
                TransitionToState(PlayerStateMachine.EState.Idle);
            }

        }
        public IEnumerator Decelerate(float minDeceleratePercentage)
        {
            while (true)
            {
                player.horizontalVelocityPercentage -= Time.deltaTime * player.Deceleration;

                if (player.horizontalVelocityPercentage < minDeceleratePercentage)
                {
                    player.horizontalVelocityPercentage = minDeceleratePercentage;
                    break;
                }
                yield return null;
            }
            if (player.horizontalVelocityPercentage <= 0)
            {
                TransitionToState(PlayerStateMachine.EState.Idle);
            }
        }
    }
}