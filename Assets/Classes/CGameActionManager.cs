using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


public class CGameActionManager
{
	private ArrayList mActiveActions = new ArrayList();

	public CMatchController mMatchController { get; set; }

	public CGameActionManager()
	{
		CObjectFactory.registerCreator("match_action_" + (int)EMatchAction.eDestroyAction, CMatchActionDestroy.create);
		CObjectFactory.registerCreator("match_action_" + (int)EMatchAction.eSwipeAction, CMatchActionSwipe.create);
		CObjectFactory.registerCreator("match_action_" + (int)EMatchAction.eMatchAction, CMatchActionMatch.create);
		CObjectFactory.registerCreator("match_action_" + (int)EMatchAction.eAutoMatchAction, CMatchAutoMatchAction.create);
		CObjectFactory.registerCreator("match_action_" + (int)EMatchAction.eRefreshPositionAction, CMatchRefreshPositionAction.create);
	}

	public IMatchAction createAction(EMatchAction aAction)
	{
//		UnityEngine.Debug.Log("createAction");

		string name_action = "match_action_" + (int) aAction;

//		UnityEngine.Debug.Log(name_action);

		IMatchAction action = CObjectFactory.createObjectByKey(name_action) as IMatchAction;

		action.initWithActionManager(this, mMatchController.mMatchView.mMatchField);

		return action;
	}

	public int addAction(IMatchAction aAction)
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

	public void removeAction(IMatchAction aAction)
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

	public void onEndAction(IMatchAction aAction)
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
