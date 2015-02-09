using Match.Gem;
using UnityEngine;

namespace Config.Match {
	public class CDie : MonoBehaviour {
		public GameObject[] mPrefabs;


		public GameObject GetPrefab (EType type) {
			return mPrefabs[(int)type];
		}
	}
}