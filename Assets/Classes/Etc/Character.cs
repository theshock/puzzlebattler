using UnityEngine;

namespace Etc {
	public class Character : MonoBehaviour {

		public enum States {
			AttackIdle,
			AttackStrong,
			AttackWeak,
			CastDamage,
			CastHeal,
			Damaged,
			Death,
			Idle,
			Victory
		}


		public delegate void OnFrame();
		public event OnFrame onIdle;
		public event OnFrame onTrigger;

		protected const string KEY = "State";
		protected Animator animator;
		protected States state;

		public void Awake () {
			animator = GetComponent<Animator>();
		}

		public void Start () {
			Idle();
		}

		public void SetState (States state) {
			this.state = state;
			animator.SetInteger(KEY, (int) state);

			if (onIdle != null && state == States.Idle) {
				onIdle.Invoke();
			}
		}

		public void Idle () {
			SetState( States.Idle );
		}

		public void OnAnimationComplete () {
			Idle();
		}

		public void OnAnimationTrigger () {
			if (onTrigger != null) {
				onTrigger.Invoke();
			}
		}

		public bool IsIdle () {
			return state == States.Idle;
		}

	}
}