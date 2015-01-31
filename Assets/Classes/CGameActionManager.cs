using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Match;
using Match.Actions;

public class CGameActionManager
{
	private ArrayList mActiveActions = new ArrayList();

	public CController mMatchController { get; set; }

	public CGameActionManager()
	{
		CObjectFactory.registerCreator("match_action_" + (int)EMatchAction.eDestroyAction, CDestroy.create);
		CObjectFactory.registerCreator("match_action_" + (int)EMatchAction.eSwipeAction, CSwipe.create);
		CObjectFactory.registerCreator("match_action_" + (int)EMatchAction.eMatchAction, CMatch.create);
		CObjectFactory.registerCreator("match_action_" + (int)EMatchAction.eAutoMatchAction, CAutoMatch.create);
		CObjectFactory.registerCreator("match_action_" + (int)EMatchAction.eRefreshPositionAction, CRefreshPosition.create);
	}

	public IAction createAction(EMatchAction aAction)
	{
//		UnityEngine.Debug.Log("createAction");

		string name_action = "match_action_" + (int) aAction;

//		UnityEngine.Debug.Log(name_action);

		IAction action = CObjectFactory.createObjectByKey(name_action) as IAction;

		action.initWithActionManager(this, mMatchController.mMatchView.mMatchField);

		return action;
	}

	public int addAction(IAction aAction)
	{
		if(!aAction.validation())
			return 0;

		aAction.startAction();
		mActiveActions.Add(aAction);

		return 1;
	}

	public void updateting()
	{

	}

	public void removeAction(IAction aAction)
	{
		try
		{
			mActiveActions.Remove(aAction);
		} 
		catch(System.Exception e)
		{
			UnityEngine.Debug.LogError(e.ToString());
		}

	}

	public void onEndAction(IAction aAction)
	{
//		UnityEngine.Debug.Log("aAction.getActionEventCount = " + aAction.getActionEvent());
		removeAction(aAction);

		if(!isHaveAction())
		{
			mMatchController.mNotificationManager.notify((int)GameNotificationEvents.eMatchEventEndAllAction, null);
		}


	}

	public bool isHaveAction()
	{
		return (mActiveActions.Count > 0);
	}
}
