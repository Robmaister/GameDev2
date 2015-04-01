using UnityEngine;
using System.Collections;

public class CTFCarrier : MonoBehaviour {

	private bool hasFlag = false;

	public bool HasFlag { get { return hasFlag; } }

	private GameObject flagobj;

	private TrailRenderer tr;
	// Use this for initialization
	void Start () {
		tr = GetComponent<TrailRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnFlagPickup(CTFFlag flag) {
		hasFlag = true;
		tr.enabled = true;
		flagobj = flag.gameObject;
	}

	void OnFlagDrop(){
		//drop the flag (if get tackled or something)
		hasFlag = false;
		tr.enabled = false;
		flagobj.transform.position = transform.position;
		flagobj.SetActive(true);
		print("dropping flag");
		flagobj = null;

	}

	void OnFlagCapture(Vector3 pos){
		//store flag in base
		hasFlag = false;
		tr.enabled = false;
		flagobj.transform.position = pos;
		flagobj.SetActive(true);
		flagobj.GetComponent<Collider>().enabled = false;
		print("capturing flag");
		flagobj = null;
	}

}
