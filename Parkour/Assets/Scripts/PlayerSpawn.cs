using UnityEngine;
using System.Collections;

public class PlayerSpawn : MonoBehaviour {

	public GameObject cameraObject;

	void OnJoinedRoom()
	{
		Vector3 position = new Vector3(227.3f, 7.2f, 152.3f);
		GameObject newPlayerObject = PhotonNetwork.Instantiate("Player", position, Quaternion.identity, 0);
		cameraObject.transform.parent = newPlayerObject.transform;
		Debug.Log ("CONNECTED");
	}
}
