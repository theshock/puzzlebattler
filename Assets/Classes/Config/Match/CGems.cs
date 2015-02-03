using Match;
using UnityEngine;

namespace Config.Match {
	public class CGems : MonoBehaviour {
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

		public float GetChance (EIconType type) {
			return (float) GetChanceValue(type) / (float) GetChanceSum();
		}

		private int GetChanceValue (EIconType type) {
			switch (type) {
				case EIconType.eRed   : return mChanceRed;
				case EIconType.eBlue  : return mChanceBlue;
				case EIconType.eGreen : return mChanceGreen;
				case EIconType.ePurple: return mChancePurple;
				case EIconType.eYellow: return mChanceYellow;
			}

			return 0;
		}

		private int GetChanceSum () {
			return mChanceRed + mChanceBlue + mChanceGreen + mChancePurple + mChanceYellow;
		}

		public Sprite GetCorrectSprite (EIconType type) {
			switch (type) {
				case EIconType.eRed   : return mImageRed;
				case EIconType.eBlue  : return mImageBlue;
				case EIconType.eGreen : return mImageGreen;
				case EIconType.ePurple: return mImagePurple;
				case EIconType.eYellow: return mImageYellow;
			}

			return null;
		}
	}
}