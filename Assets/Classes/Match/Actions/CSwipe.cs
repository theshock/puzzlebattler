using Libraries;
using Libraries.ActionSystem;
using Match;
using Match.Gem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Actions {
	public class CSwipe : CCreating, IMoveObserver {

		public struct Config {
			public CField iconField;
			public CIcon selectedIcon;
			public CIcon targetedIcon;
			public CMatcher matcher;

			public bool IsValid () {
				return iconField    != null
					&& selectedIcon != null
					&& targetedIcon != null
					&& matcher      != null;
			}
		}

		protected Config mConfig;

		public CSwipe (Config config) {
			if (config.IsValid()) {
				this.mConfig = config;
			} else {
				throw new Exception("Config not valid");
			}
		}

		public override bool Validation() {
			return mConfig.selectedIcon.IsIdle()
				&& mConfig.targetedIcon.IsIdle();
		}

		public override void StartAction() {
			var field = mConfig.iconField;
			var move = new CMove(field.mConfig);

			CCell cell = mConfig.selectedIcon.mCell.Clone();

			field.SetMatrixCell(mConfig.targetedIcon.mCell, mConfig.selectedIcon);
			field.SetMatrixCell(cell, mConfig.targetedIcon);

			move.AddMove(mConfig.selectedIcon, field.GetIconCenterByCoord(mConfig.selectedIcon.mCell));
			move.AddMove(mConfig.targetedIcon, field.GetIconCenterByCoord(mConfig.targetedIcon.mCell));

			move.SetObserver(this);
		}

		public virtual void OnMoveEnd () {
			if (!IsCorrectSwipe()) {
				CreateSwipeBack();
			}
		}

		private void CreateSwipeBack () {
			Wait(new Actions.CSwipeBack(mConfig));
			CheckCompleteness();
		}

		private bool IsCorrectSwipe () {
			var matches = mConfig.matcher.FindMatches();

			if (matches.Count > 0) {
				foreach (List<CIcon> match in matches) {
					Wait(new Actions.CMatch(match));
				}
				CheckCompleteness();

				return true;
			} else {
				return false;
			}
		}

		public override int GetIndex() {
			return (int) EEvents.Swipe;
		}
	}
}