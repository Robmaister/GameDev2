using UnityEngine;
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

	// Use this for initialization
	void Awake() {
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
			//stream.SendNext(controller.netImpulse);
			stream.SendNext(controller.can_jump);
			stream.SendNext(controller.apply_forces);
		}
		else {
			controller.networkInputH = (float)stream.ReceiveNext();
			controller.networkInputV = (float)stream.ReceiveNext();
			controller.inputJump.Pressed = (bool)stream.ReceiveNext();
			controller.inputHands.Pressed = (bool)stream.ReceiveNext();
			controller.inputFeet.Pressed = (bool)stream.ReceiveNext();
			controller.inputSprint.Pressed = (bool)stream.ReceiveNext();
			controller.inputUse.Pressed = (bool)stream.ReceiveNext();
			//controller.netImpulse = (Vector3)stream.ReceiveNext();
			controller.can_jump = (bool)stream.ReceiveNext();
			controller.apply_forces = (bool)stream.ReceiveNext();
		}
	}
}
