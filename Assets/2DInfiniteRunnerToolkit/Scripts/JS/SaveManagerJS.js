#pragma strict

static class SaveManagerJS
{
	var coinAmmount 	: int 		= 1500;					//The ammount of coins the player has
	var bestDistance 	: int 		= 0;					//The best distance the player has reached
	
	var extraSpeed 		: int 		= 0;					//The ammount of extra speed power ups the player has
	var shield 			: int 		= 0;					//The ammount of shield power ups the player has
	var sonicWave 		: int 		= 0;					//The ammount of sonic wave power ups the player has
	var revive 			: int 		= 0;					//The ammount of revive power ups the player has
	
	var mission1 		: int 		= 0;					//Mission 1 ID
	var mission2 		: int 		= 1;					//Mission 2 ID
	var mission3 		: int 		= 2;					//Mission 3 ID
	
	var mission1Data 	: int 		= 0;					//Mission 1 saved data
	var mission2Data 	: int 		= 0;					//Mission 2 saved data
	var mission3Data 	: int 		= 0;					//Mission 3 saved data
	
	var missionData		: String 	= ""; 					//Saved mission data string
	
	//If there is no save data, create it, else, load the existing
	function CreateAndLoadData() 
	{
		//If found the coin ammount data, load the datas
		if (PlayerPrefs.HasKey("Coin ammount"))
			LoadData();
		//Else, create the datas
		else
			CreateData();
	}
	//Creates a blank save
	function CreateData()
	{
		PlayerPrefs.SetInt("Coin ammount", 1500);
		PlayerPrefs.SetInt("Best Distance", 0);
		
		PlayerPrefs.SetInt("Extra Speed", 0);
		PlayerPrefs.SetInt("Shield", 0);
		PlayerPrefs.SetInt("Sonic Wave", 0);
		PlayerPrefs.SetInt("Revive", 0);
		
		PlayerPrefs.SetInt("Mission1", 0);
		PlayerPrefs.SetInt("Mission2", 1);
		PlayerPrefs.SetInt("Mission3", 2);
		
		PlayerPrefs.SetInt("Mission1Data", 0);
		PlayerPrefs.SetInt("Mission2Data", 0);
		PlayerPrefs.SetInt("Mission3Data", 0);
		
		PlayerPrefs.SetString("Missions", "");
		
		PlayerPrefs.Save();
	}
	//Loads the save
	function LoadData()
	{
		coinAmmount = PlayerPrefs.GetInt("Coin ammount");
		bestDistance = PlayerPrefs.GetInt("Best Distance");
		
		extraSpeed = PlayerPrefs.GetInt("Extra Speed");
		shield = PlayerPrefs.GetInt("Shield");
		sonicWave = PlayerPrefs.GetInt("Sonic Wave");
		revive = PlayerPrefs.GetInt("Revive");
		
		mission1 = PlayerPrefs.GetInt("Mission1");
		mission2 = PlayerPrefs.GetInt("Mission2");
		mission3 = PlayerPrefs.GetInt("Mission3");
		
		mission1Data = PlayerPrefs.GetInt("Mission1Data");
		mission2Data = PlayerPrefs.GetInt("Mission2Data");
		mission3Data = PlayerPrefs.GetInt("Mission3Data");
		
		missionData = PlayerPrefs.GetString("Missions");
	}
	//Return the coin ammounts
	function GetCoins()
	{
		return coinAmmount;
	}
	//Returns the best distance
	function GetBestDistance()
	{
		return bestDistance;
	}
	//Returns the number of extra speed power ups
	function GetExtraSpeed()
	{
		return extraSpeed;
	}
	//Returns the number of shield power ups
	function GetShield()
	{
		return shield;
	}
	//Returns the number of sonic blast power ups
	function GetSonicWave()
	{
		return sonicWave;
	}
	//Returns the number of revive power ups
	function GetRevive()
	{
		return revive;
	}
	//Returns mission 1 ID
	function GetMission1()
	{
		return mission1;
	}
	//Returns mission 2 ID
	function GetMission2()
	{
		return mission2;
	}
	//Returns mission 3 ID
	function GetMission3()
	{
		return mission3;
	}
	//Returns mission 1 data
	function GetMission1Data()
	{
		return mission1Data;
	}
	//Returns mission 2 data
	function GetMission2Data()
	{
		return mission2Data;
	}
	//Returns mission 2 data
	function GetMission3Data()
	{
		return mission3Data;
	}
	//Returns the mission data
	function GetMissionData()
	{
		return missionData;
	}
	//Modifies and saves the coin ammount
	function SetCoins(ammount : int)
	{
		coinAmmount = ammount;
		PlayerPrefs.SetInt("Coin ammount", coinAmmount);
		PlayerPrefs.Save();
	}
	//Modifies and saves the best distance
	function SetBestDistance(distance : int)
	{
		bestDistance = distance;
		PlayerPrefs.SetInt("Best Distance", bestDistance);
		PlayerPrefs.Save();
	}
	//Modifies and saves the number of extra speed power ups
	function ModifyExtraSpeedBy(modifyBy : int)
	{
		extraSpeed += modifyBy;
		PlayerPrefs.SetInt("Extra Speed", extraSpeed);
		PlayerPrefs.Save();
	}
	//Modifies and saves the number of shield power ups
	function ModifyShieldBy(modifyBy : int)
	{
		shield += modifyBy;
		PlayerPrefs.SetInt("Shield", shield);
		PlayerPrefs.Save();
	}
	//Modifies and saves the number of sonic wave power ups
	function ModifySonicWaveBy(modifyBy : int)
	{
		sonicWave += modifyBy;
		PlayerPrefs.SetInt("Sonic Wave", sonicWave);
		PlayerPrefs.Save();
	}
	//Modifies and saves the number of revive power ups
	function ModifyReviveBy(modifyBy : int)
	{
		revive += modifyBy;
		PlayerPrefs.SetInt("Revive", revive);
		PlayerPrefs.Save();
	}
	//Sets mission 1 ID
	function SetMission1(id : int)
	{
		mission1 = id;
		PlayerPrefs.SetInt("Mission1", id);
		PlayerPrefs.Save();
	}
	//Sets mission 2 ID
	function SetMission2(id : int)
	{
		mission2 = id;
		PlayerPrefs.SetInt("Mission2", id);
		PlayerPrefs.Save();
	}
	//Sets mission 3 ID
	function SetMission3(id : int)
	{
		mission3 = id;
		PlayerPrefs.SetInt("Mission3", id);
		PlayerPrefs.Save();
	}
	//Sets mission 1 data
	function SetMission1Data(id : int)
	{
		mission1Data = id;
		PlayerPrefs.SetInt("Mission1Data", id);
		PlayerPrefs.Save();
	}
	//Sets mission 2 data
	function SetMission2Data(id : int)
	{
		mission2Data = id;
		PlayerPrefs.SetInt("Mission2Data", id);
		PlayerPrefs.Save();
	}
	//Sets mission 3 data
	function SetMission3Data(id : int)
	{
		mission3Data = id;
		PlayerPrefs.SetInt("Mission3Data", id);
		PlayerPrefs.Save();
	}
	//Modifies the mission data
	function SetMissionData(s : String)
	{
		missionData = s;
		PlayerPrefs.SetString("Missions", missionData);
		PlayerPrefs.Save();
	}
}