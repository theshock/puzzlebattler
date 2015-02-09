using Libraries;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match.Gem;

namespace Match {

	public class CSearcher {
		public CMatch mController;

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

		public bool MatchesExists () {
			return FindMatches().Count > 0;
		}

		private int GetWidth () {
			return mController.mView.mField.mColumns;
		}

		private int GetHeight () {
			return mController.mView.mField.mRows;
		}

		private int GetSquare () {
			return GetWidth() * GetHeight() / 2;
		}

		private bool IsOutOfHorisontalRange (int index) {
			return index % GetWidth() >= GetWidth() - 2;
		}

		private bool IsOutOfVeticalRange (int index) {
			return index >= (GetSquare() - GetWidth() * 2);
		}

		private bool IsHorizontalMatch (int index) {
			if (IsOutOfHorisontalRange(index)) {
				return false;
			} else {
				return IsMatch(index, index + 1, index + 2);
			}
		}

		private bool IsVerticalMatch (int index) {
			if (IsOutOfVeticalRange(index)) {
				return false;
			} else {
				return IsMatch(index, index + GetWidth()*1, index + GetWidth()*2);
			}
		}

		private bool IsMatch(int first, int second, int third) {
			var icons = new List<CIcon>();
			var field = mController.mView.mField;

			icons.Add( field.GetIconByIndex(first) );
			icons.Add( field.GetIconByIndex(second) );
			icons.Add( field.GetIconByIndex(third) );

			return IsAllIconsActive(icons) && field.IsIconsTheSame(icons);
		}

		private bool IsAllIconsActive (List<CIcon> icons) {
			foreach (CIcon icon in icons) {
				if(icon && !icon.IsActionReady()) {
					return false;
				}
			}

			return true;
		}

	}
}