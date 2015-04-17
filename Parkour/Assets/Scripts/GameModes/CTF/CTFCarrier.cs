using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CTFCarrier : MonoBehaviour {
	public int team = 0;
	public string pname = "DICKBUTT";
	private bool hasFlag = false;

	public Text nameTag;

	public bool HasFlag { get { return hasFlag; } }

	public PickupItem flagobj;

	private TrailRenderer tr;

	public SkinnedMeshRenderer skm;
	public Texture bluetex;
	public Texture redtex;

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(team);
			stream.SendNext(pname);
			stream.SendNext(hasFlag);
		}
		else {
			team = (int)stream.ReceiveNext();
			pname = (string)stream.ReceiveNext();
			hasFlag = (bool)stream.ReceiveNext();

			nameTag.text = pname;
			tr.enabled = hasFlag;
		}
	}

	public void setTeam(int t){
		team = t;
		if(team == 0){
			skm.sharedMaterial.mainTexture = redtex;
		}
		else if(team == 1){
			skm.sharedMaterial.mainTexture = bluetex;
		}
	}

	void Start () {
		tr = GetComponent<TrailRenderer>();
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.R)){
			OnFlagDrop();
		}
	}

	public void OnPickedUp(PickupItem item){
		Debug.Log("carrierscript: " + item.PickupIsMine);
		if (item.PickupIsMine){
			Debug.Log("Picked up flag");
			hasFlag = true;
			tr.enabled = true;
			flagobj = item;
		}
		else{
			Debug.Log("Someone else picked the flag up");
		}
	}

	void OnFlagDrop(){
		//drop the flag (if get tackled or something)
		hasFlag = false;
		tr.enabled = false;
		if(flagobj != null){
			flagobj.Drop(transform.position);
			//flagobj.SetActive(true);
			print("dropping flag");
			flagobj = null;

		}

	}

	void OnFlagCapture(Vector3 pos){
		//store flag in base
		hasFlag = false;
		tr.enabled = false;
		flagobj.Drop(pos);
		//flagobj.SetActive(true);
		flagobj.GetComponent<Collider>().enabled = false;
		flagobj.GetComponent<Rigidbody>().isKinematic = true;
		print("capturing flag");
		flagobj = null;
	}

}
