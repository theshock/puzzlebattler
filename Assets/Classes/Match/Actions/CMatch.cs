using Match.Gem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libraries.ActionSystem;

namespace Match.Actions {
	public class CMatch : CCreating {
		private List<CIcon> icons;

		public CMatch (List<CIcon> icons) {
			this.icons = icons;
		}

		public EColor GetMatchIconType() {
			return icons[0].color;
		}

		public override bool Validation() {
			if (icons == null) {
				return false;
			}

			foreach (CIcon icon in icons) {
				if (icon == null || !icon.IsIdle()) {
					return false;
				}
			}

			return true;
		}

		public int GetCountMatchIcon() {
			return icons.Count;
		}

		public override void StartAction() {
			foreach (CIcon icon in icons) {
				// todo: fix null
				Wait(new CDestroy(icon, null));
			}
			CheckCompleteness();
		}


		public override int GetIndex() {
			return (int) EEvents.Match;
		}
	}
}