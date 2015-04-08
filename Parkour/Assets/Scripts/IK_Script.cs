using UnityEngine;
using System;
using System.Collections;

public class IK_Script : MonoBehaviour {
	Animator anim;
	public float IKWeight;
	public bool arm_ik_active = false;
	public bool head_ik_active = true;
	public Transform RHandObj,LHandObj,lookAt = null,RElbowObj,LElbowObj;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	void OnAnimatorIK()
	{
		if(anim) {
			
			//if the IK is active, set the position and rotation directly to the goal. 
			if(head_ik_active) {
				
				// Set the look target position, if one has been assigned
				if(lookAt != null) {
					anim.SetLookAtWeight(Mathf.Clamp(IKWeight,0f,1f));
					anim.SetLookAtPosition(lookAt.position);
				}  
			}
			if(arm_ik_active){
				
				// Set the right hand target position and rotation, if one has been assigned
				if(RHandObj != null) {
					anim.SetIKPositionWeight(AvatarIKGoal.RightHand,IKWeight);
					anim.SetIKRotationWeight(AvatarIKGoal.RightHand,IKWeight);  
					anim.SetIKPosition(AvatarIKGoal.RightHand,RHandObj.position);
					anim.SetIKRotation(AvatarIKGoal.RightHand,RHandObj.rotation);
				}      

				if(LHandObj != null) {
					anim.SetIKPositionWeight(AvatarIKGoal.LeftHand,IKWeight);
					anim.SetIKRotationWeight(AvatarIKGoal.LeftHand,IKWeight);  
					anim.SetIKPosition(AvatarIKGoal.LeftHand,LHandObj.position);
					anim.SetIKRotation(AvatarIKGoal.LeftHand,LHandObj.rotation);
				}

				if(RElbowObj != null){
					anim.SetIKHintPosition(AvatarIKHint.RightElbow,RElbowObj.position);
					anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow,IKWeight);
				}
				if(LElbowObj != null){
					anim.SetIKHintPosition(AvatarIKHint.LeftElbow,LElbowObj.position);
					anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow,IKWeight);
				}
				
			}
			
			//if the IK is not active, set the position and rotation of the hand and head back to the original position
			else {          
				anim.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
				anim.SetIKRotationWeight(AvatarIKGoal.RightHand,0); 
				anim.SetLookAtWeight(0);
				anim.SetIKPositionWeight(AvatarIKGoal.LeftHand,0);
				anim.SetIKRotationWeight(AvatarIKGoal.LeftHand,0); 
				
			}
		}
	}   
}
