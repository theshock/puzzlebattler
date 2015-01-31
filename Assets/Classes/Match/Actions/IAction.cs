using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public interface IAction {
		void initWithActionManager(Game.CActionManager aManager, CField aIconField);

		bool validation();

		void startAction();

		void setDelegate(Delegate aDelegate);

		EEvents getActionEvent();
	}
}