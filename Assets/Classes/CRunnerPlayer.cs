using UnityEngine;
using System.Collections;

public class CRunnerPlayer : MonoBehaviour
{
	public void doUpdatePosition(Vector2 aOffset)
	{
		transform.position += new Vector3(aOffset.x, aOffset.y , 0);
	}

	public void doJump()
	{
		Debug.Log("doJump");

		rigidbody2D.AddForce(new Vector2(400.0f,650.0f));
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
