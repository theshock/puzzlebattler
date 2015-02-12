using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public interface IAction {
		void SetActionManager(Game.CActionManager aManager, CField aIconField);

		bool Validation();

		void StartAction();

		EEvents GetActionEvent();
	}
}