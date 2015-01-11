#pragma strict

class LaserBeamJS extends MonoBehaviour 
{
	var scrollSpeed 		: float 	= 0.0f;					//Laser scroll speed
	
	private var scrolling 	: Vector2	= Vector2(0, 0);		//Scrolling vector
	
	//Called at every frame
	function Update () 
	{
		//Apply scrolling speed to scrolling vector
		scrolling.x = scrollSpeed;
		//Set the texture offset based on scrolling vector
		this.renderer.material.mainTextureOffset += scrolling * Time.deltaTime;
	}
}