using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PickupItem))]
public class CTFFlag : MonoBehaviour {

	private CTFCarrier carrier = null;
	private SphereCollider spc;

	void Awake(){
		spc = GetComponent<SphereCollider>();
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(spc.enabled);
		}
		else {
			spc.enabled = (bool)stream.ReceiveNext();
		}
	}

	public void OnPickedUp(PickupItem item){
		Debug.Log("flagscript: " + item.PickupIsMine);
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
