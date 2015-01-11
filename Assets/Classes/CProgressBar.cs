using UnityEngine;
using System.Collections;

public class CProgressBar : MonoBehaviour
{
	public float barDisplay; //current progress
	public Vector2 pos = new Vector2(20,40);
	public Vector2 size = new Vector2(60,20);
	public Vector2 other_size = new Vector2(60,20);
	public Texture2D otherTex;
	public Texture2D fullTex;
	
	void OnGUI()
	{
//		Debug.Log("OnGUI");
//		Debug.Log(barDisplay);

		float size_x = size.x * barDisplay;
		//draw the background:
		GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y + other_size.y));

		//draw the filled-in part:
		GUI.BeginGroup(new Rect(0,0, size.x * barDisplay, size.y));
		GUI.DrawTexture(new Rect(0,0, size.x, size.y), fullTex);
		GUI.EndGroup();



		GUI.EndGroup();

		GUI.DrawTexture(new Rect(pos.x + size_x - 30,pos.y -10, other_size.x, other_size.y), otherTex);

//		Debug.Log("OnGUI end");
	}

	void Start() 
	{

	}

	void Update() 
	{

	
	}
}

