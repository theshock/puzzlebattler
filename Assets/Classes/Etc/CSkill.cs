using Match.Gem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Etc {
	public class CSkill : MonoBehaviour {

		private class CRequest {
			public EColor color;
			public GameObject gameObject;
			public bool complete;
			public SpriteRenderer renderer;

			public void On () {
				this.complete = true;
				var color = renderer.material.color;
				color.a = 1.0f;
				renderer.material.color = color;
			}

			public void Off () {
				this.complete = false;
				var color = renderer.material.color;
				color.a = 0.3f;
				renderer.material.color = color;
			}
		}

		public Config.CSkill config;
		public Config.CColor colors;

		public Material enableMaterial;
		public Material disableMaterial;

		public GameObject[] requiresObjects;
		private List<CRequest> requests = new List<CRequest>();
		private SpriteRenderer renderer;

		public void Awake () {
			renderer = gameObject.GetComponent<SpriteRenderer>();
		}

		public void Start () {
			int i = 0;

			foreach (EColor color in config.colors) {
				var req = new CRequest();
				req.color      = color;
				req.gameObject = requiresObjects[i];
				req.renderer   = req.gameObject.GetComponent<SpriteRenderer>();
				req.renderer.sprite = colors.GetCorrectSprite(color);
				req.Off();

				requests.Add(req);
				++i;
			}

			for (; i < requiresObjects.Length ; ++i) {
				requiresObjects[i].SetActive(false);
			}

			CheckAvailable();
		}

		public bool IsComplete() {
			foreach (CRequest req in requests) {
				if (req.complete == false) {
					return false;
				}
			}

			return true;
		}

		public void CheckAvailable () {
			renderer.material = IsComplete() ? enableMaterial : disableMaterial;
		}

		public void OnMatch (EColor color) {
			foreach (CRequest req in requests) {
				if (req.color == color && req.complete == false) {
					req.On();
					CheckAvailable();
					break;
				}
			}
		}

		public void OffAll () {
			foreach (CRequest req in requests) {
				req.Off();
			}
			CheckAvailable();
		}

		private void Activate () {
			if (!CGame.Model.player.CanAction()) return;
			if (!IsComplete()) return;

			OffAll();
			Cast();
		}

		private void Cast () {
			CGame.Model.player.UseAction();

			if (config.isDamage) {
				CastDamage();
			} else {
				CastHeal();
			}
		}

		private void CastDamage() {
			CGame.Instance.PlayerWaitHit(config.value);
			CGame.Instance.playerCharacter.SetState( Character.States.CastDamage );
			CGame.Instance.CheckActive();
			CGame.Sounds.fireball.Play();
		}

		private void CastHeal() {
			CGame.Instance.PlayerWaitHit(-config.value);
			CGame.Instance.playerCharacter.SetState( Character.States.CastHeal );
			CGame.Instance.CheckActive();
			CGame.Sounds.heal.Play();
		}

		public void OnMouseDown () {
			Activate();
		}
	}
}