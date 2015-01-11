import System.Collections.Generic;

#pragma strict
#pragma downcast

class PowerUpMainJS extends MonoBehaviour 
{
	var verticalSpeed 				: float 		= 5.0f;								//The vertical speed of the generated power ups
	var verticalDistance 			: float 		= 1.0f;								//The vertical movement distance of the generated power ups
	
	var horizontalSpeed 			: float 		= 0;								//The horizontal speed of the generated power up
	
	private var inactive 	 		: List.<PowerUpJS> = new List.<PowerUpJS>();		//A list of the active power ups
	private var activated	 		: List.<PowerUpJS> = new List.<PowerUpJS>();		//A list of the deactivated power ups
	
	private var sonicBlastFirst		: boolean 		= false;							//Generate sonic blast first switch
	private var canGenerateRevive	: boolean 		= true;								//Can generate revive switch
	
	//Called at the start of the game
	function Start()
	{
		//Loop through children
		for (var child : Transform in transform)
		{
			//Add child to the inactive list
			inactive.Add(child.GetComponent(PowerUpJS));
		}
	}
	//Find and returns a compatible power up to spawn
	private function FindCompatiblePowerUp()
	{
		var n : int = 0;
		//If set to spawn sonic blast first
		if (sonicBlastFirst)
		{
			//Loop trought the power ups
			for (var item : PowerUpJS in inactive)
			{
				//If the item is the sonic wave, return in
				if (item.name == "SonicWave")
					return item;
			}
			
			sonicBlastFirst = false;
		}
		//If it is not set to spawn the sonic blast first
		else
		{
			//If cant generate revive
			if (!canGenerateRevive)
			{
				//Get a power up, which is not the revive
				var powerUp	: PowerUpJS = null;
				
				do
				{
					n = Random.Range(0, inactive.Count);	
					powerUp = inactive[n];
				} while (powerUp.name == "Revive");
				
				//Return it
				return powerUp;
			}
			//If can generate revive
			else
			{
				//Get a random power up, and return it
				n = Random.Range(0, inactive.Count);	
				return inactive[n];
			}
		}
		
		return null;
	}
	//Called from the level generator, spawns a power up
	function GeneratePowerUp(multiplyValue : float)
	{
		//Find a compatible power up, and remove it from the inactive list
		var powerUp : PowerUpJS = FindCompatiblePowerUp();
		inactive.Remove(powerUp);
		//Find a new y position for the power up randomly
		var newPos : Vector3 = powerUp.transform.position;
		newPos.y = Random.Range(-23, 8);
		powerUp.transform.position = newPos;
		//Activate and launch the power up
		activated.Add(powerUp);
		powerUp.Setup(verticalSpeed, verticalDistance, horizontalSpeed * multiplyValue);
	}
	//Resets a power up, called from the actual power up
	function ResetPowerUp(sender : PowerUpJS)
	{
		//Deactivate the power up, and add it to the inactive list
		activated.Remove(sender);
		inactive.Add(sender);
	}
	//Sets the sonic blast as the first power up to spawn
	function SetSonicBlastFirst()
	{
		sonicBlastFirst = true;
	}
	//Disables revive generation
	function DisableReviveGeneration()
	{
		canGenerateRevive = false;
	}
	//Resets the manager, and notify the power ups to reset
	function ResetAll()
	{
		sonicBlastFirst = false;
		canGenerateRevive = true;
		
		gameObject.BroadcastMessage ("ResetThis");
	}
	//Notifies the power ups to pause
	function PauseAll()
	{
		this.gameObject.BroadcastMessage("Pause");
	}
	//Notifies the power ups to resume
	function ResumeAll()
	{
		this.gameObject.BroadcastMessage ("Resume");
	}
}