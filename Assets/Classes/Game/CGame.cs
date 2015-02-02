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
			Debug.Log("==========");
			Debug.Log(new Point(1, 1));
			Debug.Log((Point) new Vector2(3, 2));
			Debug.Log((Vector2) new Point(4, 5));
			Debug.Log(new Point(4, 5) == new Point(4, 5));


			mMatch.mNotificationManager.addObserver((int)Match.EEvents.eMatch, this);
		}
	}
}