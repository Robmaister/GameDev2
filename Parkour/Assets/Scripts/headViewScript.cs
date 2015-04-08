﻿using UnityEngine;
using System.Collections;

public class headViewScript : MonoBehaviour {

	public SkinnedMeshRenderer guyhead;
	public Material invismat;

	// Use this for initialization
	void Start () {
		PhotonView photonView = GetComponent<PhotonView>();
		if(photonView.isMine){
			//guyhead.enabled = false;
			guyhead.sharedMaterial = invismat; 
		}
	}

}
