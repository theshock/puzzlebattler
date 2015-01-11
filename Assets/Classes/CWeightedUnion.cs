using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CWeightedUnion
{
	private Dictionary<int, ArrayList> mTrees = new Dictionary<int, ArrayList>();
	private ArrayList mId = new ArrayList();
	private ArrayList mSz = new ArrayList();
	private int mElements;
	private bool mHasUnions = false;


	public bool getHasUnions()
	{
		return mHasUnions;
	}

	public void initWithElements(int elementsCount)
	{
		mElements = elementsCount;
		
		for (int i = 0; i < mElements; i++)
		{
			mId.Add(i);
		}
		
		for (int i = 0; i < mElements; i++)
		{
			mSz.Add(1);
		}
	}

	public int find(int aParam)
	{
		while (aParam != (int)mId[aParam])
			aParam = (int) mId[aParam];
		
		return aParam;
	}

	public bool number(int p, int q)
	{
		return (this.find(p) == this.find(q));
	}

	public void unionInt(int p, int q)
	{
		int i = find(p);
		int j = find(q);
		
		if (i == j) return;
		
		// make smaller root point to larger one
		if ((int)mSz[i] < (int)mSz[j])
		{
			mId[i] = j;
			mSz[j] = i;
		}
		else
		{
			mId[j] = i;
			
			int szI = (int)mSz[i];
			int szJ = (int)mSz[j];
			
			mSz[i] = szI + szJ;
			
		}
		
		mHasUnions = true;
	}

	public Dictionary<int, ArrayList> getTrees()
	{
		for (int i = 0; i < mElements; i++)
		{
			int iVal = (int)mId[i];
			
			if (i != iVal)
			{
				int iRoot = find(i);

			//	Debug.Log("iRoot = " + iRoot);

				ArrayList it = null;

				if(mTrees.ContainsKey(iRoot))
			    {
					it = mTrees[iRoot];
				}


				 
				
				if (it == null)
				{
					ArrayList icons = new ArrayList();
					icons.Add(iRoot);

					mTrees.Add(iRoot, icons);
				}
				
				it = mTrees[iRoot];
				it.Add(i);
			}
		}
		


		return mTrees;
	}
}
