using UnityEngine;
using System.Collections;

public class DoParkour : MonoBehaviour {
	//actually do the parkour of the player

	/// Parkour moveset:
	//	Edge: Climb up/mantel, tac, vault
	//	Side: wall run, tac
	//	Top: jumping, walking/running/sprinting, 
	
	
	//tac --> if jump while hitting wall, jump off wall at angle
	//climb --> if use hands or feet while hitting wall, climb up wall
	//wall run --> if collide with wall, allow movement along wall, ignoring gravity
	//
	
	
	//run into wall --> jump up wall
	
	
	
	
	//mantle --> if player arms near edge, pull self up and over
	//wall jump --> if player contacts wall, jump off it


	private bool jumpedOnce = false;//flag to prevent multiple jumps up a surface
	private bool mantling = false;

	private ParkourController pkc;

	void Start(){
		pkc = GetComponent<ParkourController>();
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

		//mantle
		if(pkc.inputHands.pressed && (pkc.armState == (SurfaceType.top | SurfaceType.side))){//if player arms are on top and side specifically
			//print("mantling: " + pkc.armState);
			pkc.inputHands.pressed = false;
			//if(!mantling){
				//pkc.controller.Move(-pkc.controller.velocity  * Time.deltaTime);
				mantling = true;
				pkc.apply_forces = false;
				pkc.addImpulse(pkc.transform.up * .5f,.1f,true);
				pkc.addImpulse(pkc.transform.forward * .25f,.2f,true);
			//}
		}


		//if arms and top --> apply upwards force
	}
}
