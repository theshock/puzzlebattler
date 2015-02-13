using UnityEngine;
using Libraries.ActionSystem;

namespace Match.Actions {
	public class CSwipeBack : CSwipe {

		public CSwipeBack (Config config) : base(config) {}

		public override bool Validation() {
			return true;
		}

		public override void OnMoveEnd () {
			ComplateAction();
		}

		public override int GetIndex() {
			return (int) EEvents.SwipeBack;
		}
	}
}