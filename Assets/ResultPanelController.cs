using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultPanelController : BaseViewController {

	public Text Score;
	public Text Time;
	public Text HighScore;

	void OnEnable(){
		//Score.text = GameMasterController.Instance.GameScore.ToString ();
		//Time.text = GameMasterController.Instance.getGameTime ().ToString ();
		//HighScore.text = GameMasterController.Instance.getHighScore ().ToString ();
	}

	protected override void OnBackButton(){
		//to menu
	}
}
