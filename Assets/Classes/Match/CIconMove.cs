using Libraries;
using System.Collections.Generic;
using UnityEngine;

namespace Match {

	public class CIconMove {

		private float mTimeDelay = 1.5f;
		private IIconMoveListener listener;
		private List<CIcon> moving = new List<CIcon>();

		public void SetListener (IIconMoveListener listener) {
			this.listener = listener;
		}

		public bool IsFinished () {
			return moving.Count == 0;
		}

		public void OnEndMoveComplete (CIcon icon) {
			if (IsFinished()) {
				return;
			}

			icon.IconState = EIconState.eOpen;
			moving.Remove(icon);

			if (IsFinished() && listener != null) {
				listener.OnMoveEnd();
			}
		}

		public bool addMove(CIcon icon, Vector3 pos) {
			if (icon.transform.position == pos) {
				return false;
			} else {
				moving.Add(icon);
				icon.IconState = EIconState.eLock;
				icon.mMover = this;
				iTween.MoveTo(icon.gameObject, iTween.Hash("position", pos, "time", mTimeDelay, "onComplete", "OnEndMoveComplete" ));
				return true;
			}
		}

	}

}
