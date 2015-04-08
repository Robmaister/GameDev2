using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CTFCarrier : MonoBehaviour {
	public int team = 0;
	public string pname = "DICKBUTT";
	private bool hasFlag = false;

	public Text nameTag;

	public bool HasFlag { get { return hasFlag; } }

	private GameObject flagobj;

	private TrailRenderer tr;

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(team);
			stream.SendNext(pname);
		}
		else {
			team = (int)stream.ReceiveNext();
			pname = (string)stream.ReceiveNext();
			nameTag.text = pname;
		}
	}

	void Start () {
		tr = GetComponent<TrailRenderer>();
	}

	void OnFlagPickup(CTFFlag flag) {
		print("picked up flag");
		hasFlag = true;
		tr.enabled = true;
		flagobj = flag.gameObject;
	}

	void OnFlagDrop(){
		//drop the flag (if get tackled or something)
		hasFlag = false;
		tr.enabled = false;
		if(flagobj != null){
			flagobj.transform.position = transform.position;
			flagobj.SetActive(true);
			print("dropping flag");
			flagobj = null;
		}

	}

	void OnFlagCapture(Vector3 pos){
		//store flag in base
		hasFlag = false;
		tr.enabled = false;
		flagobj.transform.position = pos;
		flagobj.SetActive(true);
		flagobj.GetComponent<Collider>().enabled = false;
		flagobj.GetComponent<Rigidbody>().isKinematic = true;
		print("capturing flag");
		flagobj = null;
	}

}
