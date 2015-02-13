using System.Collections.Generic;

namespace Libraries.ActionSystem {
	public abstract class CCreating : CBase, IAnticipant {

		protected List<IAction> mWaiting = new List<IAction>();

		public void OnActionEnd (IAction action) {
			mWaiting.Remove(action);

			if (mWaiting.Count == 0) {
				ComplateAction();
			}
		}

		protected void Wait (IAction action) {
			action.SetAnticipant(this);

			if (mActionManager.AddAction(action)) {
				mWaiting.Add(action);
			}
		}

	}
}