using UnityEngine;
using System.Collections;

public class playerMinimapIcon : MonoBehaviour {

	// Use this for initialization
	void Start () {
		

		PhotonView pv = transform.root.GetComponentInChildren<PhotonView>();
		CTFCarrier ctfc = transform.root.GetComponentInChildren<CTFCarrier>();
		SpriteRenderer spr = GetComponent<SpriteRenderer>();
		if(pv.isMine){
			spr.color = Color.green;
		}else{
			if(ctfc.team == 0){
				spr.color = Color.red;
			}
			else if(ctfc.team == 1){
				spr.color = Color.blue;
			}
		}
	}
}
