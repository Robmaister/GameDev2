using UnityEngine;
using System.Collections;

public class CTFGoal : MonoBehaviour {

	bool collected = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (collected) {
			GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
		}
	}

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Player") {
			CTFCarrier carrier = col.gameObject.GetComponent<CTFCarrier>();
			if (carrier != null && carrier.HasFlag) {
				collected = true;
			}
		}
	}
}
