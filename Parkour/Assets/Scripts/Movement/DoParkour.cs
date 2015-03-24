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

	
	private bool jumpedOnce = false;//flag to prevent multiple jumps up a surface




	private bool hanging = false;
	private bool mantling = false;

	private ParkourController pkc;

	void Start(){
		pkc = GetComponent<ParkourController>();
	}

	void Update(){

		float horizspeed = Mathf.Sqrt(pkc.controller.velocity.x*pkc.controller.velocity.x + pkc.controller.velocity.z*pkc.controller.velocity.z);

		if (anim != null)
			anim.SetFloat("speed", horizspeed);

		if (iks != null) {
			if (!pkc.apply_forces) {
				iks.ikActive = true;
				l_hand_target.position = new Vector3 ((l_hand_target.position.x + pkc.curEdgeX) / 2, pkc.curEdgey, (l_hand_target.position.z + pkc.curEdgeZ) / 2);

				r_hand_target.position = new Vector3 ((r_hand_target.position.x + pkc.curEdgeX) / 2, pkc.curEdgey, (r_hand_target.position.z + pkc.curEdgeZ) / 2);
			} else {
				iks.ikActive = false;
			}
		}
	}


	void LateUpdate(){
		if(pkc.controller.isGrounded){//reset double jump flag
			jumpedOnce = false;
		}


		//wall jump
		if(pkc.inputJump.pressed && (pkc.legState & SurfaceType.side) != 0){//if player presses jump and has legs touching side
			if(!jumpedOnce){
				print("walljump");
				pkc.can_jump = true;
				jumpedOnce = true;
			}
		}
		//hang from ledge
		if(pkc.inputHands.pressed && !pkc.controller.isGrounded){
			if(pkc.hasEdge){
				//if player arms are on top and side, begin to hang

				//when hanging, player can be up to 1 arms length above or below ledge
				if(!hanging){
					hanging = true;
					pkc.apply_forces = false;

					//assume arm length = 1
					Func<bool> checkfunc1 = null;

					Func<bool> checkfunc2 = delegate(){//function to check if player should pull self up
						//if pressing forward, continue applying upward force unless player is armslength above cury
						if(!hanging){
							pkc.apply_forces = true;
							return true;
						}
						if(Input.GetAxis("Horizontal") != 0){
							pkc.addImpulse(pkc.transform.right * Input.GetAxis("Horizontal"), .05f);
						}

						if(pkc.transform.position.y > pkc.curEdgey - 1){
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

						if(!pkc.hasEdge){
							hanging = false;
							pkc.addImpulse(Input.GetAxis("Vertical")  * pkc.transform.forward * .5f,.1f);
							pkc.apply_forces = true;
							return true;
						}

						if(pkc.transform.position.y <= pkc.curEdgey + 1){
							if(Input.GetAxis("Vertical") > 0){
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
		}



		//mantle or vault
		/*if(pkc.inputHands.pressed && (pkc.armState == (SurfaceType.top | SurfaceType.side))){//if player arms are on top and side specifically

			//bool vaultMode = (pkc.inputJump.pressed && (pkc.legState & SurfaceType.side) != 0);//check if legs are involved


			//print("mantling: " + pkc.armState);
			pkc.inputHands.pressed = false;
			if(!mantling){

				Vector3 temp = pkc.currentMovementOffset;
				temp.y = 0;
				Func<bool> checkfunc = delegate(){
					if (pkc.armState == 0){//if nothing on arms anymore
						mantling = false;
						pkc.apply_forces = true;
						//pkc.currentMovementOffset = temp;
						pkc.addImpulse(pkc.transform.forward * .5f,.1f,true);
						return true;
					}
					return false;
				};

				Action endfunc = delegate {
					mantling = false;
					pkc.apply_forces = true;
				};


				//pkc.controller.Move(-pkc.controller.velocity  * Time.deltaTime);
				mantling = true;
				pkc.apply_forces = false;
				pkc.addImpulse(pkc.transform.up * 3,1f,checkfunc:checkfunc,endfunc:endfunc);

			}
		}*/




		//if arms and top --> apply upwards force
	}
}
