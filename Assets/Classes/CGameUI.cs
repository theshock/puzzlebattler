using UnityEngine;
using System.Collections;
using Match;

public class CGameUI : MonoBehaviour {
	public CGameController mGameController;
	public SpriteRenderer mIconLastMatch;
	public Sprite[] iconSprites;

	void Awake() {
		iconSprites = new Sprite[(int)EIconType.eCount];
		// load all frames in fruitsSprites array
		for (int i = 0; i < (int)EIconType.eCount; i++) {
			string name = "ui_match_icon/ui_match_icon_" + i;
			Debug.Log(name);
			var spr = Resources.Load<Sprite>(name);
			iconSprites[i] = spr;
		}

	}
}

