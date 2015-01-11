#pragma strict

class FollowJS extends MonoBehaviour 
{
	var target 				: Transform;			//The target to follow
	
	private var targetPos 	: Vector3;				//The target position to follow
	
	//Called at every frame
	function Update () 
	{
		//Get the position
		targetPos = this.transform.position;
		targetPos.y = target.position.y;
		targetPos.x = target.position.x - 12;
		//Go to the position
		this.transform.position = targetPos;
	}
}