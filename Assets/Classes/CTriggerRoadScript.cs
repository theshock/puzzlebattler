using UnityEngine;
using System.Collections;

public class CTriggerRoadScript : MonoBehaviour
{
		public CRunnerRoad mRoad;

		void OnTriggerEnter2D(Collider2D other)
		{
			if(other.gameObject.GetComponent<CRunnerPlayer>() != null)
			{
				UnityEngine.Debug.Log("OnTriggerEnter2D");
				mRoad.onTriggerAction(this);
			}

		}
		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
}

