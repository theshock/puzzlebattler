#pragma strict

public class TorpedoIndicatorJS extends MonoBehaviour 
{
	var speed 					: float 	= 5.0f;			//Vertical speed
	var distance 				: float		= 1.0f;			//Vertical distance
	
	private var offset 			: float		= 0.0f;			//Offset
	
	private var originalPos 	: float		= 0;			//The original position of the indicator
	private var nextPos			: Vector3;					//The next position of the indicator
	
	private var paused 			: boolean 	= false;		//Is the game paused
	
	//Called when the object is enabled
	function OnEnable()
	{
		//Set original position, and set pause to false
		originalPos = this.transform.position.y;
		paused = false;
	}
	//Called at every frame
	function Update () 
	{	
		//If the game is not paused
		if (!paused)
		{
			//Calculate offset
			offset = (1 + Mathf.Sin(Time.time * speed)) * distance / 2.0f;
			
			//Modify next position
			nextPos = this.transform.position;
			nextPos.y = originalPos + offset;
			
			//Apply next position
			this.transform.position = nextPos;
		}
	}
	//Pause the indicator
	function Pause()
	{
		paused = true;
	}
	//Resume the indicator
	function Resume()
	{
		paused = false;
	}
	
	
}
