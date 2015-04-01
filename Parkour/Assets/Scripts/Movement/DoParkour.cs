using UnityEngine;
using System.Collections;
using System;

public class DoParkour : MonoBehaviour {
	//actually do the parkour of the player

	public Animator anim;
	
	//ik stuff
	public Transform r_hand_target;
	public Transform l_hand_target;


	
	public IK_Script iks;

	private bool tackling = false;

	
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

	void Update(){

		float horizspeed = Mathf.Sqrt(pkc.controller.velocity.x*pkc.controller.velocity.x + pkc.controller.velocity.z*pkc.controller.velocity.z);

		if (anim != null)
			anim.SetFloat("speed", horizspeed);



		if (iks != null) {
			if (!pkc.apply_forces) {
		
				iks.arm_ik_active = true;
				//print(pkc.current_hang_point);
				l_hand_target.position = pkc.current_hang_point + lhandoffset;
				r_hand_target.position = pkc.current_hang_point + rhandoffset;
				//print(lhandoffset);
				//print (l_hand_target.transform.localPosition);
			}
			else {
				iks.arm_ik_active = false;
			}
		}
	}


	void LateUpdate(){
		if(pkc.controller.isGrounded){//reset double jump flag
			jumpedOnce = false;
		}


		//wall jump
		if(pkc.inputJump.Pressed && (pkc.legState & SurfaceType.side) != 0){//if player presses jump and has legs touching side
			if(!jumpedOnce){
				print("walljump");
				pkc.can_jump = true;
				jumpedOnce = true;
			}
		}
		//hang from ledge
		if(pkc.inputHands.Pressed && !pkc.controller.isGrounded){
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
						if(Input.GetAxis("Horizontal") != 0){
							//4564356456460000000000000333333333333333333000000000000222222222222222200000000000000000000000000000000000000000000000000000001110
							//this needs to be changed so it retargets based on interpolation on edge
							pkc.addImpulse(pkc.transform.right * Input.GetAxis("Horizontal"), .05f);
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
							pkc.addImpulse(Input.GetAxis("Vertical")  * pkc.transform.forward * .5f,.1f);
							pkc.addImpulse(Input.GetAxis("Vertical")  * pkc.transform.up * .5f,.2f);
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
							pkc.addImpulse(Input.GetAxis("Vertical")  * pkc.transform.forward * .5f,.1f);
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
		if(pkc.inputFeet.Pressed){
			//if(pkc.legState == (SurfaceType.top | SurfaceType.side)){
			if((pkc.armState & SurfaceType.top) != 0){
				if(!vaulting){
					vaulting = true;

					pkc.controller.height = 0;

					Func<bool> checkfunc = delegate {
						if(pkc.controller.isGrounded){
							return true;
						}
						return false;
					};

					/*Func<bool> checkfunc2 = delegate {
						if(pkc.armState == 0){
							return true;
						}
						return false;
					};*/

					Action endfunc = delegate {
						vaulting = false;
						pkc.controller.height = 1.5f;
					};
					//pkc.addImpulse(pkc.transform.forward * .15f,-1f,endfunc:endfunc,checkfunc:checkfunc2);
					pkc.addImpulse(pkc.transform.forward * .05f,1,true,endfunc:endfunc,checkfunc:checkfunc);
				}
				else{
					if((pkc.legState & SurfaceType.top) != 0){
						pkc.addImpulse(pkc.transform.up * .15f,0);
					}
				}
			}
		}

		if(Input.GetKeyDown(KeyCode.E)){
			if(!tackling){
				tackling = true;
				print("tackling");
				pkc.apply_forces = false;
				//pkc.canControl = false;
				anim.SetTrigger("tackle");



				/*Func<bool> checkfunc = delegate {
					if(pkc.controller.isGrounded){
						return true;
					}
					return false;
				};*/

				Action endfunc = delegate {
					tackling = false;
					pkc.apply_forces = true;
					//pkc.canControl = true;
				};

				pkc.addImpulse(transform.forward * 10,0.05f,endfunc:endfunc);
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
}
