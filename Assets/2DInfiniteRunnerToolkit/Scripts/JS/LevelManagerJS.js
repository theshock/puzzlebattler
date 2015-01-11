#pragma strict

class LevelManagerJS extends MonoBehaviour 
{
	private var coins 				: int	= 0;		//Collected coins
	
	static var myInstance			: LevelManagerJS;
    static var instances			: int	= 0;

	 //Retursn the instance
    public static function Instance()
    {
		if (myInstance == null)
			myInstance = FindObjectOfType(typeof(LevelManagerJS)) as LevelManagerJS;

		return myInstance;
    }
	
	//Called at the start of the game
	function Start()
	{
	 	//Calibrates the myInstance static variable
        instances++;

        if (instances > 1)
            Debug.Log("Warning: There are more than one Level Manager at the level");
        else
            myInstance = this;
            
		SaveManagerJS.CreateAndLoadData();				//Create or load the saved stats
		GUIManagerJS.Instance().UpdateBestDistance();	//Update best distance at the hangar
		GUIManagerJS.Instance().SetLevelResolution();	//Set the level for the current resolution
		MissionManagerJS.Instance().LoadStatus();		//Load mission status
	}
	//Called when the level is started
	function StartLevel()
	{
		StartCoroutine(LevelGeneratorJS.Instance().StartToGenerate(1.75f, 3));	//Start the level generator
		PlayerManagerJS.Instance().ResetStatus(true);							//Reset player status, and move the submarine to the starting position
		GUIManagerJS.Instance().ShowStartPowerUps();							//Show the power up activation GUI
		GUIManagerJS.Instance().ActivateMainGUI();								//Activate main GUI
	}
	//Called when the game is paused
	function PauseGame()
	{
		PlayerManagerJS.Instance().DisableControls();								//Disable sub controls
		LevelGeneratorJS.Instance().Pause();										//Pause the level generator
	}
	//Called then the game is resumed
	function ResumeGame()
	{
		PlayerManagerJS.Instance().EnableControls();								//Enable the sub controls
		LevelGeneratorJS.Instance().Resume();										//Resume level generation
	}
	//Called when the player is reviving
	function Revive()
	{
		StartCoroutine(PlayerManagerJS.Instance().Revive());						//Revive the player
	}
	//Called when a coin has been collected
	function CoinGathered()
	{
		coins++;																//Increase coin number
		MissionManagerJS.Instance().CoinEvent(coins);								//Notify the mission manager
	}
	//Returns the number of collected coins
    function Coins()
    {
        return coins;
    }
	//Called when the level is restarting
	function Restart()
	{
		coins = 0;																//Reset coin numbers
		
		LevelGeneratorJS.Instance().Restart(true);						//Restart level generator
		PlayerManagerJS.Instance().ResetStatus(true);								//Reset player status
		MissionManagerJS.Instance().Save();										//Save mission status
		
		GUIManagerJS.Instance().ShowStartPowerUps();								//Show the power up activation GUI
		GUIManagerJS.Instance().ActivateMainGUI();								//Activate main GUI
		GUIManagerJS.Instance().UpdateBestDistance();								//Update best distance at the hangar
	}
	//Called when quiting to the main menu from the level
	function QuitToMain()
	{
		LevelGeneratorJS.Instance().Restart(false);								//Disable level generator
		PlayerManagerJS.Instance().ResetStatus(false);							//Reset player status
		MissionManagerJS.Instance().Save();										//Save progress
		
		GUIManagerJS.Instance().DeactivateMainGUI();								//Deactivate the main GUI
		GUIManagerJS.Instance().ActivateMainMenu();								//Activate main menu
		GUIManagerJS.Instance().UpdateBestDistance();								//Update best distance at the hangar
	}
}