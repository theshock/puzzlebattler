using UnityEngine;
using System.Collections;

public class CGameUI : MonoBehaviour
{
	public CGameController mGameController;
	public Sprite[] iconSprites;
	public SpriteRenderer mIconLastMatch;

	void Awake()
	{
		iconSprites = new Sprite[(int)EIconType.eCount];
		// load all frames in fruitsSprites array
		for(int i = 0; i < (int)EIconType.eCount; i++)
		{
			string name = "ui_match_icon/ui_match_icon_" + i;
			Debug.Log(name);
			var spr = Resources.Load<Sprite>(name);
			iconSprites[i] = spr;
		}
		
	}



	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{

	}
}

