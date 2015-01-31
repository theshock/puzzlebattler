using UnityEngine;
using System.Collections;

namespace Libraries {
	public interface INotificationObserver {
		void handleNotification(int aEvent, Object aParam, CNotificationManager aManager);
	}
}
