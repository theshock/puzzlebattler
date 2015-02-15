using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Match {
	public class CMatch : MonoBehaviour {

		public CView mView;
		public CModel mModel;
		public CInput mInput;
		public CMatcher mMatcher;
		public Game.CGame mGame;
		public Libraries.ActionSystem.CManager mActionManager;

		public Text playerText;
		public Text opponentText;

		void Start() {
			mGame.mModel.ActivateFirst();
			mActionManager = new Libraries.ActionSystem.CManager();
			mMatcher = new CMatcher(this);
			mModel = new CModel(this);
			mInput = new CInput(this);
			mView.mField.InitMatchField();
		}

		void Update () {
			mInput.Update();
		}
	}
}