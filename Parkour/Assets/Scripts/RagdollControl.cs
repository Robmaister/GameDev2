using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RagdollControl : MonoBehaviour {

	private List<Rigidbody> jointlist;

	public Animator anim;
	public CharacterController ctrl;
	public Rigidbody player_body;
	public softParent headctrl;
	public MouseLook mlk;
	public MouseLook mlk2;
	public ParkourController pkc;
	public DoParkour dpk;
	public SphereCollider arms,legs;

	public bool is_ragdoll = false;

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			foreach(Rigidbody rb in jointlist){
				stream.SendNext(rb.isKinematic);
			}
			stream.SendNext(ctrl.enabled);
			stream.SendNext(anim.enabled);
			//stream.SendNext(player_body.isKinematic);
			if(headctrl != null){
				stream.SendNext(headctrl.enable_rotation);
			}
			stream.SendNext(mlk.enabled);
			if(mlk2!=null){
				stream.SendNext(mlk2.enabled);
			}
			stream.SendNext(dpk.enabled);
			stream.SendNext(pkc.enabled);
			stream.SendNext(arms.enabled);
			stream.SendNext(legs.enabled);
		}
		else {
			foreach(Rigidbody rb in jointlist){
				rb.isKinematic = (bool)stream.ReceiveNext();
			}
			ctrl.enabled = (bool)stream.ReceiveNext();
			anim.enabled = (bool)stream.ReceiveNext();
			//player_body.isKinematic = (bool)stream.ReceiveNext();
			if(headctrl != null){
				headctrl.enable_rotation = (bool)stream.ReceiveNext();
			}
			mlk.enabled = (bool)stream.ReceiveNext();
			if(mlk2!=null){
				mlk2.enabled = (bool)stream.ReceiveNext();
			}
			dpk.enabled = (bool)stream.ReceiveNext();
			pkc.enabled = (bool)stream.ReceiveNext();
			arms.enabled = (bool)stream.ReceiveNext();
			legs.enabled = (bool)stream.ReceiveNext();
		}
	}

	void Awake(){
		jointlist = new List<Rigidbody>();
	}

	// Use this for initialization
	void Start () {
		jointlist = new List<Rigidbody>();

		foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>()){
			jointlist.Add(rb);
		}
		if(pkc.gameObject.GetComponent<PhotonView>().isMine){
			mlk2 = Camera.main.GetComponent<MouseLook>();
		}
		disableRagdoll();
	}
	
	public void enableRagdoll(){
		foreach(Rigidbody rb in jointlist){
			rb.isKinematic = false;
			rb.velocity = pkc.controller.velocity;
		}
		ctrl.enabled = false;
		anim.enabled = false;
		//player_body.isKinematic = true;
		if(headctrl != null){
			headctrl.enable_rotation = true;
		}
		mlk.enabled = false;
		if(mlk2!=null){
			mlk2.enabled = false;
		}
		dpk.enabled = false;
		pkc.enabled = false;
		arms.enabled = false;
		legs.enabled = false;
		player_body.freezeRotation = false;
		is_ragdoll = true;

		StartCoroutine(restoreDoll());
	}

	private IEnumerator restoreDoll(){
		yield return new WaitForSeconds(3);
		disableRagdoll();
	}

	public void disableRagdoll(){
		foreach(Rigidbody rb in jointlist){
			rb.isKinematic = true;
		}

		Vector3 opos = pkc.transform.position;
		opos.y += 2;
		Ray ray = new Ray(opos,-Vector3.up);
		RaycastHit hit;
		Physics.Raycast(ray,out hit);
		pkc.transform.position = new Vector3(hit.point.x,hit.point.y+0.75f,hit.point.z);

		Vector3 vv = pkc.transform.rotation.eulerAngles;
		pkc.transform.rotation = Quaternion.Euler(new Vector3(0,vv.y,0));
		ctrl.enabled = true;
		anim.enabled = true;
		anim.SetTrigger("getUp");
		//player_body.isKinematic = false;
		if(headctrl != null){
			headctrl.enable_rotation = true;
		}
		mlk.enabled = true;
		if(mlk2!=null){
			mlk2.enabled = true;
		}
		dpk.enabled = true;
		pkc.enabled = true;
		if(headctrl != null){
			headctrl.resetRot();
		}
		arms.enabled = true;
		legs.enabled = true;
		player_body.freezeRotation = true;
		//StartCoroutine(restoreDoll());
		is_ragdoll = false;
	}

	/*void Update(){
		if(Input.GetKeyDown(KeyCode.Y)){
			enableRagdoll();
		}
		if(Input.GetKeyDown(KeyCode.U)){
			disableRagdoll();
		}
	}*/

	void OnFlagDrop(){
		enableRagdoll();
	}
}
