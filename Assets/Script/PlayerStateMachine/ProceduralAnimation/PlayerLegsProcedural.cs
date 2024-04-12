using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
namespace PlayerStateMachine
{

    [Serializable]
    public class PlayerLegsProcedural
    {
        [Header("Foot Replacement")]
        public float weightDecreaseTime = 0.1f;
        public float weightIncreaseTime = 0.5f;
        public float replacementSpeed = 25f;
        [field: SerializeField] public FootIKProperties rightFoot { get; private set; } = new FootIKProperties();
        [field: SerializeField] public FootIKProperties leftFoot { get; private set; } = new FootIKProperties();

        private Animator anim;
        private LayerMask groundLayer;

        public void Init(Animator animator, LayerMask layerMask)
        {
            anim = animator;
            groundLayer = layerMask;
            leftFoot.rotateOffset = leftFoot.rotateIK.data.offset;
            rightFoot.rotateOffset = rightFoot.rotateIK.data.offset;
        }

        public void SphereCast()
        {
            SphereCast(leftFoot);
            SphereCast(rightFoot);
        }

        public void GetAnimationIK()
        {
            rightFoot.position = anim.GetIKPosition(AvatarIKGoal.RightFoot);
            rightFoot.rotation = anim.GetIKRotation(AvatarIKGoal.RightFoot);

            leftFoot.position = anim.GetIKPosition(AvatarIKGoal.LeftFoot);
            leftFoot.rotation = anim.GetIKRotation(AvatarIKGoal.LeftFoot);
        }

        private void SphereCast(FootIKProperties foot)
        {
            foot.isGrounded = Physics.SphereCast(foot.position + foot.SphereCastOffset, foot.sphereCastRadius, Vector3.down, out foot.groundRay, foot.sphereCastDistance, groundLayer);
        }

        private float currentVelocity1;
        private float currentVelocity2;
        private void FootReplacement(FootIKProperties foot)
        {
            if (foot.isGrounded)
            {
                foot.twoBoneIK.data.target.position = Vector3.Lerp(foot.twoBoneIK.data.target.position, foot.groundRay.point + foot.replacementOffset, Time.deltaTime * replacementSpeed);

                float angleX = Vector3.Angle(foot.twoBoneIK.data.target.transform.up, foot.groundRay.normal);
                float angleY = Vector3.Angle(foot.twoBoneIK.data.target.transform.right, foot.groundRay.normal);
                foot.rotateIK.data.offset = new Vector3(angleX - 90 + foot.rotateOffset.x, 90 - angleY + foot.rotateOffset.y, foot.rotateOffset.z);

                if (foot.twoBoneIK.data.target.position.y - (foot.groundRay.point.y + foot.replacementOffset.y) < -0.05f)
                {
                    foot.twoBoneIK.weight = Mathf.SmoothDamp(foot.twoBoneIK.weight, 1f, ref currentVelocity1, 0.02f);
                    foot.rotateIK.weight = Mathf.SmoothDamp(foot.twoBoneIK.weight, 1f, ref currentVelocity2, 0.02f);
                }
                else
                {
                    foot.twoBoneIK.weight = Mathf.SmoothDamp(foot.twoBoneIK.weight, 1f, ref currentVelocity1, weightIncreaseTime);
                    foot.rotateIK.weight = Mathf.SmoothDamp(foot.twoBoneIK.weight, 1f, ref currentVelocity2, weightIncreaseTime);
                }
            }
            else
            {
                foot.twoBoneIK.weight = Mathf.SmoothDamp(foot.twoBoneIK.weight, 0f, ref currentVelocity1, weightDecreaseTime);
                foot.rotateIK.weight = Mathf.SmoothDamp(foot.twoBoneIK.weight, 0f, ref currentVelocity2, weightDecreaseTime);
            }
        }
        public void FootReplacement()
        {
            FootReplacement(leftFoot);
            FootReplacement(rightFoot);
        }
        public void ResetWeight()
        {
            leftFoot.twoBoneIK.weight = 0f;
            rightFoot.twoBoneIK.weight = 0f;
        }

        [Serializable]
        public class FootIKProperties
        {
            [HideInInspector] public Vector3 position;
            [HideInInspector] public Quaternion rotation;

            [HideInInspector] public RaycastHit groundRay;
            [HideInInspector] public bool isGrounded;

            public TwoBoneIKConstraint twoBoneIK;

            public MultiRotationConstraint rotateIK;
            [HideInInspector] public Vector3 rotateOffset;

            public Vector3 SphereCastOffset;
            public float sphereCastRadius;
            public float sphereCastDistance;

            public Vector3 replacementOffset;
        }
    }
}