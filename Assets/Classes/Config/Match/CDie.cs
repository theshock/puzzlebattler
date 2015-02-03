using Match;
using UnityEngine;

namespace Config.Match {
	public class CDie : MonoBehaviour {
		public GameObject[] mPrefabs;


		public GameObject GetPrefab (EIconType type) {
			return mPrefabs[(int)type];
		}
	}
}