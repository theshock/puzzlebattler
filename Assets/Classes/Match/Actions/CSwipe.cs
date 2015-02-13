using Match;
using Match.Gem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libraries.ActionSystem;

namespace Match.Actions {
	public class CSwipe : CCreating, IMoveObserver {

		public struct Config {
			public CField iconField;
			public CIcon selectedIcon;
			public CIcon targetedIcon;
			public CPlayer owner;
			public CMatcher matcher;

			public bool IsValid () {
				return iconField    != null
					&& selectedIcon != null
					&& targetedIcon != null
					&& owner        != null
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
			return mConfig.selectedIcon.IsActionReady()
				&& mConfig.targetedIcon.IsActionReady()
				&& mConfig.owner.active
				&& mConfig.owner.matches > 0;
		}

		public override void StartAction() {
			mConfig.iconField
				.SwipeIcons(mConfig.selectedIcon, mConfig.targetedIcon)
				.SetObserver(this);
		}

		public virtual void OnMoveEnd () {
			if (!IsCorrectSwipe()) {
				CreateSwipeBack();
			}
		}

		private void CreateSwipeBack () {
			Wait(new Actions.CSwipeBack(mConfig));
		}

		private bool IsCorrectSwipe () {
			var matches = mConfig.matcher.FindMatches();

			if (matches.Count > 0) {
				foreach (List<CIcon> match in matches) {
					Wait(new Actions.CMatch(match));
				}

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