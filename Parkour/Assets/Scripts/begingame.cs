using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class begingame : MonoBehaviour {

	private Text txt;
	public Image img;
	public AnimationCurve ac;
	public AnimationCurve ac2;

	public float fadeinduration1 = 2f;
	public float fadeinduration2 = 2f;
	private float endtime1,endtime2;

	// Use this for initialization
	void Start () {
		txt = GetComponent<Text>();
		txt.color = new Color(txt.color.r,txt.color.g,txt.color.b,0);
		img.color = new Color(img.color.r,img.color.g,img.color.b,0);
		endtime1 = Time.time + fadeinduration1;
		endtime2 = endtime1 + fadeinduration2;
	}
	
	// Update is called once per frame
	void Update () {

		if(img.color.a < 1){
			float val1 = Time.time/endtime1;
			img.color = new Color(img.color.r,img.color.g,img.color.b,ac.Evaluate(val1));
			
		}else{
			if(txt.color.a < 1){
				float val2 = Time.time/endtime2;
				txt.color = new Color(txt.color.r,txt.color.g,txt.color.b,ac2.Evaluate(val2));

			}
		}
		if(Input.anyKeyDown){
			Application.LoadLevel("main_scene");
		}
	}
}
