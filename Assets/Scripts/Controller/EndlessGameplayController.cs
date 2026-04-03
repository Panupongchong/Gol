using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessGameplayController : BaseGameplayController
{
	private float _timeLimit = 5;
	private int _startLife = 1;
	private int _operaton = 0; //0 >, 1 <
	private bool _playing;
	private int _countCombo = 0;
	private int _accuracyHit = 0;
	private int _accuracyTotal = 0;
	private float _matchTime = 0;
	private string _matchId = "";

	private void Start()
	{
		_factory = new Mini1QuizFactory();
		_playing = false;
		_quizList = new List<Quiz>();
	}

	//For mini2
	//protected overrided void OnEnable(){
	//	super.OnEnable();
	//	listen to block clicked
	//}	

	private void Update()
	{
		if (_playing)
		{
			_matchTime += Time.deltaTime;
			_timeLeft -= Time.deltaTime;
			_view.SetTimeFill(_timeLeft / _timeLimit);
			if (_timeLeft < 0)
			{
				endGame();
			}
		}
	}

	public void StartGame(int startLife = 1, string matchId = "")
	{
		Debug.Log("Game start");
		_matchId = matchId;
		_lv = 1;
		_startLife = startLife;
		_quizList.Clear();
		_view.gameObject.SetActive(true);
		_view.Reset();
		setGameParameter();
		generatePlay();
		_playing = true;
		_time = Time.time;
	}

	public void endGame()
	{
		_playing = false;
		_time = Time.time - _time;

		StartCoroutine(endGameSequence());
	}

	IEnumerator endGameSequence()
	{
		if (_countCombo > _combo)
		{
			_combo = _countCombo;
		}
		int best = GameMasterController.Instance.getBestScore();

		yield return new WaitForSeconds(1f);
		if (_matchId != "")
		{
			RoundData data = new RoundData()
			{
				Score = _score,
				Combo = _combo,
				Bonus = _bonus,
				AccuracyHit = _accuracyHit,
				AccuracyTotal = _accuracyTotal,
				AverageSpeed = _accuracyTotal / _matchTime,
			};
			FirebaseController.Instance.SaveMatch(_matchId, data, () =>
			{
				_view.gameObject.SetActive(false);
				UiMasterController.Instance.ShowResult(_score, _combo, _bonus, best, _matchId);
			});
		}
		else
		{
			if (_score > best)
			{
				best = _score;
				GameMasterController.Instance.SetBestScore(best);
				SoundController.Instance.PlaySound("Win");
			}
			else
			{
				SoundController.Instance.PlaySound("Lose");
			}
			_view.gameObject.SetActive(false);
			UiMasterController.Instance.ShowResult(_score, _combo, _bonus, best);
		}
	}

	void generatePlay()
	{
		_operaton = 0;//Random.Range(0, 2);
		_view.ShowOperator(_operaton);
		while (_quizList.Count < _maxQuiz)
		{
			Quiz _new = _factory.generateQuiz(_lv);
			_quizList.Add(_new);
			_view.AddQuiz(_new);
		}
	}

	void setGameParameter()
	{
		_score = 0;
		_combo = 0;
		_timeLeft = _timeLimit + 0.7f;
		_life = _startLife;
		_view.SetScore(_score);
		_view.SetCombo(_combo);
		_view.SetTimeFill(_timeLeft / _timeLimit);
	}

	protected override void OnSwipe()
	{
		if (!_playing) return;
		bool _result = _quizList[0].checkAnswer(2);
		if (_result)
		{
			OnCorrectAnswer(2);
		}
		else
		{
			OnIncorrectAnswer(2);
		}
	}

	protected override void OnRightTap()
	{
		if (!_playing) return;
		bool _result = _quizList[0].checkAnswer(_operaton == 0 ? 1 : 0);
		if (_result)
		{
			OnCorrectAnswer(1);
		}
		else
		{
			OnIncorrectAnswer(1);
		}
	}

	protected override void OnLeftTap()
	{
		if (!_playing) return;
		bool _result = _quizList[0].checkAnswer(_operaton);
		if (_result)
		{
			OnCorrectAnswer(0);
		}
		else
		{
			OnIncorrectAnswer(0);
		}
	}

	protected override void OnMidTap()
	{
		if (!_playing) return;
		bool _result = _quizList[0].checkAnswer(2);
		if (_result)
		{
			OnCorrectAnswer(0);
		}
		else
		{
			OnIncorrectAnswer(0);
		}
	}

	private void OnIncorrectAnswer(int _side)
	{
		_life--;
		_miss++;
		_view.PlayWrong(_side);
		if (_countCombo > _combo)
		{
			_combo = _countCombo;
		}
		_countCombo = 0;
		if (_life <= 0)
		{
			endGame();
		}
		_accuracyTotal += 1;
	}

	private void OnCorrectAnswer(int _side)
	{
		bool done = _quizList[0].next();

		_hit++;
		_score += 1;
		_countCombo++;

		if (done)
		{
			_quizList.RemoveAt(0);
			_timeLeft = _timeLimit;
			_score++;
		}
		_view.PlayCorrect(_side);
		_view.SetScore(_score);
		_view.SetCombo(_countCombo);
		if (_quizList.Count <= 0)
		{
			generatePlay();
			//Morelv
			if (GameInformationMaster.Instance.lvMasterData.ContainsKey((_lv + 1).ToString()))
			{
				_lv++;
			}
		}
		_accuracyHit += 1;
		_accuracyTotal += 1;
	}
}
