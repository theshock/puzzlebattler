using Libraries;
using Match.Gem;
using System.Collections;
using UnityEngine;
using Libraries.ActionSystem;

namespace Match.Actions {
	public class CDestroy : CBase {
		private CIcon mIconTarget = null;
		private float mTimeDelayEmpty = 0.1f;

		public CDestroy (CIcon target) {
			mIconTarget = target;
		}

		public override bool Validation() {
			return mIconTarget && mIconTarget.IsActionReady();
		}

		public override void StartAction() {
			LaunchParticles();

			CGlobalUpdateManager.shared().subscribeUpdateEvent(doUpdate);
		}

		private void LaunchParticles () {
			var transform = mIconTarget.transform;
			var prefab = mIconTarget.mField.mConfig.mDie.GetPrefab(mIconTarget.IconType);
			GameObject anim = Instantiate(prefab, transform.position, transform.rotation) as GameObject;

			anim.transform.SetParent(transform.parent);

			Destroy(anim, 2);
		}

		public void emptyCell() {
			mIconTarget.IconState = EState.Clear;
			ComplateAction();
		}

		public void doUpdate() {
			mTimeDelayEmpty -= Time.smoothDeltaTime;

			if (mTimeDelayEmpty <= 0) {
				CGlobalUpdateManager.shared().unsubscribeUpdateEvent(doUpdate);
				emptyCell();
			}
		}

		public override int GetIndex() {
			return (int) EEvents.Destroy;
		}
	}

}