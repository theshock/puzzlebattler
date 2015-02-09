using Libraries;
using Match.Gem;
using System.Collections;
using UnityEngine;

namespace Match.Actions {
	public class CDestroy : CBase {
		private float mTimeDelayEmpty = 0.1f;
		private CIcon mIconTarget = null;

		public static IAction create() {
			return new CDestroy();
		}

		public override bool validation() {
			return mIconTarget && mIconTarget.IsActionReady();
		}

		public override void startAction() {
			mIconTarget.IconState = EState.Lock;

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


		public override EEvents getActionEvent() {
			return EEvents.eDestroy;
		}

		public void configure(CIcon target) {
			mIconTarget = target;
		}

		public void destroyArray(ArrayList aCells) {}
	}

}