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
	public Character opponentCharacter;

	public CProgressBar playerProgress;
	public CProgressBar opponentProgress;

	public Text playerText;
	public Text opponentText;

	public Etc.CSkill[] skills;
	public Etc.CGoals goals;
	public EndPopup endPopup;

	public Text opponentsCount;

	public SwipesCounter playerSwipesCounter;
	public SwipesCounter opponentSwipesCounter;

	public int opponents = 3;

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
		opponentCharacter.onIdle += OnCharacterIdle;

		opponents = Config.opponentsCount;

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
		if (playerCharacter.IsIdle() == false || opponentCharacter.IsIdle() == false) {
			return;
		}

		if (model.player.GetLives() <= 0) {
			Defeat();
		} else if (model.opponent.GetLives() <= 0) {
			Victory();
		} else if (!match.actions.HasActions()) {
			match.ai.Play();
		}
	}

	//public void OnCharacterTrigger () {
	//	if (lastHitDamage < 0) {
	//		Model.player.AddDamage(lastHitDamage);
	//	} else {
	//		Model.opponent.AddDamage(lastHitDamage);
	//	}
	//}

	private void Victory () {
		if (!finished) {
			playerCharacter.SetState(Character.States.Victory);
			opponentCharacter.SetState(Character.States.Death);
			finished = true;
		} else if (--opponents > 0) {
			RoundReset();
		} else if (goals.IsComplete()) {
			endPopup.ShowVictory(Reset);
		} else {
			endPopup.ShowDefeat(Reset);
		}
	}

	private void Defeat () {
		if (finished) {
			endPopup.ShowDefeat(Reset);
		} else {
			playerCharacter.SetState(Character.States.Death);
			opponentCharacter.SetState(Character.States.Victory);
			finished = true;
		}
	}

	protected void Reset () {
		model.player.damage = 0;
		goals.Reset();
		RoundReset();
	}

	protected void RoundReset () {
		finished = false;
		model.opponent.damage = 0;
		model.ActivateFirst();
		timer.Reset();

		if (opponents == 0) {
			opponents = Config.opponentsCount;
		}

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
		opponentsCount.text = "" + opponents;

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

		goals.OnMatch(color);

		foreach (var skill in skills) {
			skill.OnMatch(color);
		}
	}
}