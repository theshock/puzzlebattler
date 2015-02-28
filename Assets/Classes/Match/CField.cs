using Libraries;
using Match.Gem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match {

	public class CField : MonoBehaviour {

		public Libraries.ActionSystem.CManager actions;

		public Text playerText;
		public Text opponentText;

		private CMatcher matcher;
		private CInput input;
		private CModel model;

		public void Start() {
			Game.Instance.mModel.ActivateFirst();

			actions = new Libraries.ActionSystem.CManager();
			input = new CInput(this);
			model = new CModel(this);
			matcher = new CMatcher(this);

			CreateIcons();
		}

		public void Update () {
			if (input != null) {
				input.Update();
			}
		}

		protected Config.Match.CField config {
			get { return Game.Config.match.field; }
		}

		public int height {
			get { return config.rows; }
		}

		public int width {
			get { return config.columns; }
		}

		public CIcon [,] icons;

		protected void CreateIcons () {
			icons = new CIcon[height, width];

			for (int r = 0; r < height; r++) {
				for (int c = 0; c < width; c++) {
					RegisterIcon(new CCell(r, c));
				}
			}
		}

		public void SetIconAt(CCell position, CIcon icon) {
			icons[position.row, position.col] = icon;
			icon.cell.Set(position);
		}

		public CIcon GetIconAt(CCell position) {
			return icons[position.row, position.col];
		}

		public Vector3 GetIconCenterByCell(CCell cell) {
			return new Vector3(
				config.from.x + cell.col * config.offset.x,
				config.from.y + cell.row * config.offset.y,
				this.transform.position.z
			);
		}

		public CIcon GetIconByPoint(Vector2 position) {
			foreach (CIcon icon in icons) {
				if (icon.HitTest(position)) {
					return icon;
				}
			}

			return null;
		}

		public List<List<CIcon>> FindMatches () {
			return matcher.FindMatches();
		}

		private CIcon RegisterIcon(CCell position) {
			GameObject gameObject = Instantiate(Game.Config.match.gems.prefab) as GameObject;
			gameObject.transform.SetParent(this.transform);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.position = GetIconCenterByCell(position);

			CIcon icon = gameObject.GetComponent<CIcon>();

			SetIconAt(position, icon);

			do {
				icon.SetRandomColor();
			} while (CanCreateMatch(icon));

			return icon;
		}

		private bool CanCreateMatch (CIcon icon) {
			CCell cell = icon.cell;

			if (cell.col >= 2
				&& GetIconAt(new CCell(cell.row, cell.col-1)).IsMatchable(icon)
				&& GetIconAt(new CCell(cell.row, cell.col-2)).IsMatchable(icon)
			) {
				return true;
			}

			if (cell.row >= 2
				&& GetIconAt(new CCell(cell.row-1, cell.col)).IsMatchable(icon)
				&& GetIconAt(new CCell(cell.row-2, cell.col)).IsMatchable(icon)
			) {
				return true;
			}

			return false;
		}

	}
}