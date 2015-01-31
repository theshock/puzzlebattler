using UnityEngine;
using System.Collections;

public class CController : MonoBehaviour , INotificationObserver
{
	public CNotificationManager mNotificationManager = new CNotificationManager();
	public CSearcher mMatchSearcher { get; set; }
	public CView mMatchView;
	public CGameActionManager mActionManager { get; set; }

	public void handleNotification(int aEvent, Object aParam, CNotificationManager aManager)
	{
		switch((GameNotificationEvents)aEvent)
		{
			case GameNotificationEvents.eMatchEventEndAllAction:
			{
//			Debug.Log("CMatchController handleNotification eMatchEventEndAllAction");

				IMatchAction action = mActionManager.createAction(EMatchAction.eRefreshPositionAction);
				mActionManager.addAction(action);
			}
				break;
		}

		return;
	}
	// Use this for initialization
	void Start ()
	{
		mActionManager = new CGameActionManager();
		mActionManager.mMatchController = this;

		mNotificationManager.addObserver((int)GameNotificationEvents.eMatchEventEndAllAction,this);

		mMatchSearcher = new CSearcher();
		mMatchSearcher.mMatchController = this;
		mMatchSearcher.subscribeNotification(mNotificationManager);

		mMatchView.init();

		IMatchAction action = mActionManager.createAction(EMatchAction.eAutoMatchAction);
		mActionManager.addAction(action);


	}
	
	// Update is called once per frame
	void Update ()
	{
		CHandlerUpdateManager.shared().doUpdate();
	}
}
