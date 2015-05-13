using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Linq;

public class PlayerSpawn : MonoBehaviour {

	public GameObject cameraObject;
	//public Vector3 cameraAttachPos;
	//public Quaternion cameraAttachRot;

	public Image staminaBar;//UI element to display stamina

	public GameObject staminaObj;
	public GameObject minimap;
	public GameObject score;

	public GameObject chooseName;//UI element to handle name choosing
	public GameObject chooseTeam;//UI element to handle team selection

	public Transform redSpawn;
	public Transform blueSpawn;
	
	public Transform flag1pos;
	public Transform flag2pos;

	private static int playercount;

	private int playerNum;
	private int teamNum; //RED = 0 BLUE = 1
	private string playerName;

	public PauseMenuScript pausemenu;

	void OnJoinedRoom(){
		chooseName.SetActive(true);
		if(PhotonNetwork.isMasterClient){
			print("I AM MASTER CLIENT");
			PhotonNetwork.Instantiate ("CTF Flag", flag1pos.position, transform.rotation,0);
			PhotonNetwork.Instantiate ("CTF Flag", flag2pos.position, transform.rotation,0);
		}
		InputField infi = chooseName.FindInChildren("InputField").GetComponent<InputField>();
		infi.Select();
		infi.ActivateInputField();
		if(PlayerPrefs.HasKey("prev_name")){
			infi.text = PlayerPrefs.GetString("prev_name");
		}

	}


	public void OnNameChosen(string pname){
		playerName = pname;
		PlayerPrefs.SetString("prev_name",pname);
		chooseName.SetActive(false);
		chooseTeam.SetActive(true);
	}

	public void OnTeamChosen(int tnum){
		teamNum = tnum;
		chooseTeam.SetActive(false);
		spawnPlayer();
	}


	private void spawnPlayer(){
		print("SPAWNING PLAYER");
		playerNum = playercount++;

		Vector3 spawnpoint = (teamNum == 0) ? redSpawn.position : blueSpawn.position;
		
		GameObject newPlayerObject = PhotonNetwork.Instantiate("Player", spawnpoint, Quaternion.identity, 0);
		newPlayerObject.SetActive(true);
		//newPlayerObject.GetComponent<CharacterController> ().enabled = false;
		
		Transform attachObj = newPlayerObject.FindInChildren("Head").transform ?? newPlayerObject.transform;
		
		cameraObject.transform.parent = newPlayerObject.transform;
		cameraObject.transform.rotation = Quaternion.identity;

		cameraObject.GetComponent<MouseLook>().axes = MouseLook.RotationAxes.MouseY;

		//cameraObject.transform.localPosition = cameraAttachPos;
		//cameraObject.transform.localRotation = cameraAttachRot;
		//Debug.Log ("CONNECTED");


		//SkinnedMeshRenderer guyBody = newPlayerObject.FindInChildren("GuyBody").GetComponent<SkinnedMeshRenderer>();
		//SkinnedMeshRenderer guyHead = newPlayerObject.FindInChildren("GuyHead").GetComponent<SkinnedMeshRenderer>();

		Transform headTarget = newPlayerObject.FindInChildren("HeadTarget").transform;

		GameObject CTFC = newPlayerObject.FindInChildren("CTF_comp");
		CTFCarrier ctfc = CTFC.GetComponent<CTFCarrier>();
		ctfc.pname = playerName;
		ctfc.setTeam(teamNum);


		headTarget.parent = cameraObject.transform;
		headTarget.localPosition = newPlayerObject.transform.forward*10;

		softParent sp = cameraObject.AddComponent<softParent>();
		newPlayerObject.GetComponent<DoParkour>().sp = sp;

		sp.localPosition = new Vector3(0,0,0.06f);
		sp.parent = attachObj;

		newPlayerObject.GetComponent<ParkourController>().staminaBar = staminaBar;

		pausemenu.player = newPlayerObject.GetComponent<ParkourController>();

		staminaObj.SetActive(true);
		minimap.SetActive(true);
		score.SetActive(true);
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