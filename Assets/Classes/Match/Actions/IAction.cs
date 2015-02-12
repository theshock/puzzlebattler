using UnityEngine;
using System.Collections;

namespace Match.Actions {
	public interface IAction {
		void SetActionManager(Game.CActionManager aManager);

		bool Validation();

		void StartAction();

		EEvents GetActionEvent();
	}
}