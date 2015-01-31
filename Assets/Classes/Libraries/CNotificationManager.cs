using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Libraries {
	public class CNotificationManager {
		private Dictionary<int, ArrayList> mDictionary = new Dictionary<int, ArrayList>();

		public void addObserver(int aEvent, INotificationObserver aObserver) {
			if (mDictionary.ContainsKey(aEvent)) {
				mDictionary[aEvent].Add(aObserver);
			} else {
				ArrayList arr = new ArrayList();
				arr.Add(aObserver);

				mDictionary.Add(aEvent, arr);
			}

			return;
		}

		public void removeObserver(int aEvent, INotificationObserver aObserver) {
			try {
				mDictionary[aEvent].Remove(aObserver);
			}
			catch (System.Exception e) {
				Trace.TraceError(e.ToString());
			}


			return;
		}

		public void removeObserver(INotificationObserver aObserver) {
			foreach (KeyValuePair<int, ArrayList> pair in mDictionary) {
				foreach (ArrayList arr in mDictionary[pair.Key]) {
					try {
						arr.Remove(aObserver);
					}
					catch (System.Exception e) {
						Trace.TraceError(e.ToString());
					}
				}
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
				ArrayList observers = mDictionary[aEvent];

				foreach (INotificationObserver ob in observers) {
					ob.handleNotification(aEvent, aParam, this);
				}
			}

		}

	}
}