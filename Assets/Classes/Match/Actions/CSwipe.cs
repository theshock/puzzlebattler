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

		protected Config config;

		public CSwipe (Config config) {
			if (config.IsValid()) {
				this.config = config;
			} else {
				throw new Exception("Config not valid");
			}
		}

		public override bool Validation() {
			return config.selectedIcon.IsIdle()
				&& config.targetedIcon.IsIdle();
		}

		public override void StartAction() {
			var field = config.iconField;
			var move = new CMove();

			CCell cell = config.selectedIcon.cell.Clone();

			field.SetIconAt(config.targetedIcon.cell, config.selectedIcon);
			field.SetIconAt(cell, config.targetedIcon);

			move.AddMove(config.selectedIcon, field.GetIconCenterByCell(config.selectedIcon.cell));
			move.AddMove(config.targetedIcon, field.GetIconCenterByCell(config.targetedIcon.cell));

			move.SetObserver(this);
		}

		public virtual void OnMoveEnd () {
			if (!IsCorrectSwipe()) {
				CreateSwipeBack();
			}
		}

		private void CreateSwipeBack () {
			Wait(new Actions.CSwipeBack(config));
			CheckCompleteness();
		}

		private bool IsCorrectSwipe () {
			var matches = config.matcher.FindMatches();

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