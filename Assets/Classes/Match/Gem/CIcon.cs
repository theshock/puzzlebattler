using Libraries;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Match.Gem {

	public class CIcon : MonoBehaviour {

		public EColor color { get; private set; }
		public EState state { get; private set; }

		public CField mField;
		public CCell mCell = CCell.zero;

		public void SetRandomColor() {
			int count = Enum.GetNames( typeof( EColor ) ).Length;

			color = (EColor) UnityEngine.Random.Range(0, count);

			GetComponent<Image>().sprite = mField.mConfig.mGems.GetCorrectSprite(color);
			UpdateName();
		}

		public void SetState (EState state) {
			this.SetState(state);
			UpdateName();
		}

		protected void UpdateName () {
			gameObject.name = String.Format("Gem {0} [{1}:{2}] {3}",
				color.ToString()[0],
				mCell.col,
				mCell.row,
				IsIdle() ? "" : "*"
			);
		}

		public bool IsIdle() {
			return (state == EState.Idle);
		}

		public bool HitTest(Vector2 coordinates) {
			var worldPoint = Camera.main.ScreenToWorldPoint((Vector3) coordinates);

			return collider2D.OverlapPoint((Vector2) worldPoint);
		}

		public bool IsNeighbour (CIcon target) {
			if (this == target) return false;

			int rowDistance = Mathf.Abs(mCell.row - target.mCell.row);
			int colDistance = Mathf.Abs(mCell.col - target.mCell.col);

			return (rowDistance == 1 && colDistance == 0) || (rowDistance == 0 && colDistance == 1);
		}

	}
}