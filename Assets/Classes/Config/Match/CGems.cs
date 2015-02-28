using Match;
using Match.Gem;
using UnityEngine;

namespace Config.Match {
	public class CGems : MonoBehaviour {
		public float movingTime = 0.3f;

		public GameObject prefab;

		public int chanceRed;
		public int chanceBlue;
		public int chanceGreen;
		public int chancePurple;
		public int chanceYellow;

		public Sprite imageRed;
		public Sprite imageBlue;
		public Sprite imageYellow;
		public Sprite imageGreen;
		public Sprite imagePurple;

		public float GetChance (EColor type) {
			return (float) GetChanceValue(type) / (float) GetChanceSum();
		}

		private int GetChanceValue (EColor type) {
			switch (type) {
				case EColor.Red   : return chanceRed;
				case EColor.Blue  : return chanceBlue;
				case EColor.Green : return chanceGreen;
				case EColor.Purple: return chancePurple;
				case EColor.Yellow: return chanceYellow;
			}

			return 0;
		}

		private int GetChanceSum () {
			return chanceRed + chanceBlue + chanceGreen + chancePurple + chanceYellow;
		}

		public Sprite GetCorrectSprite (EColor type) {
			switch (type) {
				case EColor.Red   : return imageRed;
				case EColor.Blue  : return imageBlue;
				case EColor.Green : return imageGreen;
				case EColor.Purple: return imagePurple;
				case EColor.Yellow: return imageYellow;
			}

			return null;
		}
	}
}