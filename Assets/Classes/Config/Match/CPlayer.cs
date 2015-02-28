using Match.Gem;
using UnityEngine;

namespace Config.Match {
	public class CPlayer : MonoBehaviour {
		public int mHealth  = 1000;
		public int mMatches = 3;

		public int mRedPower;
		public int mBluePower;
		public int mGreenPower;
		public int mPurplePower;
		public int mYellowPower;

		public int GetPower (EColor type) {
			switch (type) {
				case EColor.Red   : return mRedPower;
				case EColor.Blue  : return mBluePower;
				case EColor.Green : return mGreenPower;
				case EColor.Purple: return mPurplePower;
				case EColor.Yellow: return mYellowPower;
			}

			return 0;
		}
	}
}