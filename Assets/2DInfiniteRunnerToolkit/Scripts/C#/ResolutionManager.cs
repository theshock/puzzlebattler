using UnityEngine;
using System.Collections;

public class ResolutionManager : MonoBehaviour 
{
	static ResolutionManager myInstance;
    static int instances = 0;
	
	bool canModify 	= true;		//Can modify the resolution 
		
	float left		= 0;		//The left position for the left GUI elements
	float right		= 0;		//The right position for the right GUI elements
	float shop		= 0;		//The shop left/right position
	float mainHL	= 0;		//The main menu header left position
	float mainHR	= 0;		//The main menu header right position
	float scale		= 0;		//The level scale (sand, background, etc...)
	float hangar	= 0;		//The hangar location
	float subPos	= 0;		//The position of the submarine
	float subStartP	= 0;		//The starting position of the submarine
	
	//Retursn the instance
    public static ResolutionManager Instance
    {
        get
        {
            if (myInstance == null)
                myInstance = FindObjectOfType(typeof(ResolutionManager)) as ResolutionManager;

            return myInstance;
        }
    }
	
	// Use this for initialization
	void Start () 
	{
		//Calibrates the myInstance static variable
        instances++;

        if (instances > 1)
            Debug.Log("Warning: There are more than one Player Manager at the level");
        else
            myInstance = this;
	}
	//Returns the aspect ratio
	string GetAspectRation(int a, int b)
	{
		int m = GetGreatestDivider(a, b);
		return (a/m) + ":" + (b/m);
	}
	//Returns the greatest divider of a and b
	int GetGreatestDivider(int a, int b)
	{
		int m;
 
		while (b != 0) 
		{
			m = a % b;
			a = b;
			b = m;
		}
		
		return a;
	}
	//Set target resolution
	public void SetResolutionSetting(GameObject[] scalable, GameObject[] shopElements, GameObject[] leftElements, GameObject[] rightElements, GameObject h)
	{
		//Calculate aspect ratio
		string ar = GetAspectRation(Screen.width, Screen.height);
		
		//Set aspect ratio based values
		switch (ar)
		{
			case "3:2":
				left = -18;
				right = 45;
				shop = 4.5f;
				mainHL = -4;
				mainHR = 4;
				scale = 105;
				hangar = 37;
				subPos = -34;
				subStartP = -41;
				break;
			
			case "4:3":
				left = -14;
				right = 40;
				shop = 0;
				mainHL = 0;
				mainHR = 0;
				scale = 100;
				hangar = 39;
				subPos = -30;
				subStartP = -37;
				break;
			
			case "5:3":
				left = -24;
				right = 50;
				shop = 8;
				mainHL = -5;
				mainHR = 5;
				scale = 115;
				hangar = 31.5f;
				subPos = -40;
				subStartP = -47;
				break;

			case "16:9":
				left = -26.5f;
				right = 55;
				shop = 13.5f;
				mainHL = -10;
				mainHR = 10;
				scale = 121;
				hangar = 28;
				subPos = -43;
				subStartP = -50;
				break;
			
			default:
				canModify = false;
				break;
		}
		
		//If the aspect ratio is not supported, return to caller
		if (!canModify)
			return;
		
		//Declare temp values
		Vector3 temp;
		Vector2 offset;
			
		//Set left GUI elements
		foreach(GameObject element in leftElements)
		{
			temp = element.transform.position;
			
			if (element.name == "HeaderL")
				temp.x = mainHL;
			else
				temp.x = left;
			
			element.transform.position = temp;
		}
		
		//Set right GUI elements
		foreach(GameObject element in rightElements)
		{
			temp = element.transform.position;
			
			if (element.name == "HeaderR")
				temp.x = mainHR;
			else
				temp.x = right;
			element.transform.position = temp;
			
		}
		
		//Set position/scale of shop elements
		foreach(GameObject element in shopElements)
		{
			switch (element.name)
			{
				case "Bar":
				case "Header":
					temp = element.transform.localScale;
					temp.x = scale;
					element.transform.localScale = temp;
					break;
				
				case "Left":
					temp = element.transform.position;
					temp.x = -shop;
					element.transform.position = temp;
					break;
				
				case "Right":
					temp = element.transform.position;
					temp.x = shop;
					element.transform.position = temp;
					break;
			}
		}
		
		//Scale the background/GUI elements
		foreach(GameObject element in scalable)
		{
			temp = element.transform.localScale;
			temp.x = scale;
			element.transform.localScale = temp;
			
			offset = element.renderer.material.mainTextureScale;
			offset.x = scale/100;
			element.transform.renderer.material.mainTextureScale = offset;
		}
		
		//Set the hangar's current and starting position
		temp = h.transform.position;
		temp.x = hangar;
		h.transform.position = temp;
		
		LevelGenerator.Instance.SetHangarPos(hangar);
		
		//Set the submarine starting and main position
		GameObject sub = PlayerManager.Instance.gameObject;
		temp = sub.transform.position;
		temp.x = subStartP;
		sub.transform.position = temp;
		
		PlayerManager.Instance.SetPositions(subStartP, subPos);
	}
	//Returns the right position
	public float RightPosition()
	{
		return right;
	}
}
