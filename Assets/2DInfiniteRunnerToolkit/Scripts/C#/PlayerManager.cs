using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour 
{
	public Texture2D[] subTextures;										//The array containing the sub and sub damaged textures
	public Renderer subMaterial;										//A link to the sub material
	
	public Transform shield;											//The sub's shield
	public SphereCollider shieldCollider;								//The shield's collider
	public GameObject speedParticle;									//The speed effect
	public GameObject speedTrail;										//The speed trail effect
	public GameObject sonicWave;										//The sonic wave particle
	
	public float minDepth 					= 26f;						//Minimum depth
	public float maxDepth 					= -26f;						//Maximum depth
	public float maxRotation 				= 25f;						//The maximum rotation of the submarine
	
	public float maxVerticalSpeed 			= 45.0f;					//The maximum vertical speed
	public float depthEdge					= 10.0f;					//The edge fo the smoothing zone (minDepth- depthEdge and maxDepth - depthEdge)
	
	public ParticleSystem smoke;										//The smoke particle
	public ParticleSystem bubbles;										//The submarine bubble particle system
	public ParticleSystem reviveParticle;								//The revive particle

    static PlayerManager myInstance;
    static int instances = 0;

	float speed 							= 0.0f;						//The actual vertical speed of the submarine
	float newSpeed 							= 0.0f;						//The new speed of the submarine, used at the edges
	
	float rotationDiv;													//A variable used to calculate rotation
	Vector3 newRotation						= new Vector3 (0, 0, 0);	//Stores the new rotation angles
	
	float distanceToMax;												//The current distance to the maximum depth
	float distanceToMin;												//The current distance to the minimum depth
	
	float xPos								= -30;						//The x position of the submarine
	float startingPos						= -37;						//The starting position of the submarine
	
	bool movingUpward 						= false;					//The submarine is rising
	bool subEnabled							= false;					//The submarine control enabled/disabled
	bool canSink							= true;						//Can the player sink?
	bool sinking							= false;					//The submarine is sinking
	bool crashed							= false;					//The submarine crashed
	bool firstObstacleGenerated				= false;					//The first obstacle generated
	bool hasRevive							= false;					//Can the player revive?
	bool inRevive							= false;					//The player is currently reviving
	bool shieldActive						= false;					//Shield enabled/disabled
	bool inExtraSpeed						= false;					//The submarine is using the extra speed power up
	bool paused								= false;					//The game is paused/unpaused
	bool shopReviveUsed						= false;					//The shop revive is used/unused
	
	bool powerUpUsed						= false;					//A power up is used/unused
	
	Transform thisTransform;											//The transform of this object stored

    //Retursn the instance
    public static PlayerManager Instance
    {
        get
        {
            if (myInstance == null)
                myInstance = FindObjectOfType(typeof(PlayerManager)) as PlayerManager;

            return myInstance;
        }
    }

	//Called at the beginning the game
	void Start()
	{
        //Calibrates the myInstance static variable
        instances++;

        if (instances > 1)
            Debug.Log("Warning: There are more than one Player Manager at the level");
        else
            myInstance = this;

		//Set transform and rotationDiv
		thisTransform = this.GetComponent<Transform>();
		rotationDiv = maxVerticalSpeed / maxRotation;
	}
	//Called at every frame
	void Update()
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
	void OnTriggerEnter (Collider other)
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
				other.transform.FindChild("CoinParticle").gameObject.GetComponent<ParticleSystem>().Play();
                LevelManager.Instance.CoinGathered();
				
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
                LevelGenerator.Instance.GenerateObstacles();
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
			other.GetComponent<PowerUp>().ResetThis();
		}
	}
	//Calculate distances to minDepth and maxDepth
	void CalculateDistances()
	{
		distanceToMax = thisTransform.position.y - maxDepth;
		distanceToMin = minDepth - thisTransform.position.y;
	}
	//Calculate movement based on input
	void CalculateMovement()
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
	void MoveAndRotate()
	{
		//Calculate new rotation
		newRotation.z = speed / rotationDiv;
		
		//Apply new rotation and position
		thisTransform.eulerAngles = newRotation;
		thisTransform.position += Vector3.up * speed * Time.deltaTime;
	}
	//Update mission manager based on name
	void UpdateMission(string name)
	{
		//Obstacle based update
		if (name == "Mine" || name == "MineChain" || name == "Chain")
			MissionManager.Instance.ObstacleEvent("Mine");
		else if (name == "Torpedo")
            MissionManager.Instance.ObstacleEvent("Torpedo");
		else if (name == "Laser" || name == "LaserBeam")
            MissionManager.Instance.ObstacleEvent("Laser");
		//Power up based update
		else if (name == "ExtraSpeed")
            MissionManager.Instance.PowerUpEvent("Extra Speed");
		else if (name == "Shield")
            MissionManager.Instance.PowerUpEvent("Shield");
		else if (name == "SonicWave")
            MissionManager.Instance.PowerUpEvent("Sonic Wave");
		else if (name == "Revive")
            MissionManager.Instance.PowerUpEvent("Revive");
	}
	//Play an explosion			
	void PlayExplosion(Transform expParent)
	{
		//If the sub collided with a torpedo
		if (expParent.name == "Torpedo")
		{
			//Notify torpedo manager
			expParent.transform.parent.gameObject.GetComponent<Torpedo>().TargetHit(true);
		}
		//If the sub collided with something else
		else
		{
			//Find the particle child, and play it
			ParticleSystem explosion = expParent.FindChild("ExplosionParticle").gameObject.GetComponent("ParticleSystem") as ParticleSystem;
			explosion.Play();
			//Disable the object's renderer and collider
			expParent.renderer.enabled = false;
			expParent.collider.enabled = false;
		}
	}
	//Wreck the sub
	void WreckSub()
	{
		//Set crashed = true, and change texture
		crashed = true;
		subMaterial.material.mainTexture = subTextures[1];
		//Disable bubble particle
		bubbles.Stop();
		bubbles.Clear();
	}
	//Sink the submarine
	void Sink()
	{
		//Set crash related variables
		crashed = true;
		
		float crashDepth = maxDepth - 5;
		float crashDepthEdge = 4;
		
		float distance = thisTransform.position.y - crashDepth;
		
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
			StartCoroutine ("SinkEffects");
		}
	}
	//Called when a revive is collected
	void ReviveCollected()
	{
		//Register revive, Show hearth on the GUI, and disable additional revive generation
		hasRevive = true;
		GUIManager.Instance.RevivePicked();
        LevelGenerator.Instance.powerUpMain.DisableReviveGeneration();
	}
    //Enables/disables the object with childs based on platform
    void EnableDisable(GameObject what, bool state)
    {
        #if UNITY_3_5
            what.SetActiveRecursively(state);
        #else
            what.SetActive(state);
        #endif
    }
	//Called when a sonic wave power up is picked up, launches the sonic wave particle
	IEnumerator LaunchSonicWave()
	{
		//Activate sonic wave, and set powerUpUsed to true
        EnableDisable(sonicWave, true);
		powerUpUsed = true;
		
		//Move the wave across the screen
		StartCoroutine(MoveToPosition(sonicWave.transform, new Vector3(65, 0, -5), 1.25f, false));
		
		//Wait for 2 seconds
		double waited = 0;
		while (waited <= 2)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield return 0;
		}
		//Call the sonic wave's disable function
		sonicWave.GetComponent<SonicWave>().Disable();
	}
	//Scale object to scale under time
	IEnumerator ScaleObject (Transform obj, Vector3 scale, float time, bool deactivate)
	{
		//Set the object active
        EnableDisable(obj.gameObject, true);
		//Get it's current scale
		Vector3 startScale = obj.localScale;
		
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
			
			yield return new WaitForEndOfFrame();
	    }
		
		//If the object is makred, disable it
        if (deactivate)
            EnableDisable(obj.gameObject, false);
		
		//If the object is the shield, enable it's shield
		if (obj.name == "Shield")
			shieldCollider.enabled = true;
	}
	//Disable shield
	IEnumerator DisableShield()
	{
		//Scale the shield to 0.01, and disable it's collider
		StartCoroutine(ScaleObject(shield.transform, new Vector3(0.01f, 1, 0.01f), 0.35f, true));
		shieldCollider.enabled = false;
		
		//Wait for 0.4 seconds
		double waited = 0;
		while (waited <= 0.4f)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield return 0;
		}
		
		//Mark shield as inactive
		shieldActive = false;
	}
	//Activate extra speed effects for time
	IEnumerator ExtraSpeedEffect(float time)
	{
		//Get the current scroll speed, then set scroll speed to 3
        float newSpeed = LevelGenerator.Instance.scrollSpeed;
        LevelGenerator.Instance.scrollSpeed = 3;
		
		//Wait for time
		double waited = 0;
		while (waited <= time)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield return 0;
		}
		
		//Set variabler for extra speed
        LevelGenerator.Instance.scrollSpeed = newSpeed;
		inExtraSpeed = false;
		canSink = true;
		
		//Activate extra speed visual effects
        EnableDisable(speedParticle, false);
        EnableDisable(speedTrail, false);
		
		//Allow the level generator to increase scrolling speed
        LevelGenerator.Instance.ExtraSpeedOver();
	}
	//The sink effects
	IEnumerator SinkEffects()
	{
		//Wait for 0.5 seconds, and stop the level scrolling in 2.5 seconds
		yield return new WaitForSeconds(0.5f);
        LevelGenerator.Instance.StartCoroutine("StopScrolling", 2.5f);
		
		//Wait for 2.75 seconds, and disable smoke emission
		yield return new WaitForSeconds(2.75f);
		smoke.enableEmission = false;
	}
	//Move obj to endPos in time
	IEnumerator MoveToPosition (Transform obj, Vector3 endPos, float time, bool enableSub) 
	{
		//Enable the sub bubble particle with increased emission, if needed
		if (enableSub)
		{
            bubbles.enableEmission = true;
            bubbles.Play();
		}
		
		//Declare variables, get the starting position, and move the object
		float i = 0.0f;
		float rate = 1.0f / time;
		
		Vector3 startPos = obj.position;
			
		while (i < 1.0) 
		{
			//If the game is not paused, increase t, and scale the object
			if (!paused)
			{
				i += Time.deltaTime * rate;
				obj.position = Vector3.Lerp(startPos, endPos, i);
			}
			
			//Wait for the end of frame
			yield return 0;
		}
		
		//The the sub is set to enable, activate it, and decrease emission rate
		if (enableSub)
		{
			subEnabled = true;
		}
	}
	//Called when the player collects/activates an extra speed
	public void ExtraSpeed()
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
        LevelGenerator.Instance.ExtraSpeedEffect();
		StartCoroutine (ExtraSpeedEffect(3));
	}
	//Called when the player collects/activate a shield power up
	public void RaiseShield()
	{
		//If a shield is already activates,  or sinking, or the controls are not enabled, return
		if (shieldActive || sinking || !subEnabled)
			return;
		
		//Set power up based variables, and raise the shield
		powerUpUsed = true;
		shieldActive = true;
		StartCoroutine(ScaleObject(shield.transform, new Vector3(18, 16.2f, 1), 0.35f, false));
	}
	//Called from the Input manager
	public void MoveUp()
	{
		//If the player is not at the min depth, and the controls are enabled, move up
		if (distanceToMin > 0 && subEnabled)	
			movingUpward = true;
	}
	//Called from the Input manager
	public void MoveDown()
	{
		//If the player is not at the max depth, and the controls are enabled, move down
		if (distanceToMax > 0 && subEnabled)	
			movingUpward = false;
	}
	//Enalbe submarine controls
	public void EnableControls()
	{
		subEnabled = true;
	}
	//Reset submarine status
	public void ResetStatus(bool moveToStart)
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
		shield.transform.localScale = new Vector3(0.01f, 1, 0.01f);
		EnableDisable(speedParticle, false);
		EnableDisable(speedTrail, false);
		firstObstacleGenerated = false;
		
		//Reset rotation and position
		newRotation	= new Vector3(0, 0, 0);
		
		this.transform.position = new Vector3(startingPos, -25.5f, thisTransform.position.z);
		this.transform.eulerAngles = newRotation;
		//Reset texture
		subMaterial.material.mainTexture = subTextures[0];
		
		//If moveToStart, move the submarine from the resting position to the starting position
		if (moveToStart)
		{
			StartCoroutine(MoveToPosition(this.transform, new Vector3(xPos, -23, thisTransform.position.z), 1.0f, true));
		}
	}
	//Disable submarine controls
	public void DisableControls()
	{
		subEnabled = false;
	}
	//Pause the submarine/particles
	public void Pause()
	{
		paused = true;
		bubbles.Pause();
	}
	//Resume the submarine/particles
	public void Resume()
	{
		paused = false;
		bubbles.Play();
	}
	//Returns crashed state
	public bool Crashed()
	{
		return crashed;
	}
	//Return revive state
	public bool HasRevive()
	{
		if (hasRevive || (SaveManager.GetRevive() > 0 && !shopReviveUsed))
			return true;
		else
			return false;
	}
	//Return power up used state
	public bool PowerUpUsed()
	{
		return powerUpUsed;
	}
	//Set starting and main position
	public void SetPositions(float starting, float main)
	{
		startingPos = starting;
		xPos = main;
	}
	//Use revive
	public IEnumerator Revive()
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
                GUIManager.Instance.DisableReviveGUI(0);
			}
			//If the player has not collected a revive, but has a revive from the store
			else
			{
				//Set variable, remove revive from account, and disable it's hearth
				shopReviveUsed = true;
                SaveManager.ModifyReviveBy(-1);
                GUIManager.Instance.DisableReviveGUI(1);
			}
			
			//Reset speed, play revive particle, and change texture to intact
			speed = 0;
			reviveParticle.Play();
			subMaterial.material.mainTexture = subTextures[0];
			
			//Reset rotation, and launch a sonic wave to clear the scene from obstacles
			newRotation	= new Vector3(0, 0, 0);
			this.transform.eulerAngles = newRotation;
			StartCoroutine("LaunchSonicWave");
			
			//Wait for 0.4 seconds, and move to starting position
			yield return new WaitForSeconds(0.4f);
			StartCoroutine(MoveToPosition(this.transform, new Vector3(xPos, -23, thisTransform.position.z), 1.0f, false));
			
			//Wait for 1.2 seconds, and restart level scrolling
			yield return new WaitForSeconds(1.2f);
            LevelGenerator.Instance.ContinueScrolling();
			
			//Restart the bubble particle, and set variables
			bubbles.Play();
			crashed = false;
			canSink = true;
			subEnabled = true;
			movingUpward = false;
			inRevive = false;	
		}
		
		//If the player is already in revive, wait for the end of frame, and return to caller
		yield return 0;
	}
}