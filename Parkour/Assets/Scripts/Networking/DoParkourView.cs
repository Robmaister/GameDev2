using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PhotonView ) )]
[RequireComponent( typeof( DoParkour ) )]
[AddComponentMenu("Photon Networking/Do Parkour View")]
public class DoParkourView : MonoBehaviour {

	DoParkour dps;

	// Use this for initialization
	void Awake() {
		dps = GetComponent<DoParkour>();
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(dps.tackling);
		}
		else {
			dps.tackling = (bool)stream.ReceiveNext();
		}
	}
}
