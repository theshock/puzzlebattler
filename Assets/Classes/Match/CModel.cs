using Etc;
using Libraries.ActionSystem;
using Match.Actions;
using Model;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Match {
	public class CModel {

		CField field;

		private int score = 0;

		public CModel (CField field) {
			CGame.Model.ActivateFirst();

			this.field = field;

			var actionEvents = field.actions.events;

			actionEvents.OnFinish(OnActionsFinished);
			actionEvents.OnBreak((int) EEvents.FreeFall  , OnFreeFallBreak  );
			actionEvents.OnEnd  ((int) EEvents.FreeFall  , OnFreeFallEnd    );
			actionEvents.OnEnd  ((int) EEvents.Match     , OnMatchEnd       );
			actionEvents.OnBegin((int) EEvents.Swipe     , OnSwipeBegin     );
			actionEvents.OnBegin((int) EEvents.SwipeBack , OnSwipeBackBegin );
		}

		public void OnFreeFallBreak (IAction action) {
			Character source = IsOpponent()
				? CGame.Instance.opponentCharacter
				: CGame.Instance.playerCharacter;

			Character target = !IsOpponent()
				? CGame.Instance.opponentCharacter
				: CGame.Instance.playerCharacter;

			Model.CPlayer targetModel = !IsOpponent()
				? CGame.Model.opponent
				: CGame.Model.player;

			if (score > 0) {
				source.SetState(
					score >= CGame.Config.attackStrongScore
						? Character.States.AttackStrong
						: Character.States.AttackWeak,

					() => {
						CGame.Sounds.ouch.Play();
						target.SetState( Character.States.Damaged );
						targetModel.AddDamage(score);
					}
				);
			}

			CGame.Instance.CheckActive();
		}

		public void OnFreeFallEnd(IAction action) {
			var matcher = new CMatcher(field).FindMatches();

			field.actions.Add(new CAutoMatch(matcher.GetMatches()));
		}

		public void OnActionsFinished () {
			field.actions.Add(new CFreeFall(field));
		}

		public void OnMatchEnd (IAction action) {
			var match = action as Actions.CMatch;
			var count = match.GetCountMatchIcon();
			score += CalculateScore(count);

			CGame.Instance.OnMatchEnd(match.GetMatchIconType());
		}

		public void OnSwipeBegin (IAction action) {
			score = 0;
			if (IsOpponent()) {
				CGame.Instance.opponentCharacter.SetState( Character.States.AttackIdle );
			} else {
				CGame.Instance.playerCharacter.SetState( Character.States.AttackIdle );
			}
			CGame.Model.GetActivePlayer().UseAction();
		}

		public void OnSwipeBackBegin (IAction action) {
			if (IsOpponent()) {
				CGame.Instance.opponentCharacter.Idle();
			} else {
				CGame.Instance.playerCharacter.Idle();
			}
			CGame.Model.GetActivePlayer().RestoreAction();
		}

		public bool IsOpponent() {
			return CGame.Model.GetActivePlayer() == CGame.Model.opponent;
		}

		private int CalculateScore (int count) {
			if (count < 4) {
				return 3;
			} else {
				return 3 + (count - 3) * count / 2;
			}
		}
	}
}