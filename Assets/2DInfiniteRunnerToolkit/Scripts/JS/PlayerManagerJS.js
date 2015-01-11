#pragma strict

class PlayerManagerJS extends MonoBehaviour 
{
	var subTextures				: Texture2D[];								//The array containing the sub and sub damaged textures
	var subMaterial				: Renderer;									//A link to the sub material
	
	var shield					: Transform;								//The sub's shield
	var shieldCollider			: SphereCollider;							//The shield's collider
	var speedParticle			: GameObject;								//The speed effect
	var speedTrail				: GameObject;								//The speed trail effect
	var sonicWave				: GameObject;								//The sonic wave particle
	
	var minDepth				: float  				= 26f;				//Minimum depth
	var maxDepth				: float 				= -26f;				//Maximum depth
	var maxRotation 			: float  				= 25f;				//The maximum rotation of the submarine
	
	var maxVerticalSpeed		: float		 			= 45.0f;			//The maximum vertical speed
	var depthEdge 				: float 				= 10.0f;			//The edge fo the smoothing zone (minDepth- depthEdge and maxDepth - depthEdge)
	
	var smoke 					: ParticleSystem;							//The smoke particle
	var bubbles					: ParticleSystem;							//The submarine bubble particle system
	var reviveParticle			: ParticleSystem;							//The revive particle
	
	static var myInstance		: PlayerManagerJS;
    static var instances 		: int 					= 0;
	
	private var speed 					: float			= 0.0f;				//The actual vertical speed of the submarine
	private var newSpeed 				: float			= 0.0f;				//The new speed of the submarine, used at the edges
	
	private var rotationDiv				: float;							//A variable used to calculate rotation
	private var newRotation				: Vector3		= Vector3(0,0,0);	//Stores the new rotation angles
	
	private var distanceToMax			: float;							//The current distance to the maximum depth
	private var distanceToMin			: float;							//The current distance to the minimum depth
	
	private var xPos					: float			= -30;				//The x position of the submarine
	private var startingPos				: float			= -37;				//The starting position of the submarine
	
	private var movingUpward 			: boolean		= false;			//The submarine is rising
	private var subEnabled				: boolean		= false;			//The submarine control enabled/disabled
	private var canSink					: boolean		= true;				//Can the player sink?
	private var sinking					: boolean		= false;			//The submarine is sinking
	private var crashed					: boolean		= false;			//The submarine crashed
	private var firstObstacleGenerated	: boolean		= false;			//The first obstacle generated
	private var hasRevive				: boolean		= false;			//Can the player revive?
	private var inRevive				: boolean		= false;			//The player is currently reviving
	private var shieldActive			: boolean		= false;			//Shield enabled/disabled
	private var inExtraSpeed			: boolean		= false;			//The submarine is using the extra speed power up
	private var paused					: boolean		= false;			//The game is paused/unpaused
	private var shopReviveUsed			: boolean		= false;			//The shop revive is used/unused
	
	private var powerUpUsed				: boolean		= false;			//A power up is used/unused	
	private var thisTransform			: Transform;						//The transform of this object stored
	
	//Retursn the instance
    public static function Instance()
    {
		if (myInstance == null)
			myInstance = FindObjectOfType(typeof(PlayerManagerJS)) as PlayerManagerJS;

		return myInstance;
    }
	
