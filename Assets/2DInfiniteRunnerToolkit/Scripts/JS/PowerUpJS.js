#pragma strict

class PowerUpJS extends MonoBehaviour 
{
	var parent 							: PowerUpMainJS;				//The power up manager parent object
	var trail							: GameObject;					//The trail renderer gameobject
	
	private var verticalSpeed 			: float			= 5.0f;			//Vertical speed
	private var verticalDistance 		: float			= 1.0f;			//Vertical distance
	
	private var horizontalSpeed 		: float			 = 0;			//Horizontal speed
	
	private var offset 					: float 		= 0.0f;			//Vertical offset
	private var originalPos 			: float 		= 0;			//Original y position
	
	private var nextPos					: Vector3;						//Stores the next position
	private var startingPos				: Vector3;						//The starting position of the object
	
	private var paused 					: boolean 		= false;		//Is the game paused
	private var canMove					: boolean		 = false;		//Can this object move
	
	//Called at the beginning of the game
	function Start()
	{
		//Saves the starting position
		startingPos = this.transform.position;
	}
	//Called at every frame
	function Update () 
	{	
		//If the game is not paused, and the power up can move
		if (!paused && canMove)
		{
			//Get current position
			nextPos = this.transform.position;
			
			//Calculate new vertical position
			offset = (1 + Mathf.Sin(Time.time * verticalSpeed)) * verticalDistance / 2.0f;
			nextPos.y = originalPos + offset;
			
			//Calculate new horizontal position
			nextPos.x -= horizontalSpeed * Time.deltaTime;
			
			//Apply new position
			this.transform.position = nextPos;
		}
	}
	//Enables/disables the object with childs based on platform
	private function EnableDisable(what : GameObject, activate : boolean)
    {
        #if UNITY_3_5
        	what.SetActiveRecursively(activate);
        #else
        	what.SetActive(activate);
        #endif
    }
	//Called when the power up manager activates this power up
	function Setup(vSpeed : float, vDist : float, hSpeed : float)
	{
		//Set speed related variables
		this.verticalSpeed = vSpeed;
		this.verticalDistance = vDist;
		this.horizontalSpeed = hSpeed;
		
		//Get original y position
		originalPos = this.transform.position.y;
		
		//Activate trail particle
		EnableDisable(trail, true);
		
		//Enable movement
		canMove = true;
		paused = false;
	}
	//Disable trail
    function DisableTrail()
    {
        EnableDisable(trail, false);
    }
	//Pause the power up
	function Pause()
	{
		paused = true;
	}
	//Resume the power up
	function Resume()
	{
		paused = false;
	}
	//Reset the power up
	function ResetThis()
	{
		//Disable movement and deactivate trail effect
		canMove = false;
		EnableDisable(trail, false);
		//Reset position and notify the power up manager
		this.transform.position = startingPos;
		parent.ResetPowerUp(this);
	}
}
