using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeometryManager : Singleton<GeometryManager> {
	public Dictionary<GameObject,ObjectData> objectDict;
	
	void Awake () {
		objectDict = new Dictionary<GameObject, ObjectData>();
	}

	void Start(){
		GameObject[] allObjs = GameObject.FindGameObjectsWithTag("Parkour");
		for (int i=0; i<allObjs.Length; i++){
			GameObject OBJ = allObjs[i];
			objectDict[OBJ] = Tracer.Trace(OBJ);
		}
	}

	void Update () {
	
	}
};


public class ObjectData{
	//this class holds data generated from tracing the level and stores it for each object
	public GameObject obj;
	public HalfEdge[] edges;
	public int[] tris;
	public int[] tritype;

	public ObjectData(GameObject g){
		obj = g;
	}
}
