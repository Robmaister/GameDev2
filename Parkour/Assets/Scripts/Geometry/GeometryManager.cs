using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeometryManager : Singleton<GeometryManager> {
	//this class keeps track of the parkourable objects
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
	public HalfEdge[] edges;//each index is the edge for a triagles vert to another of that triangles
	public int[] tris;
	public SurfaceType[] triType; //1=top 2=scramble 4=side 8=bottom
	public Vector3[] verts;

	public ObjectData(GameObject g){
		obj = g;
	}
}
