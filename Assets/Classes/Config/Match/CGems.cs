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

		public float GetChance (EType type) {
			return (float) GetChanceValue(type) / (float) GetChanceSum();
		}

		private int GetChanceValue (EType type) {
			switch (type) {
				case EType.Red   : return mChanceRed;
				case EType.Blue  : return mChanceBlue;
				case EType.Green : return mChanceGreen;
				case EType.Purple: return mChancePurple;
				case EType.Yellow: return mChanceYellow;
			}

			return 0;
		}

		private int GetChanceSum () {
			return mChanceRed + mChanceBlue + mChanceGreen + mChancePurple + mChanceYellow;
		}

		public Sprite GetCorrectSprite (EType type) {
			switch (type) {
				case EType.Red   : return mImageRed;
				case EType.Blue  : return mImageBlue;
				case EType.Green : return mImageGreen;
				case EType.Purple: return mImagePurple;
				case EType.Yellow: return mImageYellow;
			}

			return null;
		}
	}
}