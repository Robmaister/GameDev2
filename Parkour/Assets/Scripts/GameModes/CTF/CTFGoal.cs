﻿using UnityEngine;
using System.Collections;

public class CTFGoal : MonoBehaviour {

	bool collected1 = false;
	bool collected2 = false;

	public int team = 0;

	public int flagcount = 0;


	public Transform flag1slot, flag2slot;
	public PickupItem flagobj1, flagobj2;

	AudioSource ads;

	public AudioClip bluecapture;
	public AudioClip redcapture;
	public AudioClip bluesteal;
	public AudioClip redsteal;
	public AudioClip bluewin;
	public AudioClip redwin;

	bool gameOver = false;

	void Awake(){
		ads = GetComponent<AudioSource>();
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(collected1);
			stream.SendNext(collected2);
			if(flagobj1 != null){
				stream.SendNext(flagobj1.gameObject.GetComponent<PhotonView>().viewID);
			}
			else{
				stream.SendNext(-1);
			}
			if(flagobj2 != null){
				stream.SendNext(flagobj2.gameObject.GetComponent<PhotonView>().viewID);
			}
			else{
				stream.SendNext(-1);
			}
		}
		else {
			collected1 = (bool)stream.ReceiveNext();
			collected2 = (bool)stream.ReceiveNext();
			int tmp = (int)stream.ReceiveNext();
			flagobj1 = (tmp != -1) ? PhotonView.Find(tmp).gameObject.GetComponent<PickupItem>() : null;
			tmp = (int)stream.ReceiveNext();
			flagobj2 = (tmp != -1) ? PhotonView.Find(tmp).gameObject.GetComponent<PickupItem>() : null;
		
		}
	}

	void endGame(){
		print("WINNER: Team " + team);
		if(team == 0){
			ads.PlayOneShot(redwin);
		}
		else if(team == 1){
			ads.PlayOneShot(bluewin);
		}
		gameOver = true;
	}
	
	// Update is called once per frame
	void Update () {
		int t1 = collected1 ? 1 : 0;
		int t2 = collected2 ? 1 : 0;
		flagcount = t1 + t2;

		if (!gameOver && collected1 && collected2){
			endGame();
		}
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Player") {
			CTFCarrier carrier = col.gameObject.GetComponentInChildren<CTFCarrier>();
			if (carrier != null && carrier.team == team && carrier.HasFlag) {//only friendlies can place it in the flag
				if(!collected1){
					flagobj1 = carrier.flagobj;
					carrier.SendMessage("OnFlagCapture",flag1slot.position); 
					flagobj1.GetComponent<CTFFlag>().DisableLights();
					collected1 = true;

					if(carrier.team == 0){
						ads.PlayOneShot(redcapture);
					}
					else if(carrier.team == 1){
						ads.PlayOneShot(bluecapture);
					}

					return;
				}else if(!collected2){
						flagobj2 = carrier.flagobj;
						carrier.SendMessage("OnFlagCapture",flag2slot.position);
						flagobj2.GetComponent<CTFFlag>().DisableLights();
						collected2 = true;

						if(collected1 && collected2){
							endGame();
							return;
						}

						if(carrier.team == 0){
							ads.PlayOneShot(redcapture);
						}
						else if(carrier.team == 1){
							ads.PlayOneShot(bluecapture);
						}

						return;
				}
			}
			if (carrier != null && carrier.team != team && !carrier.HasFlag) {//only enemies can steal flag
				print("STEALING FLAG");
				if(collected2){
					collected2 = false;
					flagobj2.Drop(carrier.transform.position);
					flagobj2.GetComponent<Collider>().enabled = true;
					carrier.SendMessage("OnPickedUp",flagobj2);
					flagobj2.GetComponent<CTFFlag>().EnableLights();
					flagobj2 = null;

					if(carrier.team == 0){
						ads.PlayOneShot(redsteal);
					}
					else if(carrier.team == 1){
						ads.PlayOneShot(bluesteal);
					}
					return;
				}
				else if(collected1){
					collected1 = false;
					flagobj1.Drop(carrier.transform.position);
					flagobj1.GetComponent<Collider>().enabled = true;
					carrier.SendMessage("OnPickedUp",flagobj1);
					flagobj1.GetComponent<CTFFlag>().EnableLights();
					flagobj1 = null;

					if(carrier.team == 0){
						ads.PlayOneShot(redsteal);
					}
					else if(carrier.team == 1){
						ads.PlayOneShot(bluesteal);
					}
					return;
				}
			}
		}
	}
}
