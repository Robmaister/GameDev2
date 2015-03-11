using UnityEngine;
using System;
using System.Collections;

public class IK_Script : MonoBehaviour {
	Animator anim;

	public bool ikActive = false;
	public Transform RHand,RArm,LHand,LArm,lookAt = null;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	void OnAnimatorIK()
	{
		if(anim) {
			
			//if the IK is active, set the position and rotation directly to the goal. 
			if(ikActive) {
				
				// Set the look target position, if one has been assigned
				if(lookAt != null) {
					anim.SetLookAtWeight(1);
					anim.SetLookAtPosition(lookAt.position);
				}    
				
				// Set the right hand target position and rotation, if one has been assigned
				if(RHand != null) {
					anim.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
					anim.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
					anim.SetIKPosition(AvatarIKGoal.RightHand,RHand.position);
					anim.SetIKRotation(AvatarIKGoal.RightHand,RHand.rotation);
				}      

				if(LHand != null) {
					anim.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
					anim.SetIKRotationWeight(AvatarIKGoal.LeftHand,1);  
					anim.SetIKPosition(AvatarIKGoal.LeftHand,LHand.position);
					anim.SetIKRotation(AvatarIKGoal.LeftHand,LHand.rotation);
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
	// Update is called once per frame
	void Update () {
	
	}
}
