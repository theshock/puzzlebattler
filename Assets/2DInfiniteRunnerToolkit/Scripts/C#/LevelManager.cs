using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour 
{
	int coins 	= 0;							//Collected coins

    static LevelManager myInstance;
    static int instances = 0;

    //Retursn the instance
    public static LevelManager Instance
    {
        get
        {
            if (myInstance == null)
                myInstance = FindObjectOfType(typeof(LevelManager)) as LevelManager;

            return myInstance;
        }
    }

	//Called at the start of the game
	void Start()
	{
        //Calibrates the myInstance static variable
        instances++;

        if (instances > 1)
            Debug.Log("Warning: There are more than one Level Manager at the level");
        else
            myInstance = this;

		SaveManager.CreateAndLoadData();		        //Create or load the saved stats
		GUIManager.Instance.UpdateBestDistance();		//Update best distance at the hangar
		GUIManager.Instance.SetLevelResolution();		//Set the level for the current resolution
        MissionManager.Instance.LoadStatus();			//Load mission status
	}
	//Called when the level is started
	public void StartLevel()
	{
        StartCoroutine(LevelGenerator.Instance.StartToGenerate(1.25f, 3));	//Start the level generator
        PlayerManager.Instance.ResetStatus(true);							//Reset player status, and move the submarine to the starting position
        GUIManager.Instance.ShowStartPowerUps();								//Show the power up activation GUI
        GUIManager.Instance.ActivateMainGUI();								//Activate main GUI
	}
	//Called when the game is paused
	public void PauseGame()
	{
        PlayerManager.Instance.DisableControls();				//Disable sub controls
        LevelGenerator.Instance.Pause();							//Pause the level generator
	}
	//Called then the game is resumed
	public void ResumeGame()
	{
        PlayerManager.Instance.EnableControls();					//Enable the sub controls
        LevelGenerator.Instance.Resume();						//Resume level generation
	}
	//Called when the player is reviving
	public void Revive()
	{
        StartCoroutine(PlayerManager.Instance.Revive());			//Revive the player
	}
	//Called when a coin has been collected
	public void CoinGathered()
	{
		coins++;										//Increase coin number
        MissionManager.Instance.CoinEvent(coins);				//Notify the mission manager
	}
    //Returns the number of collected coins
    public int Coins()
    {
        return coins;
    }
	//Called when the level is restarting
	public void Restart()
	{
		coins = 0;										//Reset coin numbers

        LevelGenerator.Instance.Restart(true);					//Restart level generator
        PlayerManager.Instance.ResetStatus(true);				//Reset player status
        MissionManager.Instance.Save();							//Save mission status

        GUIManager.Instance.ShowStartPowerUps();					//Show the power up activation GUI
        GUIManager.Instance.ActivateMainGUI();					//Activate main GUI
        GUIManager.Instance.UpdateBestDistance();				//Update best distance at the hangar
	}
	//Called when quiting to the main menu from the level
	public void QuitToMain()
	{
        LevelGenerator.Instance.Restart(false);				//Disable level generator
        PlayerManager.Instance.ResetStatus(false);			//Reset player status
        MissionManager.Instance.Save();						//Save progress

        GUIManager.Instance.DeactivateMainGUI();				//Deactivate the main GUI
        GUIManager.Instance.ActivateMainMenu();				//Activate main menu
        GUIManager.Instance.UpdateBestDistance();			//Update best distance at the hangar
	}
}
