#pragma strict

@System.Serializable
public class MissionTemplateJS
{
	//Enumeration definitions
	enum MissionType {Distance, DistanceWithNoCoins, DistanceWithNoPowerUps, SinkBetween, Coin, PowerUps, ExtraSpeed, Shield, SonicWave, Obstacles, Revive, Mine, Torpedo, Laser};
	enum GoalType {InOneRun, InMultipleRun, InShop, Other};
	
	var name							: String 		= "";						//Mission name
	
	var missionType						: MissionType  	= MissionType.Distance;		//Mission type selection
	var goalType						: GoalType  	= GoalType.InOneRun;		//Mission goal selection
	
	var description 					: String		= "";						//Mission description
	
	var valueA 							: int			= 0;						//Goal value A
	var valueB 							: int			= 0;						//Goal value B	
	
	private var storedValue 			: int			= 0;						//Stored value
	private var startingValue 			: int			= 0;						//Starting Value
	
	private var completed 				: boolean		= false;					//Completition status
	
	//Sets the stored value
	function SetStoredValue(v : int)
	{
		storedValue = v;
		startingValue = v;
	}
	//Returns stored value
	function StoredValue()
	{
		return storedValue;
	}
	//Modifies stored value
	function ModifyStoredValue(addValue : boolean, ammount : int)
	{
		if (addValue)
			storedValue += ammount;
		else
			storedValue = startingValue + ammount;
	}
	//Reset stored value
	function ResetThis()
	{
		storedValue = 0;
	}
	//Returns completion status
	function Completed()
	{
		return completed;
	}
	//Set the mission to completed status
	function Complete()
	{
		completed = true;
	}
}
