using UnityEngine;
using System.Collections;

public class softParent : MonoBehaviour {
	//simple script to parent an object by proxy
	public Transform parent;
	public Vector3 localPosition = Vector3.zero;
	//private Quaternion localRotation = Quaternion.identity;

	public bool enable_rotation = false; //allow object to rotate with parent

	// Use this for initialization
	void Awake () {
		//localPosition = transform.localPosition;
		//localRotation = transform.localRotation;
	}
	void Start(){
		transform.position = parent.position +  parent.rotation*localPosition;
		if(enable_rotation){
			transform.rotation = parent.rotation;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = parent.position + parent.rotation*localPosition;
		if(enable_rotation){
			transform.rotation = parent.rotation;
		}
	}

	public void resetRot(){
		transform.rotation = Quaternion.identity;
	}
}
