using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PickupItem))]
public class CTFFlag : MonoBehaviour {

	public GameObject light1,light2;

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
			int tmp = (int)stream.ReceiveNext();
			carrier = (tmp != -1) ? PhotonView.Find(tmp).gameObject.GetComponent<CTFCarrier>() : null;
		}
	}


	public void OnPickedUp(PickupItem item){
		//Debug.Log("flagscript: " + item.PickupIsMine);
		if(carrier != null){
			carrier.SendMessage("OnPickedUp", item); 
			carrier = null;
		}else{
			print("No carrier to pick up!");
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
				}
			}
		}
	}

	public void EnableLights(){
		light1.SetActive(true);
		light2.SetActive(true);
	}

	public void DisableLights(){
		light1.SetActive(false);
		light2.SetActive(false);
	}
}
