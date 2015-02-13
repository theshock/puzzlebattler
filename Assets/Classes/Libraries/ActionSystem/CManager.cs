using Libraries;
using Match.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Libraries.ActionSystem {
	public class CManager : IAnticipant {
		private List<IAction> mActiveActions = new List<IAction>();

		public CEvents events = new CEvents();

		public bool AddAction(IAction aAction) {
			aAction.SetActionManager(this);

			if (aAction.Validation()) {
				mActiveActions.Add(aAction);
				events.Invoke(EActionState.Begin, aAction);
				aAction.StartAction();
				return true;
			} else {
				return false;
			}
		}

		public void OnActionEnd (IAction aAction) {
			mActiveActions.Remove(aAction);
			events.Invoke(EActionState.End, aAction);

			if (!HasActions()) {
				events.InvokeFinish();
			}
		}

		public bool HasActions() {
			return (mActiveActions.Count > 0);
		}
	}
}