using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System.Linq;

public class PlayerSpawn : MonoBehaviour {

	public GameObject cameraObject;
	//public Vector3 cameraAttachPos;
	//public Quaternion cameraAttachRot;


	public GameObject chooseName;//UI element to handle name choosing
	public GameObject chooseTeam;//UI element to handle team selection

	public Transform redSpawn;
	public Transform blueSpawn;

	private static int playercount;

	private int playerNum;
	private int teamNum; //RED = 0 BLUE = 1
	private string playerName;

	void OnJoinedRoom(){
		chooseName.SetActive(true);
	}


	public void OnNameChosen(string pname){
		playerName = pname;
		chooseName.SetActive(false);
		chooseTeam.SetActive(true);
	}

	public void OnTeamChosen(int tnum){
		teamNum = tnum;
		chooseTeam.SetActive(false);
		spawnPlayer();
	}


	private void spawnPlayer(){
		playerNum = playercount++;

		Vector3 spawnpoint = (teamNum == 0) ? redSpawn.position : blueSpawn.position;
		
		GameObject newPlayerObject = PhotonNetwork.Instantiate("Player", spawnpoint, Quaternion.identity, 0);
		//newPlayerObject.GetComponent<CharacterController> ().enabled = false;
		
		Transform attachObj = newPlayerObject.FindInChildren("Head").transform ?? newPlayerObject.transform;
		
		cameraObject.transform.parent = newPlayerObject.transform;
		cameraObject.transform.rotation = Quaternion.identity;
		//cameraObject.transform.localPosition = cameraAttachPos;
		//cameraObject.transform.localRotation = cameraAttachRot;
		//Debug.Log ("CONNECTED");


		SkinnedMeshRenderer guyBody = newPlayerObject.FindInChildren("GuyBody").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer guyHead = newPlayerObject.FindInChildren("GuyHead").GetComponent<SkinnedMeshRenderer>();


		GameObject CTFC = newPlayerObject.FindInChildren("CTF_comp");
		CTFCarrier ctfc = CTFC.GetComponent<CTFCarrier>();
		ctfc.pname = playerName;
		ctfc.team = teamNum;

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