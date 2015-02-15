using Libraries;
using Match.Gem;
using System.Collections;
using UnityEngine;
using Libraries.ActionSystem;

namespace Match.Actions {
	public class CDestroy : CBase, IDieObserver {
		private CIcon mIconTarget = null;

		public CDestroy (CIcon target) {
			mIconTarget = target;
		}

		public override bool Validation() {
			return mIconTarget && mIconTarget.IsActionReady();
		}

		public override void StartAction() {
			new CDie(mIconTarget, this);
		}

		public void OnDieEnd () {
			ComplateAction();
		}

		public override int GetIndex() {
			return (int) EEvents.Destroy;
		}
	}

}