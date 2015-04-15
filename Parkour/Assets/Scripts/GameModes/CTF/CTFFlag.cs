using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PickupItem))]
public class CTFFlag : MonoBehaviour {

	private CTFCarrier carrier = null;

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
