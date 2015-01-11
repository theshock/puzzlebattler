#pragma strict
#pragma downcast

class TorpedoMainJS extends MonoBehaviour 
{
	var movementSpeed  			: float = 0;										//The movement speed of the torpedoes
	
	private var inactive 	 	: List.<TorpedoJS> = new List.<TorpedoJS>();		//A list of the inactive torpedoes
	private var activated	 	: List.<TorpedoJS> = new List.<TorpedoJS>();		//A list of the active torpedoes
	
	//Called at the beginning of the game
	function Start()
	{
		//Loop trought the children
		for (var child : Transform in transform)
		{
			//Add child to inactive
			inactive.Add(child.GetComponent(TorpedoJS));
		}
	}
	//Called from the level generator, launches a torpedo
	function LaunchTorpedo()
	{
		//Get the first torpedo from the inactive list
		var t : TorpedoJS = inactive[0];
		
		//Remove it from the inactived, and add it to the actives
		inactive.Remove(t);
		activated.Add(t);
		
		//Launch the torpedo with the modified movement speed
		t.Launch(movementSpeed);
	}
	//Reset the torpedo, called from the actual torpedo
	function ResetTorpedo(sender : TorpedoJS)
	{
		//Remove it from the actives, and add it to the inactive
		activated.Remove(sender);
		inactive.Add(sender);
	}
	//Resets the torpedoes
	function ResetAll()
	{
		gameObject.BroadcastMessage ("ResetThis");
	}
	//Pause the torpedoes
	function PauseAll()
	{
		this.gameObject.BroadcastMessage("Pause");
	}
	//Resume the torpedoes
	function ResumeAll()
	{
		this.gameObject.BroadcastMessage ("Resume");
	}
}
