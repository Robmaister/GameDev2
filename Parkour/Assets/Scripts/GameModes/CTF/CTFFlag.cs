using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PickupItem))]
public class CTFFlag : MonoBehaviour {

	private CTFCarrier carrier = null;
	private Collider spc;
	private Rigidbody rb;

	void Awake(){
		spc = GetComponent<Collider>();
		rb = GetComponent<Rigidbody>();
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(spc.enabled);
			stream.SendNext(rb.isKinematic);
		}
		else {
			spc.enabled = (bool)stream.ReceiveNext();
			rb.isKinematic = (bool)stream.ReceiveNext();
		}
	}

	public void OnPickedUp(PickupItem item){
		//Debug.Log("flagscript: " + item.PickupIsMine);
		carrier.SendMessage("OnPickedUp", item); 
	}

	void OnTriggerEnter(Collider col) {//check if a player picks up flag
		if (col.gameObject.tag == "Player") {
			carrier = col.gameObject.GetComponentInChildren<CTFCarrier>();
			if (carrier != null && !carrier.HasFlag) {
				PickupItem pi = GetComponent<PickupItem>();
				pi.Pickup();
				//gameObject.SetActive(false);
			}
		}
	}
}
