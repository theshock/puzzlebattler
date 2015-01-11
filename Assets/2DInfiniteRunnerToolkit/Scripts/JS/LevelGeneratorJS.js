import System.Collections.Generic;

#pragma strict
#pragma downcast
 
class LevelGeneratorJS extends MonoBehaviour 
{
	var torpedoMain						: TorpedoMainJS;					//A link to the torpedo manager
	var powerUpMain						: PowerUpMainJS;					//A link to the power up manager
	
	var submarine						: GameObject;						//A link to the submarine
	var hangar							: GameObject;						//A link to the hangar
	
	var scrollSpeed						: float			= 0.4f;				//Starting scroll speed
	var maxScrollSpeed					: float  		= 0.7f;				//Maximum scroll speed
	var maxScrollSpeedDist				: float  		= 1500;				//Maximum scroll speed at thid distance
	
	var distance						: float;							//Current distance
	
	var background						: Renderer;							//The background renderer
	var sand							: Renderer;							//The sand renderer
	
	var obstacles						: List.<GameObject>;				//The array containing the obstacles
	
	var secondLayer						: List.<GameObject>; 				//The list containing the second layer elements 
	var thirdLayer			 			: List.<GameObject>; 				//The list containing the third layer elements 
	var fourthLayer						: List.<GameObject>; 				//The list containing the fourth layer elements 
	
	static var myInstance				: LevelGeneratorJS;
    static var instances 				: int			= 0;
	
	private var activeElements 			: List.<GameObject> = new List.<GameObject>();	//A list containing the active elements
	
	private var scrolling 				= Vector2(0, 0);					//A vector for the seaBackground scrolling
	
	private var defaultScroll			: float;							//The default scrolling speed
	private var scrollBackg				: float;							//The background scrolling speed
	private var scrollMiddle			: float;							//The middleground scrolling speed
	private var scrollForg				: float;							//The foreground scrolling speed
	private var scrollAtCrash			: float;							//The scrolling speed before a crash
	
	private var hangarPos				: float			= 39;				//The starting x position of the hangar
	
	private var canGeneratePowerUp		: boolean 		= true;				//Power up generation switch
	private var canGenerateTorpedo		: boolean		= true;				//Torpedo generation switch
	
	private var canGenerate				: boolean		= false;			//Generation switch
	private var paused					: boolean		= true;				//Pause switch
	
	private var scrollHangar			: boolean		= true;				//Scroll hangar switch
	private var canModifySpeed			: boolean		= true;				//Modify speed switch
	
	//Returns the instance
    public static function Instance()
    {
        if (myInstance == null)
             myInstance = FindObjectOfType(typeof(LevelGeneratorJS)) as LevelGeneratorJS;

        return myInstance;
    }
	
