using UnityEngine;

namespace Match.Gem.Animations {
	public class CAnimations : Object {

		public static CBolt      Bolt     (Transform transform) { return (CBolt)      Req( EAnimations.Bolt     , transform ); }
		public static CContour   Contour  (Transform transform) { return (CContour)   Req( EAnimations.Contour  , transform ); }
		public static CExplosion Explosion(Transform transform) { return (CExplosion) Req( EAnimations.Explosion, transform ); }
		public static CHighlight Highlight(Transform transform) { return (CHighlight) Req( EAnimations.Highlight, transform ); }
		public static CSpark     Spark    (Transform transform) { return (CSpark)     Req( EAnimations.Spark    , transform ); }
		public static CTransform Transform(Transform transform) { return (CTransform) Req( EAnimations.Transform, transform ); }

		protected static CAnimations instance;

		protected static CAnimation Req (EAnimations type, Transform transform) {
			if (instance == null) {
				instance = new CAnimations();
			}

			return instance.Require(type, transform);
		}

		public void OnAnimationFinish (CAnimation animation) {
			MonoBehaviour.Destroy(animation.gameObject);
		}

		public CAnimation Require (EAnimations type, Transform transform) {
			var anim = Create(type, transform);
			anim.SetListener(null);
			return anim;
		}

		public CAnimation Create (EAnimations type, Transform transform) {
			var prefab = CGame.Config.match.die.GetPrefab(type);

			GameObject gameObject = Instantiate(prefab) as GameObject;
			gameObject.transform.SetParent(transform.parent);
			gameObject.transform.localScale = new Vector3(100, 100, 1);
			gameObject.transform.position = transform.position;

			var anim = gameObject.GetComponent<CAnimation>();
			anim.SetAnimations(this);

			return anim;
		}

	}
}