using DG.Tweening;
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

		private Tweener FadeTo (float value, float time) {
			return DOTween.ToAlpha(
				() => this.renderer.material.color,
				x => this.renderer.material.color = x,
				value,
				time
			);
		}

		public void SetState (States state, OnFrame onTrigger) {
			onTriggerSingle += onTrigger;
			SetState(state);
		}

		public void Idle () {
			SetState( States.Idle );
		}

		public void OnAnimationComplete () {
			if (state == States.Death) {
				FadeTo(0.0f, CGame.Config.fadeOutTime).OnComplete(() => {
					Idle();
					FadeTo(1.0f, CGame.Config.fadeInTime);
				});
			} else {
				Idle();
			}
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