using System;
using UnityEngine;

[Serializable]
public class PlayerLegsProcedural
{
    private FootIKProperties rightFootIK;
    private FootIKProperties leftFootIK;

    private Animator anim;

    public void SetAnimator(Animator animator)
    {
        anim = animator;
    }

    public void LateUpdate()
    {

    }
    public void GetAnimationIK()
    {
        rightFootIK.position = anim.GetIKPosition(AvatarIKGoal.LeftFoot);
        rightFootIK.rotation = anim.GetIKRotation(AvatarIKGoal.LeftFoot);

        leftFootIK.position = anim.GetIKPosition(AvatarIKGoal.RightFoot);
        leftFootIK.rotation = anim.GetIKRotation(AvatarIKGoal.RightFoot);
    }


    [Serializable]
    class FootIKProperties
    {
        public Vector3 position;
        public Quaternion rotation;
        public bool isGrounded;
    }
}
