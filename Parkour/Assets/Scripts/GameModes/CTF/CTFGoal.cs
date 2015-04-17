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
			if(flagobj1 != null){
				stream.SendNext(flagobj1.GetComponent<PhotonView>().viewID);
			}
			else{
				stream.SendNext(-1);
			}
			if(flagobj1 != null){
				stream.SendNext(flagobj2.GetComponent<PhotonView>().viewID);
			}
			else{
				stream.SendNext(-1);
			}
		}
		else {
			collected1 = (bool)stream.ReceiveNext();
			collected2 = (bool)stream.ReceiveNext();
			int tmp = (int)stream.ReceiveNext();
			flagobj1 = (tmp != -1) ? PhotonView.Find(tmp).gameObject.GetComponent<PickupItem>() : null;
			tmp = (int)stream.ReceiveNext();
			flagobj2 = (tmp != -1) ? PhotonView.Find(tmp).gameObject.GetComponent<PickupItem>() : null;
		
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
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPaused = true;
#else
			Application.Quit();
#endif



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
					//flagobj1.Drop(transform.position);
					flagobj1.GetComponent<Collider>().enabled = true;
					carrier.SendMessage("OnPickedUp",flagobj1);
					flagobj1 = null;
					return;
				}else{
					if(collected2){
						collected2 = false;
						//flagobj2.Drop(transform.position);
						flagobj2.GetComponent<Collider>().enabled = true;
						carrier.SendMessage("OnPickedUp",flagobj2);
						flagobj2 = null;
						return;
					}
				}
			}






		}
	}
}
