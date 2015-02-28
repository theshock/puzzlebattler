using Match.Gem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libraries.ActionSystem;

namespace Match.Actions {
	public class CMatch : CCreating {
		private List<CIcon> mMatchIcons;

		public CMatch (List<CIcon> aMatchIcons) {
			mMatchIcons = aMatchIcons;
		}

		public EColor GetMatchIconType() {
			return mMatchIcons[0].color;
		}

		public override bool Validation() {
			if (mMatchIcons == null) {
				return false;
			}

			foreach (CIcon icon in mMatchIcons) {
				if (icon == null || !icon.IsIdle()) {
					return false;
				}
			}

			return true;
		}

		public int GetCountMatchIcon() {
			return mMatchIcons.Count;
		}

		public override void StartAction() {
			foreach (CIcon icon in mMatchIcons) {
				Wait(new CDestroy(icon));
			}
			CheckCompleteness();
		}


		public override int GetIndex() {
			return (int) EEvents.Match;
		}
	}
}