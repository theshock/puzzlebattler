using UnityEngine;

namespace Audio {
	public class Sound : MonoBehaviour {

		private AudioSource audioCached;

		public void Awake () {
			audioCached = audio;
		}

		public void Play () {
			if (!audioCached.isPlaying && CGame.Config.sound) {
				audioCached.Play();
			}
		}

	}
}