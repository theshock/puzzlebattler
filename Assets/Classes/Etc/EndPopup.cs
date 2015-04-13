using UnityEngine;
using UnityEngine.UI;

namespace Etc {
	public class EndPopup : MonoBehaviour {

		public Button restart;
		public Button close;
		public Text title;
		public Text descr;
		public GameObject overlay;

		public delegate void OnRestart ();

		private OnRestart onRestart;

		public void Awake () {
			restart.onClick.AddListener( Restart );
			close  .onClick.AddListener( Restart );
			Hide();
		}

		public void ShowVictory (OnRestart onRestart) {
			title.text = "Level completed";
			descr.text = "You are winner!";

			this.Show(onRestart);
		}


		public void ShowDefeat (OnRestart onRestart) {
			title.text = "Level uncompleted";
			descr.text = "You are loser";

			this.Show(onRestart);
		}

		private void Show (OnRestart onRestart) {
			this.onRestart = onRestart;
			overlay.SetActive(true);
		}

		private void Hide () {
			overlay.SetActive(false);
		}

		private void Restart () {
			Hide();

			if (onRestart != null) {
				onRestart();
				onRestart = null;
			}
		}


	}
}