#pragma strict

class SpriteAnimJS extends MonoBehaviour 
{
	var frameA				: Texture2D;					//The first frame of the animation
	var frameB 				: Texture2D;					//The second frame of the animation
	
	private var currentId 	: int 			= 0;			//The ID of the current frame
	private var canAnimate 	: boolean 		= false;		//Animation enabled/disabled
	
	//Called when the object is enabled
	function OnEnable () 
	{
		//Enable the animation
		canAnimate = true;
	}
	//Called when the object is disabled
	function OnDisable()
	{
		//Stop the animation coroutine, and disable the animation
		StopCoroutine("Animate");
		canAnimate = false;
	}
	//Called on every frame
	function Update()
	{
		//If the animation is enabled
		if (canAnimate)
		{
			//Start the animation coroutine
			StartCoroutine(Animate());
		}
	}
	//The animation coroutine
	function Animate()
	{
		//Disable the calling of additional coroutines
		canAnimate = false;
		
		//Wait for 0.1 seconds
		yield WaitForSeconds(0.1f);
		
		//If the current animation frame is 0
		if (currentId == 0)
		{
			//Go to animation frame 1
			this.renderer.material.mainTexture = frameB;
			currentId = 1;
		}
		//If the current animation frame is 1
		else
		{
			//Go to animation frame 0
			this.renderer.material.mainTexture = frameA;
			currentId = 0;
		}
		
		//Enable the calling of a new coroutine
		canAnimate = true;
	}
}
