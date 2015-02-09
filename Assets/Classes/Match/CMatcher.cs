using Libraries;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match.Gem;

namespace Match {

	public class CMatcher : CFieldHandler {

		public CMatcher (CMatch controller) : base(controller) {}

		public List<List<int>> FindMatches() {
			var union = new CWeightedUnion(GetSquare());

			for (int i = 0; i < GetSquare(); i++) {
				if (IsHorizontalMatch(i)) {
					union.unite(i, i + 1);
					union.unite(i, i + 2);
				}

				if (IsVerticalMatch(i)) {
					union.unite(i, i + GetWidth());
					union.unite(i, i + GetWidth()*2);
				}
			}

			return union.GetTrees();
		}

		protected bool IsHorizontalMatch (int index) {
			if (IsOutOfHorisontalRange(index)) {
				return false;
			} else {
				return IsMatch(index, index + 1, index + 2);
			}
		}

		protected bool IsVerticalMatch (int index) {
			if (IsOutOfVeticalRange(index)) {
				return false;
			} else {
				return IsMatch(index, index + GetWidth()*1, index + GetWidth()*2);
			}
		}

		public bool MatchesExists () {
			return FindMatches().Count > 0;
		}

	}
}