using Match.Gem;
using UnityEngine;

namespace Config {
	public class CConfig : MonoBehaviour {

		public Match.CMatch match;

		public CGoals goals;
		public CSkill heal;
		public CSkill fireball;

		public bool sound = true;

		public int attackStrongScore = 10;

		public float turnTime = 60f;
		public int opponentsCount = 3;
		public float fadeOutTime = 2.0f;
		public float fadeInTime = 1.0f;


	}
}