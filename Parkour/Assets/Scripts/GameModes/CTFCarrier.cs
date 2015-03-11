using UnityEngine;
using System.Collections;

public class CTFCarrier : MonoBehaviour {

	private bool hasFlag;

	public bool HasFlag { get { return hasFlag; } }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnFlagPickup(CTFFlag flag) {
		hasFlag = true;
	}
}
