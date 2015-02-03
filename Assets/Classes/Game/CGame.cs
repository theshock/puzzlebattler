using UnityEngine;
using System.Collections;
using Libraries;

namespace Game {
	public class CGame : MonoBehaviour, INotificationObserver {
		public CNotificationManager mNotificationManager = new CNotificationManager();
		public Match.CMatch mMatch;
		public Config.CConfig mConfig;

		public void handleNotification(int aEvent, Object aParam, CNotificationManager aManager) {
		}

		int calculateMult(int aCountMatch) {
			if (aCountMatch < 4) {
				return 3;
			} else {
				return ((aCountMatch - 1) * aCountMatch) / 2 - aCountMatch - 3;
			}
		}

		void Start() {
			mMatch.mNotificationManager.addObserver((int)Match.EEvents.eMatch, this);
		}
	}
}