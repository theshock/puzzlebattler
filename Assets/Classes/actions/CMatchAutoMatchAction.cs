using UnityEngine;
using System.Collections;

public class CMatchAutoMatchAction : CMatchBaseAction
{
	private ArrayList mAutoMatches = null;
	public int mCountStartMatch = 0;

	public static IMatchAction create()
	{
//		Debug.Log("CMatchAutoMatchAction create");

		return new CMatchAutoMatchAction();
	}

	public void onEndAction(IMatchAction aAction)
	{
//		Debug.Log("CMatchAutoMatchAction onEndAction");
//		
//		mCountStartMatch--;
//		
//		if(mCountStartMatch == 0)
//		{
//			Debug.Log("CMatchAutoMatchAction onEndAction last");
//			
//			complateAction();
//		}
	}

	public override bool validation()
	{
//		Debug.Log("CMatchAutoMatchAction validation");

		mAutoMatches = mActionManager.mMatchController.mMatchSearcher.findMatches(true);

		if(mAutoMatches.Count > 0)
		{
			return true;
		}
		
		return false;
	}
	
	public override void startAction()
	{
		mCountStartMatch = 0;

		foreach(ArrayList match in mAutoMatches)
		{
			bool is_hor = true;
			
			CMatchActionMatch action = mActionManager.createAction(EMatchAction.eMatchAction) as CMatchActionMatch;
			
			Hashtable hash = new Hashtable();
			hash["matchIcons"] = match;
			hash["directionSwap"] = is_hor;

			action.doUpdateActionParam(hash);
			action.setDelegate(onEndAction);

			mCountStartMatch += mActionManager.addAction(action);
		}

		complateAction();
	}
	

	public void doUpdate()
	{

	}
	
	
	public override GameNotificationEvents getActionEvent()
	{
		return  GameNotificationEvents.eMatchActionAutoMatchEvent;
	}
	
	public override void doUpdateActionParam(Hashtable aData)
	{
		mAutoMatches = aData["matches"] as ArrayList;
		
		return;
	}

}

