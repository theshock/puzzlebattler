using Libraries;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match {

	public class CSearcher : INotificationObserver {
		public CMatch mController { get; set; }

		public void handleNotification(int aEvent, Object aParam, CNotificationManager aManager) { }

		public void subscribeNotification(CNotificationManager aCenter) { }

		public bool isHaveMatches() {
			return false;
		}

		public ArrayList findMatches(bool aOnlyOpenedCells = false) {
			ArrayList matches = new ArrayList();
			CField field = mController.mView.mField;

			int col_field = field.mColumns;
			int row_field = field.mRows;

			CWeightedUnion weightedUnionInstance = new CWeightedUnion(col_field * row_field);

			for (int i = 0; i < col_field * row_field / 2; i += col_field) {
				for (int j = i; j < i + (col_field - 2); j++) {
					ArrayList icons = new ArrayList();

					icons.Add(field.getIconByIndex(j));
					icons.Add(field.getIconByIndex(j + 1));
					icons.Add(field.getIconByIndex(j + 2));

					if (aOnlyOpenedCells) {
						for (int index = 0; index < icons.Count; index++) {
							CIcon icon = icons[index] as CIcon;
							if (icon && !icon.getIsReadyAction()) {
								continue;
							}
						}
					}

					if (field.isTheSameIconOne(icons)) {
						weightedUnionInstance.unite(j, (j + 1));
						weightedUnionInstance.unite(j, (j + 2));
					}
				}
			}
			//
			for (int i = 0; i < row_field / 2; i++) {
				for (int j = i; j < i + (col_field * (row_field / 2 - 2)); j += col_field) {
					ArrayList icons = new ArrayList();

					icons.Add(field.getIconByIndex(j));
					icons.Add(field.getIconByIndex(j + (col_field)));
					icons.Add(field.getIconByIndex(j + (col_field * 2)));

					if (aOnlyOpenedCells) {
						for (int index = 0; index < icons.Count; index++) {
							CIcon icon = icons[index] as CIcon;
							if (icon && !icon.getIsReadyAction()) {
								continue;
							}
						}
					}

					if (field.isTheSameIconOne(icons)) {
						weightedUnionInstance.unite(j, (j + col_field));
						weightedUnionInstance.unite(j, (j + col_field * 2));
					}
				}
			}


			if (weightedUnionInstance.HasUnions()) {
				List<List<int>> foundMatches = weightedUnionInstance.GetTrees();

				foreach (List<int> branch in foundMatches) {
					matches.Add(new ArrayList(branch));
				}

			}

			return matches;
		}
	}
}