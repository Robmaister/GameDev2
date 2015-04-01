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

	// Use this for initialization
	void Start () {
		jointlist = new List<Rigidbody>();

		foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>()){
			jointlist.Add(rb);
		}

		disableRagdoll();
	}
	
	public void enableRagdoll(){
		foreach(Rigidbody rb in jointlist){
			rb.isKinematic = false;
		}
		ctrl.enabled = false;
		anim.enabled = false;
		player_body.isKinematic = true;
		if(headctrl != null){
			headctrl.enable_rotation = true;
		}
		mlk.enabled = false;
		if(mlk2!=null){
			mlk2.enabled = true;
		}
		dpk.enabled = false;
		pkc.enabled = false;
	}

	public void disableRagdoll(){
		foreach(Rigidbody rb in jointlist){
			rb.isKinematic = true;
		}
		ctrl.enabled = true;
		anim.enabled = true;
		player_body.isKinematic = false;
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
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.Y)){
			enableRagdoll();
		}
		if(Input.GetKeyDown(KeyCode.U)){
			disableRagdoll();
		}
	}
}
