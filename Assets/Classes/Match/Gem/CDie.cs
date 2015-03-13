using Match.Gem.Animations;
using System.Collections;
using UnityEngine;

namespace Match.Gem {
	public class CDie : Animations.IListener {

		protected CIcon icon;
		protected IDieObserver observer;

		public CDie (CIcon icon, IDieObserver observer) {
			this.icon = icon;
			this.observer = observer;

			CAnimations
				.Highlight( icon.transform )
				.SetColor( icon.color )
				.SetListener( this );
		}

		protected void StartDying () {
			CAnimations
				.Spark( icon.transform )
				.SetColor( icon.color )
				.SetListener( this );

			CAnimations
				.Transform( icon.transform )
				.SetColor( icon.color )
				.SetListener( this );

			icon.SetState(EState.Hidden);
		}

		public void OnTransformBoltStart(CAnimation animation) {
			CAnimations
				.Contour( icon.transform )
				.SetColor( icon.color )
				.SetListener( this );
		}

		public void OnTransformExplosionStart(CAnimation animation) {
			CAnimations
				.Explosion( icon.transform )
				.SetColor( icon.color );
		}

		public void OnAnimationFinish(CAnimation animation) {
			if (animation.GetType() == EAnimations.Highlight) {
				StartDying();
			}
		}

		public void OnIconDied (CAnimation animation) {
			icon.SetState(EState.Death);
			observer.OnDieEnd();
		}


	}
}