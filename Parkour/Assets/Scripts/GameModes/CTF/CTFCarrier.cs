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

	public GameObject dummyflag;

	public AudioClip bluepickup;
	public AudioClip redpickup;

	public AudioSource ads;

	public void OnPhotonPlayerDisconnected(PhotonPlayer player){
		print("player disconnected");
		if(flagobj != null){
			flagobj.Drop(transform.position);
			this.enabled = false;
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(team);
			stream.SendNext(pname);
			stream.SendNext(hasFlag);
			stream.SendNext(dummyflag.GetActive());
			if(flagobj != null){
				stream.SendNext(flagobj.gameObject.GetComponent<PhotonView>().viewID);
			}
			else{
				stream.SendNext(-1);
			}
			nameTag.text = pname;
		}
		else {
			setTeam((int)stream.ReceiveNext());
			pname = (string)stream.ReceiveNext();
			hasFlag = (bool)stream.ReceiveNext();
			dummyflag.SetActive((bool)stream.ReceiveNext());
			int tmp = (int)stream.ReceiveNext();
			flagobj = (tmp != -1) ? PhotonView.Find(tmp).gameObject.GetComponent<PickupItem>() : null;
			nameTag.text = pname;
			tr.enabled = hasFlag;
		}
	}

	public void setTeam(int t){
		team = t;
		if(team == 0){
			skm.material.mainTexture = redtex;
			tr.material.SetColor("_TintColor", Color.red);
		}
		else if(team == 1){
			skm.material.mainTexture = bluetex;
			tr.material.SetColor("_TintColor", Color.blue);
		}
	}

	void Awake () {
		tr = GetComponent<TrailRenderer>();
		ads = GetComponent<AudioSource>();
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
			dummyflag.SetActive(true);

			if(team == 0){
				ads.PlayOneShot(redpickup);
			}
			if(team == 1){
				ads.PlayOneShot(bluepickup);
			}
		}
		else{
			Debug.Log("Someone else picked the flag up");
		}
	}

	void OnFlagDrop(){
		//drop the flag (if get tackled or something)
		if(flagobj != null){
			hasFlag = false;
			dummyflag.SetActive(false);
			tr.enabled = false;
			flagobj.Drop(transform.position);
			print("dropping flag");
			flagobj = null;

		}

	}

	void OnFlagCapture(Vector3 pos){
		//store flag in base
		if(flagobj != null){
			hasFlag = false;
			tr.enabled = false;
			dummyflag.SetActive(false);
			flagobj.transform.rotation = Quaternion.identity;
			flagobj.GetComponent<Collider>().enabled = false;
			flagobj.GetComponent<Rigidbody>().isKinematic = true;
			flagobj.Drop(pos);
			print("capturing flag");
			flagobj = null;
		}
	}

}
