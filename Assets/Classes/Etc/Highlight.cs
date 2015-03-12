using System.Collections;
using UnityEngine;

namespace Etc {
	public class Highlight : MonoBehaviour {

		public void Start () {
			StartCoroutine(Delayed());
		}

		public IEnumerator Delayed () {
			yield return new WaitForSeconds(1);

			/*
			Debug.Log("Launch");

			var anim = GetComponent<Animation>();

			//anim["GemHighlightBlue"].blendMode = AnimationBlendMode.Additive;
			anim.Play("BlueBolt");            */
		}

	}
}