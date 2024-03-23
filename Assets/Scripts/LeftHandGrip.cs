using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandGrip : MonoBehaviour
{
    public Transform HandGrip;

    Animator myAnim;

    private void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        myAnim.SetIKPosition(AvatarIKGoal.LeftHand, HandGrip.position);
        myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        myAnim.SetIKRotation(AvatarIKGoal.LeftHand, HandGrip.rotation);
        myAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        
    }
}
