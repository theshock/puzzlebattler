using Match.Gem;
using UnityEngine;

namespace Config.Match {
	public class CPlayer : MonoBehaviour {
		public int mHealth = 1000;

		public int mRedPower;
		public int mBluePower;
		public int mGreenPower;
		public int mPurplePower;
		public int mYellowPower;

		public int GetPower (EType type) {
			switch (type) {
				case EType.Red   : return mRedPower;
				case EType.Blue  : return mBluePower;
				case EType.Green : return mGreenPower;
				case EType.Purple: return mPurplePower;
				case EType.Yellow: return mYellowPower;
			}

			return 0;
		}
	}
}