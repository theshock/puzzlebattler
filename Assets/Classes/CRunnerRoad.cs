using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CRunnerRoad : MonoBehaviour
{
	public CRunnerPlayer mPlayer = null;
	public CRunnerController mRunnerController;
	public List<GameObject> mPartSprArr = new List<GameObject>();
	private int mCurrentIndex = 0;
	private float mOffsetX;
	public Sprite[] iconSprites;
	public CTriggerScript mFinishTrigger;
	public bool mIsFinish = false;
	public float mOffsetFixX = 0.0f;

	void Awake()
	{
		iconSprites = new Sprite[(int)EMatchIconType.eMatchIconTypeCount];
		// load all frames in fruitsSprites array
		for(int i = 0; i < (int)EMatchIconType.eMatchIconTypeCount; i++)
		{
			string name = "barricade_match_icon/barricade_match_icon_" + i;
			Debug.Log(name);
			var spr = Resources.Load<Sprite>(name);
			iconSprites[i] = spr;
		}
		
	}

	public void doTryJump()
	{
		mPlayer.doJump();

//		mOffsetFixX += 2.0f;
//		transform.position += new Vector3(2, 0, 0);
	}

	public void doBreakOnBarricade()
	{
		mRunnerController.mGameController.mGameData.decreaseLevelModel();
	}

	public void doScrollRoad(Vector2 aOffset)
	{
		if(mIsFinish)
			return;

		if(mOffsetFixX <= 0.0f)
		{
			transform.position -= new Vector3(aOffset.x, aOffset.y, 0);
			mPlayer.doUpdatePosition(aOffset);
		}
		else
		{
			mOffsetFixX -= aOffset.x;

			if(mOffsetFixX < 0)
			{
				mOffsetFixX = 0;
			}
		}


	}

	public void onTriggerAction(CTriggerRoadScript aTrigger)
	{
		UnityEngine.Debug.Log("onTriggerAction");

		GameObject obj = mPartSprArr[mCurrentIndex];
		obj.transform.position += new Vector3((mOffsetX * 2.0f), 0 , 0);

		if(mCurrentIndex == 0)
		{
			mCurrentIndex = 1;
		}
		else
		{
			mCurrentIndex = 0;
		}

	}
	// Use this for initialization
	void Start ()
	{
		mOffsetX = mPartSprArr[1].transform.position.x - mPartSprArr[0].transform.position.x;
		mFinishTrigger.mDelegate = onTriggerDelegate;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void onTriggerDelegate(Collider2D aObj, CTriggerScript aScript)
	{
		if(aObj.gameObject.GetComponent<CRunnerPlayer>() != null)
		{
			if(aScript == mFinishTrigger)
			{
				mIsFinish = true;
				mFinishTrigger.gameObject.GetComponent<SpriteRenderer>().enabled = false;
			}
		}
		
	}
}

