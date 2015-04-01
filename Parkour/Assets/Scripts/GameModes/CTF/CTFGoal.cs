using UnityEngine;
using System.Collections;

public class CTFGoal : MonoBehaviour {

	bool collected1 = false;
	bool collected2 = false;


	public Transform flag1slot, flag2slot;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (collected1 || collected2) {
			GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
		}

		if (collected1 && collected2){
			//win condition
			print("YOU WIN");
			UnityEditor.EditorApplication.isPaused = true;



		}
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Player") {
			CTFCarrier carrier = col.gameObject.GetComponentInChildren<CTFCarrier>();
			if (carrier != null && carrier.HasFlag) {

				if(!collected1){
					collected1 = true;
					carrier.SendMessage("OnFlagCapture",flag1slot.position); 
					return;
				}else{
					if(!collected2){
						collected2 = true;
						carrier.SendMessage("OnFlagCapture",flag2slot.position); 
						return;
					}
				}





			}
		}
	}
}
