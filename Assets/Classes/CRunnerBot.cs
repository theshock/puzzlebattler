using UnityEngine;
using System.Collections;

public class CRunnerBot : MonoBehaviour
{
	public Vector3 mCurrentSpead = new Vector3(0.01f, 0, 0);
	public Vector3 mMaxSpead = new Vector3(0.3f, 0, 0);
	public Vector3 mAccelerationSpead = new Vector3(0.01f, 0, 0);
	public float mTimeDelay = 1.0f;
	public float mCurrentTimeDelay = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		mCurrentTimeDelay -= Time.deltaTime;
		
		if(mCurrentTimeDelay <= 0.0f)
		{
			mCurrentTimeDelay = mTimeDelay;

			if(mCurrentSpead.x < mMaxSpead.x)
			{
				mCurrentSpead += mAccelerationSpead;
			}

		}


		transform.position += mCurrentSpead;
	}
}
