using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public interface IAction {
		void initWithActionManager(Game.CActionManager aManager, CField aIconField);

		void doUpdateActionParam(Hashtable aData);

		bool validation();

		void startAction();

		void setDelegate(Match.Actions.Delegate aDelegate);

		EEvents getActionEvent();
	}
}