import System.Collections.Generic;

#pragma strict
#pragma downcast

class ObstaclesJS extends MonoBehaviour 
{
	var elements 		: List.<GameObject> = new List.<GameObject>();	//A list that holds the obstacle childs
	
	//Called at the beginning of the game
	function Start()
	{
		//Loop through children
		for (var child : Transform in transform)
		{
			//If the child is not a triggerer
			if (child.name != "SpawnTriggerer" && child.name != "ResetTriggerer")
			{
				//Add it to the elements list, and deactivate it
	            elements.Add(child.gameObject);
	            EnableDisable(child.gameObject, false);
			}
		}
	}
	//Called when the obstacle is reseting
	function DeactivateChild()
	{
		//Loop through the elements list
		for (var i : int = 0; i < elements.Count; i++)
		{	
			//Look for the child's explosion
			var go : Transform = elements[i].transform.FindChild("ExplosionParticle");
			
			//If we found it
			if (go != null)
				//Stop the particle
				go.GetComponent(ParticleSystem).Stop();
			//Deactivate the child
			EnableDisable(elements[i], false);
		}
	}
	//Called when the obstacle is spawned
	function ActivateChild()
	{
		//Loop through the elements array
		for (var i : int = 0; i < elements.Count; i++)
		{
			//Activate the child
			EnableDisable(elements[i], true);
			
			//Active the child's renderer and collider
			elements[i].renderer.enabled = true;
			elements[i].collider.enabled = true;
		}
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
}
