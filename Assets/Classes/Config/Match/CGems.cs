using Config;
using Match;
using Match.Gem;
using UnityEngine;

namespace Config.Match  {
	public class CGems : Config.CColor {
		public float movingTime = 0.3f;

		public GameObject prefab;

		public int chanceBlue;
		public int chanceGreen;
		public int chancePurple;
		public int chanceRed;
		public int chanceYellow;

		public float GetChance (EColor type) {
			return (float) GetChanceValue(type) / (float) GetChanceSum();
		}

		private int GetChanceValue (EColor type) {
			switch (type) {
				case EColor.Blue  : return chanceBlue;
				case EColor.Green : return chanceGreen;
				case EColor.Purple: return chancePurple;
				case EColor.Red   : return chanceRed;
				case EColor.Yellow: return chanceYellow;
			}

			return 0;
		}

		private int GetChanceSum () {
			return chanceBlue + chanceGreen + chancePurple + chanceRed + chanceYellow;
		}
	}
}