	//Called at the beginning the game
	function Start()
	{
		//Calibrates the myInstance static variable
        instances++;

        if (instances > 1)
            Debug.Log("Warning: There are more than one Player Manager at the level");
        else
            myInstance = this;
		
		//Set transform and rotationDiv
		thisTransform = this.GetComponent(Transform);
		rotationDiv = maxVerticalSpeed / maxRotation;
	}
	//Called at every frame
	function Update()
	{
		//If the control are enabled
		if (subEnabled)
		{
			//Calculate smooth zone distance
			CalculateDistances();
						
			//Calculate player movement
			CalculateMovement();
			
			//Move and rotate the submarine
			MoveAndRotate();
		}
		//If the sub is sinking
		else if (sinking)
			Sink ();
		//If the controls are disabled and not sinking, set the speed to 0
		else
			speed = 0;
	}
	//Called when the submarine trigger with something
	function OnTriggerEnter (other : Collider)
	{
		//If the sub triggered with an obstacle
		if (other.tag == "Obstacles")
		{
			//If it is a coin
			if (other.transform.name == "Coin")
			{
				//Disable the coin's renderer and collider
				other.renderer.enabled = false;
				other.collider.enabled = false;
				
				//Play it's particle system, and increase coin ammount
				other.transform.FindChild("CoinParticle").gameObject.GetComponent(ParticleSystem).Play();				
				LevelManagerJS.Instance().CoinGathered();
				
			}
			//If the sub triggered with a hazard
			else if (other.transform.name == "Mine" || other.transform.name == "Chain" || other.transform.name == "MineChain" || other.transform.name == "Torpedo" || other.transform.name == "Laser" || other.transform.name == "LaserBeam")
			{
				//Notify the mission manager
				UpdateMission(other.transform.name);
				//If the sub is not sinking, and doesn't have a protection
				if (!sinking && canSink && !shieldActive)
				{
					//Sink it, disable controls, and wreck it
					sinking = true;
					DisableControls();
					WreckSub();
				}
				//If the shield is active, and not in extra speed
				else if (shieldActive && !inExtraSpeed)
				{
					//Disable the shield
					StartCoroutine(DisableShield());
				}
				//Play explosion particle
				PlayExplosion (other.transform);
			}
			//If the sub triggered with the obstacle generation triggerer, and it is not triggered before
			else if (other.name == "ObstacleGenTriggerer" && !firstObstacleGenerated)
			{
				//Trigger it, and start obstacle generation
				firstObstacleGenerated = true;
				LevelGeneratorJS.Instance().GenerateObstacles();
			}
		}
		//If the sub triggered with a power up
		else if (other.tag == "PowerUps")
		{
			//Notify mission manager
			UpdateMission(other.transform.name);
			
			//Activate proper function based on name
			switch (other.transform.name)
			{
				case "ExtraSpeed":
					ExtraSpeed();
					break;
				
				case "Shield":
					RaiseShield();
					break;
					
				case "Revive":
					if (subEnabled)
						ReviveCollected();
					break;
					
				case "SonicWave":
					if (subEnabled)
						StartCoroutine("LaunchSonicWave");
					break;
			}
			
			//Reset the power up
			other.GetComponent(PowerUpJS).ResetThis();
		}
	}
	//Calculate distances to minDepth and maxDepth
	function CalculateDistances()
	{
		distanceToMax = thisTransform.position.y - maxDepth;
		distanceToMin = minDepth - thisTransform.position.y;
	}
	//Calculate movement based on input
	private function CalculateMovement()
	{
		//If the sub is moving up
		if (movingUpward)
		{
			//Increase speed
			speed += Time.deltaTime * maxVerticalSpeed;
			
			//If the sub is too close to the minDepth
			if (distanceToMin < depthEdge)
			{
				//Calculate maximum speed at this depth (without this, the sub would leave the gameplay are)
				newSpeed = maxVerticalSpeed * (minDepth - thisTransform.position.y) / depthEdge;
				
				//If the newSpeed is lesser the the current speed
				if (newSpeed < speed)
					//Make newSpeed the current speed
					speed = newSpeed;
			}
			//If the sub is too close to the maxDepth
			else if (distanceToMax < depthEdge)
			{
				//Calculate maximum speed at this depth (without this, the sub would leave the gameplay are)
				newSpeed = maxVerticalSpeed * (maxDepth - thisTransform.position.y) / depthEdge;
				
				//If the newSpeed is greater the the current speed
				if (newSpeed > speed)
					//Make newSpeed the current speed
					speed = newSpeed;
			}
		}
		//If the sub is moving down
		else
		{
			//Decrease speed
			speed -= Time.deltaTime * maxVerticalSpeed;
			
			//If the sub is too close to the maxDepth
			if (distanceToMax < depthEdge)
			{
				//Calculate maximum speed at this depth (without this, the sub would leave the gameplay are)
				newSpeed = maxVerticalSpeed * (maxDepth - thisTransform.position.y) / depthEdge;
				
				//If the newSpeed is greater the the current speed
				if (newSpeed > speed)
					//Make newSpeed the current speed
					speed = newSpeed;
			}
			//If the sub is too close to the minDepth
			else if (distanceToMin < depthEdge)
			{
				//Calculate maximum speed at this depth (without this, the sub would leave the gameplay are)
				newSpeed = maxVerticalSpeed * (minDepth - thisTransform.position.y) / depthEdge;
				
				//If the newSpeed is lesser the the current speed
				if (newSpeed < speed)
					//Make newSpeed the current speed
					speed = newSpeed;
			}
		}
	}
	//Move and rotate the submarine based on speed
	private function MoveAndRotate()
	{
		//Calculate new rotation
		newRotation.z = speed / rotationDiv;
		
		//Apply new rotation and position
		thisTransform.eulerAngles = newRotation;
		thisTransform.position += Vector3.up * speed * Time.deltaTime;
	}
	//Update mission manager based on name
	private function UpdateMission(name : String)
	{
		//Obstacle based update
		if (name == "Mine" || name == "MineChain" || name == "Chain")
			MissionManagerJS.Instance().ObstacleEvent("Mine");
		else if (name == "Torpedo")
			MissionManagerJS.Instance().ObstacleEvent("Torpedo");
		else if (name == "Laser" || name == "LaserBeam")
			MissionManagerJS.Instance().ObstacleEvent("Laser");
		//Power up based update
		else if (name == "ExtraSpeed")
			MissionManagerJS.Instance().PowerUpEvent("Extra Speed");
		else if (name == "Shield")
			MissionManagerJS.Instance().PowerUpEvent("Shield");
		else if (name == "SonicWave")
			MissionManagerJS.Instance().PowerUpEvent("Sonic Wave");
		else if (name == "Revive")
			MissionManagerJS.Instance().PowerUpEvent("Revive");
	}
	//Play an explosion			
	private function PlayExplosion(expParent : Transform)
	{
		//If the sub collided with a torpedo
		if (expParent.name == "Torpedo")
		{
			//Notify torpedo manager
			expParent.transform.parent.gameObject.GetComponent(TorpedoJS).TargetHit(true);
		}
		//If the sub collided with something else
		else
		{
			//Find the particle child, and play it
			var explosion : ParticleSystem = expParent.FindChild("ExplosionParticle").gameObject.GetComponent(ParticleSystem);
			explosion.Play();
			//Disable the object's renderer and collider
			expParent.renderer.enabled = false;
			expParent.collider.enabled = false;
		}
	}
	//Wreck the sub
	private function WreckSub()
	{
		//Set crashed = true, and change texture
		crashed = true;
		subMaterial.material.mainTexture = subTextures[1];
		//Disable bubble particle
		bubbles.Stop();
		bubbles.Clear();
	}
	//Sink the submarine
	private function Sink()
	{
		//Set crash related variables
		crashed = true;
		
		var crashDepth : float = maxDepth - 5;
		var crashDepthEdge : float = 4;
		
		var distance : float = thisTransform.position.y - crashDepth;
		
		//If the sub is too close to minDepth
		if (distanceToMin < depthEdge)
		{
			//Calculate maximum speed at this depth (without this, the sub would leave the gameplay are)
			newSpeed = maxVerticalSpeed * (minDepth - thisTransform.position.y) / depthEdge;
			
			//If the newSpeed is greater the the current speed
			if (newSpeed < speed)
				//Make newSpeed the current speed
				speed = newSpeed;
		}
		//If the distance to the sand is greater than 0.1
		if (distance > 0.1f)
		{
			//Reduce speed
			speed -= Time.deltaTime * maxVerticalSpeed * 0.6f;
			
			//If the distance to the sand smaller than the crashDepthEdge
			if (distance < crashDepthEdge)
			{
				//Calculate new speed for impact
				newSpeed = maxVerticalSpeed * (crashDepth - thisTransform.position.y) / crashDepthEdge;
				
				//If newSpeed is greater than speed
				if (newSpeed > speed)
					//Apply new speed to speed
					speed = newSpeed;
			}
			
			//Apply the above to the submarine
			MoveAndRotate();
			
			//If distance to sand smaller than 2.5
			if (distance < 2.5f)
				//Enable smoke emission
				smoke.enableEmission = true;
		}
		//If the distance to the sand is smaller than 0.1
		else
		{
			//Disable this function from calling
			sinking = false;
			//Show sink effects
			StartCoroutine (SinkEffects());
		}
	}
	//Called when a revive is collected
	private function ReviveCollected()
	{
		//Register revive, Show hearth on the GUI, and disable additional revive generation
		hasRevive = true;
		GUIManagerJS.Instance().RevivePicked();
		LevelGeneratorJS.Instance().powerUpMain.DisableReviveGeneration();
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
	//Scale object to scale under time
	private function ScaleObject(obj : Transform, scale : Vector3, time : float, deactivate : boolean)
	{
		//Set the object active
		EnableDisable(obj.gameObject, true);
		//Get it's current scale
		var startScale : Vector3 = obj.localScale;
		
		//Scale the object
		var rate = 1.0f / time;
	    var t = 0.0f;
		
	    while (t < 1.0f) 
	    {
			//If the game is not paused, increase t, and scale the object
			if (!paused)
			{
		        t += Time.deltaTime * rate;
		        obj.localScale = Vector3.Lerp(startScale, scale, t);
			}
			
			yield;
	    }
		
		//If the object is makred, disable it
		if (deactivate)
			EnableDisable(obj.gameObject, false);
		
		//If the object is the shield, enable it's shield
		if (obj.name == "Shield")
			shieldCollider.enabled = true;
	}
	//Disable shield
	private function DisableShield()
	{
		//Scale the shield to 0.01, and disable it's collider
		StartCoroutine(ScaleObject(shield.transform, Vector3(0.01f, 1, 0.01f), 0.35f, true));
		shieldCollider.enabled = false;
		
		//Wait for 0.4 seconds
		var waited : double = 0;
		while (waited <= 0.4f)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield;
		}
		
		//Mark shield as inactive
		shieldActive = false;
	}
	//Activate extra speed effects for time
	private function ExtraSpeedEffect(time : float)
	{
		//Get the current scroll speed, then set scroll speed to 3
		var newSpeed : float = LevelGeneratorJS.Instance().scrollSpeed;
		LevelGeneratorJS.Instance().scrollSpeed = 3;
		
		//Wait for time
		var waited : double = 0;
		while (waited <= time)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield;
		}
		
