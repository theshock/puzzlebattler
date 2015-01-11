#pragma strict

class TorpedoJS extends MonoBehaviour 
{
	var parent						: TorpedoMainJS;				//A link to the torpedo manager
	
	var indicator					: GameObject;					//The torpedo's indicator
	var torpedo						: GameObject;					//The torpedo object
	var explosion					: ParticleSystem;				//The torpedo's explosion
	
	private var canMove 			: boolean			= false;	//Torpedo movement enabled/disabled
	private var paused 				: boolean 			= false;	//Game paused switch
	
	private var originalSpeed       : float				= 0;		//The original speed, before the modification
	private var speed				: float				= 0;		//Speed of the torpedo
	private var originalPos			: Vector3;						//The orogonal position of the torpedo
	private var originalExpPos		: Vector3;						//The original position of the explosion
	
	private var explosionPlaying	: boolean			= false;	//Explosion playing/not playing
	
	//Called at the beginning of the game
	function Start()
	{
		//Records original position
		originalPos = torpedo.transform.position;
		originalExpPos = explosion.transform.position;
	}
	//Called at every frame
	function Update () 
	{
		//If the torpedo is enabled, and the game is not paused
		if (canMove && !paused)
			//Move the torpedo
			torpedo.transform.position -= Vector3.right * speed * Time.deltaTime;
	}
	//Enables/disables the object with childs based on platform
	function EnableDisable(what : GameObject, activate : boolean)
    {
        #if UNITY_3_5
        	what.SetActiveRecursively(activate);
        #else
        	what.SetActive(activate);
        #endif
    }
	//Place torpedo indicator
	private function PlaceIndicator()
	{
		//Get a random y position
		var yPos : int = Random.Range(-23, 23);
		
		//Get the right position from the resolution manager
		var indPos : float = ResolutionManagerJS.Instance().RightPosition();
		
		//If the aspect ratio is not supported, use the default value
		if (indPos == 0)
			indPos = 41;
		
		//Place the indicator
		indicator.transform.position = Vector3 (indPos, yPos, -4.9f);
		
		//Activates the indicator
		EnableDisable(indicator, true);
		
		//Wait for 3 seconds
		var waited : double = 0;
		while (waited <= 3)
		{
			//If the game is not paused
			if (!paused)
				//Increase waited time
				waited += Time.deltaTime;
			
			//Wait for the end of frame
			yield;
		}
		
		//Deactivate and reset indicator
		EnableDisable(indicator, false);
		indicator.transform.position = Vector3 (41, 0, -4.9f);
		
		//Place torpedo at yPos
		var pos : Vector3 = torpedo.transform.position;
		pos.y = yPos;
		torpedo.transform.position = pos;
		
		//Set torpedo speed
        this.speed = originalSpeed * LevelGeneratorJS.Instance().SpeedMultiplier();
		
		//Activate torpedo
		EnableDisable(torpedo, true);
		
		//Enable movement
		canMove = true;
	}
	//Place explosion at a given location
	private function PlaceExplosion(x : float, y : float)
	{
		//Place the explosion at the given position, and play it
		explosion.transform.position = Vector3(x - 6, y, originalExpPos.z);
		explosion.Play ();
		
		//Set explosionPlaying variable and add the explosion to the level generator to scroll it
		explosionPlaying = true;
		LevelGeneratorJS.Instance().AddExplosion(explosion.gameObject);
		
		//Wait for 2 seconds
		var waited : double = 0;
		while (waited <= 2)
		{
			//If the game is not paused
			if (!paused)
				//Increase waited time
				waited += Time.deltaTime;
			
			//Wait for the end of frame
			yield;
		}
		
		//Remove the explosion from the level generator, and modify explosionPlaying variable
		LevelGeneratorJS.Instance().RemoveExplosion(explosion.gameObject);
		explosionPlaying = false;
		
		//Reset explosion position
		explosion.transform.position = originalExpPos;
	}
	//Launch a torpedo, Called from level generator
	function Launch (s : float) 
	{	
		//Set speed, and place indicator
		originalSpeed = s;
		StartCoroutine(PlaceIndicator());
	}
	//Reset the torpedo
	function ResetThis()
	{
		//Stop the coroutines
		StopAllCoroutines();
		
		//If the explosion is playing, reset it
		if (explosionPlaying)
			LevelGeneratorJS.Instance().RemoveExplosion(explosion.gameObject);
		
		//Modify variables
		canMove = false;
		paused = false;
		
		//Reset torpedo
		torpedo.transform.position = originalPos;
		EnableDisable(torpedo, false);
		
		//Deactivate and reset indicator
		EnableDisable(indicator, false);
		indicator.transform.position = Vector3 (41, 0, -4.9f);
		
		//Notify torpedo manager
		parent.ResetTorpedo(this);
	}
	//Called when the torpedo collides with the player, or with the sonic wave
	function TargetHit(playExplosion : boolean)
	{
		//Modify variables
		canMove = false;
		paused = false;
		
		//Play explosion, if set
		if (playExplosion)
			StartCoroutine (PlaceExplosion(torpedo.transform.position.x, torpedo.transform.position.y));
		
		//Reset torpedo
		torpedo.transform.position = originalPos;
		EnableDisable(torpedo, false);
		
		//Notify torpedo manager
		parent.ResetTorpedo(this);
	}
	//Pause the torpedo
	function Pause()
	{
		paused = true;
	}
	//Resume the torpedo
	function Resume()
	{
		paused = false;
	}
}
