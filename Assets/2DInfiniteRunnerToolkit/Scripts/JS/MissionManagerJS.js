#pragma strict

class MissionManagerJS extends MonoBehaviour 
{
	var missions						: MissionTemplateJS[];			//List of missions
	
	static var myInstance				: MissionManagerJS;
    static var instances				: int 		= 0;
	
	private var activeMissionIDs		: int[] 	= new int[3];		//The ID of the active missions
	private var activeMissionComplete	: boolean[]	= new boolean[3];	//Store which mission is completed from the active missions
	private var data 					: String 	= "";				//The data string containing the saved status
	
	//Returns the instance
    public static function Instance()
    {
		if (myInstance == null)
			myInstance = FindObjectOfType(typeof(MissionManagerJS)) as MissionManagerJS;

		return myInstance;
    }

    //Called at the biginning of the game
    function Start()
    {
        //Calibrates the myInstance static variable
        instances++;

        if (instances > 1)
            Debug.Log("Warning: There are more than one Mission Manager at the level");
        else
            myInstance = this;
    }
	//Loads the saved status
	function LoadStatus()
	{
		//Loads the data string
		data = SaveManagerJS.GetMissionData();
		
		//If a mission is removed, reset the data string
		if (data.Length > missions.length)
			ResetDataString();
		//If a mission was added, update the data string
		else if (data.Length < missions.length)
			UpdateDataString();
		
		//Get the active missions
		activeMissionIDs[0] = SaveManagerJS.GetMission1();
		activeMissionIDs[1] = SaveManagerJS.GetMission2();
		activeMissionIDs[2] = SaveManagerJS.GetMission3();
		
		//If a mission slot is empty, look for a new mission, if possible
		for (var i : int = 0; i < 3; i++)
		{
			if (activeMissionIDs[i] == -1)
				GetNewMission(i);
		}
		
		//If a mission slot is empty, set it to complete status
		for (var j : int = 0; j < 3; j++)
		{
			if (activeMissionIDs[j] != -1)
				activeMissionComplete[j] = false;
			else
				activeMissionComplete[j] = true;
		}
		
		//Get the mission data for active missions
		if (!activeMissionComplete[0] && missions[activeMissionIDs[0]].goalType != MissionTemplateJS.GoalType.InOneRun)
			missions[activeMissionIDs[0]].SetStoredValue(SaveManagerJS.GetMission1Data());
		
		if (!activeMissionComplete[1] && missions[activeMissionIDs[1]].goalType != MissionTemplateJS.GoalType.InOneRun)
			missions[activeMissionIDs[1]].SetStoredValue(SaveManagerJS.GetMission2Data());
		
		if (!activeMissionComplete[2] && missions[activeMissionIDs[2]].goalType != MissionTemplateJS.GoalType.InOneRun)
			missions[activeMissionIDs[2]].SetStoredValue(SaveManagerJS.GetMission3Data());
		
		//Update mission GUI texts
		UpdateGUITexts();
	}
	//Save progress
	function Save()
	{
		//Loop trought the missions
		for (var i : int = 0; i < 3; i++)
		{
			//If the mission is completed
			if (activeMissionComplete[i] && activeMissionIDs[i] != -1)
			{
				//Modify the data string
				var newData : char[] = data.ToCharArray();
				newData[activeMissionIDs[i]] = "1"[0];
				data = new String (newData);
				
				//Reset mission data
				if (i == 0)
					SaveManagerJS.SetMission1Data(0);
				else if (i == 1)
					SaveManagerJS.SetMission2Data(0);
				else
					SaveManagerJS.SetMission3Data(0);
				
				//Save the modifies
				SaveManagerJS.SetMissionData(data);
				
				//Find a new mission
				GetNewMission(i);
			}
			//If the mission is not completed
			else if (!activeMissionComplete[i])
			{
				//Set a reference
				var mission : MissionTemplateJS = missions[activeMissionIDs[i]];
				
				//If the mission requires data save
				if (mission.goalType == MissionTemplateJS.GoalType.InMultipleRun || mission.goalType == MissionTemplateJS.GoalType.InShop)
				{
					//Save the data
					if (i == 0)
						SaveManagerJS.SetMission1Data(mission.StoredValue());
					else if (i == 1)
						SaveManagerJS.SetMission2Data(mission.StoredValue());
					else
						SaveManagerJS.SetMission3Data(mission.StoredValue());
				}
			}
		}
		
		UpdateGUITexts();
	}
	//Called on distance based events
	function DistanceEvent (number : int)					
	{
		var mission : MissionTemplateJS;
		
		for (var i : int = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{	
				mission = missions[activeMissionIDs[i]];
				
				if (mission.missionType == MissionTemplateJS.MissionType.Distance)
					CheckDistanceIn(mission, number, i);
				else if (mission.missionType == MissionTemplateJS.MissionType.DistanceWithNoCoins)
					CheckDistanceNoCoin(mission, number, i);
				else if (mission.missionType == MissionTemplateJS.MissionType.DistanceWithNoPowerUps)
					CheckDistanceNoPowerUp(mission, number, i);
			}
		}
	}
	//Called on sink based events
	function SinkEvent(number : int)
	{
		var mission : MissionTemplateJS;
		
		for (var i : int = 0; i < 3; i++)
		{	
			if (!activeMissionComplete[i])
			{	
				mission = missions[activeMissionIDs[i]];
				
				if (mission.missionType == MissionTemplateJS.MissionType.SinkBetween)
					CheckSindBetween(mission, number, i);
			}
		}
	}
	//Called on coin based events
	function CoinEvent(number : int)
	{
		var mission : MissionTemplateJS;
		
		for (var i : int = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{	
				mission = missions[activeMissionIDs[i]];
				
				if (mission.missionType == MissionTemplateJS.MissionType.Coin)
					CheckCoinIn(mission, number, i);
			}
		}
	}
	//Called on power up based events
	function PowerUpEvent(name : String)
	{
		var mission : MissionTemplateJS;
		
		for (var i : int = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{	
				mission = missions[activeMissionIDs[i]];
				
				if (mission.missionType == MissionTemplateJS.MissionType.PowerUps)
					CheckPowerUpBased(mission, i);
				else if (mission.missionType == MissionTemplateJS.MissionType.ExtraSpeed && name == "Extra Speed")
					CheckPowerUpBased(mission, i);
				else if (mission.missionType == MissionTemplateJS.MissionType.Shield && name == "Shield")
					CheckPowerUpBased(mission, i);
				else if (mission.missionType == MissionTemplateJS.MissionType.SonicWave && name == "Sonic Wave")
					CheckPowerUpBased(mission, i);
				else if (mission.missionType == MissionTemplateJS.MissionType.Revive && name == "Revive")
					CheckPowerUpBased(mission, i);
			}
		}
	}
	//Called on obstacle based events
	function ObstacleEvent(name : String)
	{
		var mission : MissionTemplateJS;
		
		for (var i : int = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{	
				mission = missions[activeMissionIDs[i]];
				
				if (mission.missionType == MissionTemplateJS.MissionType.Obstacles)
					CheckObstacleBased(mission, i);
				else if (mission.missionType == MissionTemplateJS.MissionType.Mine && name == "Mine")
					CheckObstacleBased(mission, i);
				else if (mission.missionType == MissionTemplateJS.MissionType.Laser && name == "Laser")
					CheckObstacleBased(mission, i);
				else if (mission.missionType == MissionTemplateJS.MissionType.Torpedo && name == "Torpedo")
					CheckObstacleBased(mission, i);
			}
		}
	}
	//Called on shop based events
	function ShopEvent(name : String)
	{
		var mission : MissionTemplateJS;
		
		for (var i : int = 0; i < 3; i++)
		{
			if (!activeMissionComplete[i])
			{	
				mission = missions[activeMissionIDs[i]];
				
				if (mission.goalType == MissionTemplateJS.GoalType.InShop)
				{
					if (mission.missionType == MissionTemplateJS.MissionType.PowerUps)
						CheckShopBased(mission, i);
					else if (mission.missionType == MissionTemplateJS.MissionType.ExtraSpeed && name == "Extra Speed")
						CheckShopBased(mission, i);
					else if (mission.missionType == MissionTemplateJS.MissionType.Shield && name == "Shield")
						CheckShopBased(mission, i);
					else if (mission.missionType == MissionTemplateJS.MissionType.SonicWave && name == "Sonic Wave")
						CheckShopBased(mission, i);
					else if (mission.missionType == MissionTemplateJS.MissionType.Revive && name == "Revive")
						CheckShopBased(mission, i);
				}
			}
		}
	}
	
