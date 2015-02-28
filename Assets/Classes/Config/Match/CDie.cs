using Match.Gem;
using UnityEngine;

namespace Config.Match {
	public class CDie : MonoBehaviour {
		public GameObject[] prefabs;

		public GameObject GetPrefab (EColor type) {
			return prefabs[(int)type];
		}
	}
}