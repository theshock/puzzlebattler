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

		void Start() {
			mMatch.mNotificationManager.addObserver((int)Match.EEvents.Match, this);
		}
	}
}