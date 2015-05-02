using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class lightStrobe : MonoBehaviour {
	
	public float speed = 1f; //duration between strobes, in seconds
	private Light li;
	// Use this for initialization
	void Start () {
		li = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
		li.intensity = Mathf.PingPong(Time.time * speed,8);
	}
}
