using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		print(other.name);
		
		RaycastHit hit;
		if (Physics.Raycast(transform.position, transform.forward, out hit))
		{
			print(gameObject.name + ": " + Vector3.Angle(Vector3.up, hit.normal));
		}
	}
}
