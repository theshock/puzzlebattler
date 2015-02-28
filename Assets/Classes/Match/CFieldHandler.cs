using Match.Gem;
using System.Collections.Generic;

namespace Match {
	public abstract class CFieldHandler {
		public CMatch mController;

		public CFieldHandler (CMatch controller) {
			this.mController = controller;
		}

		protected int GetWidth () {
			return mController.mView.mField.mColumns;
		}

		protected int GetHeight () {
			return mController.mView.mField.mRows;
		}

		protected int GetSquare () {
			return GetWidth() * GetHeight();
		}

		protected bool IsOutOfHorisontalRange (int index) {
			return index % GetWidth() >= GetWidth() - 2;
		}

		protected bool IsOutOfVeticalRange (int index) {
			return index >= (GetSquare() - GetWidth() * 2);
		}

		public List<CIcon> GetNeighbours (CIcon icon) {
			var list = new List<CIcon>();
			var field = mController.mView.mField;

			if (icon.mCell.col > 0) {
				list.Add(field.GetIconByIndex(icon.mIndex - 1));
			}
			if (icon.mCell.col < field.mColumns - 1) {
				list.Add(field.GetIconByIndex(icon.mIndex + 1));
			}
			if (icon.mCell.row > 0) {
				list.Add(field.GetIconByIndex(icon.mIndex - GetWidth()));
			}
			if (icon.mCell.row < field.mRows - 1) {
				list.Add(field.GetIconByIndex(icon.mIndex + GetWidth()));
			}

			return list;
		}

		protected bool IsMatch(int first, int second, int third) {
			var icons = new List<CIcon>();
			var field = mController.mView.mField;

			icons.Add( field.GetIconByIndex(first) );
			icons.Add( field.GetIconByIndex(second) );
			icons.Add( field.GetIconByIndex(third) );

			return IsAllIconsActive(icons) && field.AreIconsTheSame(icons);
		}

		protected bool IsAllIconsActive (List<CIcon> icons) {
			foreach (CIcon icon in icons) {
				if(icon && !icon.IsIdle()) {
					return false;
				}
			}

			return true;
		}
	}
}