using UnityEngine;
using System.Collections;

public class CTFFlag : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Player") {
			CTFCarrier carrier = col.gameObject.GetComponent<CTFCarrier>();
			if (carrier != null && !carrier.HasFlag) {
				carrier.SendMessage("OnFlagPickup", this); 
				Destroy(gameObject);
			}
		}
	}
}
