using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CMatchField : MonoBehaviour {
	private CMatchIcon [,] mIconMatrix;
	public int mRows;
	public int mColumns;
	public Vector2 mStartPoint;
	public Vector2 mOffset;
	public delegate void UpdatePositionDelegate();
	private UpdatePositionDelegate mDelegate;
	private int mCountStartMoveCells;
	public Sprite[] iconSprites;
	public GameObject[] mPrefab = null;

	void Awake()
	{
		iconSprites = new Sprite[(int)EMatchIconType.eMatchIconTypeCount];
		// load all frames in fruitsSprites array
		for(int i = 0; i < (int)EMatchIconType.eMatchIconTypeCount; i++)
		{
			string name = "match_icon/match_icon_" + i;
			var spr = Resources.Load<Sprite>(name);
			iconSprites[i] = spr;
		}

	}

	// Use this for initialization
	void Start () 
	{

	}

	public void initMatchField()
	{
		mIconMatrix = new CMatchIcon[mRows, mColumns];
		
		for(int r = 0; r < mRows; r++)
		{
			for(int c = 0; c < mColumns; c++)
			{
				mIconMatrix[r,c] = null;
				createIconByPos(new Vector2(r,c), EMatchIconType.eMatchIconTypeCount, true);
			}
		}

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void printfField()
	{
//		for(int r = 0; r < mRows; r++)
//		{
//			for(int c = 0; c < mColumns; c++)
//			{
//
//			}
//		}
	}

	public void swipeCellInMatrix(CMatchIcon aFirstIcon, CMatchIcon aSecondIcon)
	{

		int t_r = aFirstIcon.mRow;
		int t_c = aFirstIcon.mColumn;
		int t_i = aFirstIcon.mIndex;

		mIconMatrix[aFirstIcon.mRow, aFirstIcon.mColumn] = aSecondIcon;
		mIconMatrix[aSecondIcon.mRow, aSecondIcon.mColumn] = aFirstIcon;

		aFirstIcon.mRow = aSecondIcon.mRow;
		aFirstIcon.mColumn = aSecondIcon.mColumn;
		aFirstIcon.mIndex = aSecondIcon.mIndex;

		aSecondIcon.mRow = t_r;
		aSecondIcon.mColumn = t_c;
		aSecondIcon.mIndex = t_i;

	}

	EMatchIconType genIconType()
	{
		int count = EMatchIconType.eMatchIconTypeCount.GetHashCode();
		int type = Random.Range(0, count);

		EMatchIconType res = (EMatchIconType) type;

		return res;
	}

	public void onEndMoveComplete(CMatchIcon aIcon)
	{
		mCountStartMoveCells--;

		if(mCountStartMoveCells == 0)
		{
			mDelegate.Invoke();
		}
	}

	public int updatePositionIcons(UpdatePositionDelegate aDelegate)
	{
		mCountStartMoveCells = 0;
		mDelegate = aDelegate;
//		
//		// TRACE("START MOVE CELLS DOWN")	
//		
//		
		for ( int c = 0; c < mColumns; c++ )
		{
			bool moved = true;
			while (moved)
			{
				moved = false;
				for( int r = 0; r < mRows; r++ )
				{
					if ( mIconMatrix[ r , c ].IconState == EMatchIconState.eClearIcon && (r + 1) < mRows && mIconMatrix[ r + 1 , c ].IconState != EMatchIconState.eClearIcon )
					{
						swipeCellInMatrix(mIconMatrix[r,c], mIconMatrix[r + 1,c]);
						
						printfField();
						moved = true;
					}
				}
			}
		}

		fillFreeIcons();
//		//  TRACE("END MOVE CELLS DOWN")
//		
		for ( int c = 0; c < mColumns; c++ )
		{
			float delay = 0;
			for( int r = 0; r < mRows; r++ )
			{
				if ( mIconMatrix[ r , c ].moveTo(r, c, delay) )
				{
					mCountStartMoveCells++;
					delay += 0.01f;
				}
				
			}
		}

		return mCountStartMoveCells;
	}

	void createIconByPos(Vector2 aIconPos, EMatchIconType aIconType, bool aIsSetStartPosition)
	{
		if(aIconType == EMatchIconType.eMatchIconTypeCount)
		{
			aIconType = genIconType();
		}

		int r = (int)aIconPos.x;
		int c = (int)aIconPos.y;

		if(mIconMatrix[r,c] == null)
		{
//			Debug.Log("createIconByPos new Object");

 			GameObject icon = createIcon();
			CMatchIcon component = icon.GetComponent<CMatchIcon>();
			Image render = icon.GetComponent<Image>();
			BoxCollider2D collider = icon.GetComponent<BoxCollider2D>();

			collider.size = new Vector2(157.0f,158.0f);

			mIconMatrix[r,c] = component;
			
			component.initWithParams(this, new Vector2(r,c), aIconType, r * mColumns + c);
			component.mIconSpriteRenderer = render;
			
			if(r < mRows / 2)
			{
				component.IconState = EMatchIconState.eOpenIcon;
			}
			else
			{
				component.IconState = EMatchIconState.eInvisibleIcon;
			}
			
			
//			icon.transform.SetParent(this.transform);
			icon.transform.localScale = new Vector3(1,1,1);
			if(aIsSetStartPosition)
			{
				icon.transform.position = getIconCenterByIndex(r,c);
			}
			else
			{
				icon.transform.position = getIconCenterByIndex(r + mRows/2,c);
			}
			
			component.IconType = aIconType;
		}
		else
		{
//			Debug.Log("createIconByPos reinit Object");

			CMatchIcon component = mIconMatrix[r,c];
			GameObject icon = component.gameObject;

			component.initWithParams(this, new Vector2(r,c), aIconType, r * mColumns + c);
			
			if(r < mRows / 2)
			{
				component.IconState = EMatchIconState.eOpenIcon;
			}
			else
			{
				component.IconState = EMatchIconState.eInvisibleIcon;
			}
			


			if(aIsSetStartPosition)
			{
				icon.transform.position = getIconCenterByIndex(r,c);
			}
			else
			{
				icon.transform.position = getIconCenterByIndex(r + mRows/2,c);
			}
			
			component.IconType = aIconType;
		}


	}

	GameObject createIcon()
	{
		GameObject icon = new GameObject();
		icon.AddComponent<CanvasRenderer>();
		icon.AddComponent<BoxCollider2D>();
		icon.AddComponent<Image>();
		icon.AddComponent("CMatchIcon");
		icon.transform.SetParent(this.transform);

		return icon;
	}

	void fillFreeIcons()
	{
		for ( int c = 0; c < mColumns; c++ )
		{
			for( int r = 0; r < mRows; r++ )
			{
				if ( mIconMatrix[ r , c ].IconState ==  EMatchIconState.eClearIcon)
				{
					createIconByPos(new Vector2(r,c), EMatchIconType.eMatchIconTypeCount, false);
				}
				
			}
		}
	}

	public ArrayList getIconsByRow(int aRow)
	{
		ArrayList res = new ArrayList();

		for(int c = 0; c < mColumns; c++)
		{
			res.Add(mIconMatrix[aRow, c]);
		}


		return res;
	}

	public ArrayList getIconsByColumn(int aColumn)
	{
		ArrayList res = new ArrayList();
		
		for(int r = 0; r < mRows; r++)
		{
			res.Add(mIconMatrix[r, aColumn]);
		}
		
		
		return res;
	}

	public bool iconsInTheSameRow(ArrayList  aSlice)
	{
		if(aSlice.Count < 1)
			return false;

		CMatchIcon first_icon = aSlice[0] as CMatchIcon;

		int standart = first_icon.mRow;
		int count = 0;

		foreach(CMatchIcon icon in aSlice)
		{
			if(icon.mRow == standart)
			{
				count++;
			}
		}

		bool res = (count == aSlice.Count);

		return res;
	}

	public bool iconsInTheSameCol(ArrayList  aSlice)
	{
		if(aSlice.Count < 1)
			return false;
		
		CMatchIcon first_icon = aSlice[0] as CMatchIcon;
		
		int standart = first_icon.mColumn;
		int count = 0;
		
		foreach(CMatchIcon icon in aSlice)
		{
			if(icon.mColumn == standart)
			{
				count++;
			}
		}
		
		bool res = (count == aSlice.Count);
		
		return res;
	}

	public bool isTheSameIconOne(ArrayList  aSlice)
	{
		if(aSlice.Count < 1)
			return false;
		
		CMatchIcon first_icon = aSlice[0] as CMatchIcon;
		
		EMatchIconType standart = first_icon.IconType;
		int count = 0;

//		Debug.Log("aSlice.Count = " + aSlice.Count);

		foreach(CMatchIcon icon in aSlice)
		{
			if(!icon)
			{
//				Debug.Log("isTheSameIconOne fail");
			}

			if(icon.IconType == standart)
			{
				count++;
			}
		}
		
		bool res = (count == aSlice.Count);
		
		return res;
	}

	public bool isHaveEmptyIcon()
	{
//		Debug.Log("isHaveEmptyIcon");
		for(int r = 0; r < mRows; r++)
		{
			for(int c = 0; c < mColumns; c++)
			{
//				Debug.Log("isHaveEmptyIcon Icon State [r =" +r + "], [c = " +c + "], [State = " + mIconMatrix[r,c].IconState + "]");

				if(mIconMatrix[r,c].IconState == EMatchIconState.eClearIcon)
				{
					return true;
				}
			}
		}

		return false;
	}

//	IMatchIcon* CDefaultMatchCellField::getCentralIcon(std::vector<IMatchIcon*> aSlice)
//	{
//		std::sort(aSlice.begin(), aSlice.end(), IMatchIcon::predicateColumn);
//		IMatchIcon* centr_cell = aSlice[(aSlice.size() - 1)/2];
//		
//		return centr_cell;
//	}
//	
	public Vector3 getIconCenterByIndex(int aRow, int aColumn)
	{
		return new Vector3(mStartPoint.x + aColumn * mOffset.x,mStartPoint.y + aRow * mOffset.y, 0);
	}

	public Vector3 getIconCenterByIndex(int aIndex)
	{
		int row = aIndex/mRows;
		int column = aIndex - row * mColumns;

		return new Vector3(mStartPoint.x + column * mOffset.x,mStartPoint.y + row * mOffset.y, 0);
	}

	public CMatchIcon getIconByPos(int aRow, int aColumn)
	{
		return mIconMatrix[aRow, aColumn];
	}

	public CMatchIcon getIconByPos(Vector2 aPos)
	{
//		Debug.Log(aPos);

		for(int r = 0; r < mRows; r++)
		{
			for(int c = 0; c < mColumns; c++)
			{
				CMatchIcon icon = mIconMatrix[r,c];

//				if(!icon)
//				{
//					Debug.Log("fail icon");
//					Debug.Log(r);
//					Debug.Log(c);
//				}

				if(icon && icon.hitTest(aPos) && icon.getIsReadyMove())
				{
					return icon;
				}
			}
		}

		return null;
	}

	public CMatchIcon getIconByIndex(int aIndex)
	{
		if(aIndex >= mRows * mColumns)
			return null;

//		Debug.Log("[mRows " + mRows + "] [mColumns " + mColumns + "]");
//		Debug.Log("[aIndex " + aIndex + "]");
		int row = aIndex / mColumns;
		int column = aIndex - row * mColumns;

//		Debug.Log("getIconByIndex [row " + row + "] [column " + column + "]");

		return mIconMatrix[row,column];
	}
}
