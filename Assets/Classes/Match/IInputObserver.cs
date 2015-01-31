using UnityEngine;

namespace Match {

	public interface IInputObserver {
		void OnInputBegin(Vector2 position);
		void OnInputMove (Vector2 position);
		void OnInputEnd  ();
	}

}