#pragma strict

class InputManagerJS extends MonoBehaviour
{
	var useTouch				: boolean  		= false;	//Use touch based controls
	
	var mask					: LayerMask 	= -1;		//Set input layer mask
	
	private var ray				: Ray;						//The hit ray
	private var hit				: RaycastHit;				//The hit raycast
	
	private var button			: Transform;				//The triggered button
	
	function Update () 
	{
		if (useTouch)
			GetTouches();
		else
			GetClicks();
	}
	//If playing with mouse
	private function GetClicks()
	{
		//If we pressed the mouse
		if(Input.GetMouseButtonDown(0))
		{
			//Cast a ray
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			//If the ray hit something in the set layer
			if (Physics.Raycast(ray, hit, Mathf.Infinity, mask))
			{
				//Register it, and send it to the GUI manager
				button = hit.transform;
				GUIManagerJS.Instance().ButtonDown(button);
			}
			//If the ray didn't hit a GUI object
			else
			{
				//Set the button to null, and move the sub up
				button = null;
				PlayerManagerJS.Instance().MoveUp();
			}
		}
		//If the click was released
		else if (Input.GetMouseButtonUp(0))
		{
			//If there is no button registered previousely
			if (button == null)
				//Move the sub down
				PlayerManagerJS.Instance().MoveDown();
			//If there is a button registered
			else
				//Send it to the GUI manager
				GUIManagerJS.Instance().ButtonUp(button);
		}
	}
	//If playing with touch screen
	private function GetTouches()
	{
		//Loop through the touches
		for (var touch : Touch in Input.touches) 
		{
			//If a touch has happened
            if (touch.phase == TouchPhase.Began && touch.phase != TouchPhase.Canceled)
			{
				//Cast a ray
				ray = Camera.main.ScreenPointToRay(touch.position);
				
				//If the ray hit something in the set layer
				if (Physics.Raycast(ray, hit, Mathf.Infinity, mask))
				{
					//Register it, and send it to the GUI manager
					button = hit.transform;
					GUIManagerJS.Instance().ButtonDown(button);
				}
				//If the ray didn't hit a GUI object
				else
				{
					//Set the button to null, and move the sub up
					button = null;
					PlayerManagerJS.Instance().MoveUp();
				}
			}
			//If a touch has ended
			else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				//If there is no button registered previousely
				if (button == null)
					//Move the sub down
					PlayerManagerJS.Instance().MoveDown();
				//If there is a button registered
				else
					//Send it to the GUI manager
					GUIManagerJS.Instance().ButtonUp(button);
			}
		}
	}
}