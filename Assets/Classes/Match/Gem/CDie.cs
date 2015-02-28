using System.Collections;
using UnityEngine;

namespace Match.Gem {
	public class CDie : Object {

		protected CIcon mIcon;
		protected IDieObserver mObserver;

		public CDie (CIcon icon, IDieObserver observer) {
			mIcon = icon;
			mObserver = observer;
			LaunchParticles();
			icon.StartCoroutine(DieEnd());
		}

		protected IEnumerator DieEnd () {
			yield return new WaitForSeconds(0.1f);

			mIcon.State = EState.Death;
			mObserver.OnDieEnd();
		}

		protected void LaunchParticles () {
			var transform = mIcon.transform;
			var prefab = mIcon.mField.mConfig.mDie.GetPrefab(mIcon.Type);
			GameObject anim = Instantiate(prefab, transform.position, transform.rotation) as GameObject;

			anim.transform.SetParent(transform.parent);

			Destroy(anim, 2);
		}


	}
}