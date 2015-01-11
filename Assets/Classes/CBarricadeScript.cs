using UnityEngine;
using System.Collections;

public class CBarricadeScript : MonoBehaviour
{
	public SpriteRenderer mIconNeedSpr;
	public CRunnerRoad mRoad;
	public CTriggerScript mBarricadeTriger;
	public CTriggerScript mTryJumpTrigger;

	// Use this for initialization
	void Start ()
	{
//		mBarricadeTriger.mDelegate = onTriggerDelegate;
		mTryJumpTrigger.mDelegate = onTriggerDelegate;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void updateNeedIcon(EMatchIconType aIcon)
	{
		mIconNeedSpr.sprite = mRoad.iconSprites[(int) aIcon];
	}

	public void onTriggerDelegate(Collider2D aObj, CTriggerScript aScript)
	{
		if(aObj.gameObject.GetComponent<CRunnerPlayer>() != null)
		{
			if(aScript == mTryJumpTrigger)
			{
				mRoad.doTryJump();
			}
			else if(aScript == mBarricadeTriger)
			{
//				mRoad.doTryJump();
			}
		}

	}
}

