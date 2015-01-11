using UnityEngine;
using System.Collections;

public class CMatchActionDestroy : CMatchBaseAction
{
	private float mTimeDelayEmpty = 0.1f;
	private CMatchIcon mIconTarget = null;

	public static IMatchAction create()
	{
		return new CMatchActionDestroy();
	}

	public override bool validation()
	{
//		Debug.Log("CMatchActionDestroy validation");
//		Debug.Log("CMatchActionDestroy validation mIconTarget State" + mIconTarget.IconState);

		if(!mIconTarget)
		{
//			Debug.Log("CMatchActionDestroy validation fail 1"); 

			return false;
		}
		
		if(!mIconTarget.getIsReadyAction())
		{
//			Debug.Log("CMatchActionDestroy validation fail 2"); 

			return false;
		}

//		Debug.Log("CMatchActionDestroy validation ok"); 

		return true;
	}

	public override void startAction()
	{
//		Debug.Log("CMatchActionDestroy startAction"); 

		mIconTarget.IconState = EMatchIconState.eLockIcon;
		mIconTarget.onDestroyIcon();

//		Debug.Log("CMatchActionDestroy startAction 1"); 

		CHandlerUpdateManager.shared().subscribeUpdateEvent(doUpdate);
	}

	public void emptyCell()
	{
//		Debug.Log("CMatchActionDestroy emptyCell"); 

		mIconTarget.IconState = EMatchIconState.eClearIcon;
		complateAction();
	}

	public void doUpdate()
	{
		mTimeDelayEmpty -= Time.smoothDeltaTime;
//		Debug.Log("CMatchActionDestroy doUpdate mTimeDelayEmpty = " + mTimeDelayEmpty); 

		if(mTimeDelayEmpty <= 0)
		{
			CHandlerUpdateManager.shared().unsubscribeUpdateEvent(doUpdate);
			emptyCell();
		}
	}
	

	public override GameNotificationEvents getActionEvent()
	{
		return  GameNotificationEvents.eMatchActionDestroyEvent;
	}

	public override void doUpdateActionParam(Hashtable aData)
	{
		mIconTarget = aData["target"] as CMatchIcon;

		return;
	}

	public void destroyArray(ArrayList aCells)
	{

	}
}

