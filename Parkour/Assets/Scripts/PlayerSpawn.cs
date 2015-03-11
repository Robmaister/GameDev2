using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour {

	public GameObject cameraObject;
	public Vector3 cameraAttachPos;
	public Quaternion cameraAttachRot;

	void OnJoinedRoom()
	{
		Vector3 position = new Vector3(227.3f, 15.2f, 152.3f);
		GameObject newPlayerObject = PhotonNetwork.Instantiate("Player", position, Quaternion.identity, 0);
		//newPlayerObject.GetComponent<CharacterController> ().enabled = false;

		cameraObject.transform.parent = newPlayerObject.transform;
		cameraObject.transform.localPosition = cameraAttachPos;
		cameraObject.transform.localRotation = cameraAttachRot;
		//Debug.Log ("CONNECTED");
	}
}
