using UnityEngine;
using System.Collections;

public class ReturnToGame : MonoBehaviour {

	public void goback(){
		Application.LoadLevel("main_scene");
	}

	public void Quit(){
		Application.Quit();
	}
}
