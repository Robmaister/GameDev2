using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PickupItem))]
public class CTFFlag : MonoBehaviour {

	private CTFCarrier carrier = null;
	private Collider spc;
	private Rigidbody rb;
	private PickupItem waititem = null;

	void Awake(){
		spc = GetComponent<Collider>();
		rb = GetComponent<Rigidbody>();

	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(spc.enabled);
			stream.SendNext(rb.isKinematic);
			//stream.SendNext(gameObject.GetActive());
			if(carrier != null){
				stream.SendNext(carrier.GetComponent<PhotonView>().viewID);
			}
			else{
				stream.SendNext(-1);
			}
		}
		else {
			spc.enabled = (bool)stream.ReceiveNext();
			rb.isKinematic = (bool)stream.ReceiveNext();
			//gameObject.SetActive((bool)stream.ReceiveNext());
			int tmp = (int)stream.ReceiveNext();
			carrier = (tmp != -1) ? PhotonView.Find(tmp).gameObject.GetComponent<CTFCarrier>() : null;
		}
	}

	public IEnumerator fmsgwait(){
		float starttime = Time.time;
		while(carrier = null){
			if(Time.time > starttime + 10){
				print("flag wait duration expired");
				break;
			}
		}
		carrier.SendMessage("OnPickedUp", waititem); 
		waititem = null;
	}

	public void OnPickedUp(PickupItem item){
		//Debug.Log("flagscript: " + item.PickupIsMine);
		if(carrier != null){
			carrier.SendMessage("OnPickedUp", item); 
		}else{
			waititem = item;
			StartCoroutine(fmsgwait());
		}
	}

	void OnTriggerEnter(Collider col) {//check if a player picks up flag
		if (col.gameObject.tag == "Player") {
			RagdollControl rdc = col.gameObject.GetComponentInChildren<RagdollControl>();
			if(!rdc.is_ragdoll){
				carrier = col.gameObject.GetComponentInChildren<CTFCarrier>();
				if (carrier != null && !carrier.HasFlag) {
					PickupItem pi = GetComponent<PickupItem>();
					pi.Pickup();
					//gameObject.SetActive(false);
				}
			}
		}
	}
}
