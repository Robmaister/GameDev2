using UnityEngine;
using System.Collections;

public class PauseMenuScript : MonoBehaviour {

	public bool is_paused = false;
	public GameObject pauseMenu;
	public ParkourController player;

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			if(!is_paused){
				Pause();
			}else{
				UnPause();
			}

		}
	}

	public void Pause(){
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPaused = true;
#endif
		is_paused = true;
		pauseMenu.SetActive(true);
		if(player != null){
			player.canControl = false;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

	}

	public void UnPause(){
		is_paused = false;
		pauseMenu.SetActive(false);
		if(player != null){
			player.canControl = true;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

	}

	public void Quit(){
		Application.Quit();
	}


}
