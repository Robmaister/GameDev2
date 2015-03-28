using UnityEngine;
using System.Collections;

public class softParent : MonoBehaviour {
	//simple script to parent an object without changing the rotation
	public Transform parent;
	public Vector3 localPosition = Vector3.zero;

	// Use this for initialization
	void Start () {
		transform.position = parent.position + localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = parent.position + localPosition;
	}
}
