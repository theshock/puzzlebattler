using Config;
using Libraries;
using Model;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	public Config.CConfig mConfig;
	public Model.CGame mModel;
	public Match.CField mMatch;

	public Image playerValue;
	public Image opponentValue;

	public static CConfig Config {
		get { return Instance.mConfig; }
	}

	public static Game Instance { get; private set; }

	public void Awake() {
		Instance = this;
		mModel = new Model.CGame();
	}

	public void Start() {
		mModel.player  .onScoreChange += OnScoreChange;
		mModel.opponent.onScoreChange += OnScoreChange;

		Update();
	}

	public void OnScoreChange(CPlayer player, int scoreChange) {
		Update();
	}

	public void Update() {
		Count(mModel.player, Config.match.opponent.health, opponentValue);
		Count(mModel.opponent, Config.match.player.health, playerValue);
	}

	protected void Count(CPlayer player, int maxHealth, Image image) {
		var currentHealth = (maxHealth - player.score);
		if (currentHealth < 0) {
			currentHealth = 0;
		}

		float bar = currentHealth / (float) maxHealth;

		var rect = image.GetComponent<RectTransform>();
		image.GetComponent<RectTransform>().sizeDelta = new Vector2(24 + bar * 300, rect.sizeDelta.y);

		image.GetComponent<Image>().color = new Color(bar >= 0.5f ? (1 - bar) * 2 : 1, bar <= 0.5f ?      bar * 2 : 1, 0);
	}
}