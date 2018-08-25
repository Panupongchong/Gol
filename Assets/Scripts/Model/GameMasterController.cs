using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameMasterController : MonoBehaviour {

	private int best = 0;

	public static GameMasterController _instance;
	public static GameMasterController Instance
	{
		get {
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<GameMasterController>();
				
				if (_instance == null)
				{
					GameObject container = new GameObject("GameController");
					_instance = container.AddComponent<GameMasterController>();
				}
			}
			
			return _instance;
		}
	}

	public void Awake(){
		best = PlayerPrefs.HasKey (Utility.PlayerPrefKey.BEST.ToString ()) ? PlayerPrefs.GetInt (Utility.PlayerPrefKey.BEST.ToString ()) : 0;
	}

	public void SetBestScore(int _best){
		if (_best > best) {
			best = _best;
			PlayerPrefs.SetInt (Utility.PlayerPrefKey.BEST.ToString (), _best);
		}
	}

	public int getBestScore(){
		return best;
	}

	public void startGame (int startLife = 1, string matchId = ""){
		GetComponent<EndlessGameplayController> ().startGame (startLife, matchId);
	}
	/*
	void Update () {
		//Check GameState
		switch(GameState){
		case 0:
			//Main menu
			if(ReturnState == 0){
				//Enable Mode selection
				//_ModeSelection.SetActive(true);
				_StartButton.SetActive(true);
				m_swipe.SetActive (false);
				ReturnState = -1;
				CurrentNumberBlockList.Clear();
				_Life.enabled = false;
			}
			//Swipe disabled
			break;
		case 1:
			//Play 1
			CurrentGameMode = 1;
			//Swipe disabled
			m_swipe.SetActive (false);
			//Process
			if (CurrentNumberBlockList.Count > 0) {
				foreach (List<int> T in CurrentNumberBlockList) {
					T.Clear ();
				}
				CurrentNumberBlockList.Clear ();
			}
			//Generate a set of number block
			float _randomMode = Random.Range (0.0f, 1.0f);
			int _lines = 1;
			CurrentSecondaryType = 0;
			if (_randomMode < ModeOneDuoChance) {
				_lines = 2;
				CurrentSecondaryType = 1;
			} else if (_randomMode < ModeOneDoubleChance) {
				_lines = 1;
				CurrentSecondaryType = 2;
			} else if (_randomMode < ModeOneTripleChance) {
				_lines = 3;
				CurrentSecondaryType = 3;
			}

			//Random type
			CurrentPrimaryType = Random.Range(0,RandomMaxByType.Length);

			for (int i = 0; i < _lines; i++) {
				List<int> _newPlay = new List<int> ();
				_newPlay.Add (getRandomNumberBlock (CurrentPrimaryType));
				_newPlay.Add (getRandomNumberBlock (CurrentPrimaryType));
				CurrentNumberBlockList.Add (_newPlay);
			}
			if (RoundCount % OperatorInterval == 0) {
				CurrectOperator = CurrectOperator == 0 ? 1 : 0;
			}
			//Generate block instances
			m_play.generateBlockModeOne (CurrentNumberBlockList, CurrectOperator, typeName[CurrentPrimaryType]);
			//Switch to 'Play' when finish
			ReturnState = 0;
			GameState = 9;
			GameRemainingTime = StartTimeLimitModeOne;
			isAllCorrect = true;
			//Swipe enabled
			m_swipe.SetActive(true);
			break;
		case 2:
			CurrentGameMode = 2;
			//Swipe disabled
			m_swipe.SetActive (false);
			if (CurrentNumberBlockList.Count > 0) {
				foreach (List<int> T in CurrentNumberBlockList) {
					T.Clear ();
				}
				CurrentNumberBlockList.Clear ();
			}
			//Generate a set of number block
			//Random type
			CurrentPrimaryType = Random.Range(0,RandomMaxByType.Length);
			int _set = 1; // currently, only one set for mode 2
			for (int i = 0; i < _set; i++) {
				List<int> _newPlay = new List<int> ();
				int BlockNumber = Random.Range (ModeTwoMinBlock, ModeTwoMaxBlock + 1);
				_newPlay = getRandomNumberBlocks (CurrentPrimaryType, BlockNumber);
				CurrentNumberBlockList.Add (_newPlay);
			}
			randomOperator = Random.Range (0.0f, 1.0f) < 0.5f ? 0 : 1;
			//Generate block instances
			m_play.generateBlockModeTwo (CurrentNumberBlockList [0], randomOperator, typeName[CurrentPrimaryType]);
			//Removed generated one
			CurrentNumberBlockList [0].Clear ();
			CurrentNumberBlockList.RemoveAt (0);
			//Switch to 'Play' when finish
			ReturnState = 0;
			GameState = 9;
			GameRemainingTime = StartTimeLimitModeTwo;
			isAllCorrect = true;
			break;
		case 9: //Play, Each line has Y/X seconds before timeout.
			//Count down time
			GameRemainingTime -= Time.deltaTime;
			_Timer.text = GameRemainingTime.ToString ();
			_Life.text = CurrentHeart.ToString();
			//Switch to 'End' if time ends
			if (m_play.isDone ()) {
				RoundCount++;
				//Calculate Point
				if (GameRemainingTime >= 0) {
					addGameScore ();
				}
				if (RoundCount % ModeOneInterval == 0 && RoundCount > 0) {
					GameState = 2;
				} else {
					GameState = 1;
				}
			}
			//Deduct heart when out of time
			if (GameRemainingTime < 0) {
				RoundCount++;
				CurrentHeart -= HeartPerMistake;
				m_play.RemovePlayBlocks(m_play.m_currentMode);
				if (RoundCount % ModeOneInterval == 0 && RoundCount > 0) {
					GameState = 2;
				} else {
					GameState = 1;
				}
			}
			//Game Ends when die
			if(CurrentHeart <= 0){
				if (GameScore > HighScore) {
					HighScore = GameScore;
				}
				GameState = 0;
				//Remove current play
				m_play.RemovePlayBlocks(m_play.m_currentMode);
				//Add panelty if any
				_Timer.text = "0";
				EndTime = Time.time;
				//to start
				_ResultPanel.SetActive(true);
				_StartButton.SetActive(true);
			}
			break;
		case 5:
			//End
			CurrentGameMode = -1;
			//Swipe disabled
			//Show score
			break;
		}
	}*/
}
