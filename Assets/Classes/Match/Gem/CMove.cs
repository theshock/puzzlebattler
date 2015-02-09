using Config;
using Libraries;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Gem {

	public class CMove {

		private float mMovingTime = 0.3f;
		private IMoveObserver listener;
		private List<CIcon> moving = new List<CIcon>();

		public CMove (Config.Match.CMatch config) {
			mMovingTime = config.mGems.mMovingTime;
		}

		public void SetObserver(IMoveObserver listener) {
			this.listener = listener;
		}

		public bool IsFinished () {
			return moving.Count == 0;
		}

		public void OnEndMoveComplete (CIcon icon) {
			if (IsFinished()) {
				return;
			}

			icon.IconState = EState.Open;
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
				icon.IconState = EState.Lock;
				icon.mMover = this;
				iTween.MoveTo(icon.gameObject, iTween.Hash("position", pos, "time", mMovingTime, "onComplete", "OnEndMoveComplete" ));
				return true;
			}
		}

	}

}
