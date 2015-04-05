using System;
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
		public event OnFrame onTriggerSingle;

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

		public void SetState (States state, OnFrame onTrigger) {
			onTriggerSingle += onTrigger;
			SetState(state);
		}

		public void Idle () {
			SetState( States.Idle );
		}

		public void OnAnimationComplete () {
			Idle();
		}

		public void OnAnimationTrigger () {
			if (onTriggerSingle != null) {
				onTriggerSingle.Invoke();

				foreach(Delegate d in onTriggerSingle.GetInvocationList()) {
					onTriggerSingle -= (OnFrame) d;
				}
			}
			if (onTrigger != null) {
				onTrigger.Invoke();
			}
		}

		public bool IsIdle () {
			return state == States.Idle;
		}

	}
}