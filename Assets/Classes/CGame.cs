using Audio;
using Config;
using Etc;
using Libraries;
using Match.Gem;
using Model;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CGame : MonoBehaviour {

	public Config.CConfig config;
	public Model.CModel model;
	public Match.CField match;
	public CInput input;
	public Etc.Timer timer;

	public Audio.AudioSystem sounds;

	public Character playerCharacter;

	public CProgressBar playerProgress;
	public CProgressBar opponentProgress;

	public Text playerText;
	public Text opponentText;

	public Etc.CSkill[] skills;

	public SwipesCounter playerSwipesCounter;
	public SwipesCounter opponentSwipesCounter;

	public static CConfig Config {
		get { return Instance.config; }
	}

	public static CModel Model {
		get { return Instance.model; }
	}

	public static CInput Input {
		get { return Instance.input; }
	}

	public static Audio.AudioSystem Sounds {
		get { return Instance.sounds; }
	}

	public static CGame Instance { get; private set; }

	public void Awake() {
		Instance = this;
		model = new Model.CModel(this);
		input = new Libraries.CInput();

		  playerSwipesCounter.Watch(model.player);
		opponentSwipesCounter.Watch(model.opponent);

		timer.SetTurnTime(Config.turnTime);
		timer.onEnd += OnTimerEnd;
	}

	public void Start() {
		model.player  .onDamageChange += OnScoreChange;
		model.opponent.onDamageChange += OnScoreChange;

		model.player  .SetMaxDamage(Config.match.player  .health);
		model.opponent.SetMaxDamage(Config.match.opponent.health);

		playerCharacter.onIdle += OnCharacterIdle;
		playerCharacter.onTrigger += OnCharacterTrigger;

		Recount();
	}

	public void OnScoreChange(CPlayer player, int scoreChange) {
		Recount();
	}

	public void OnTimerEnd () {
		if (!match.actions.HasActions()) {
			Model.SwitchPlayer();
			match.ai.Play();
		}
	}

	private bool finished = false;

	public void OnCharacterIdle () {
		if (finished) {
			StartCoroutine(DelayedReset());
		} else if (model.player.GetLives() <= 0) {
			Defeat();
		} else if (model.opponent.GetLives() <= 0) {
			Victory();
		} else if (!match.actions.HasActions()) {
			match.ai.Play();
		}
	}

	private int lastHitDamage = 0;

	public void PlayerWaitHit(int damage) {
		lastHitDamage = damage;
	}

	public void OnCharacterTrigger () {
		if (lastHitDamage < 0) {
			Model.player.AddDamage(lastHitDamage);
		} else {
			Model.opponent.AddDamage(lastHitDamage);
		}
		lastHitDamage = 0;
	}

	private void Victory () {
		playerCharacter.SetState( Character.States.Victory );
		finished = true;
	}

	private void Defeat () {
		playerCharacter.SetState( Character.States.Death );
		finished = true;
	}

	protected IEnumerator DelayedReset () {
		yield return new WaitForSeconds(2f);

		finished = false;
		model.player  .damage = 0;
		model.opponent.damage = 0;
		model.ActivateFirst();
		timer.Reset();

		foreach (Etc.CSkill skill in skills) {
			skill.OffAll();
		}

		Recount();
		match.ai.Play();
	}

	public void CheckActive () {
		if (CGame.Model.GetActivePlayer().IsStepFinished()) {
			CGame.Model.SwitchPlayer();
		}
		match.ai.Play();
	}

	public void Recount() {
		opponentProgress.SetValue( model.opponent.GetLives() );
		  playerProgress.SetValue( model.  player.GetLives() );

		  playerText.text = "" + CGame.Instance.model.player.damage;
		opponentText.text = "" + CGame.Instance.model.opponent.damage;
	}

	public void Update () {
		input.Check();
	}

	public void OnMatchEnd (EColor color) {
		if (Model.opponent.isActive) return;

		foreach (var skill in skills) {
			skill.OnMatch(color);
		}
	}
}