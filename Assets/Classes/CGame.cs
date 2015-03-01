using Config;
using Libraries;
using Model;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CGame : MonoBehaviour {

	public Config.CConfig config;
	public Model.CModel model;
	public Match.CField match;
	public CInput input;

	public Image playerValue;
	public Image opponentValue;

	public Text playerText;
	public Text opponentText;

	public static CConfig Config {
		get { return Instance.config; }
	}

	public static CModel Model {
		get { return Instance.model; }
	}

	public static CInput Input {
		get { return Instance.input; }
	}

	public static CGame Instance { get; private set; }

	public void Awake() {
		Instance = this;
		model = new Model.CModel();
		input = new Libraries.CInput();
	}

	public void Start() {
		model.player  .onScoreChange += OnScoreChange;
		model.opponent.onScoreChange += OnScoreChange;

		Recount();
	}

	public void OnScoreChange(CPlayer player, int scoreChange) {
		Recount();
	}

	public void Recount() {
		Count(model.player, Config.match.opponent.health, opponentValue);
		Count(model.opponent, Config.match.player.health, playerValue);

		  playerText.text = "" + CGame.Instance.model.player.score;
		opponentText.text = "" + CGame.Instance.model.opponent.score;
	}

	public void Update () {
		input.Check();
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