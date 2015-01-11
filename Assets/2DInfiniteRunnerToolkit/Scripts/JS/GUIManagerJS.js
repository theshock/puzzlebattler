#pragma strict

class GUIManagerJS extends MonoBehaviour
{
 	var overlay						: Renderer;				//The overlay between the game and menus

    var bestDist					: TextMesh; 			//Displays the best distance in the hangar
    var menuTextures				: Texture2D[];			//The main menu textures (arrow, audio)
    var mainMenuElements			: GameObject[];			//The main menu elements

    var shopTexts					: TextMesh[];			//Contains the shop texts
    var shopPrices					: int[];				//Contains the shop prices
	var shopElements				: GameObject[];			//Contains the shop elements

    var startPowerUps				: GameObject[];			//The power up activation buttoms
    var guiTexts					: TextMesh[];			//The main GUI coin and distance indicator
    var mainGUIElements				: GameObject[];			//The main GUI and hearts

    var pauseElements				: GameObject[];			//The pause menu elements

    var finishMenu					: GameObject;			//The finish menu to show after a crash
    var finishTexts					: TextMesh[];			//The finish menu coin and distance indicators

    var missionNotification			: GameObject[];			//The mission complete notifications

    var missionTexts				: TextMesh[];			//The mission GUI texts
    var missionStatus				: TextMesh[];			//The mission status indicator texts

    static var myInstance			: GUIManagerJS;
    static var instances			: int		= 0;
		
	private var showMainGUI			: boolean	= false;	//The main GUI showed
	private var showPause			: boolean 	= false;	//The pause showed
	private var mainMenuTopHidden	: boolean	= true;		//The main menu top hidden
	private var audioEnabled		: boolean	= true;		//The audio enabled
	private var mainMissionHidden	: boolean	= true;		//The mission list in the main menu hidden
	private var shopHidden			: boolean	= true;		//The shop hidden
		
	private var starting 			: boolean 	= false;	//The level is in the starting stage
	private var startPowerAct		: boolean	= false;	//A power up is activated in the start
	private var reviveActivated		: boolean	= false;	//A revive is activated
		
	private var canClick			: boolean	= true;		//The player can click
		
	private var mNotification1Used	: boolean	= false;	//The mission notification 1 is used
	private var mNotification2Used 	: boolean 	= false;	//The mission notification 2 is used
	private var mNotification3Used	: boolean 	= false;	//The mission notification 3 is used
	
	//Returns the instance
    public static function Instance()
    {
        if (myInstance == null)
             myInstance = FindObjectOfType(typeof(GUIManagerJS)) as GUIManagerJS;

        return myInstance;
    }
	
