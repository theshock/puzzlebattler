using Libraries;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match.Gem;

namespace Match {

	public class CSearcher : CFieldHandler {

		public struct Move {
			public CIcon from, to;
		}

		private List<Move> moves;

		public CSearcher (CMatch controller) : base(controller) {}

		public void FindMoves() {
			moves = new List<Move>();

			for (int i = 0; i < GetSquare(); i++) {
				if (!IsOutOfHorisontalRange(i)) {
					SearchAllMoves(i, i+1, i+2);
				}

				if (!IsOutOfVeticalRange(i)) {
					SearchAllMoves(i, i+GetWidth(), i+GetWidth()*2);
				}
			}
		}

		protected void SearchAllMoves (int firstIndex, int secondIndex, int thirdIndex) {
			CIcon first  = mController.mView.mField.GetIconByIndex(firstIndex);
			CIcon second = mController.mView.mField.GetIconByIndex(secondIndex);
			CIcon third  = mController.mView.mField.GetIconByIndex(thirdIndex);

			SearchTargetMoves(first, second, third);
			SearchTargetMoves(third, first, second);
			SearchTargetMoves(second, third, first);
		}

		protected void SearchTargetMoves (CIcon first, CIcon second, CIcon target) {
			var field = mController.mView.mField;

			if (first.IconType == second.IconType) {
				foreach (CIcon cell in GetNeighbours(target)) {
					if (cell != first && cell != second && cell.IconType == first.IconType) {
						moves.Add(new Move(){
							from = target,
							to   = cell
						});
					}
				}
			}
		}

		public bool MovesExists () {
			return GetMoves().Count > 0;
		}

		public List<Move> GetMoves() {
			return moves;
		}

	}
}