	//Checks DistanceIn based mission status
	private function CheckDistanceIn(mission : MissionTemplateJS, number : int, i : int)
	{
		if (mission.goalType == MissionTemplateJS.GoalType.InOneRun)
		{
			GUIManagerJS.Instance().UpdateMissionStatus(i, number, mission.valueA);
			
			if (mission.valueA <= number)
				MissionCompleted(mission, i);
		}
		else if (mission.goalType == MissionTemplateJS.GoalType.InMultipleRun)
		{
			mission.ModifyStoredValue(false, number);
			GUIManagerJS.Instance().UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);
			
			if (mission.valueA <= mission.StoredValue())
				MissionCompleted(mission, i);
		}
	}
	//Checks DistanceNoCoin based mission status
	private function CheckDistanceNoCoin(mission : MissionTemplateJS, number : int, i : int)
	{
		if (mission.goalType == MissionTemplateJS.GoalType.InOneRun)
		{
			GUIManagerJS.Instance().UpdateMissionStatus(i, number, mission.valueA);
			
			if (mission.valueA <= number && LevelManagerJS.Instance().Coins() == 0)
				MissionCompleted(mission, i);
		}
	}
	//Checks DistanceNoPowerUp based mission status
	private function CheckDistanceNoPowerUp(mission : MissionTemplateJS, number : int, i : int)
	{
		if (mission.goalType == MissionTemplateJS.GoalType.InOneRun)
		{
			GUIManagerJS.Instance().UpdateMissionStatus(i, number, mission.valueA);
			
			if (mission.valueA <= number && !PlayerManagerJS.Instance().PowerUpUsed())
				MissionCompleted(mission, i);
		}
	}
	//Checks SinkBetween based mission status
	private function CheckSindBetween(mission : MissionTemplateJS, number : int, i : int)
	{
		if (mission.goalType == MissionTemplateJS.GoalType.InOneRun)
		{
			GUIManagerJS.Instance().UpdateMissionStatus(i, number, mission.valueA);
			
			if (mission.valueA <= number && number <= mission.valueB)
				MissionCompleted(mission, i);
		}
	}
	//Checks Coin based mission status
	private function CheckCoinIn(mission : MissionTemplateJS, number : int, i : int)
	{
		if (mission.goalType == MissionTemplateJS.GoalType.InOneRun)
		{
			GUIManagerJS.Instance().UpdateMissionStatus(i, number, mission.valueA);
			
			if (mission.valueA <= number)
				MissionCompleted(mission, i);
		}
		else if (mission.goalType == MissionTemplateJS.GoalType.InMultipleRun)
		{
			mission.ModifyStoredValue(false, number);
			GUIManagerJS.Instance().UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);
			
			if (mission.valueA <= mission.StoredValue())
				MissionCompleted(mission, i);
		}
	}
	//Checks Power Up based mission status
	private function CheckPowerUpBased(mission : MissionTemplateJS, i : int)
	{
		if (mission.goalType == MissionTemplateJS.GoalType.InOneRun || mission.goalType == MissionTemplateJS.GoalType.InMultipleRun)
		{
			mission.ModifyStoredValue(true, 1);
			GUIManagerJS.Instance().UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);
			
			if (mission.valueA == mission.StoredValue())
				MissionCompleted(mission, i);
		}
	}
	//Checks Obstalce based mission status
	private function CheckObstacleBased(mission : MissionTemplateJS, i : int)
	{
		if (mission.goalType == MissionTemplateJS.GoalType.InOneRun || mission.goalType == MissionTemplateJS.GoalType.InMultipleRun)
		{
			mission.ModifyStoredValue(true, 1);
			GUIManagerJS.Instance().UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);
			
			if (mission.valueA == mission.StoredValue())
				MissionCompleted(mission, i);
		}
	}
	//Check shop based mission status
	private function CheckShopBased(mission : MissionTemplateJS, i : int)
	{
		mission.ModifyStoredValue(true, 1);
		GUIManagerJS.Instance().UpdateMissionStatus(i, mission.StoredValue(), mission.valueA);
		
		if (mission.valueA == mission.StoredValue())
		{
			MissionCompleted(mission, i);
			Save();
		}
	}
	
	//Completes the mission
	private function MissionCompleted(mission : MissionTemplateJS, missionID : int)
	{
		//Flag the mission as completed
		activeMissionComplete[missionID] = true;
		//Show the GUI notification
		StartCoroutine(GUIManagerJS.Instance().ShowMissionComplete(mission.description));
	}
	//Get new mission
	private function GetNewMission(i : int)
	{
		//Loop trought the mission data
		for (var j : int = 0; j < data.Length; j++)
		{
			//If a mission is uncompleted
			if (data[j] == '0')
			{
				//If we are at mission slot 1
				if (i == 0)
				{
					//If the uncompleted mission is not used in the other 2 slot
					if (activeMissionIDs[1] != j && activeMissionIDs[2] != j)
					{
						//Assign the mission to the slot, and return
						activeMissionIDs[i] = j;
						activeMissionComplete[i] = false;
						SaveManagerJS.SetMission1(j);
						
						return;
					}
				}
				//If we are at mission slot 2
				else if (i == 1)
				{
					//If the uncompleted mission is not used in the other 2 slot
					if (activeMissionIDs[0] != j && activeMissionIDs[2] != j)
					{
						//Assign the mission to the slot, and return
						activeMissionIDs[i] = j;
						activeMissionComplete[i] = false;
						SaveManagerJS.SetMission2(j);
						
						return;
					}
				}
				//If we are at mission slot 3
				else
				{
					//If the uncompleted mission is not used in the other 2 slot
					if (activeMissionIDs[0] != j && activeMissionIDs[1] != j)
					{
						//Assign the mission to the slot, and return
						activeMissionIDs[i] = j;
						activeMissionComplete[i] = false;
						SaveManagerJS.SetMission3(j);
						
						return;
					}
				}
			}
		}
		
		//If we did not find a suitable mission, we set the slot inactive
		activeMissionIDs[i] = -1;
		
		if (i == 0)
			SaveManagerJS.SetMission1(-1);
		else if (i == 1)
			SaveManagerJS.SetMission2(-1);
		else
			SaveManagerJS.SetMission3(-1);
	}
	//Gets new missions
	function GetNextMissions()
	{
		//Loop trought the mission slots
		for (var i : int = 0; i < 3; i++)
		{
			//If the mission is completed in the mission slot
			if (activeMissionComplete[i])
			{
				var found : boolean = false;
				//Loop trought the mission data
				for (var j : int = 0; j < data.Length; j++)
				{
					//If we found an empty mission
					if (data[j] == '0')
					{
						//If we are at mission slot 1
						if (i == 0)
						{
							//If the uncompleted mission is not used in the other 2 slot
							if (activeMissionIDs[1] != j && activeMissionIDs[2] != j)
							{
								//Assign the mission to the slot
								activeMissionIDs[i] = j;
								SaveManagerJS.SetMission1(j);
							}
						}
						//If we are at mission slot 2
						else if (i == 1)
						{
							//If the uncompleted mission is not used in the other 2 slot
							if (activeMissionIDs[0] != j && activeMissionIDs[2] != j)
							{
								//Assign the mission to the slot
								activeMissionIDs[i] = j;
								SaveManagerJS.SetMission2(j);
							}
						}
						//If we are at mission slot 3
						else
						{
							//If the uncompleted mission is not used in the other 2 slot
							if (activeMissionIDs[0] != j && activeMissionIDs[1] != j)
							{
								//Assign the mission to the slot
								activeMissionIDs[i] = j;
								SaveManagerJS.SetMission3(j);
							}
						}
						
						found = true;
					}
				}
				
				//If there is no next mission
				if (!found)
				{
					//Flag the mission inactive
					if (i == 0)
						SaveManagerJS.SetMission1(-1);
					else if (i == 1)
						SaveManagerJS.SetMission2(-1);
					else
						SaveManagerJS.SetMission3(-1);
				}
			}
		}
	}
	//Updates the GUI mission texts
	private function UpdateGUITexts()
	{
		//Declare 3 string
		var text1 : String;
		var text2 : String;
		var text3 : String;
		
		//If mission 1 is active, give it's description to text1
		if (!activeMissionComplete[0])
		{
			text1 = missions[activeMissionIDs[0]].description;
			GUIManagerJS.Instance().UpdateMissionStatus(0, missions[activeMissionIDs[0]].StoredValue(), missions[activeMissionIDs[0]].valueA);
		}
		//Else set it's text to "Completed"
		else
		{
			text1 = "Completed";
			GUIManagerJS.Instance().UpdateMissionStatus(0, 0, 0);
		}
		
		//If mission 2 is active, give it's description to text2
		if (!activeMissionComplete[1])
		{
			text2 = missions[activeMissionIDs[1]].description;
			GUIManagerJS.Instance().UpdateMissionStatus(1, missions[activeMissionIDs[1]].StoredValue(), missions[activeMissionIDs[1]].valueA);
		}
		//Else set it's text to "Completed"
		else
		{
			text2 = "Completed";
			GUIManagerJS.Instance().UpdateMissionStatus(0, 0, 0);
		}
		
		//If mission 3 is active, give it's description to text3
		if (!activeMissionComplete[2])
		{
			text3 = missions[activeMissionIDs[2]].description;
			GUIManagerJS.Instance().UpdateMissionStatus(2, missions[activeMissionIDs[2]].StoredValue(), missions[activeMissionIDs[2]].valueA);
		}
		//Else set it's text to "Completed"
		else
		{
			text3 = "Completed";
			GUIManagerJS.Instance().UpdateMissionStatus(0, 0, 0);
		}
		
		//Update the GUI texts
		GUIManagerJS.Instance().UpdateMissionTexts(text1, text2, text3);
	}
	//Updates the data string
	private function UpdateDataString()
	{
		//Update the data string
		for (var i : int = data.Length; i < missions.length; i++)
			data += "0";
		
		//Save the data string
		SaveManagerJS.SetMissionData(data);
	}
	//Resets the data string
	function ResetDataString()
	{
		//Create a new data string
		var s : String = "";
		for (var i : int = 0; i < missions.length; i++)
			s += "0";
		
		//Assign the new data string to the data, and save it
		data = s;
		SaveManagerJS.SetMissionData(data);
		
		//Reassign the missions
		ResetMissions();
	}
	//Reset missions
	private function ResetMissions()
	{
		//Reset missions
		SaveManagerJS.SetMission1(0);
		SaveManagerJS.SetMission2(1);
		SaveManagerJS.SetMission3(2);
		
		//Reset mission data
		SaveManagerJS.SetMission1Data(0);
		SaveManagerJS.SetMission2Data(0);
		SaveManagerJS.SetMission3Data(0);
	}
}
