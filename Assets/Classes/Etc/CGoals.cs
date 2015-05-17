using Config;
using Match.Gem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Etc {
	public class CGoals : MonoBehaviour {


		private class CGoal {
			public int max = 0;
			public int current = 0;
			public EColor color;
			public GameObject gameObject;
			public Text text;
			public SpriteRenderer renderer;

			private void SetView (bool isOn) {
				var color = renderer.material.color;
				color.a = isOn ? 1.0f : 0.3f;
				renderer.material.color = color;
			}

			public bool IsComplete () {
				return current >= max;
			}

			public void UpdateView () {
				text.text = "" + (current > max ? 0 : max - current);
				SetView( IsComplete() );
			}

			public void Increase () {
				current++;
				UpdateView();
			}

			public void Reset () {
				current = 0;
				UpdateView();
			}

		}

		public GameObject[] objects;
		public Text[] texts;
		public Config.CGoals config;
		public Config.CColor colors;
		private List<CGoal> goals = new List<CGoal>();

		public void Start () {

			int i = 0;

			foreach (Config.CGoal cfg in config.goals) {
				if (!cfg.isActive) {
					objects[i].SetActive(false);
					++i;
					continue;
				}

				var goal = new CGoal();
				goal.max        = cfg.count;
				goal.color      = cfg.color;
				goal.gameObject = objects[i];
				goal.text       = texts[i];
				goal.renderer   = goal.gameObject.GetComponent<SpriteRenderer>();
				goal.renderer.sprite = colors.GetCorrectSprite(cfg.color);
				goal.UpdateView();

				goals.Add(goal);
				++i;
			}

			if (i < objects.Length) {
				Debug.Log("Required goals: " + objects.Length);
			}
		}

		public void OnMatch (EColor color) {
			foreach (CGoal goal in goals) {
				if (goal.color == color) {
					goal.Increase();
				}
			}
		}

		public void Reset () {
			foreach (CGoal goal in goals) {
				goal.Reset();
			}
		}

		public bool IsComplete() {
			foreach (CGoal goal in goals) {
				if (goal.IsComplete() == false) {
					return false;
				}
			}

			return true;
		}

	}
}