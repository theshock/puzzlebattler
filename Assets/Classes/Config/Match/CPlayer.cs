using Match;
using UnityEngine;

namespace Config.Match {
	public class CPlayer : MonoBehaviour {
		public int mHealth = 1000;

		public int mRedPower;
		public int mBluePower;
		public int mGreenPower;
		public int mPurplePower;
		public int mYellowPower;

		public int GetPower (EIconType type) {
			switch (type) {
				case EIconType.eRed   : return mRedPower;
				case EIconType.eBlue  : return mBluePower;
				case EIconType.eGreen : return mGreenPower;
				case EIconType.ePurple: return mPurplePower;
				case EIconType.eYellow: return mYellowPower;
			}

			return 0;
		}
	}
}