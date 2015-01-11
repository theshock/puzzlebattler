using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct test
{
	int a;
	int b;
};

public class CGameData : MonoBehaviour
{
	public List<CLevelModel> mLevelModelArr = new List<CLevelModel>();
	public CLevelModel mCurrentLevel = null;
	private int mIndexCurrentLevel = 0;
	private float mCurrentExp = 0;
	private float mWaitIncreaseExp = 0;
	public float mPrExp;
	public EMatchIconType mLastIconMatch;

	public float mStartTimeWaitAction = 3.0f;
	private float mCurrentTimeWaitAction;

	private bool mIsDecreaseEnergy = false;

	public float mStartTimeDecrease = 1.0f;
	private float mTimeDecrease = 1.0f;

	public void doIncreaseWaitExp(int aExp)
	{
		mWaitIncreaseExp += aExp;
	}

	public void doResetTimer()
	{
		mCurrentTimeWaitAction = mStartTimeWaitAction;
		mTimeDecrease = mStartTimeDecrease;
		mIsDecreaseEnergy = false;
	}

	private void doDecreaseEnergy()
	{
		if(mCurrentLevel != null && mCurrentExp > 0)
		{
			mCurrentExp -= mCurrentLevel.mEnergyFallPerSecond;

			if(mCurrentExp < mCurrentLevel.mMinExp)
			{
				mCurrentExp = mCurrentLevel.mMinExp;
			}
		}
	}

	private void doUpdateCurrentExp(float aDelta)
	{
		float delta_val = 0;
		if(mWaitIncreaseExp > mCurrentLevel.mMinExp * 0.05f)
		{
			delta_val = mWaitIncreaseExp * aDelta;
			mWaitIncreaseExp -= delta_val;
		}
		else
		{
			delta_val = mWaitIncreaseExp;
			mWaitIncreaseExp = 0;
		}


		mCurrentExp += delta_val;

		if(mCurrentExp >= mCurrentLevel.mMaxExp)
		{
			mCurrentExp = mCurrentLevel.mMaxExp;
			mIndexCurrentLevel++;

			if(mIndexCurrentLevel < mLevelModelArr.Count)
			{
				mCurrentLevel = mLevelModelArr[mIndexCurrentLevel];
				mCurrentExp = mCurrentLevel.mMinExp;
			}
		}
	}

	public void decreaseLevelModel()
	{
		if(mIndexCurrentLevel > 0)
		{
			mIndexCurrentLevel--;
			mCurrentLevel = mLevelModelArr[mIndexCurrentLevel];
			mCurrentExp = mCurrentLevel.mMinExp;
		}
	}

	public Vector2 getCurrentSpeed()
	{
		Vector2 min_speed = mCurrentLevel.mMinSpeed;
		Vector2 max_speed = mCurrentLevel.mMaxSpeed;

		float pr = mPrExp;

		Vector2 difference_speed = max_speed - min_speed;
		Vector2 res = min_speed + difference_speed * pr;

		return res;

	}

	// Use this for initialization
	void Start ()
	{
		mCurrentTimeWaitAction = mStartTimeWaitAction;
		mTimeDecrease = mStartTimeDecrease;
		mCurrentLevel = mLevelModelArr[0];
	}

	// Update is called once per frame
	void Update ()
	{
		if(mIsDecreaseEnergy)
		{
			if(mTimeDecrease > 0)
			{
				mTimeDecrease -= Time.deltaTime;

				if(mTimeDecrease <= 0)
				{
					mTimeDecrease = mStartTimeDecrease;
					doDecreaseEnergy();
				}
			}
		}
		else if(mWaitIncreaseExp <= 0)
		{
			mCurrentTimeWaitAction -= Time.deltaTime;
			
			if(mCurrentTimeWaitAction <= 0)
			{
				mCurrentTimeWaitAction = mStartTimeWaitAction;
				mIsDecreaseEnergy = true;

				mTimeDecrease = mStartTimeDecrease;
				doDecreaseEnergy();
			}
		}

		if(mWaitIncreaseExp > 0)
		{
			doUpdateCurrentExp(Time.deltaTime);
		}

		float exp = mCurrentExp - mCurrentLevel.mMinExp;
		float def = mCurrentLevel.mMaxExp - mCurrentLevel.mMinExp;
		
		mPrExp = (exp / def);

//		UnityEngine.Debug.Log(mPrExp);
	}
}

