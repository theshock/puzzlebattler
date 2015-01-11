using UnityEngine;
using System.Collections;

public class CRunnerController : MonoBehaviour , INotificationObserver
{
	public CNotificationManager mNotificationManager { get; set; }
	public CGameController mGameController;
	public CRunnerRoad mRunnerRoad = null;

	public void handleNotification(int aEvent, Object aParam, CNotificationManager aManager)
	{
		return;
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector2 speed = mGameController.mGameData.getCurrentSpeed() * 1.0f;
		mRunnerRoad.doScrollRoad(speed);
	}
}
