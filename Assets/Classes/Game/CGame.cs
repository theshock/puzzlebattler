using UnityEngine;
using System.Collections;
using Libraries;

namespace Game {
	public class CGame : MonoBehaviour {
		public Config.CConfig mConfig;
		public Model.CGame mModel;
		public Match.CMatch mMatch;

		public void Start () {
			mModel = new Model.CGame(mConfig);
		}
	}
}