		//Set variabler for extra speed
		LevelGeneratorJS.Instance().scrollSpeed = newSpeed;
		inExtraSpeed = false;
		canSink = true;
		
		//Activate extra speed visual effects
		EnableDisable(speedParticle, false);
		EnableDisable(speedTrail, false);
		
		//Allow the level generator to increase scrolling speed
		LevelGeneratorJS.Instance().ExtraSpeedOver();
	}
	//The sink effects
	private function SinkEffects()
	{
		//Wait for 0.5 seconds, and stop the level scrolling in 2.5 seconds
		yield WaitForSeconds(0.5f);
		LevelGeneratorJS.Instance().StartCoroutine("StopScrolling", 2.5f);
		
		//Wait for 2.75 seconds, and disable smoke emission
		yield WaitForSeconds(2.75f);
		smoke.enableEmission = false;
	}
	//Move obj to endPos in time
	private function MoveToPosition (obj : Transform, endPos : Vector3, time : float, enableSub : boolean) 
	{
		//Enable the sub bubble particle with increased emission, if needed
		if (enableSub)
		{
			bubbles.enableEmission = true;
			bubbles.Play();
		}
		
		//Declare variables, get the starting position, and move the object
		var i : float = 0.0f;
		var rate : float = 1.0f / time;
		
		var startPos : Vector3 = obj.position;
			
		while (i < 1.0) 
		{
			//If the game is not paused, increase t, and scale the object
			if (!paused)
			{
				i += Time.deltaTime * rate;
				obj.position = Vector3.Lerp(startPos, endPos, i);
			}
			
			//Wait for the end of frame
			yield;
		}
		
		//The the sub is set to enable, activate it, and decrease emission rate
		if (enableSub)
		{
			subEnabled = true;
		}
	}
	//Called when the player collects/activates an extra speed
	function ExtraSpeed()
	{
		//If the player is already using an extra speed, or sinking, or the controls are not enabled, return
		if (inExtraSpeed || sinking || !subEnabled)
			return;
		
		//Set power up based variables
		powerUpUsed = true;
		inExtraSpeed = true;
		canSink = false;
		
		//Activate particles
		EnableDisable(speedParticle, true);
		EnableDisable(speedTrail, true);
		
		//Set level generator for extra speed, and activate it
		LevelGeneratorJS.Instance().ExtraSpeedEffect();
		StartCoroutine (ExtraSpeedEffect(3));
	}
	//Called when the player collects/activate a shield power up
	function RaiseShield()
	{
		//If a shield is already activates,  or sinking, or the controls are not enabled, return
		if (shieldActive || sinking || !subEnabled)
			return;
		
		//Set power up based variables, and raise the shield
		powerUpUsed = true;
		shieldActive = true;
		StartCoroutine(ScaleObject(shield.transform, Vector3(18, 16.2f, 1), 0.35f, false));
	}
	//Called from the Input manager
	function MoveUp()
	{
		//If the player is not at the min depth, and the controls are enabled, move up
		if (distanceToMin > 0 && subEnabled)	
			movingUpward = true;
	}
	//Called from the Input manager
	function MoveDown()
	{
		//If the player is not at the max depth, and the controls are enabled, move down
		if (distanceToMax > 0 && subEnabled)	
			movingUpward = false;
	}
	//Enalbe submarine controls
	function EnableControls()
	{
		subEnabled = true;
	}
	//Reset submarine status
	function ResetStatus(moveToStart : boolean)
	{
		//Stop the coroutines
		StopAllCoroutines();
		
		//Disable the bubble particles
		bubbles.Clear();
		bubbles.Stop();
		
		//Reset variables
		speed = 0;
		crashed = false;
		paused = false;
		movingUpward = false;
		canSink = true;
		
		inRevive = false;
		hasRevive = false;
		shopReviveUsed = false;
		inExtraSpeed = false;
		shieldActive = false;
		powerUpUsed = false;
		
		//Reset power up particles/effects
		shield.transform.localScale = Vector3(0.01f, 1, 0.01f);
		EnableDisable(speedParticle, false);
		EnableDisable(speedTrail, false);
		firstObstacleGenerated = false;
		
		//Reset rotation and position
		newRotation	= Vector3(0, 0, 0);
		
		this.transform.position = Vector3(startingPos, -25.5f, thisTransform.position.z);
		this.transform.eulerAngles = newRotation;
		//Reset texture
		subMaterial.material.mainTexture = subTextures[0];
		
		//If moveToStart, move the submarine from the resting position to the starting position
		if (moveToStart)
			StartCoroutine(MoveToPosition(this.transform, Vector3(xPos, -23, thisTransform.position.z), 1.0f, true));
	}
	//Disable submarine controls
	function DisableControls()
	{
		subEnabled = false;
	}
	//Pause the submarine/particles
	function Pause()
	{
		paused = true;
		bubbles.Pause();
	}
	//Resume the submarine/particles
	function Resume()
	{
		paused = false;
		bubbles.Play();
	}
	//Returns crashed state
	function Crashed()
	{
		return crashed;
	}
	//Return revive state
	function HasRevive()
	{
		if (hasRevive || (SaveManagerJS.GetRevive() > 0 && !shopReviveUsed))
			return true;
		else
			return false;
	}
	//Return power up used state
	function PowerUpUsed()
	{
		return powerUpUsed;
	}
	//Set starting and main position
	function SetPositions(starting : float, main : float)
	{
		startingPos = starting;
		xPos = main;
	}
	//Called when a sonic wave power up is picked up, launches the sonic wave particle
	function LaunchSonicWave()
	{
		//Activate sonic wave, and set powerUpUsed to true
		EnableDisable(sonicWave, true);
		powerUpUsed = true;
		
		//Move the wave across the screen
		StartCoroutine(MoveToPosition(sonicWave.transform, Vector3(65, 0, -5), 1.25f, false));
		
		//Wait for 2 seconds
		var waited : double = 0;
		while (waited <= 2)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield;
		}
		//Call the sonic wave's disable function
		sonicWave.GetComponent(SonicWaveJS).Disable();
	}
	//Use revive
	function Revive()
	{
		//If the submarine is not reviving
		if (!inRevive)
		{
			//Set revive based variables
			inRevive = true;
			powerUpUsed = true;
			
			//If the player has collected a revive
			if (hasRevive)
			{
				//Set variable, and disable it's hearth
				hasRevive = false;
				GUIManagerJS.Instance().DisableReviveGUI(0);
			}
			//If the player has not collected a revive, but has a revive from the store
			else
			{
				//Set variable, remove revive from account, and disable it's hearth
				shopReviveUsed = true;
				SaveManagerJS.ModifyReviveBy(-1);
				GUIManagerJS.Instance().DisableReviveGUI(1);
			}
			
			//Reset speed, play revive particle, and change texture to intact
			speed = 0;
			reviveParticle.Play();
			subMaterial.material.mainTexture = subTextures[0];
			
			//Reset rotation, and launch a sonic wave to clear the scene from obstacles
			newRotation	= Vector3(0, 0, 0);
			this.transform.eulerAngles = newRotation;
			StartCoroutine("LaunchSonicWave");
			
			//Wait for 0.4 seconds, and move to starting position
			yield WaitForSeconds(0.4f);
			StartCoroutine(MoveToPosition(this.transform, Vector3(xPos, -23, thisTransform.position.z), 1.0f, false));
			
			//Wait for 1.2 seconds, and restart level scrolling
			yield WaitForSeconds(1.2f);
			LevelGeneratorJS.Instance().ContinueScrolling();
			
			//Restart the bubble particle, and set variables
			bubbles.Play();
			crashed = false;
			canSink = true;
			subEnabled = true;
			movingUpward = false;
			inRevive = false;	
		}
		
		//If the player is already in revive, wait for the end of frame, and return to caller
		yield;
	}
}