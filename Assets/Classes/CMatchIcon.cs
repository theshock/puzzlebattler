using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum EMatchIconType
{
	eMatchIconRed,
	eMatchIconGreen,
	eMatchIconBlue,
	eMatchIconPurple,
	eMatchIconYellow,
	eMatchIconTypeCount
};

public enum EMatchIconState
{
	eLockIcon,
	eClearIcon,
	eOpenIcon,
	eInvisibleIcon,
	eMatchIconStateCount
};

public class CMatchIcon : MonoBehaviour 
{
	float mTimeDelay = 0.1f;

	public delegate void SwipeDelegate();
	public SwipeDelegate mDelegate = null;

	private EMatchIconType mIconType;
	public EMatchIconType IconType
	{ 
		get
		{
			return mIconType;
		}
		set
		{
			mIconType = value;

			if(mIconSpriteRenderer)
			{
				Sprite spr = mMatchField.iconSprites[(int)mIconType];

				mIconSpriteRenderer.sprite = spr;
				mIconSpriteRenderer.SetNativeSize();
			}

		}
	}


	private EMatchIconState mIconState;
	public EMatchIconState IconState { 
		get
		{
			return mIconState;
		}
		set
		{
			mIconState = value;

			if(mIconSpriteRenderer)
			{
				switch(mIconState)
				{
					case EMatchIconState.eClearIcon:
					{
						mIconSpriteRenderer.enabled = false;
					}
						break;
						
//					case EMatchIconState.eInvisibleIcon:
//					{
//						mIconSpriteRenderer.enabled = false;
//					}
//						break;
						
					default:
					{
						mIconSpriteRenderer.enabled = true;
					}
						break;
				}
			}
			else
			{
				Debug.LogError("mIconSpriteRenderer fail");
			}

		}
	}

	public int mRow;
	public int mColumn;
	public int mIndex { get; set; }
	public Image mIconSpriteRenderer = null;
	public CMatchField mMatchField { get; set; }

	public string getLogInfo()
	{
		return "Icon Info [Row : " + mRow + "] [mCol : " + mColumn + "] [State : " + mIconState + "]";
	}

	CMatchIcon()
	{

	}
	// Use this for initialization
	void Start () 
	{
		if(mIconSpriteRenderer != null)
		{
//			mIconSpriteRenderer.sortingLayerID = 1;
//			mIconSpriteRenderer.sortingLayerName = "match_view";
		}
		else
		{
			mIconSpriteRenderer = GetComponent<Image>();
//			mIconSpriteRenderer.sortingLayerID = 1;
		}

//		gameObject.layer = 8;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void onDestroyIcon()
	{
		GameObject anim = Instantiate(mMatchField.mPrefab[(int)mIconType], transform.position, transform.rotation) as GameObject;
		anim.transform.SetParent (transform.parent);

		Destroy (anim, 2);
	}

	public bool getIsReadyAction()
	{
		return (mIconState == EMatchIconState.eOpenIcon);
	}

	public bool getIsReadyMove()
	{
		return (mIconState == EMatchIconState.eOpenIcon);
	}

	public bool hitTest(Vector2 aPos)
	{
		bool res = false;
		Vector2 convert_pos = Camera.main.ScreenToWorldPoint(aPos);


		if (collider2D.OverlapPoint(convert_pos))
		{
			res = true;			
		}

		return res;
	}

	public void initWithParams(CMatchField aMatchField, Vector2 aIconPos, EMatchIconType aIconType, int aIndex)
	{
		mMatchField = aMatchField;
		mRow = (int)aIconPos.x;
		mColumn = (int) aIconPos.y;
		mIndex = aIndex;
	}

	public void onEndSwipeAnimation()
	{
//		Debug.Log("CMatchIcon onEndSwipeAnimation");

		if(mDelegate != null)
		{
//			Debug.Log("CMatchIcon onEndSwipeAnimation call delegate");

			mDelegate.Invoke();
		}
//		Debug.Log("CMatchIcon onEndSwipeAnimation end");
	}

	public bool moveTo(int aRow, int aCol, float aDelay)
	{
		Vector2 pos = mMatchField.getIconCenterByIndex(aRow, aCol);
		Vector3 pos3d = new Vector3(pos.x, pos.y, this.transform.position.z);

		if(this.transform.position == pos3d)
			return false;

//		if(mIconState == EMatchIconState.eInvisibleIcon || mIconState == EMatchIconState.eClearIcon)
//		{
//			this.transform.position = pos3d;
//			return false;
//		}

		mIconState = EMatchIconState.eLockIcon;
		iTween.MoveTo(this.gameObject, iTween.Hash("position",pos3d,"time",mTimeDelay,"onComplete", "onEndMoveComplete"));

		return true;
	}

	public void onEndMoveComplete()
	{
		mIconState = EMatchIconState.eOpenIcon;
		mMatchField.onEndMoveComplete(this);
	}
}
