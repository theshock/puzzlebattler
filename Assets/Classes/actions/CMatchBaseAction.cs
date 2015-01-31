using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


public class CMatchBaseAction : UnityEngine.Object, IMatchAction
{
	protected CGameActionManager mActionManager;
	protected MatchActionDelegate mDelegate;
	protected CField mIconField;

	public CMatchBaseAction()
	{
//		UnityEngine.Debug.Log("create Action " + getActionEvent());
	}

	public void initWithActionManager(CGameActionManager aManager, CField aIconField)
	{
		mActionManager = aManager;
		mIconField = aIconField;
	}

	public void setDelegate(MatchActionDelegate aDelegate)
	{
		mDelegate = aDelegate;
	}

	public void updateting()
	{
		
	}

	public virtual void doUpdateActionParam(Hashtable aData)
	{
		return;
	}
	
	public virtual bool validation()
	{
		return false;
	}
	
	public virtual void startAction()
	{

	}

	public virtual void complateAction()
	{
//		UnityEngine.Debug.Log("end Action " + getActionEvent());
//		UnityEngine.Debug.Log("complateAction");
		try
		{
//			UnityEngine.Debug.Log("complateAction 1");
			mActionManager.mMatchController.mNotificationManager.notify((int)getActionEvent(), this);
//			UnityEngine.Debug.Log("complateAction 1_1");

			if(mDelegate != null)
			{
				mDelegate.Invoke(this);
			}


//			UnityEngine.Debug.Log("complateAction 2");
			mActionManager.onEndAction(this);

//			UnityEngine.Debug.Log("complateAction 3");

		}
		catch(System.Exception e)
		{
			UnityEngine.Debug.LogError(e.ToString());
		}
			
	}

	public virtual GameNotificationEvents getActionEvent()
	{
		return GameNotificationEvents.eGameNotificationEventsCount;
	}
}

