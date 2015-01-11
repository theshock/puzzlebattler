using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour 
{
    public Renderer overlay;					//The overlay between the game and menus

    public TextMesh bestDist; 					//Displays the best distance in the hangar
    public Texture2D[] menuTextures;			//The main menu textures (arrow, audio)
    public GameObject[] mainMenuElements;       //The main menu elements

    public TextMesh[] shopTexts;				//Contains the shop texts
    public int[] shopPrices;					//Contains the shop prices
	public GameObject[] shopElements;			//Contains the shop elements

    public GameObject[] startPowerUps;			//The power up activation buttoms
    public TextMesh[] guiTexts;                 //The main GUI coin and distance indicator
    public GameObject[] mainGUIElements;        //The main GUI and hearts

    public GameObject[] pauseElements;          //The pause menu elements

    public GameObject finishMenu;				//The finish menu to show after a crash
    public TextMesh[] finishTexts;              //The finish menu coin and distance indicators

    public GameObject[] missionNotification;	//The mission complete notifications

    public TextMesh[] missionTexts;				//The mission GUI texts
    public TextMesh[] missionStatus;			//The mission status indicator texts

    static GUIManager myInstance;
    static int instances = 0;

	bool showMainGUI		= false;			//The main GUI showed
	bool showPause			= false;			//The pause showed
	bool mainMenuTopHidden	= true;				//The main menu top hidden
	bool audioEnabled		= true;				//The audio enabled
	bool mainMissionHidden	= true;				//The mission list in the main menu hidden
	bool shopHidden			= true;				//The shop hidden
	
	bool starting 			= false;			//The level is in the starting stage
	bool startPowerAct		= false;			//A power up is activated in the start
	bool reviveActivated	= false;			//A revive is activated
	
	bool canClick			= true;				//The player can click
	
	bool mNotification1Used	= false;			//The mission notification 1 is used
	bool mNotification2Used	= false;			//The mission notification 2 is used
	bool mNotification3Used	= false;			//The mission notification 3 is used

    //Retursn the instance
    public static GUIManager Instance
    {
        get
        {
            if (myInstance == null)
                myInstance = FindObjectOfType(typeof(GUIManager)) as GUIManager;

            return myInstance;
        }
    }

    //Called at the biginning of the game
    void Start()
    {
        //Calibrates the myInstance static variable
        instances++;

        if (instances > 1)
            Debug.Log("Warning: There are more than one GUIManager at the level");
        else
            myInstance = this;
    }
	//Called at every frame
	void Update()
	{
		//If the main GUI is activated and the pause menu is not activated
		if (showMainGUI && !showPause)
		{
			//Update the coin and distance indicator
            DisplayStat(guiTexts[0], LevelManager.Instance.Coins(), 4);
            DisplayStat(guiTexts[1], (int)LevelGenerator.Instance.distance, 5);
		}
	}
	//Displays a stat on the screen
	void DisplayStat(TextMesh target, int data, int digitNumbers)
	{
		//Display a stat, with a length equals to digitNumber
		//Example: if the data is 23, and digitNumbers is 5, It will display 00023
		string dataS = "";
		int lz = digitNumbers - data.ToString().Length;
		
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
	void Pause()
	{
		//If the player is crashed, do no allow a pause
        if (PlayerManager.Instance.Crashed())
			return;
		
		//Set the game to pause
        PlayerManager.Instance.Pause();
        LevelManager.Instance.PauseGame();
		showPause = true;

        EnableDisable(pauseElements[0], true);

        //Show the pause GUI elements
		StartCoroutine(FadeScreen(0.4f, 0.7f));
        StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 29, 0.45f, false));
        StartCoroutine(MoveMenu(pauseElements[2].transform, 0, 0, 0.45f, false));
	}
	//Show/Hide main menu
	void MainMenuArrowTrigger(Material arrow)
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
	void AuidoTriggerer(Material audioButton)
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
	void TriggerMainMissionList()
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
	void TriggerShop()
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
	void UpdateShop()
	{
		//Update coin display
		shopTexts[0].text = SaveManager.GetCoins().ToString();
		//Update extra speed display
		shopTexts[2].text = shopPrices[0].ToString();
		shopTexts[1].text = "x " + SaveManager.GetExtraSpeed();
		//Update shield display
		shopTexts[4].text = shopPrices[1].ToString();
		shopTexts[3].text = "x " + SaveManager.GetShield();
		//Update sonic wave display
		shopTexts[6].text = shopPrices[2].ToString();
		shopTexts[5].text = "x " + SaveManager.GetSonicWave();
		//Update revive display
		shopTexts[8].text = shopPrices[3].ToString();
		shopTexts[7].text = "x " + SaveManager.GetRevive();
	}
	//Buy a speed power up, if has the money
	void BuySpeed()
	{
		//If the playe can afford the power up
		if (SaveManager.GetCoins() >= shopPrices[0])
		{
			//Remove the coins from the account, and add the power up to the account
			SaveManager.SetCoins(SaveManager.GetCoins() - shopPrices[0]);
            SaveManager.ModifyExtraSpeedBy(1);
			//Update the shop, and notify the mission manager
			UpdateShop();
            MissionManager.Instance.ShopEvent("Extra Speed");
		}
	}
	//Buy a shield power up, if has the money
	void BuyShield()
	{
		//If the playe can afford the power up
		if (SaveManager.GetCoins() >= shopPrices[1])
		{
			//Remove the coins from the account, and add the power up to the account
			SaveManager.SetCoins(SaveManager.GetCoins() - shopPrices[1]);
            SaveManager.ModifyShieldBy(1);
			//Update the shop, and notify the mission manager
			UpdateShop();
            MissionManager.Instance.ShopEvent("Shield");
		}
	}
	//Buy a sonic wave power up, if has the money
	void BuySonicWave()
	{
		//If the playe can afford the power up
		if (SaveManager.GetCoins() >= shopPrices[2])
		{
			//Remove the coins from the account, and add the power up to the account
			SaveManager.SetCoins(SaveManager.GetCoins() - shopPrices[2]);
            SaveManager.ModifySonicWaveBy(1);
			//Update the shop, and notify the mission manager
			UpdateShop();
            MissionManager.Instance.ShopEvent("Sonic Wave");
		}
	}	
	//Buy a revive power up, if has the money
	void BuyRevive()
	{
		//If the playe can afford the power up
		if (SaveManager.GetCoins() >= shopPrices[3])
		{
			//Remove the coins from the account, and add the power up to the account
			SaveManager.SetCoins(SaveManager.GetCoins() - shopPrices[3]);
            SaveManager.ModifyReviveBy(1);
			//Update the shop, and notify the mission manager
			UpdateShop();
            MissionManager.Instance.ShopEvent("Revive");
		}
	}	
	//Activates the extra speed power up at start
	void ActivateSpeed()
	{
		//If this is the first activated power up, and the game is not paused
		if (startPowerAct || showPause)
			return;
		
		//Set startPowerAct true, activate the power up, and remove it from the account
		startPowerAct = true;
        PlayerManager.Instance.ExtraSpeed();
        SaveManager.ModifyExtraSpeedBy(-1);
	}
	//Activates the shield power up at start
	void ActivateShield()
	{
		//If this is the first activated power up, and the game is not paused
		if (startPowerAct || showPause)
			return;
		
		//Set startPowerAct true, activate the power up, and remove it from the account
		startPowerAct = true;
        PlayerManager.Instance.RaiseShield();
        SaveManager.ModifyShieldBy(-1);
	}
	//Activates the sonic wave power up at start
	void ActivateSonicWave()
	{
		//If this is the first activated power up, and the game is not paused
		if (startPowerAct || showPause)
			return;
		
		//Set startPowerAct true, activate the power up, and remove it from the account
		startPowerAct = true;
        LevelGenerator.Instance.powerUpMain.SetSonicBlastFirst();
        SaveManager.ModifySonicWaveBy(-1);
	}
	//Activates the revive power up at sinking
	void ActivateRevive()
	{
		//Set reviveActivated to true
		reviveActivated = true;
	}
	//Starts the level
	void StartToPlay()
	{
		//If the main mission list and the shop is hidden, and the level is not starting
		if (!mainMissionHidden || !shopHidden || starting)
			return;
		
		starting = true;
		
		//If the player owns a revive power up, display it's heart
        if (SaveManager.GetRevive() > 0)
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
        LevelManager.Instance.StartLevel();
	}
    //Enables/disables the object with childs based on platform
    void EnableDisable(GameObject what, bool activate)
    {
        #if UNITY_3_5
        what.SetActiveRecursively(activate);
        #else
        what.SetActive(activate);
        #endif
    }
    //Resume from pause, and dismiss the pause menu
    IEnumerator Resume()
    {
        //Hide the pause elements
        StartCoroutine(FadeScreen(0.4f, 0));
        StartCoroutine(MoveMenu(pauseElements[1].transform, 0, 59, 0.45f, false));
        StartCoroutine(MoveMenu(pauseElements[2].transform, 0, -60, 0.45f, false));

        //Wait for the pause elements to move out of the screen
        yield return new WaitForSeconds(0.6f);
        showPause = false;

        //Resume the game
        PlayerManager.Instance.Resume();
        LevelManager.Instance.ResumeGame();
    }
    //Hides the menu, and restarts the game
    IEnumerator Restart()
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
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeScreen(0.4f, 0.0f));

        //Disable the logo in the hangar, and restart the game
        EnableDisable(mainMenuElements[0], false);
        LevelManager.Instance.Restart();
    }
    //Returns to the main menu
    IEnumerator QuitToMain()
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
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeScreen(0.4f, 0.0f));

        //Active the logo at the hangar, and go to the main menu
        EnableDisable(mainMenuElements[0], true);
        LevelManager.Instance.QuitToMain();
    }
	//Moves a selected menu item to a specified space with speed
	IEnumerator MoveMenu(Transform menuTransform, float endPosX, float endPosY, float time, bool hide)
	{
		canClick = false;
		
		//Move the menu to the designated position under time
		float i = 0.0f;
		float rate = 1.0f / time;
		
		Vector3 startPos = menuTransform.localPosition;
		Vector3 endPos = new Vector3(endPosX, endPosY, menuTransform.localPosition.z);	
		
		while (i < 1.0) 
		{
			i += Time.deltaTime * rate;
			menuTransform.localPosition = Vector3.Lerp(startPos, endPos, i);
			yield return 0;
		}
		
		//If it is marked to hide, hide it
        if (hide)
            EnableDisable(menuTransform.gameObject, false);
		
		canClick = true;
	}
	//Show start power up selection
	IEnumerator MovePowerUpSelection(bool speed, bool shield, bool sonic)
	{
		//Wait 0.5 second, and show the activation menu
		yield return new WaitForSeconds(0.5f);
		
		//Show the power ups
        if (speed)
		    StartCoroutine(MoveMenu(startPowerUps[0].transform, startPowerUps[0].transform.localPosition.x, -29.5f, 0.4f, false));
		if (shield)
            StartCoroutine(MoveMenu(startPowerUps[1].transform, startPowerUps[1].transform.localPosition.x, -29.5f, 0.4f, false));
        if (sonic)
           StartCoroutine(MoveMenu(startPowerUps[2].transform, startPowerUps[2].transform.localPosition.x, -29.5f, 0.4f, false));
		
		//Wait for 3 seconds to hide the activation menu
		double waited = 0;
		while (waited <= 3)
		{
			//If the game is not paused, cound
			if (!showPause)
				waited += Time.deltaTime;
			
			//If a power is activated
			if(startPowerAct)
			{
				//Wait 0.1 seconds, and hide the menu
				yield return new WaitForSeconds(0.1f);

                if (speed)
				    StartCoroutine(MoveMenu(startPowerUps[0].transform, startPowerUps[0].transform.localPosition.x, -45, 0.4f, true));
                if (shield)
                    StartCoroutine(MoveMenu(startPowerUps[1].transform, startPowerUps[1].transform.localPosition.x, -45, 0.4f, true));
                if (sonic)
                    StartCoroutine(MoveMenu(startPowerUps[2].transform, startPowerUps[2].transform.localPosition.x, -45, 0.4f, true));			
				
				//Wait for 0.5 seconds, set startPowerAct to false, and return
				double waited2 = 0;
				while (waited2 <= 0.5f)
				{
					if (!showPause)
						waited2 += Time.deltaTime;
				}
				
				startPowerAct = false;

                StopCoroutine("MovePowerUpSelection");
			}

			yield return 0;
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
	public void SetLevelResolution()
	{
		GameObject[] left = {mainGUIElements[3], mainMenuElements[6]};
		GameObject[] right = {mainGUIElements[4], mainMenuElements[7]};
		GameObject[] scale = {mainMenuElements[5], pauseElements[3], LevelGenerator.Instance.background.gameObject, LevelGenerator.Instance.sand.gameObject, overlay.gameObject};
		
		ResolutionManager.Instance.SetResolutionSetting(scale, shopElements, left, right, LevelGenerator.Instance.hangar);
	}
	//Button Down event
	public void ButtonDown(Transform button)
	{
		//If the player cant click, return to caller
		if (!canClick)
			return;
		
		//Scale the buttom down
		Vector3 scale = button.transform.localScale;
		button.transform.localScale = scale * 0.8f;
	}
	//Button Up event
	public void ButtonUp(Transform button)
	{
		//If the playe cant click, return to caller
		if (!canClick)
			return;
		
		//Scale the menu up to it's original position
		Vector3 scale = button.transform.localScale;
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
	//Shows the end menu after a crash
	public void ShowEnd()
	{
		//Save the mission and activate the finish menu
        MissionManager.Instance.Save();
        EnableDisable(finishMenu, true);
		
		//Get the current coin and distance data
        int currentDist = (int)LevelGenerator.Instance.distance;
        int currentCoins = LevelManager.Instance.Coins();
		
		//Apply the data to the finish menu
        finishTexts[0].text = currentDist + "M";
        finishTexts[1].text = currentCoins.ToString();
		
		//If the current distance is greater than the best distance
		if (currentDist > SaveManager.GetBestDistance())
			//Set the current distance as the best distance
			SaveManager.SetBestDistance(currentDist);
		
		//Add the collected coins to the account
		SaveManager.SetCoins(SaveManager.GetCoins() + currentCoins);
		
		//Show the finish menu
		StartCoroutine(FadeScreen(0.4f, 0.7f));
		StartCoroutine(MoveMenu(finishMenu.transform, 0, 67, 0.55f, false));
	}
	//Shows the available power ups at start
	public void ShowStartPowerUps()
	{
		//Check the available power ups
		bool hasSpeed = SaveManager.GetExtraSpeed() > 0;
		bool hasShield = SaveManager.GetShield() > 0;
		bool hasSonicWave = SaveManager.GetSonicWave() > 0;
		
		//Get the number of the availabe power ups
		int numberOfPowerUps = 0;
		
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
	public void ActivateMainGUI()
	{
		showMainGUI = true;
        EnableDisable(mainGUIElements[0], true);
	}
	//Deactivates the main GUI
	public void DeactivateMainGUI()
	{
		showMainGUI = false;
        EnableDisable(mainGUIElements[0], false);
	}
	//Activated the main menu
	public void ActivateMainMenu()
	{
        mainMenuElements[4].renderer.material.mainTexture = menuTextures[0];
        EnableDisable(mainMenuElements[1], true);
	}
	//Deactivated the main menu
	public void DeactivateMainMenu()
	{
        EnableDisable(mainMenuElements[1], false);
	}
	//Revive picked up
	public void RevivePicked()
	{
		//Activate the revive heart
        EnableDisable(mainGUIElements[1], true);
	}
	//Disables a revive gui icon
	public void DisableReviveGUI(int num)
	{
		//Check if num is 1 or 0 to validate it
		if (num == 0 || num == 1)
			//Deactive the heart
            EnableDisable(mainGUIElements[num+1], false);
	}
	//Updates the best distance board at the main menu
	public void UpdateBestDistance()
	{
		bestDist.text = SaveManager.GetBestDistance() + "M";
	}
	//Updates mission text
	public void UpdateMissionTexts(string text1, string text2, string text3)
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
	public void UpdateMissionStatus(int i, int a, int b)
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
	public IEnumerator ShowRevive()
	{
		//Show the revive button
        StartCoroutine(MoveMenu(startPowerUps[3].transform, startPowerUps[3].transform.localPosition.x, -29.5f, 0.4f, false));
		
        //Variable to detect activation
        bool activated = false;

		//Wait for 3 seconds
		double waited = 0;
		while (waited <= 3)
		{
			waited += Time.deltaTime;
			
			//If the revive is activated during the 3 seconds
			if(reviveActivated)
			{
				//Wait 0.1 seconds, and hide the revive button
				yield return new WaitForSeconds(0.2f);
				StartCoroutine(MoveMenu(startPowerUps[3].transform, startPowerUps[3].transform.localPosition.x, -45, 0.4f, false));
				
				//Revive the player, and reset reviveActivated
				yield return new WaitForSeconds(0.5f);
                LevelManager.Instance.Revive();
				reviveActivated = false;
                activated = true;
			}

			yield return 0;
		}
        
		//If the revive was not activated, hide the revive button
        if (!activated)
        {
            StartCoroutine(MoveMenu(startPowerUps[3].transform, startPowerUps[3].transform.localPosition.x, -45, 0.4f, false));
            yield return new WaitForSeconds(0.5f);

            //show the finish menu
            ShowEnd();
        }
	}
	//Shows mission complete notification
	public IEnumerator ShowMissionComplete(string text)
	{
		//Declare the varialbes
		GameObject go = null;
		TextMesh t = null;
		
		int nIndex = 0;
		float yPosTarget = 0;
		
		//If notification 1 is not used
		if (!mNotification1Used)
		{
			//Use notification 1
			go = missionNotification[0];
			t = go.transform.FindChild("Text").GetComponent<TextMesh>() as TextMesh;
			
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
			t = go.transform.FindChild("Text").GetComponent<TextMesh>() as TextMesh;
			
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
			t = go.transform.FindChild("Text").GetComponent<TextMesh>() as TextMesh;
			
			//Set varialbes to notification 3
			yPosTarget = 20;
			mNotification3Used = true;
			nIndex = 3;
		}
		//If every notification is used
		else
		{
			//Stop coroutine
            StopCoroutine("ShowMissionComplete");
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
		double waited = 0;
		while (waited <= 2)
		{
			//If the game is not paused
			if (!showPause)
				//Increase waited time
				waited += Time.deltaTime;
			
			//Wait for the end of the frame
			yield return 0;
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
			yield return 0;
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
	public IEnumerator FadeScreen(float time, float to)
	{
		//Set the screen fade's color to end in time
		float i = 0.0f;
		float rate = 1.0f / time;
		
		Color start = overlay.material.color;
		Color end = new Color(start.r, start.g, start.b, to);
		
		while (i < 1.0) 
		{
			i += Time.deltaTime * rate;
			overlay.material.color = Color.Lerp(start, end, i);
			yield return 0;
		}
	}
}
