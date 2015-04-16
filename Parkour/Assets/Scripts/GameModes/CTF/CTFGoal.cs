using UnityEngine;
using System.Collections;

public class CTFGoal : MonoBehaviour {

	bool collected1 = false;
	bool collected2 = false;

	public int team = 0;

	public int flagcount = 0;


	public Transform flag1slot, flag2slot;
	private PickupItem flagobj1, flagobj2;


	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(collected1);
			stream.SendNext(collected2);
			stream.SendNext(flagobj1);
			stream.SendNext(flagobj2);
		}
		else {
			collected1 = (bool)stream.ReceiveNext();
			collected2 = (bool)stream.ReceiveNext();
			flagobj1 = (PickupItem)stream.ReceiveNext();
			flagobj2 = (PickupItem)stream.ReceiveNext();
		}
	}
	
	// Update is called once per frame
	void Update () {

		int t1 = collected1 ? 1 : 0;
		int t2 = collected2 ? 1 : 0;
		flagcount = t1 + t2;


		if (collected1 && collected2){
			//win condition
			print("YOU WIN");
			UnityEditor.EditorApplication.isPaused = true;



		}
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Player") {
			CTFCarrier carrier = col.gameObject.GetComponentInChildren<CTFCarrier>();
			if (carrier != null && carrier.team == team && carrier.HasFlag) {//only friendlies can place it in the flag

				if(!collected1){
					collected1 = true;
					carrier.SendMessage("OnFlagCapture",flag1slot.position); 
					flagobj1 = carrier.GetComponent<PickupItem>();
					return;
				}else{
					if(!collected2){
						collected2 = true;
						carrier.SendMessage("OnFlagCapture",flag2slot.position); 
						flagobj2 = carrier.GetComponent<PickupItem>();
						return;
					}
				}
			}

			if (carrier != null && carrier.team != team && !carrier.HasFlag) {//only enemies can steal flag
				
				if(collected1){
					collected1 = false;
					flagobj1.Drop(transform.position);
					carrier.SendMessage("OnPickedUp",flagobj1);
					return;
				}else{
					if(collected2){
						collected2 = false;
						flagobj2.Drop(transform.position);
						carrier.SendMessage("OnPickedUp",flagobj2);
						return;
					}
				}
			}






		}
	}
}
