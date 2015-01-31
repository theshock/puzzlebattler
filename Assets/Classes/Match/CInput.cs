using UnityEngine;

namespace Match {
	public class CInput {

		private bool mIsBlocked = false;
		private bool mIsClickStart = false;
		private IInputObserver mObserver;

		public CInput (IInputObserver observer) {
			mObserver = observer;
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

		public void Block () {
			mIsBlocked = true;
		}

		private void Reset() {
			mIsClickStart = false;
			mIsBlocked = false;
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
				case TouchPhase.Began:
					OnBegin(aTouch.position);
					break;

				case TouchPhase.Moved:
					OnMove(aTouch.position);
					break;

				case TouchPhase.Ended:
					OnEnd();
					break;
			}
		}

		private void OnBegin(Vector2 aPosition) {
			mObserver.OnInputBegin(aPosition);
		}

		private void OnMove(Vector2 aPosition) {
			if (!mIsBlocked) {
				mObserver.OnInputMove(aPosition);
			}
		}

		private void OnEnd () {
			Reset();
			mObserver.OnInputEnd();
		}

	}
}