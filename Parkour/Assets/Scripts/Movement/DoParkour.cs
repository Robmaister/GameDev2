﻿using UnityEngine;
using System.Collections;
using System;


public class DoParkour : MonoBehaviour {
	//actually do the parkour of the player

	public Animator anim;
	
	//ik stuff
	public Transform r_hand_target;
	public Transform l_hand_target;


	
	public IK_Script iks;

	public bool tackling = false;

	
	private bool jumpedOnce = false;//flag to prevent multiple jumps up a surface

	private bool vaulting = false;


	private bool hanging = false;
	//private bool mantling = false;

	private ParkourController pkc;

	private Vector3 lhandoffset, rhandoffset;

	void Start(){
		pkc = GetComponent<ParkourController>();

		lhandoffset = l_hand_target.localPosition;
		rhandoffset = r_hand_target.localPosition;
	}

	void LateUpdate(){
		if (iks != null) {
			if (!pkc.apply_forces) {
				
				iks.arm_ik_active = true;
				//print(pkc.current_hang_point);
				//l_hand_target.position = pkc.current_hang_point + lhandoffset;
				//r_hand_target.position = pkc.current_hang_point + rhandoffset;
				l_hand_target.position = pkc.current_hang_point - pkc.current_hang_point_direction_vector_right;
				r_hand_target.position = pkc.current_hang_point + pkc.current_hang_point_direction_vector_right;
				//print(lhandoffset);
				//print (l_hand_target.transform.localPosition);
			}
			else {
				iks.arm_ik_active = false;
			}
		}
		pkc.controller.height = (pkc.controller.height < 1.5f) ? pkc.controller.height +.1f : 1.5f;
		
		Vector3 localVel = transform.InverseTransformDirection(pkc.controller.velocity);
		bool isbkwd = (localVel.z < 0.5f);

		float horizspeed = Mathf.Sqrt(pkc.controller.velocity.x*pkc.controller.velocity.x + pkc.controller.velocity.z*pkc.controller.velocity.z);

		horizspeed = isbkwd ? -horizspeed : horizspeed;
		
		if (anim != null)
			anim.SetFloat("speed", horizspeed);


		if(pkc.controller.isGrounded){//reset double jump flag
			jumpedOnce = false;
		}


		//wall jump
		if(pkc.inputJump.Pressed && (pkc.legState & SurfaceType.side) != 0){//if player presses jump and has legs touching side
			if(!jumpedOnce){
				pkc.can_jump = true;
				jumpedOnce = true;
			}
		}
		//hang from ledge
		if(pkc.inputHands.Pressed){// && !pkc.controller.isGrounded){
			//if(pkc.armState == (SurfaceType.side | SurfaceType.top)){
			if(pkc.current_ledge_object != null){
				//if player arms are on top and side, begin to hang

				//when hanging, player can be up to 1 arms length above or below ledge
				if(!hanging){
					hanging = true;
					pkc.apply_forces = false;

					//anim.SetTrigger("grabbing");
					//anim.MatchTarget(pkc.current_hang_point,Quaternion.identity,AvatarTarget.LeftHand,
					  //               new MatchTargetWeightMask(Vector3.one,1f),0.0f);

					//assume arm length = 1
					Func<bool> checkfunc1 = null;

					Func<bool> checkfunc2 = delegate(){//function to check if player should pull self up
						//if pressing forward, continue applying upward force unless player is armslength above cury
						if(!hanging){
							pkc.apply_forces = true;
							return true;
						}
						//if(Input.GetAxis("Horizontal") != 0){
						if(pkc.networkInputH != 0){
							//4564356456460000000000000333333333333333333000000000000222222222222222200000000000000000000000000000000000000000000000000000001110
							//this needs to be changed so it retargets based on interpolation on edge
							//pkc.addImpulse(pkc.transform.right * Input.GetAxis("Horizontal"), .05f);		

							//the following forces the lateral movement while on a ledge to be constrained to
							//the plane of the ledge
							pkc.addImpulse(
								((Vector3.Dot( pkc.transform.right, pkc.current_hang_point_direction_vector_right.normalized)) 
								* pkc.current_hang_point_direction_vector_right.normalized)
							               * pkc.networkInputH, .05f);

						}

						if(pkc.transform.position.y > pkc.current_hang_point.y - .75f){//-1 because arm length

							//anim.SetTrigger("onTop");

							return false;
						}
						return true;
					};

					Action endfunc2 = delegate {//if the player has dropped too low
						hanging = false;
					};

					checkfunc1 = delegate(){//function to check if player should pull self up
						//if pressing forward, continue applying upward force unless player is armslength above cury
						if(!hanging){
							pkc.apply_forces = true;
							return true;
						}

						//if(!(pkc.armState == (SurfaceType.top | SurfaceType.side))){
						if(pkc.current_ledge_object == null){ // <-- check if hanging has ended
							hanging = false;
							//print ("Topping out");
							//pkc.addImpulse(Input.GetAxis("Vertical")  * pkc.transform.forward * .5f,.1f);
							//pkc.addImpulse(Input.GetAxis("Vertical")  * pkc.transform.up * .5f,.2f);
							pkc.addImpulse(pkc.networkInputV  * pkc.transform.forward * .9f,.1f);
							pkc.addImpulse(pkc.networkInputV  * pkc.transform.up * .9f,.2f);

							pkc.apply_forces = true;
							return true;
						}

						if(pkc.transform.position.y <= pkc.current_hang_point.y + .75f){
							if(pkc.networkInputV > 0){

							//if(Input.GetAxis("Vertical") > 0){
								return false;
							}else{

								pkc.addImpulse(-pkc.transform.up * pkc.gravity/3,-1,checkfunc:checkfunc2,endfunc:endfunc2);
								return true;
							}
						}else{//else if pulled self over ledge
							//pkc.addImpulse(Input.GetAxis("Vertical")  * pkc.transform.forward * .5f,.1f);
							pkc.addImpulse(pkc.networkInputV  * pkc.transform.forward * .5f,.1f);
							pkc.apply_forces = true;
							//hanging = false;
						}
						return true;
					};

					//initialize the hanging process
					//print("hanging start: " + Time.time);
					pkc.addImpulse(pkc.transform.up * 2,-1,checkfunc:checkfunc1);
				}
			}
			//print("hanging");
		}else{//if player isn't pressing hands button while on an edge
			//print("nothanging");
			hanging = false;
			pkc.apply_forces = true;
			//anim.SetTrigger("letGo");
		}


		//vaulting
		//input feet mapped to mouse 2
		if(pkc.inputFeet.Pressed){
			//if(pkc.legState == (SurfaceType.top | SurfaceType.side)){
			if(((pkc.armState & SurfaceType.side) != 0) && anim.GetBool("jumping")&& ((pkc.armState & SurfaceType.top) != 0)){
				if(!vaulting ){
					print ("I AM VAULTING");
					vaulting = true;
					anim.SetTrigger("vaulting");

					pkc.controller.height = .5f;

					Func<bool> checkfunc = delegate {
						if(pkc.controller.isGrounded){
							return true;
						}
						return false;
					};


					Action endfunc = delegate {
						vaulting = false;

					};
				

					pkc.addImpulse(pkc.transform.forward * .03f+pkc.transform.up*.03f,.25f,true,endfunc:endfunc,checkfunc:checkfunc);
				}
				else{
					print("AM I VAULTING?");
					if((pkc.legState & SurfaceType.top) != 0){
						pkc.addImpulse(pkc.transform.up * .03f+ pkc.transform.forward*.03f,.01f,true);
					}
					if (vaulting==false){
						
					}
				}
			}
		}


		if(!tackling){
			if(pkc.inputUse.Pressed){
				tackling = true;
				//print("tackling");
				pkc.apply_forces = false;
				//pkc.canControl = false;
				anim.SetTrigger("tackle");
				pkc.controller.height = .5f;

				pkc.addImpulse(transform.forward * 10,0.05f);
			}
		}else{
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("Get_up") ){

				//pkc.controller.height = (pkc.controller.height < 1.5f) ? pkc.controller.height +.1f : 1.5f;
				if(pkc.controller.height == 1.5f){
					tackling = false;
					pkc.apply_forces = true;
				}
			}
				
		}

		/*if(vaulting){
			if(pkc.armState == 0){
				print("kekking");
				pkc.addImpulse(pkc.transform.forward * .5f,0);
			}
		}*/




		//if arms and top --> apply upwards force
	}


	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Player") {
			//print("COLLIDING WITH PLAYER");
			if(pkc.controller.enabled && col.collider.enabled){
				Physics.IgnoreCollision(pkc.controller, col.collider);
			}
			
			if(tackling){
				anim.SetTrigger("hitPlayer");
				//print("I AM TACKLING AND I COLLIDED WITH PLAYER SO PLAYER SHOULD DROP FLAG");
				col.gameObject.BroadcastMessage("OnFlagDrop");
			}
		}
	}
}
