using UnityEngine;
using System.Collections;

public class HalfEdge {

	public HalfEdge oppositeEdge;
	public HalfEdge adjacentEdge;
	public int leftVert;
	public int rightVert;
	public int triangle;
	public bool ledge;


	public HalfEdge(){
		oppositeEdge = null;
		adjacentEdge = null;
		leftVert = 0;
		rightVert = 0;
		triangle = 0;
		ledge = false;
		//Debug.Log ("initilizing half edge, no verts");
	}

	public HalfEdge(int LV, int RV, int tri){
		oppositeEdge = null;
		adjacentEdge = null;
		leftVert = LV;
		rightVert = RV;
		triangle = tri;
		ledge = false;
		//Debug.Log ("initilizing half edge");
	}

}