	//This function is called at the start of the game
	function Start()
	{
		//Calibrates the myInstance static variable
        instances++;

        if (instances > 1)
            Debug.Log("Warning: There are more than one Level Generator at the level");
        else
            myInstance = this;
		
		//Sets the starting values
		defaultScroll = scrollSpeed;
		
		scrollBackg = scrollSpeed * 12.5f;
		scrollMiddle = scrollBackg * 3;
		scrollForg = scrollBackg * 8;
		
		//Generate a seconds and third layer in the middle
		GenerateSecondLayer(1);
		GenerateThirdLayer(1);
	}
	//This function is called every frame
	function Update()
	{
		//If the generation is enabled, and the game is not paused
		if (canGenerate && !paused)
		{
			//If power up generation enabled
			if (canGeneratePowerUp)
				//Generate a power up
				StartCoroutine(GeneratePowerUp());
			//If a torpedo generation is enalbed
			if (canGenerateTorpedo)
				//Generate a torpedo
				StartCoroutine(GenerateTorpedo());
			
			//Scroll the level
			ScrollLevel();
		}
		
		//If not paused
		if (!paused)
		{
			//Increase the distance, and notify the mission manager
			distance += scrollSpeed * Time.deltaTime * 25;
			MissionManagerJS.Instance().DistanceEvent(parseInt(distance));
		}
	}
	//Scroll the level
	private function ScrollLevel()
	{
		//If can modify speed
		if (canModifySpeed)
			//Calculate layer scrolling speed
			scrollSpeed = defaultScroll + (((maxScrollSpeedDist - (maxScrollSpeedDist - distance)) / maxScrollSpeedDist) * (maxScrollSpeed - defaultScroll));
		
		//Calculate the other scrolling speeds
		scrollBackg = scrollSpeed * 12.5f;
		scrollMiddle = scrollBackg * 3;
		scrollForg = scrollBackg * 8;
		
		//Scroll the elements in the list activeElements, with a speed maching their layer
		for (var i : int = 0; i < activeElements.Count; i++)
		{
			switch(activeElements[i].tag)
			{
				case "SecondLayer":
					activeElements[i].transform.position -= Vector3.right * scrollBackg * Time.deltaTime;
					break;
				
				case "ThirdLayer":
					activeElements[i].transform.position -= Vector3.right * scrollMiddle * Time.deltaTime;
					break;
				
				case "FourthLayer":
				case "Obstacles":
				case "Particle":
					activeElements[i].transform.position -= Vector3.right * scrollForg * Time.deltaTime;
					break;
			}
		}
		
		//If the hangar is scrollable
		if (scrollHangar)
		{
			//If it's position is lesser than -200
			if (hangar.transform.position.x < -200)
				//Disable it's scrolling
				scrollHangar = false;
			//Scroll the hangar
			hangar.transform.position -= Vector3.right * scrollForg * Time.deltaTime;
		}
		
		//Apply scroll speed to scroll vector
		scrolling.x = scrollSpeed;
		
		//Scroll the sand
		sand.material.mainTextureOffset += scrolling * Time.deltaTime;
	}
	//Clear the level, and empty the active elements list
	private function ClearMap()
	{
		//Stop all coroutines
		StopAllCoroutines();
		
		//Reset the power up and torpedo managers
		torpedoMain.ResetAll();
		powerUpMain.ResetAll();
		
		//Go while there is an element in activeElements
		while (activeElements.Count > 0)
		{
			//Reset and remove the element based on their layer
			switch(activeElements[0].tag)
			{
				case "SecondLayer":
					secondLayer.Add(activeElements[0]);
					activeElements[0].transform.localPosition = new Vector3(100, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
					EnableDisable(activeElements[0], false);
					activeElements.Remove(activeElements[0]);
					break;
				
				case "ThirdLayer":
					thirdLayer.Add(activeElements[0]);
					activeElements[0].transform.localPosition = new Vector3(115, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
					EnableDisable(activeElements[0], false);
					activeElements.Remove(activeElements[0]);
					break;
				
				case "FourthLayer":
					fourthLayer.Add(activeElements[0]);
					activeElements[0].transform.localPosition = new Vector3(125, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
					EnableDisable(activeElements[0], false);
					activeElements.Remove(activeElements[0]);
					break;
				
				case "Obstacles":
					obstacles.Add(activeElements[0]);
					activeElements[0].transform.localPosition = new Vector3(175, activeElements[0].transform.localPosition.y, activeElements[0].transform.localPosition.z);
					EnableDisable(activeElements[0], false);
					activeElements.Remove(activeElements[0]);
					break;
			}
		}		
	}
	//Randomize the order of the obstacles in the obstacles list
	private function RandomizeObstacles()  
	{
		//Get the number of obstacles in the array
	    var n : int = obstacles.Count;  
		var temp : GameObject;
		//Randomize them
	    while (n > 1) 
		{  
	        n--;  
	        var k : int = Random.Range(0, n + 1);
	        temp = obstacles[k];  
	        obstacles[k] = obstacles[n];  
	        obstacles[n] = temp;  
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
    //Generate a power up
	private function GeneratePowerUp()
	{	
		//Set power up generation switch to false
		canGeneratePowerUp = false;
		
		//Get a random number between 15 and 30
		var n : int = Random.Range(15, 30);
		
		//Wait for n seconds
		var waited : double = 0;
		while (waited <= n)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield;
		}
		//Generate a power up
		powerUpMain.GeneratePowerUp(scrollSpeed/defaultScroll);
		//Set power up generation switch to true
		canGeneratePowerUp = true;
	}
	//Generate a torpedo
	private function GenerateTorpedo()
	{
		//Set torpedo generation switch to false
		canGenerateTorpedo = false;
		
		//Get a random number between 15 and 35
		var n : int = Random.Range(15, 35);
		
		//Wait for n seconds
		var waited : double = 0;	
		while (waited <= n)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame
			yield;
		}
		
		//If the distance is greater that 1000
		if (distance < 1000)
			//Get a random number between 10 and 30
			n = Random.Range(10, 30);
		else
			//Else, get a random number between 10 and 40
			n = Random.Range(10, 40);
		
		//Divide n by 10 to get a number between 1 and 2 or 1 and 3
		n = n / 10;
		
		//Generate n torpedoes
		for (var i : int = 0; i < n; i++)
		{
			//Generate a torpedo
			torpedoMain.LaunchTorpedo();
			
			//Wait for 1 seconds
			waited = 0;
			while (waited <= 1)
			{
				//If the game is not paused, increase waited time
				if (!paused)
					waited += Time.deltaTime;
				//Wait for the end of the frame
				yield;
			}
		}
		//Set torpedo generation switch to true
		canGenerateTorpedo = true;
	}
	//Stop scrolling speed after a crash
	function StopScrolling (time : float)
	{
		//Disable speed modification
		canModifySpeed = false;
		
		//Set startValue and scrollAtCrash
		var startValue : float = scrollSpeed;
		scrollAtCrash = scrollSpeed;
		
		//Slow down to 0 in time
		var rate = 1.0f / time;
	    var t = 0.0f;
	    
	    while (t < 1.0f) 
	    {
	        t += Time.deltaTime * rate;
	        scrollSpeed = Mathf.Lerp(startValue, 0, t);
	        yield WaitForEndOfFrame();
	    }
		
		//Disable generation
		canGenerate = false;
		paused = true;
		
		//Notify the mission manager
		MissionManagerJS.Instance().SinkEvent(parseInt(distance));
		
		//Wait for a second
		yield WaitForSeconds(1);
		
		//If the player has a revive
		if (PlayerManagerJS.Instance().HasRevive())
			//Show revive button
			StartCoroutine(GUIManagerJS.Instance().ShowRevive());
		else
			//Else, show finish menu
			GUIManagerJS.Instance().ShowEnd();
	}
	//Generate a second layer element
	function GenerateSecondLayer(placeAtLoc : int)
	{
		//Get a random element from the second layer
		var n : int = Random.Range(0, secondLayer.Count);
		var go : GameObject = secondLayer[n];
		
		//Remove it from the second layer list, and add it to the active elements
		secondLayer.Remove(go);
		activeElements.Add(go);
		
		//If place at middle
		if (placeAtLoc == 1)
		{
			//Place it in the middle
			var newPos : Vector3 = go.transform.localPosition;

			newPos.x = 0;
			go.transform.localPosition = newPos;
		}
		//Activate the object
		EnableDisable(go, true);
	}
	//Generate a third layer element
	function GenerateThirdLayer(placeAtLoc : int)
	{
		//Get a random element from the third layer
		var n : int = Random.Range(0, thirdLayer.Count);
		var go : GameObject = thirdLayer[n];
		
		//Remove it from the third layer list, and add it to the active elements
		thirdLayer.Remove(go);
		activeElements.Add(go);
		
		//If place at middle
		if (placeAtLoc == 1)
		{
			//Place it in the middle
			var newPos : Vector3 = go.transform.localPosition;
			
			newPos.x = 0;
			go.transform.localPosition = newPos;
		}
		//Activate the object
		EnableDisable(go, true);
	}
	//Generate a fourth layer element
	function GenerateFourthLayer(placeAtLoc : int)
	{	
		//Get a random element from the fourth layer
		var n : int = Random.Range(0, fourthLayer.Count);
		var go : GameObject = fourthLayer[n];
		
		//Remove it from the fourth layer list, and add it to the active elements
		fourthLayer.Remove(go);
		activeElements.Add(go);
		
		//If place at middle
		if (placeAtLoc == 1)
		{
			//Place it in the middle
			var newPos : Vector3 = go.transform.localPosition;
			newPos.x = 0;
			
			go.transform.localPosition = newPos;
		}
		//Activate the object
		EnableDisable(go, true);
	}
	//Generate an obstacle element
	function GenerateObstacles()
	{	
		//Get a random element from the obstacles
		var n : int = Random.Range(0, obstacles.Count);
		var go : GameObject = obstacles[n];
		
		//Remove it from the obstacles layer list, and add it to the active elements
		obstacles.Remove(go);
		activeElements.Add(go);
		
		//Activate the object
		go.GetComponent(ObstaclesJS).ActivateChild();
	}
	//Disable, remove and reset an object
	function SleepGameObject(go : GameObject)
	{
		//Disable and reset an object based on it's layer
		switch (go.tag)
		{
			case "SecondLayer":
				activeElements.Remove(go);
				secondLayer.Add(go);
				go.transform.localPosition = new Vector3(100, go.transform.localPosition.y, go.transform.localPosition.z);
				break;
			
			case "ThirdLayer":
				activeElements.Remove(go);
				thirdLayer.Add(go);
				go.transform.localPosition = new Vector3(115, go.transform.localPosition.y, go.transform.localPosition.z);
				break;
			
			case "FourthLayer":
				activeElements.Remove(go);
				fourthLayer.Add(go);
				go.transform.localPosition = new Vector3(125, go.transform.localPosition.y, go.transform.localPosition.z);
				break;
			
			case "Obstacles":
				activeElements.Remove(go);
				obstacles.Add(go);
				go.transform.localPosition = new Vector3(175, go.transform.localPosition.y, go.transform.localPosition.z);
				break;
		}
		
		if (go.tag != "Obstacles")
			EnableDisable(go, false);
	}
	//Add an explosion to the active elements
	function AddExplosion(exp : GameObject)
	{
		activeElements.Add(exp);
	}
	//Remove an explosion to the active elements
	function RemoveExplosion(exp : GameObject)
	{
		activeElements.Remove(exp);
	}
	//Restart the level generator
	function Restart(startToScroll : boolean)
	{
		//Clear the map, and reset the hangar
		ClearMap();
		hangar.transform.localPosition = new Vector3(hangarPos, 1, hangar.transform.localPosition.z);
		
		//Reset speed modify, torpedo and power up generation switch
		canGenerateTorpedo = true;
		canGeneratePowerUp = true;
		canModifySpeed = true;
		
		//Reset distance and scroll speed
		distance = 0;
		scrollSpeed = defaultScroll;
		
		//Generate a second and third layer element at the middle
		GenerateSecondLayer(1);
		GenerateThirdLayer(1);
		
		//Enalbe hangar scroll
		scrollHangar = true;
		
		//If start scrolling now
		if (startToScroll)
			//Start to generate objects
			StartCoroutine(StartToGenerate(1.75f, 3));
	}
	//Pause the level generator
	function Pause()
	{
		//Pause the generator
		canGenerate = false;	
		paused = true;
		//Pause the torpedo and power up manager
		torpedoMain.PauseAll();
		powerUpMain.PauseAll();
	}
	//Resume the level generator
	function Resume()
	{
		//Resume the generator
		canGenerate = true;	
		paused = false;
		//Resume the torpedo and power up manager
		torpedoMain.ResumeAll();
		powerUpMain.ResumeAll();
	}
	//Disable speed modification
	function ExtraSpeedEffect()
	{
		canModifySpeed = false;
	}
	//Enable speed modification
	function ExtraSpeedOver()
	{
		canModifySpeed = true;
	}
	//Contiune to scroll the level
	function ContinueScrolling()
	{
		//Reset scroll speed to before crash
		scrollSpeed = scrollAtCrash;
		
		//Enable generation and speed modification
		paused = false;
		canGenerate = true;
		canModifySpeed = true;
	}
	//Returns the speed multiplier for the powerup/torpedo
    function SpeedMultiplier()
    {
        return scrollSpeed / defaultScroll;
    }
	//Sets the starting position for the hangar
	function SetHangarPos(pos : float)
	{
		hangarPos = pos;
	}
	//Scroll an explosion particle
	function ScrollExplosion(particle : ParticleSystem)
	{
		//Get the particle gameobject, and add it to the active elements
		var particleGO : GameObject = particle.gameObject;	
		activeElements.Add(particleGO);
		
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
		
		//Remove the particle from the active elements
		activeElements.Remove(particleGO);
	}
	//Start the level generation system
	function StartToGenerate(waitTime : float, obstacleWaitTime : float)
	{
		//Randomize the obstalce list
		RandomizeObstacles();
		
		//Enable generation
		canGenerate = true;
		paused = false;
		
		//Wait for waitTime
		var waited : double = 0;
		while (waited <= waitTime)
		{
			//If the game is not paused, increase waited time
			if (!paused)
				waited += Time.deltaTime;
			//Wait for the end of the frame*/
			yield;
		}
		
		//Generate a fourth layer element
		GenerateFourthLayer(0);
	}
}
