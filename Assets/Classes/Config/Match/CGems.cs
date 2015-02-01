using Match;
using UnityEngine;

namespace Config.Match {
	public class CGems : MonoBehaviour {
		public GameObject mPrefab;

		public int mRedChance;
		public int mBlueChance;
		public int mGreenChance;
		public int mPurpleChance;
		public int mYellowChance;

		public float GetChance (EIconType type) {
			return (float) GetChanceValue(type) / (float) GetChanceSum();
		}

		private int GetChanceValue (EIconType type) {
			switch (type) {
				case EIconType.eRed   : return mRedChance;
				case EIconType.eBlue  : return mBlueChance;
				case EIconType.eGreen : return mGreenChance;
				case EIconType.ePurple: return mPurpleChance;
				case EIconType.eYellow: return mYellowChance;
			}

			return 0;
		}

		private int GetChanceSum () {
			return mRedChance + mBlueChance + mGreenChance + mPurpleChance + mYellowChance;
		}
	}
}