	//Called at the biginning of the game
    function Start()
    {
        //Calibrates the myInstance static variable
        instances++;

        if (instances > 1)
            Debug.Log("Warning: There are more than one GUIManager at the level");
        else
            myInstance = this;
    }
	//Called at every frame
	function Update () 
	{
		//If the main GUI is activated and the pause menu is not activated
		if (showMainGUI && !showPause)
		{
			//Update the coin and distance indicator
			DisplayStat(guiTexts[0], LevelManagerJS.Instance().Coins(), 4);
			DisplayStat(guiTexts[1], parseInt(LevelGeneratorJS.Instance().distance), 5);
		}
	}
	//Displays a stat on the screen
	private function DisplayStat(target : TextMesh, data : int, digitNumbers : int)
	{
		//Display a stat, with a length equals to digitNumber
		//Example: if the data is 23, and digitNumbers is 5, It will display 00023
		var dataS : String = "";
		var lz : int = digitNumbers - data.ToString().Length;
		
		while (lz > 0)
		{
			dataS += "0";
			lz--;
		}
		
		//Display the data string
		dataS += data;
		target.text = dataS;
	}
	//Pause the game, and shows the pause menu
	private function Pause()
	{
		//If the player is crashed, do no allow a pause
		if (PlayerManagerJS.Instance().Crashed())
			return;
		
		//Set the game to pause
		PlayerManagerJS.Instance().Pause();
		LevelManagerJS.Instance().PauseGame();
		showPause = true;
		
		EnableDisable(pauseElements[0], true);
		
		//Show the pause GUI elements
		StartCoroutine(FadeScreen(0.4f, 0.7f));
		StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 29, 0.45f, false));
		StartCoroutine(MoveMenu(pauseElements[2].transform, 0, 0, 0.45f, false));
	}
	//Show/Hide main menu
	private function MainMenuArrowTrigger(arrow : Material)
	{
		//If the main menu is not called yet
		if (mainMenuTopHidden)
		{
			//Show the main menu header
			mainMenuTopHidden = false;
			arrow.mainTexture = menuTextures[1];
			StartCoroutine(FadeScreen(0.25f, 0.7f));
			StartCoroutine(MoveMenu(mainMenuElements[1].transform, 0, 22, 0.25f, false));
		}
		//If the mission list is open
		else if (!mainMissionHidden)
		{
			//Close the mission list
			mainMissionHidden = true;
			StartCoroutine(MoveMenu(mainMenuElements[3].transform, 0, -31, 0.4f, false));
		}
		//Else the main menu header is showed, and the mission list is closed
		else
		{
			//Hide the main menu header
			mainMenuTopHidden = true;
			arrow.mainTexture = menuTextures[0];
			StartCoroutine(FadeScreen(0.25f, 0));
			StartCoroutine(MoveMenu(mainMenuElements[1].transform, 0, 32, 0.25f, false));
		}
	}
	//Audio Icon Triggerer
	private function AuidoTriggerer(audioButton : Material)
	{
		//If the shop is showed, return to the caller
		if (!shopHidden)
			return;
		
		//If the audio enabled
		if (audioEnabled)
		{
			//Disable it
			audioEnabled = false;
			audioButton.mainTexture = menuTextures[3];
		}
		//Else it is disabled
		else
		{
			//Enable it
			audioButton.mainTexture = menuTextures[2];
			audioEnabled = true;
		}
	}
	//Show/Hide main menu mission list
	private function TriggerMainMissionList()
	{
		//If the shop is showned, return
		if (!shopHidden)
			return;
		
		//If the main menu mission list is hidden
		if (mainMissionHidden)
		{
			//Show it
			mainMissionHidden = false;
			StartCoroutine(MoveMenu(mainMenuElements[3].transform, 0, -75, 0.4f, false));
		}
		//Else it is not hidden
		else
		{
			//Hide it
			mainMissionHidden = true;
			StartCoroutine(MoveMenu(mainMenuElements[3].transform, 0, -31, 0.4f, false));
		}
	}
	//Show/Hide the shop
	private function TriggerShop()
	{
		//If the shop is hidden
		if (shopHidden)
		{
			//Update the show, and show it
			UpdateShop();
			shopHidden = false;
			StartCoroutine(MoveMenu(mainMenuElements[2].transform, 0, -21.85f, 0.45f, false));
		}
		//If the shop is not hidden
		else
		{
			//Hide it
			shopHidden = true;
			StartCoroutine(MoveMenu(mainMenuElements[2].transform, 0, -94, 0.45f, false));
		}
	}
	//Update shop distplay
	private function UpdateShop()
	{
		//Update coin display
		shopTexts[0].text = SaveManagerJS.GetCoins().ToString();
		//Update extra speed display
		shopTexts[2].text = shopPrices[0].ToString();
		shopTexts[1].text = "x " + SaveManagerJS.GetExtraSpeed();
		//Update shield display
		shopTexts[4].text = shopPrices[1].ToString();
		shopTexts[3].text = "x " + SaveManagerJS.GetShield();
		//Update sonic wave display
		shopTexts[6].text = shopPrices[2].ToString();
		shopTexts[5].text = "x " + SaveManagerJS.GetSonicWave();
		//Update revive display
		shopTexts[8].text = shopPrices[3].ToString();
		shopTexts[7].text = "x " + SaveManagerJS.GetRevive();
	}
	//Buy a speed power up, if has the money
	private function BuySpeed()
	{
		//If the playe can afford the power up
		if (SaveManagerJS.GetCoins() > shopPrices[0])
		{
			//Remove the coins from the account, and add the power up to the account
			SaveManagerJS.SetCoins(SaveManagerJS.GetCoins() - shopPrices[0]);
			SaveManagerJS.ModifyExtraSpeedBy(1);
			//Update the shop, and notify the mission manager
			UpdateShop();
			MissionManagerJS.Instance().ShopEvent("Extra Speed");
		}
	}
	//Buy a shield power up, if has the money
	private function BuyShield()
	{
		//If the playe can afford the power up
		if (SaveManagerJS.GetCoins() > shopPrices[1])
		{
			//Remove the coins from the account, and add the power up to the account
			SaveManagerJS.SetCoins(SaveManagerJS.GetCoins() - shopPrices[1]);
			SaveManagerJS.ModifyShieldBy(1);
			//Update the shop, and notify the mission manager
			UpdateShop();
			MissionManagerJS.Instance().ShopEvent("Shield");
		}
	}
	//Buy a sonic wave power up, if has the money
	private function BuySonicWave()
	{
		//If the playe can afford the power up
		if (SaveManagerJS.GetCoins() > shopPrices[2])
		{
			//Remove the coins from the account, and add the power up to the account
			SaveManagerJS.SetCoins(SaveManagerJS.GetCoins() - shopPrices[2]);
			SaveManagerJS.ModifySonicWaveBy(1);
			//Update the shop, and notify the mission manager
			UpdateShop();
			MissionManagerJS.Instance().ShopEvent("Sonic Wave");
		}
	}	
	//Buy a revive power up, if has the money
	private function BuyRevive()
	{
		//If the playe can afford the power up
		if (SaveManagerJS.GetCoins() > shopPrices[3])
		{
			//Remove the coins from the account, and add the power up to the account
			SaveManagerJS.SetCoins(SaveManagerJS.GetCoins() - shopPrices[3]);
			SaveManagerJS.ModifyReviveBy(1);
			//Update the shop, and notify the mission manager
			UpdateShop();
			MissionManagerJS.Instance().ShopEvent("Revive");
		}
	}	
	//Activates the extra speed power up at start
	private function ActivateSpeed()
	{
		//If this is the first activated power up, and the game is not paused
		if (startPowerAct || showPause)
			return;
		
		//Set startPowerAct true, activate the power up, and remove it from the account
		startPowerAct = true;
		PlayerManagerJS.Instance().ExtraSpeed();
		SaveManagerJS.ModifyExtraSpeedBy(-1);
	}
	//Activates the shield power up at start
	private function ActivateShield()
	{
		//If this is the first activated power up, and the game is not paused
		if (startPowerAct || showPause)
			return;
		
		//Set startPowerAct true, activate the power up, and remove it from the account
		startPowerAct = true;
		PlayerManagerJS.Instance().RaiseShield();
		SaveManagerJS.ModifyShieldBy(-1);
	}
	//Activates the sonic wave power up at start
	private function ActivateSonicWave()
	{
		//If this is the first activated power up, and the game is not paused
		if (startPowerAct || showPause)
			return;
		
		//Set startPowerAct true, activate the power up, and remove it from the account
		startPowerAct = true;
		LevelGeneratorJS.Instance().powerUpMain.SetSonicBlastFirst();
		SaveManagerJS.ModifySonicWaveBy(-1);
	}
	//Activates the revive power up at sinking
	private function ActivateRevive()
	{
		//Set reviveActivated to true
		reviveActivated = true;
	}
	//Starts the level
	private function StartToPlay()
	{
		//If the main mission list and the shop is hidden, and the level is not starting
		if (!mainMissionHidden || !shopHidden || starting)
			return;
		
		starting = true;
		
		//If the player owns a revive power up, display it's heart
		if (SaveManagerJS.GetRevive() > 0)
			EnableDisable(mainGUIElements[2], true);
		
		//If the main menu header is not hidden
		if (!mainMenuTopHidden)
		{
			//Hide and disable it
			StartCoroutine(FadeScreen(0.25f, 0));
			StartCoroutine(MoveMenu(mainMenuElements[1].transform, 0, 32, 0.25f, true));
		}
		//If it is hidden
		else
		{
			//Disable it
			EnableDisable(mainMenuElements[1], false);
		}
		
		//Start the level
		LevelManagerJS.Instance().StartLevel();
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
	//Moves a selected menu item to a specified space with speed
	private function MoveMenu(menuTransform : Transform, endPosX : float, endPosY : float, time : float, hide : boolean)
	{
		canClick = false;
		
		//Move the menu to the designated position under time
		var i : float = 0.0f;
		var rate : float = 1.0f / time;
		
		var startPos : Vector3 = menuTransform.localPosition;
		var endPos : Vector3 = Vector3(endPosX, endPosY, menuTransform.localPosition.z);	
		
		while (i < 1.0) 
		{
			i += Time.deltaTime * rate;
			menuTransform.localPosition = Vector3.Lerp(startPos, endPos, i);
			yield;
		}
		
		//If it is marked to hide, hide it
		if (hide)
			EnableDisable(menuTransform.gameObject, false);
		
		canClick = true;
	}
	//Show start power up selection
	private function MovePowerUpSelection(speed : boolean, shield : boolean, sonic : boolean)
	{
		//Wait 0.5 second, and show the activation menu
		yield WaitForSeconds(0.5f);
		
		//Show the power ups
		if (speed)
			StartCoroutine(MoveMenu(startPowerUps[0].transform, startPowerUps[0].transform.localPosition.x, -29.5f, 0.4f, false));
		if (shield)
			StartCoroutine(MoveMenu(startPowerUps[1].transform, startPowerUps[1].transform.localPosition.x, -29.5f, 0.4f, false));
		if (sonic)
			StartCoroutine(MoveMenu(startPowerUps[2].transform, startPowerUps[2].transform.localPosition.x, -29.5f, 0.4f, false));
		
		//Wait for 3 seconds to hide the activation menu
		var waited : double = 0;
		while (waited <= 3)
		{
			//If the game is not paused, cound
			if (!showPause)
				waited += Time.deltaTime;
			
			//If a power is activated
			if(startPowerAct)
			{
				//Wait 0.1 seconds, and hide the menu
				yield WaitForSeconds(0.1f);
				
				if (speed)
					StartCoroutine(MoveMenu(startPowerUps[0].transform, startPowerUps[0].transform.localPosition.x, -45, 0.4f, true));
				if (shield)
					StartCoroutine(MoveMenu(startPowerUps[1].transform, startPowerUps[1].transform.localPosition.x, -45, 0.4f, true));
				if (sonic)
					StartCoroutine(MoveMenu(startPowerUps[2].transform, startPowerUps[2].transform.localPosition.x, -45, 0.4f, true));			
				
				//Wait for 0.5 seconds, set startPowerAct to false, and return
				var waited2 : double = 0;
				while (waited2 <= 0.5f)
				{
					if (!showPause)
						waited2 += Time.deltaTime;
				}
				
				startPowerAct = false;
				
				StopCoroutine("MovePowerUpSelection");
			}

			yield;
		}
		
		//If no power up is activated for 3 seconds, hide the menu
		if (speed)
			StartCoroutine(MoveMenu(startPowerUps[0].transform, startPowerUps[0].transform.localPosition.x, -45, 0.4f, true));
		if (shield)
			StartCoroutine(MoveMenu(startPowerUps[1].transform, startPowerUps[1].transform.localPosition.x, -45, 0.4f, true));
		if (sonic)
			StartCoroutine(MoveMenu(startPowerUps[2].transform, startPowerUps[2].transform.localPosition.x, -45, 0.4f, true));
	}
	//Set level for current resolution
	function SetLevelResolution()
	{
		var left : GameObject[] = [mainGUIElements[3], mainMenuElements[6]];
		var right : GameObject[] = [mainGUIElements[4], mainMenuElements[7]];
		var scale : GameObject[] = [mainMenuElements[5], pauseElements[3], LevelGeneratorJS.Instance().background.gameObject, LevelGeneratorJS.Instance().sand.gameObject, overlay.gameObject];
		
		ResolutionManagerJS.Instance().SetResolutionSetting(scale, shopElements, left, right, LevelGeneratorJS.Instance().hangar);
	}
	//Button Down event
	function ButtonDown(button : Transform)
	{
		//If the player cant click, return to caller
		if (!canClick)
			return;
		
		//Scale the buttom down
		var scale : Vector3 = button.transform.localScale;
		button.transform.localScale = scale * 0.8f;
	}
	//Button Up event
	function ButtonUp(button : Transform)
	{
		//If the playe cant click, return to caller
		if (!canClick)
			return;
		
		//Scale the menu up to it's original position
		var scale : Vector3 = button.transform.localScale;
		button.transform.localScale = scale * 1.25f;
		
		//Activate the correct function
		switch (button.name)
		{
			case "PauseShowButton":
				Pause();
				break;
			
			case "Resume":
				StartCoroutine("Resume");
				break;
			
			case "Retry":
				StartCoroutine("Restart");
				break;
			
			case "Quit":
				StartCoroutine("QuitToMain");
				break;
			
			case "MainArrow":
				MainMenuArrowTrigger(button.renderer.material);
				break;
			
			case "AudioEnabler":
				AuidoTriggerer(button.renderer.material);
				break;
			
			case "Missions":
				TriggerMainMissionList();
				break;
			
			case "Shop":
			case "Back":
				TriggerShop();
				break;
			
			case "PlayTriggerer":
				StartToPlay();
				break;
			
			case "BuySpeed":
				BuySpeed();
				break;
			
			case "BuyShield":
				BuyShield();
				break;
			
			case "BuyRevive":
				BuyRevive();
				break;
			
			case "BuySonicWave":
				BuySonicWave();
				break;
			
			case "ExtraSpeedActivation":
				ActivateSpeed();
				break;
			
			case "ShieldActivation":
				ActivateShield();
				break;
			
			case "SonicWaveActivation":
				ActivateSonicWave();
				break;
			
			case "ReviveActivation":
				ActivateRevive();
				break;
		}
	}
	//Resume from pause, and dismiss the pause menu
	function Resume()
	{
		//Hide the pause elements
		StartCoroutine(FadeScreen(0.4f, 0));
		StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 59, 0.45f, false));
		StartCoroutine(MoveMenu(pauseElements[2].transform, 0, -60, 0.45f, false));
		
		//Wait for the pause elements to move out of the screen
		yield WaitForSeconds(0.6f);
		showPause = false;
		
		//Resume the game
		PlayerManagerJS.Instance().Resume();
		LevelManagerJS.Instance().ResumeGame();
	}
	//Hides the menu, and restarts the game
	function Restart()
	{
		//Fade the screen
		StartCoroutine(FadeScreen(0.4f, 1.0f));
		//If the restart is called from the pause menu
		if (showPause)
		{
			//Hide the pause menu
			showPause = false;
			StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 59, 0.45f, false));
			StartCoroutine(MoveMenu(pauseElements[2].transform, 0, -60, 0.45f, false));
		}
		//Else, it is called from the finish menu
		else
		{
			//Hide the finish menu
			StartCoroutine(MoveMenu(finishMenu.transform, 0, -60, 0.55f, false));
		}
		
		//Wait for the caller menu to disappear, and fade the screen
		yield WaitForSeconds(0.5f);
		StartCoroutine(FadeScreen(0.4f, 0.0f));
		
		//Disable the logo in the hanger, and restart the game
		EnableDisable(mainMenuElements[0], false);
		LevelManagerJS.Instance().Restart();
	}
	//Returns to the main menu
	function QuitToMain()
	{
		//Set starting to false, and fade the screen
		starting = false;
		StartCoroutine(FadeScreen(0.4f, 1.0f));
		
		//If the quit is called from the pause menu
		if (showPause)
		{
			//Hide the pause menu
			showPause = false;
			StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 59, 0.45f, false));
			StartCoroutine(MoveMenu(pauseElements[2].transform, 0, -60, 0.45f, false));
		}
		//Else, it is called from the finish menu
		else
		{
			//Hide the finish menu
			StartCoroutine(MoveMenu(finishMenu.transform, 0, -60, 0.55f, false));
		}
		
		//Disable the hearts
		EnableDisable(mainGUIElements[1], false);
		EnableDisable(mainGUIElements[2], false);
		
		//Wait for the caller menu to disappear, and fade the screen
		yield WaitForSeconds(0.5f);
		StartCoroutine(FadeScreen(0.4f, 0.0f));
		
		//Active the logo at the hangar, and go to the main menu
		EnableDisable(mainMenuElements[0], true);
		LevelManagerJS.Instance().QuitToMain();
	}
	//Shows the end menu after a crash
	function ShowEnd()
	{
		//Save the mission and activate the finish menu
		MissionManagerJS.Instance().Save();
		EnableDisable(finishMenu, true);
		
		//Get the current coin and distance data
		var currentDist : int = parseInt(LevelGeneratorJS.Instance().distance);
		var currentCoins : int = LevelManagerJS.Instance().Coins();
		
		//Apply the data to the finish menu
		finishTexts[0].text = currentDist + "M";
		finishTexts[1].text = currentCoins.ToString();
		
		//If the current distance is greater than the best distance
		if (currentDist > SaveManagerJS.GetBestDistance())
			//Set the current distance as the best distance
			SaveManagerJS.SetBestDistance(currentDist);
		
		//Add the collected coins to the account
		SaveManagerJS.SetCoins(SaveManagerJS.GetCoins() + currentCoins);
		
		//Show the finish menu
		StartCoroutine(FadeScreen(0.4f, 0.7f));
		StartCoroutine(MoveMenu(finishMenu.transform, 0, 67, 0.55f, false));
	}
	//Shows the available power ups at start
	function ShowStartPowerUps()
	{
		//Check the available power ups
		var hasSpeed : boolean = SaveManagerJS.GetExtraSpeed() > 0;
		var hasShield : boolean = SaveManagerJS.GetShield() > 0;
		var hasSonicWave : boolean = SaveManagerJS.GetSonicWave() > 0;
		
		//Get the number of the availabe power ups
		var numberOfPowerUps : int = 0;
		
		if (hasSpeed)
			numberOfPowerUps++;
		if (hasShield)
			numberOfPowerUps++;
		if (hasSonicWave)
			numberOfPowerUps++;
		
		//If there is only 1 power up availabe
		if (numberOfPowerUps == 1)
		{
			//If the speed is available
			if (hasSpeed)
			{
				//Set it's location to the middle, and activate it's buttom
				startPowerUps[0].transform.localPosition = new Vector3(0, -40, 0);
				EnableDisable(startPowerUps[0], true);
			}
			//If the shield is available
			else if (hasShield)
			{
				//Set it's location to the middle, and activate it's buttom
				startPowerUps[1].transform.localPosition = new Vector3(0, -40, 0);
				EnableDisable(startPowerUps[1], true);
			}
			//If the sonic wave is available
			else
			{
				//Set it's location to the middle, and activate it's buttom
				startPowerUps[2].transform.localPosition = new Vector3(0, -40, 0);
				EnableDisable(startPowerUps[2], true);
			}
		}
		//If there are two available power up
		else if (numberOfPowerUps == 2)
		{
			//If the speed is available
			if (hasSpeed)
			{
				//Set it's position to the left
				startPowerUps[0].transform.localPosition = new Vector3(-7.5f, -40, 0);
				EnableDisable(startPowerUps[0], true);
			}
			//If the shield is active
			if (hasShield)
			{
				//If the speed is active as well
				if (hasSpeed)
					//Set it's position to the right
					startPowerUps[1].transform.localPosition = new Vector3(7.5f, -40, 0);
				//If the speed power up is not available
				else
					//Set the shield position to the left
					startPowerUps[1].transform.localPosition = new Vector3(-7.5f, -40, 0);
				
				//Active the shield power up buttom
				EnableDisable(startPowerUps[1], true);
			}
			//If the sonic wave is active
			if (hasSonicWave)
			{
				//Set it's position to the right, and activate it's buttom
				startPowerUps[2].transform.localPosition = new Vector3(7.5f, -40, 0);
				EnableDisable(startPowerUps[2], true);
			}
		}
		//If every power up is available
		else if (numberOfPowerUps == 3)
		{
			//Set their position, and activate them
			startPowerUps[0].transform.localPosition = new Vector3(-15, -40, 0);
			EnableDisable(startPowerUps[0], true);
			startPowerUps[1].transform.localPosition = new Vector3(0, -40, 0);
			EnableDisable(startPowerUps[1], true);
			startPowerUps[2].transform.localPosition = new Vector3(15, -40, 0);
			EnableDisable(startPowerUps[2], true);
		}
		
		//Show the power up buttoms
		StartCoroutine (MovePowerUpSelection(hasSpeed, hasShield, hasSonicWave));
	}
	//Activates the main GUI
	function ActivateMainGUI()
	{
		showMainGUI = true;
		EnableDisable(mainGUIElements[0], true);
	}
	//Deactivates the main GUI
	function DeactivateMainGUI()
	{
		showMainGUI = false;
		EnableDisable(mainGUIElements[0], false);
	}
	//Activated the main menu
	function ActivateMainMenu()
	{
		mainMenuElements[4].renderer.material.mainTexture = menuTextures[0];
		EnableDisable(mainMenuElements[1], true);
	}
	//Deactivated the main menu
	function DeactivateMainMenu()
	{
		EnableDisable(mainMenuElements[1], false);
	}
	//Revive picked up
	function RevivePicked()
	{
		//Activate the revive heart
		EnableDisable(mainGUIElements[1], true);
	}
	//Disables a revive gui icon
	function DisableReviveGUI(num : int)
	{
		//Check if num is 1 or 0 to validate it
		if (num == 0 || num == 1)
			//Deactive the heart
			EnableDisable(mainGUIElements[num+1], false);
	}
	//Updates the best distance board at the main menu
	function UpdateBestDistance()
	{
		bestDist.text = SaveManagerJS.GetBestDistance() + "M";
	}
	//Updates mission text
	function UpdateMissionTexts(text1 : String, text2 : String, text3 : String)
	{
		//Set mission 1 GUI text size
		if (text1.Length < 26)
		{
			missionTexts[0].fontSize = 24;
			missionTexts[1].fontSize = 24;
			missionTexts[2].fontSize = 24;
		}
		else if (text1.Length < 31)
		{
			missionTexts[0].fontSize = 21;
			missionTexts[1].fontSize = 21;
			missionTexts[2].fontSize = 21;
		}
		else
		{
			missionTexts[0].fontSize = 17;
			missionTexts[1].fontSize = 17;
			missionTexts[2].fontSize = 17;
		}
		
		//Apply mission 1 text
		missionTexts[0].text = text1;
		missionTexts[1].text = text1;
		missionTexts[2].text = text1;
		
		//Set mission 2 GUI text size
		if (text2.Length < 26)
		{
			missionTexts[3].fontSize = 24;
			missionTexts[4].fontSize = 24;
			missionTexts[5].fontSize = 24;
		}
		else if (text2.Length < 31)
		{
			missionTexts[3].fontSize = 21;
			missionTexts[4].fontSize = 21;
			missionTexts[5].fontSize = 21;
		}
		else
		{
			missionTexts[3].fontSize = 19;
			missionTexts[4].fontSize = 19;
			missionTexts[5].fontSize = 19;
		}
		
		//Apply mission 2 text
		missionTexts[3].text = text2;
		missionTexts[4].text = text2;
		missionTexts[5].text = text2;
		
		//Set mission 3 GUI text size
		if (text3.Length < 26)
		{
			missionTexts[6].fontSize = 24;
			missionTexts[7].fontSize = 24;
			missionTexts[8].fontSize = 24;
		}
		else if (text3.Length < 31)
		{
			missionTexts[6].fontSize = 21;
			missionTexts[7].fontSize = 21;
			missionTexts[8].fontSize = 21;
		}
		else
		{
			missionTexts[6].fontSize = 19;
			missionTexts[7].fontSize = 19;
			missionTexts[8].fontSize = 19;
		}
		
		//Apply mission 3 text		
		missionTexts[6].text = text3;
		missionTexts[7].text = text3;
		missionTexts[8].text = text3;
	}
	//Update mission i status display
	function UpdateMissionStatus(i : int, a : int, b : int)
	{
		//Check mission number
		switch (i)
		{
			//Update mission 1
			case 0:
				missionStatus[0].text = a + "/" + b;
				missionStatus[1].text = a + "/" + b;
				missionStatus[2].text = a + "/" + b;
				break;
			
			//Update mission 2
			case 1:
				missionStatus[3].text = a + "/" + b;
				missionStatus[4].text = a + "/" + b;
				missionStatus[5].text = a + "/" + b;
				break;
			
			//Update mission 3
			case 2:
				missionStatus[6].text = a + "/" + b;
				missionStatus[7].text = a + "/" + b;
				missionStatus[8].text = a + "/" + b;
				break;
		}
	}
	//Shows revive power up
	function ShowRevive()
	{
		//Show the revive button
		StartCoroutine(MoveMenu(startPowerUps[3].transform, startPowerUps[3].transform.localPosition.x, -29.5f, 0.4f, false));
		
		//Variable to detect activation
        var activated : boolean = false;

		
		//Wait for 3 seconds
		var waited : double = 0;
		while (waited <= 3)
		{
			waited += Time.deltaTime;
			
			//If the revive is activated during the 3 seconds
			if(reviveActivated)
			{
				//Wait 0.1 seconds, and hide the revive button
				yield WaitForSeconds(0.2f);
				StartCoroutine(MoveMenu(startPowerUps[3].transform, startPowerUps[3].transform.localPosition.x, -45, 0.4f, false));
				
				//Revive the player, and reset reviveActivated
				yield WaitForSeconds(0.5f);
				LevelManagerJS.Instance().Revive();
				reviveActivated = false;
				activated = true;
			}

			yield;
		}
		
		//If the revive was not activated, hide the revive button
		if (!activated)
        {
			StartCoroutine(MoveMenu(startPowerUps[3].transform, startPowerUps[3].transform.localPosition.x, -45, 0.4f, false));	
			yield WaitForSeconds(0.5f);
			
			//show the finish menu
			ShowEnd();
		}
	}
	//Shows mission complete notification
	function ShowMissionComplete(text : String)
	{
		//Declare the varialbes
		var go : GameObject = null;
		var t : TextMesh = null;
		
		var nIndex : int = 0;
		var yPosTarget : float = 0;
		
		//If notification 1 is not used
		if (!mNotification1Used)
		{
			//Use notification 1
			go = missionNotification[0];
			t = go.transform.FindChild("Text").GetComponent(TextMesh);
			
			//Set varialbes to notification 1
			mNotification1Used = true;
			nIndex = 1;
			yPosTarget = 32;
		}
		//If notification 1 is used, but notification 2 is not
		else if (mNotification1Used && !mNotification2Used)
		{
			//Use notification 2
			go = missionNotification[1];
			t = go.transform.FindChild("Text").GetComponent(TextMesh);
			
			//Set varialbes to notification 2
			yPosTarget = 26;
			mNotification2Used = true;
			nIndex = 2;
		}
		//If notification 1 and 2is used, but notification 3 is not
		else if (mNotification1Used && mNotification2Used && !mNotification3Used)
		{
			//Use notification 3
			go = missionNotification[2];
			t = go.transform.FindChild("Text").GetComponent(TextMesh);
			
			//Set varialbes to notification 3
			yPosTarget = 20;
			mNotification3Used = true;
			nIndex = 3;
		}
		//If every notification is used
		else
		{
			//Return
			return;
		}
		
		//Set font size
		if (text.Length < 26)
			t.fontSize = 24;
		else if (text.Length < 31)
			t.fontSize = 21;
		else if (text.Length < 36)
			t.fontSize = 19;
		else
			t.fontSize = 14;
		
		//Apply the text
		t.text = text;
		
		//Show the notification
		StartCoroutine(MoveMenu(go.transform, 0, yPosTarget, 0.4f, false));
		
		//Wait for 2 seconds
		var waited : double = 0;
		while (waited <= 2)
		{
			//If the game is not paused
			if (!showPause)
				//Increase waited time
				waited += Time.deltaTime;
			
			//Wait for the end of the frame
			yield;
		}
		
		//Hide the notification
		StartCoroutine(MoveMenu(go.transform, 0, 38.5f, 0.4f, false));
		
		//Wait for 0.5 seconds
		waited = 0;
		while (waited <= 0.5f)
		{
			//If the game is not paused
			if (!showPause)
				//Increase waited time
				waited += Time.deltaTime;
			
			//Wait for the end of the frame
			yield;
		}
		
		//Set the current notification to false
		if (nIndex == 1)
			mNotification1Used = false;
		else if (nIndex == 2)
			mNotification2Used = false;
		else if (nIndex == 3)
			mNotification3Used = false;
	}
	//Fade screen opacity
	function FadeScreen(time : float, to : float)
	{
		//Set the screen fade's color to end in time
		var i : float = 0.0f;
		var rate : float = 1.0f / time;
		
		var start : Color = overlay.material.color;
		var end : Color = new Color(start.r, start.g, start.b, to);
		
		while (i < 1.0) 
		{
			i += Time.deltaTime * rate;
			overlay.material.color = Color.Lerp(start, end, i);
			yield;
		}
	}
}