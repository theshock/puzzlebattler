using Match.Gem;
using UnityEngine;

namespace Config {
	public class CConfig : MonoBehaviour {

		public Match.CMatch match;

		public CSkill heal;
		public CSkill fireball;

		public int attackStrongScore = 10;

		public float turnTime = 60f;


	}
}