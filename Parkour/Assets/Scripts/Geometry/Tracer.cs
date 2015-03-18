using UnityEngine;
using System.Collections;

//notes:
//need something to deal with multiple meshes making the found ledges not ledges
//or multiple meshes intersected

//also need to fix situations where scrambles should be climbable... or something
//something to do with sharp edges between sides and scrambles and similar situations

//too scrambles that are 90 from eachother are totaly usable to grab the edge of.
//once you are on the top... not sure, you could definitly straddle the edge, like waliking on the top of a roof, but im not sure
//can definitly get away with forcing the player to just hold on, or climb over and slide down.
//nevermind, if an edge is there then you can balence on that edge.

//also related if the edge between too sides is enough to be considered an edge, make sure its not a vertival edge
//should have the same angle to the up vec as a top.

//++need to fix scaleing problem

//need to fix scramble to flat top situation.  if the change in angle is around 45 degrees, then it should be an edge


//
[System.Flags]
public enum SurfaceType{
	top = 1,
	scramble = 2,
	side = 4,
	bottom = 8
}

public class Tracer : MonoBehaviour {

	public static float edge_radius = .5f;

	public static ObjectData Trace (GameObject obj) {
		MeshFilter cube = obj.GetComponent<MeshFilter>();
		ObjectData objd = new ObjectData(obj);
		if (cube != null){  //Sanity check, shouldnt be nessisary
			Mesh objMesh = cube.mesh;


			Vector3[] verts = objMesh.vertices;
			int[] tris = objMesh.triangles;
			HalfEdge[] edges = new HalfEdge[tris.Length]; //each index is the edge for a triagles vert to another of that triangles

			Vector3[] triNorm = new Vector3[tris.Length / 3];
			Vector3[] triCent = new Vector3[tris.Length / 3];
			SurfaceType[] triType = new SurfaceType[tris.Length / 3];



			//need to calculate new position of verts based on the scaleing/translation/rotation of the geometry
			Vector3 geoTrans = cube.transform.localPosition;
			Quaternion geoQuat = cube.transform.localRotation;
			Vector3 geoScale = cube.transform.localScale;//ector3 geoScale = cube.transform.lossyScale;
			Matrix4x4 matrixAreForKids = Matrix4x4.identity;
			matrixAreForKids.SetTRS(geoTrans, geoQuat, geoScale);
			for (int v=0; v<verts.Length; v++){
				verts[v] = matrixAreForKids.MultiplyPoint3x4(verts[v]);
			}

			for (int t3 = 0; t3 < tris.Length; t3+=3) {
				triCent[t3/3] += verts[tris[t3]]; 
				triCent[t3/3] += verts[tris[t3+1]];
				triCent[t3/3] += verts[tris[t3+2]];
				triCent[t3/3] = triCent[t3/3]/3;
				triCent[t3/3][0] = triCent[t3/3][0];
				triCent[t3/3][1] = triCent[t3/3][1]; 
				triCent[t3/3][2] = triCent[t3/3][2]; 
				triCent[t3/3][2] = triCent[t3/3][2];

			}

			for (int tri = 0; tri < tris.Length; tri += 3) {
				edges[tri] = new HalfEdge(tris[tri], tris[tri+1], tri/3);
				edges[tri+1] = new HalfEdge(tris[tri+1], tris[tri+2], tri/3);
				edges[tri+2] = new HalfEdge(tris[tri+2], tris[tri], tri/3);

				edges[tri].adjacentEdge = edges[tri+1];
				edges[tri+1].adjacentEdge = edges[tri+2];
				edges[tri+2].adjacentEdge = edges[tri];
			}

			for (int indX=0; indX<edges.Length; indX++) {
				for (int indY = (indX/3 + 1) * 3; indY<edges.Length; indY++){
					int E1LV = edges[indX].leftVert;
					int E1RV = edges[indX].rightVert;
					int E2LV = edges[indY].leftVert;
					int E2RV = edges[indY].rightVert;

					if (verts[E1LV] == verts[E2RV] && verts[E1RV] == verts[E2LV]){
						if (edges[indX].oppositeEdge != null){Debug.Log("SOMETHINGS WRONG: edge1 already has opposite");}
						if (edges[indY].oppositeEdge != null){Debug.Log("SOMETHINGS WRONG: edge2 already has opposite");}
						edges[indX].oppositeEdge = edges[indY];
						edges[indY].oppositeEdge = edges[indX];
					}
				} 
			}
			//Sanity check on edge opposites
			for (int x=0; x<edges.Length; x++) {
				if (edges[x].oppositeEdge == null){
					Debug.Log("Something is wrong: edge has no opposite");
				}
				Debug.DrawLine(verts[edges[x].leftVert], verts[edges[x].rightVert], Color.white, 200);
			}

			//find face normal
			for (int t=0; t<tris.Length/3; t++){ 
				triNorm[t] = Vector3.Cross(
						(verts[edges[t*3+1].rightVert] - verts[edges[t*3].rightVert]),
						(verts[edges[t*3].leftVert] - verts[edges[t*3].rightVert])
						).normalized;
			} 

			float angleVal = 0;
			for (int tri=0; tri<triNorm.Length; tri++){
				angleVal = Vector3.Dot(triNorm[tri],Vector3.up);
				if (angleVal >= 0.75){ //0.6   max 41 degrees from normal directly up
					//triType[tri] = 0; //top
					triType[tri] = SurfaceType.top;
					Debug.DrawRay(triCent[tri], triNorm[tri], Color.blue, 200);
				} else if (angleVal >= 0.4){  //  24 to 48 from parallel to ground
					//triType[tri] = 1; //scramble
					triType[tri] = SurfaceType.scramble;
					Debug.DrawRay(triCent[tri], triNorm[tri], Color.yellow, 200);
				} else if (angleVal >= -0.4){ //0.2   max 23 degrees from parallel to ground plane
					//triType[tri] = 2; //side        min -23 from parllel
					triType[tri] = SurfaceType.side;
					Debug.DrawRay(triCent[tri], triNorm[tri], Color.red, 200);
				} else if (angleVal >= -1){          //min -24 from parallel
					//triType[tri] = 3; //bottom
					triType[tri] = SurfaceType.bottom;
					Debug.DrawRay(triCent[tri], triNorm[tri], Color.green, 200);
				}
			}

			for (int e=0; e<edges.Length; e++) {
				if (triType[edges[e].triangle] == SurfaceType.top 
				    	&& 	(triType[edges[e].oppositeEdge.triangle] == SurfaceType.side 
				    	|| triType[edges[e].oppositeEdge.triangle] == SurfaceType.bottom) 
				    	){ //top and side
					if (triCent[edges[e].triangle][1] > triCent[edges[e].oppositeEdge.triangle][1]){ 
						//if top center y value is above side center
						//then the edges are part of a ledge.
						//Now do I set both to be the edge or just the top side....  now setting both

						//Debug.DrawLine(verts[edges[e].leftVert], verts[edges[e].rightVert], Color.magenta, 200);


						edges[e].ledge = true;
						edges[e].oppositeEdge.ledge = true;
					}
				}
				//if scramble and the opposite side is top, scramble or side, and is enough of an angle appart from
				//the first side, and if the resulting edge is flat enogh to be a top
				if(triType[edges[e].triangle] == SurfaceType.scramble && triType[edges[e].oppositeEdge.triangle] != SurfaceType.bottom){
					//if the scramble edge is connecting to anything other than a bottom.
					//need to calculate the differnce between the angles, they need to be 225 degrees appart or 45
					//im confusing myself with the math and geometry at the moment....
					//take face normals, calculate dot, if dot > .5 not an edge, else, it should be good
					//then check flatness of edge
					if (Vector3.Dot( triNorm[edges[e].triangle], triNorm[edges[e].oppositeEdge.triangle]) < 0.75){  //Side to top angle tolerance
						float edgeToUpAngle = Vector3.Dot( 
						            new Vector3(verts[edges[e].leftVert].x - verts[edges[e].rightVert].x , 
						        			verts[edges[e].leftVert].y - verts[edges[e].rightVert].y , 
						        verts[edges[e].leftVert].z - verts[edges[e].rightVert].z) , Vector3.up);
						//print(edgeToUpAngle);
						if ( edgeToUpAngle < 0.25 && edgeToUpAngle > -0.25){  //angle to decide if edge is within the line to be a top
							//Debug.DrawLine(verts[edges[e].leftVert], verts[edges[e].rightVert], Color.magenta, 200);
							//print("setting edge");
							edges[e].ledge = true;
							edges[e].oppositeEdge.ledge = true;
						}
					}
				}
			}
			string tmpname = "";
			for (int e=0; e<edges.Length; e++) { //this is where you would construct the trigger

				if (edges[e].ledge){
					Debug.DrawLine(verts[edges[e].leftVert], verts[edges[e].rightVert], Color.magenta, 200, true);

					GameObject tmp = new GameObject();//create empty child to hold collider
					tmp.tag = "edge";
					tmpname = tmpname + "1";
					tmp.name = tmpname;

					tmp.transform.localPosition = (verts[edges[e].leftVert] + verts[edges[e].rightVert])/2;

					Vector3 dir = (verts[edges[e].leftVert] - verts[edges[e].rightVert]).normalized;
					tmp.transform.rotation = Quaternion.LookRotation(Vector3.up,dir);
		


					CapsuleCollider col = tmp.AddComponent<CapsuleCollider>();
					col.isTrigger = true;

					col.radius = edge_radius;
					col.height = Vector3.Distance(verts[edges[e].leftVert], verts[edges[e].rightVert]);

					tmp.transform.parent = cube.transform;


				}
			}

			//assign values and return object data 
			objd.edges = edges;
			objd.tris = tris;
			objd.triType = triType;
			objd.verts = verts;

		}
		return objd;
	}

}
