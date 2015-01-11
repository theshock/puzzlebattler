#pragma strict

class SonicWaveJS extends MonoBehaviour 
{
	private var canDisable		: boolean	= false;		//Can the power up disable itself?
	
	//If the wave collides with something
	function OnTriggerEnter (other : Collider)
	{
		//If it is an obstacle
		if (other.transform.name == "Mine" || other.transform.name == "Chain" || other.transform.name == "MineChain" || other.transform.name == "Laser" || other.transform.name == "LaserBeam")
		{
			//Explode it
			PlayExplosion (other.transform);
		}
		//If it is a torpedo
		else if (other.name == "Torpedo")
		{
			//If the sonic wave is in the screen, disable it
			if (!canDisable)
				other.transform.parent.GetComponent(TorpedoJS).TargetHit(true);
		}
		//If the sonic wave is collided with and obstacle reset triggerer, and the wave can disable itself
		else if (other.name == "ResetTriggerer" && other.tag == "Obstacles" && canDisable)
		{
			//Reset the wave
			ResetThis();
		}
	}
	//Called when the wave collides with an obstacle
	private function PlayExplosion(expParent : Transform)
	{	
		//Disable obstacle's renderer and collider
		expParent.renderer.enabled = false;
		expParent.collider.enabled = false;
		
		//If the wave can disable, return
		if (canDisable)
			return;
		
		//Plays the explosion effect
		var explosion : ParticleSystem = expParent.FindChild("ExplosionParticle").gameObject.GetComponent(ParticleSystem);
		explosion.Play();
	}
	//Reset the sonic wave
	private function ResetThis()
	{
		//Reset variable and position, then disable
		canDisable = false;
		this.transform.localPosition = Vector3(-70, 0, -5);
		EnableDisable(this.gameObject, false);
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
    //Prepare for disable
	function Disable()
	{
		canDisable = true;
	}
}
