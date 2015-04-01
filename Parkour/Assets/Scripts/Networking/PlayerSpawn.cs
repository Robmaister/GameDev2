using UnityEngine;
using System.Collections;

using System.Linq;

public class PlayerSpawn : MonoBehaviour {

	public string attachName;
	public GameObject cameraObject;
	//public Vector3 cameraAttachPos;
	//public Quaternion cameraAttachRot;

	private int playerNum;

	void OnJoinedRoom()
	{
		//Vector3 position = new Vector3(227.3f, 15.2f, 152.3f);
		GameObject newPlayerObject = PhotonNetwork.Instantiate("Player", transform.position + Vector3.right * playerNum * 3f, Quaternion.identity, 0);
		//newPlayerObject.GetComponent<CharacterController> ().enabled = false;

		Transform attachObj = newPlayerObject.FindInChildren(attachName).transform ?? newPlayerObject.transform;

		cameraObject.transform.parent = newPlayerObject.transform;
		cameraObject.transform.rotation = Quaternion.identity;
		//cameraObject.transform.localPosition = cameraAttachPos;
		//cameraObject.transform.localRotation = cameraAttachRot;
		//Debug.Log ("CONNECTED");


		softParent sp = cameraObject.AddComponent<softParent>();
		sp.parent = attachObj;

	}
}


public static class ExtensionMethods {
	
	public static Transform FindInChildren(this Transform self, string name) {
		int count = self.childCount;
		for(int i = 0; i < count; i++) {
			Transform child = self.GetChild(i);
			if(child.name == name) return child;
			Transform subChild = child.FindInChildren(name);
			if(subChild != null) return subChild;
		}
		return null;
	}
	
	public static GameObject FindInChildren(this GameObject self, string name) {
		Transform transform = self.transform;
		Transform child = transform.FindInChildren(name);
		return child != null ? child.gameObject : null;
	}
}