using System.Collections.Generic;

namespace Libraries.ActionSystem {

	/*
		You can subscribe for each actionIndex and each eventType. "callbacks" is something like this:

		callbacks = {
			5: {
				Begin: [ OnAction(IAction), OnAction(IAction) ]
				End: [ OnAction(IAction) ]
			}
			7: {
				End: [ OnAction(IAction), OnAction(IAction), OnAction(IAction) ]
			}
			12: {
				Begin: [ OnAction(IAction), OnAction(IAction) ]
			}
		}
	 */
	public class CEvents {

		public void On (EActionState eventType, int actionIndex, OnAction callback) {
			GetList(eventType, actionIndex).Add(callback);
		}
		public void Off (EActionState eventType, int actionIndex, OnAction callback) {
			GetList(eventType, actionIndex).Remove(callback);
		}

		public void OnFinish (OnFinish finish) {
			onFinish += finish;
		}

		public void OffFinish (OnFinish finish) {
			onFinish -= finish;
		}

		public void InvokeFinish() {
			onFinish.Invoke();
		}

		public void Invoke (EActionState eventType, IAction action) {
			foreach (OnAction callback in GetList(eventType, action.GetIndex())) {
				callback.Invoke(action);
			}
		}

		protected event OnFinish onFinish;
		protected Dictionary<int, Dictionary<EActionState, List<OnAction>>> callbacks = new Dictionary<int, Dictionary<EActionState, List<OnAction>>>();

		protected List<OnAction> GetList (EActionState eventType, int actionIndex) {
			if (callbacks.ContainsKey(actionIndex) == false) {
				callbacks[actionIndex] = new Dictionary<EActionState, List<OnAction>>();
			}

			if (callbacks[actionIndex].ContainsKey(eventType) == false) {
				callbacks[actionIndex][eventType] = new List<OnAction>();
			}

			return callbacks[actionIndex][eventType];
		}

	}

}