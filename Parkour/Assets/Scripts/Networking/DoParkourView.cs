using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PhotonView ) )]
[RequireComponent( typeof( DoParkour ) )]
[AddComponentMenu("Photon Networking/Do Parkour View")]
public class DoParkourView : MonoBehaviour {

	DoParkour script;

	// Use this for initialization
	void Awake() {
		script = GetComponent<DoParkour>();
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			//stream.SendNext(controller.networkInputH);
		}
		else {
			//controller.networkInputH = (float)stream.ReceiveNext();
		}
	}
}
