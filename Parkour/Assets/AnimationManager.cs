using UnityEngine;
using System.Collections;

public class AnimationManager : MonoBehaviour {
	public Animator anim;
	public GameObject player;
	public CharacterController control;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		player = gameObject;
		control = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

	}
}
