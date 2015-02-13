using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Libraries {
	public class CInput {

		private bool mIsBlocked = false;
		private bool mIsClickStart = false;
		private IInputObserver mCurrentObserver = null;
		private Dictionary<int, List<IInputObserver>> mDictionary = new Dictionary<int, List<IInputObserver>>();

		public CInput () {
			Input.simulateMouseWithTouches = true;
		}

		public void registerObserver(IInputObserver aObserver, int aPriority) {
			if (!mDictionary.ContainsKey(aPriority)) {
				mDictionary.Add(aPriority, new List<IInputObserver>());
			}
			mDictionary[aPriority].Add(aObserver);
		}

		public void unregisterObserver(int aPriority, IInputObserver aObserver) {
			try {
				mDictionary[aPriority].Remove(aObserver);
			} catch (System.Exception e) {
				Trace.TraceError(e.ToString());
			}
		}

		public void unregisterObserver(IInputObserver aObserver) {
			foreach (KeyValuePair<int, List<IInputObserver>> pair in mDictionary) {
				unregisterObserver(pair.Key, aObserver);
			}
		}


		public void Check() {
			if (Input.touchCount == 1) {
				HandleTouch(Input.GetTouch(0));
			} else if ( Input.GetMouseButton(0) ) {
				HandleMouse((Vector2) Input.mousePosition);
			} else if (mIsClickStart) {
				Reset();
			}
		}

		public void Block() {
			mIsBlocked = true;
		}

		private void Reset() {
			mIsClickStart = false;
			mIsBlocked = false;
			mCurrentObserver = null;
		}

		private void HandleMouse(Vector2 mousePosition) {
			if (mIsClickStart) {
				OnMove(mousePosition);
			} else {
				mIsClickStart = true;
				OnBegin(mousePosition);
			}
		}

		private void HandleTouch(Touch aTouch) {
			switch (aTouch.phase) {
				case TouchPhase.Began: OnBegin(aTouch.position); break;
				case TouchPhase.Moved: OnMove (aTouch.position); break;
				case TouchPhase.Ended: OnEnd  (aTouch.position); break;
			}
		}

		private void OnBegin(Vector2 aPosition) {
			foreach (KeyValuePair<int, List<IInputObserver>> pair in mDictionary) {
				for (int i = 0; i < pair.Value.Count; i++) {
					if (pair.Value[i].OnInputBegin(aPosition)) {
						mCurrentObserver = pair.Value[i];
						return;
					}
				}
			}
		}

		private void OnMove(Vector2 aPosition) {
			if (!mIsBlocked && mCurrentObserver != null) {
				mCurrentObserver.OnInputMove(aPosition);
			}
		}

		private void OnEnd(Vector2 aPosition) {
			if (mCurrentObserver != null) {
				mCurrentObserver.OnInputEnd(aPosition);
			}

			Reset();
		}

	}
}