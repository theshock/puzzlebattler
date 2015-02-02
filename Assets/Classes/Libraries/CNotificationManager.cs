using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Libraries {
	public class CNotificationManager {
		private Dictionary<int, List<INotificationObserver>> mDictionary = new Dictionary<int, List<INotificationObserver>>();

		public void addObserver(int aEvent, INotificationObserver aObserver) {

			if (!mDictionary.ContainsKey(aEvent)) {
				mDictionary.Add(aEvent, new List<INotificationObserver>());
			}
			mDictionary[aEvent].Add(aObserver);

		}

		public void removeObserver(int aEvent, INotificationObserver aObserver) {
			try {
				mDictionary[aEvent].Remove(aObserver);
			} catch (System.Exception e) {
				Trace.TraceError(e.ToString());
			}
		}

		public void removeObserver(INotificationObserver aObserver) {
			foreach (KeyValuePair<int, List<INotificationObserver>> pair in mDictionary) {
				removeObserver(pair.Key, aObserver);
			}
		}

		public void removeObservers(int aEvent) {
			mDictionary.Remove(aEvent);
		}

		public void removeAllObservers() {
			mDictionary.Clear();
		}

		public void notify(int aEvent, Object aParam) {
			if (mDictionary.ContainsKey(aEvent)) {
				List<INotificationObserver> observers = mDictionary[aEvent];

				foreach (INotificationObserver ob in observers) {
					ob.handleNotification(aEvent, aParam, this);
				}
			}

		}

	}
}