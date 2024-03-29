﻿using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PhotonView ) )]
[RequireComponent( typeof( ParkourController ) )]
[AddComponentMenu("Photon Networking/Parkour Controller View")]
public class ParkourControllerView : MonoBehaviour {

	public ParkourController controller;

	/*public float inputH, inputV;
	public bool inputJump, inputHands, inputFeet;
	public Vector3 netImpulse;

	public bool canJump, applyForces;*/

	private Rigidbody rb;

	// Use this for initialization
	void Awake() {
		rb = GetComponent<Rigidbody>();
		controller = GetComponent<ParkourController>();
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(controller.networkInputH);
			stream.SendNext(controller.networkInputV);
			stream.SendNext(controller.inputJump.Pressed);
			stream.SendNext(controller.inputHands.Pressed);
			stream.SendNext(controller.inputFeet.Pressed);
			stream.SendNext(controller.inputSprint.Pressed);
			stream.SendNext(controller.inputUse.Pressed);
			stream.SendNext(controller.inputFlip.Pressed);

			stream.SendNext(controller.can_jump);
			stream.SendNext(controller.apply_forces);

			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
		}
		else {
			controller.networkInputH = (float)stream.ReceiveNext();
			controller.networkInputV = (float)stream.ReceiveNext();
			controller.inputJump.Pressed = (bool)stream.ReceiveNext();
			controller.inputHands.Pressed = (bool)stream.ReceiveNext();
			controller.inputFeet.Pressed = (bool)stream.ReceiveNext();
			controller.inputSprint.Pressed = (bool)stream.ReceiveNext();
			controller.inputUse.Pressed = (bool)stream.ReceiveNext();
			controller.inputFlip.Pressed = (bool)stream.ReceiveNext();
			
			controller.can_jump = (bool)stream.ReceiveNext();
			controller.apply_forces = (bool)stream.ReceiveNext();

			Vector3 tmp = (Vector3)stream.ReceiveNext();
			transform.rotation = (Quaternion)stream.ReceiveNext();

			if(Vector3.Distance(transform.position,tmp) >= 1f){
				transform.position = tmp + rb.velocity;
			}

		}
	}
}
