using UnityEngine;
using System.Collections;

public class CTFGoal : MonoBehaviour {

	bool collected1 = false;
	bool collected2 = false;

	public int team = 0;


	public Transform flag1slot, flag2slot;
	private PickupItem flagobj1, flagobj2;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//if (collected1 || collected2) {
		//	GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
		//}

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
