using UnityEngine;
using System.Collections;


public class CGameController : MonoBehaviour,  INotificationObserver
{
	public CNotificationManager mNotificationManager = new CNotificationManager();
	public CGameData mGameData;
	public CGameUI mGameUI;
	public CMatchController mMatchController;
	public CRunnerController mRunnerController;

	public void handleNotification(int aEvent, Object aParam, CNotificationManager aManager)
	{
		if(mMatchController.mNotificationManager == aManager)
		{
			switch((GameNotificationEvents)aEvent)
			{
				case GameNotificationEvents.eMatchActionMatchEvent:
				{
//					CMatchActionMatch action = aParam as CMatchActionMatch;
//					int count = action.getCountMatchIcon();
//					int mult = calculateMult(count);
//					int value = count * mGameData.mCurrentLevel.mEnergyForOneIcon * mult;
//
//					mGameData.doIncreaseWaitExp(value);
//					mGameData.doResetTimer();
//					mGameData.mLastIconMatch = action.getMatchIconType();
//					mGameUI.updateIconLastMatch();
				}
					break;
			}
		}
		return;
	}

	int calculateMult(int aCountMatch)
	{
		int res = 1;
		if (aCountMatch < 4)
			res = 3;
		else
			res = (((aCountMatch-1) * aCountMatch) / 2) - (aCountMatch - 3);
		
		return res;

	}
	// Use this for initialization
	void Start ()
	{
		mMatchController.mNotificationManager.addObserver((int)GameNotificationEvents.eMatchActionMatchEvent, this);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
