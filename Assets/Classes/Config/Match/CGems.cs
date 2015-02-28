using Match;
using Match.Gem;
using UnityEngine;

namespace Config.Match {
	public class CGems : MonoBehaviour {
		public float mMovingTime = 0.3f;

		public GameObject mPrefab;

		public int mChanceRed;
		public int mChanceBlue;
		public int mChanceGreen;
		public int mChancePurple;
		public int mChanceYellow;

		public Sprite mImageRed;
		public Sprite mImageBlue;
		public Sprite mImageYellow;
		public Sprite mImageGreen;
		public Sprite mImagePurple;

		public float GetChance (EColor type) {
			return (float) GetChanceValue(type) / (float) GetChanceSum();
		}

		private int GetChanceValue (EColor type) {
			switch (type) {
				case EColor.Red   : return mChanceRed;
				case EColor.Blue  : return mChanceBlue;
				case EColor.Green : return mChanceGreen;
				case EColor.Purple: return mChancePurple;
				case EColor.Yellow: return mChanceYellow;
			}

			return 0;
		}

		private int GetChanceSum () {
			return mChanceRed + mChanceBlue + mChanceGreen + mChancePurple + mChanceYellow;
		}

		public Sprite GetCorrectSprite (EColor type) {
			switch (type) {
				case EColor.Red   : return mImageRed;
				case EColor.Blue  : return mImageBlue;
				case EColor.Green : return mImageGreen;
				case EColor.Purple: return mImagePurple;
				case EColor.Yellow: return mImageYellow;
			}

			return null;
		}
	}
}