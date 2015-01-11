using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMatchSearcher : INotificationObserver
{
	public CMatchController mMatchController { get; set; }

	public void handleNotification(int aEvent, Object aParam, CNotificationManager aManager)
	{
		return;
	}

	public void subscribeNotification(CNotificationManager aCenter)
	{

	}

	public bool isHaveMatches()
	{
		return false;
	}

	public ArrayList findMatches(bool aOnlyOpenedCells = false)
	{
		ArrayList matches = new ArrayList();
		CWeightedUnion weightedUnionInstance = new CWeightedUnion();
		CMatchField field = mMatchController.mMatchView.mMatchField;

		int col_field = field.mColumns;
		int row_field = field.mRows;

		weightedUnionInstance.initWithElements(col_field * row_field);

		for (int i = 0; i < col_field * row_field/2; i += col_field)
		{
			for (int j = i; j < i + (col_field - 2); j++)
			{
				ArrayList icons = new ArrayList();
				
				icons.Add(field.getIconByIndex(j));
				icons.Add(field.getIconByIndex(j + 1));
				icons.Add(field.getIconByIndex(j + 2));
				
				if(aOnlyOpenedCells)
				{
					for(int index = 0; index < icons.Count; index++)
					{
						CMatchIcon icon = icons[index] as CMatchIcon;
						if(icon && !icon.getIsReadyAction())
						{
							continue;
						}
					}
				}
				
				if (field.isTheSameIconOne(icons))
				{
					weightedUnionInstance.unionInt(j,(j + 1));
					weightedUnionInstance.unionInt(j,(j + 2));
				}
			}
		}
//		
		for (int i = 0; i < row_field/2; i++)
		{
			for (int j = i; j < i + (col_field * (row_field/2 - 2)); j += col_field)
			{
				ArrayList icons = new ArrayList();
				
				icons.Add(field.getIconByIndex(j));
				icons.Add(field.getIconByIndex(j + (col_field)));
				icons.Add(field.getIconByIndex(j + (col_field * 2)));

				if(aOnlyOpenedCells)
				{
					for(int index = 0; index < icons.Count; index++)
					{
						CMatchIcon icon = icons[index] as CMatchIcon;
						if(icon && !icon.getIsReadyAction())
						{
							continue;
						}
					}
				}
				
				if (field.isTheSameIconOne(icons))
				{
					weightedUnionInstance.unionInt(j,(j + col_field));
					weightedUnionInstance.unionInt(j,(j + col_field * 2));
				}
			}
		}


		if (weightedUnionInstance.getHasUnions())
		{
			Dictionary<int, ArrayList> foundMatches = weightedUnionInstance.getTrees();

//			Debug.Log(foundMatches);

			foreach (var pair in foundMatches)
			{
//				Debug.Log(pair.Key);

				matches.Add(pair.Value);				
			}
			
		}
	
		return matches;
	}
}
