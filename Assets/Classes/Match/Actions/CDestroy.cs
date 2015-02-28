using Libraries;
using System.Collections;
using UnityEngine;
using Libraries.ActionSystem;

namespace Match.Actions {
	public class CDestroy : CBase, Gem.IDieObserver {
		private Gem.CIcon mIconTarget = null;

		public CDestroy (Gem.CIcon target) {
			mIconTarget = target;
		}

		public override bool Validation() {
			return mIconTarget && mIconTarget.IsIdle();
		}

		public override void StartAction() {
			new Gem.CDie(mIconTarget, this);
		}

		public void OnDieEnd () {
			ComplateAction();
		}

		public override int GetIndex() {
			return (int) EEvents.Destroy;
		}
	}

}