using UnityEngine;
using System.Collections;

public delegate void TriggerDelegate(Collider2D aObj, CTriggerScript aScript);

public class CTriggerScript : MonoBehaviour
{
	public TriggerDelegate mDelegate = null;

	void OnTriggerEnter2D(Collider2D other)
	{
		UnityEngine.Debug.Log("CTriggerScript OnTriggerEnter2D");

		if(mDelegate != null)
		{
			UnityEngine.Debug.Log("CTriggerScript OnTriggerEnter2D 1");
			mDelegate.Invoke(other, this);
		}

		UnityEngine.Debug.Log("CTriggerScript OnTriggerEnter2D end");
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

