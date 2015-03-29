using System.Collections;
using UnityEngine;

namespace Match {
	public class CAi {

		private CField field;
		private bool turning = false;

		public CAi (CField field) {
			this.field = field;
		}

		public void Play() {
			if (turning) return;

			turning = true;
			field.StartCoroutine(DelayedTurn());
		}

		protected float GetDelay() {
			return Random.Range(2f, 2.5f);
		}

		protected IEnumerator DelayedTurn () {
			yield return new WaitForSeconds(GetDelay());
			turning = false;

			if (!CGame.Model.opponent.CanAction()) yield break;

			var searcher = new CSearcher(field).FindMoves();

			if (searcher.MovesExists()) {
				var move = searcher.GetMoves()[0];

				field.actions.Add(new Actions.CSwipe(field, move.from, move.to));
			} else {
				Debug.Log("No moves for AI");
			}
		}

	}
}