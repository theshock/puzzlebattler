#pragma strict

class LevelEdgeJS extends MonoBehaviour 
{
	//Called when something triggerer the object
	function OnTriggerEnter(other : Collider) 
	{
		//If a spawn triggerer is collided with this
		if (other.name == "SpawnTriggerer")
		{
			//Spawn a proper object
			switch (other.tag)
			{
				case "SecondLayer":
					LevelGeneratorJS.Instance().GenerateSecondLayer(0);
					break;
				
				case "ThirdLayer":
					LevelGeneratorJS.Instance().GenerateThirdLayer(0);
					break;
					
				case "FourthLayer":
					LevelGeneratorJS.Instance().GenerateFourthLayer(0);
					break;
				
				case "Obstacles":
					LevelGeneratorJS.Instance().GenerateObstacles();
					break;
			}
		}
		//If a reset triggerer is collided with this
		else if (other.name == "ResetTriggerer")
		{
			//Reset the proper object
			switch (other.tag)
			{
				case "SecondLayer":
				case "ThirdLayer":
				case "FourthLayer":
					LevelGeneratorJS.Instance().SleepGameObject(other.transform.parent.gameObject);
					break;
				
				case "Obstacles":
					other.transform.parent.GetComponent(ObstaclesJS).DeactivateChild();
					LevelGeneratorJS.Instance().SleepGameObject(other.transform.parent.gameObject);
					break;
			}
		}
		//If a power up is collided with this
		else if (other.tag == "PowerUps")
		{
			//Reset the power up
			other.GetComponent(PowerUpJS).ResetThis();
		}
		//If a torpedo is collided with this
		else if (other.name == "Torpedo")
		{
			//Reset the torpedo
			other.transform.parent.gameObject.GetComponent(TorpedoJS).ResetThis();
		}
	}
}
