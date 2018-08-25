using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessGameplayController : BaseGameplayController {

	//Object
	public GameObject m_swipe;

	private float m_timeLimit = 5;
	private int m_startLife = 1;
	private int m_operaton = 0; //0 >, 1 <
	private bool m_playing;
	private int m_countCombo = 0;
    private int accuracyHit = 0;
    private int accuracyTotal = 0;
    private float matchTime = 0;
    private string m_matchId = "";
	void Start(){
		m_factory = new Mini1QuizFactory ();
		m_playing = false;
		quizList = new List<Quiz> ();
	}

	//For mini2
	//protected overrided void OnEnable(){
	//	super.OnEnable();
	//	listen to block clicked
	//}	

	void Update(){
		if (m_playing) {
            matchTime += Time.deltaTime;
			timeLeft -= Time.deltaTime;
			m_view.SetTimeFill (timeLeft / m_timeLimit);
			if (timeLeft < 0) {
				endGame ();
			}
		}
	}

    public void startGame(int startLife = 1, string matchId = ""){
		Debug.Log ("Game start");
        m_matchId = matchId;
		lv = 1;
        m_startLife = startLife;
		quizList.Clear ();
		m_view.gameObject.SetActive (true);
		m_view.reset ();
		setGameParameter ();
		generatePlay ();
		m_playing = true;
		time = Time.time;
	}

	public void endGame(){
		m_playing = false;
		time = Time.time - time;

		StartCoroutine (endGameSequence());
	}

	IEnumerator endGameSequence(){
		if (m_countCombo > combo) {
			combo = m_countCombo;
		}
		int best = GameMasterController.Instance.getBestScore ();

		yield return new WaitForSeconds (1f);
        if (m_matchId != "")
        {
            RoundData data = new RoundData()
            {
                Score = score,
                Combo = combo,
                Bonus = bonus,
                AccuracyHit = accuracyHit,
                AccuracyTotal = accuracyTotal,
                AverageSpeed = accuracyTotal / matchTime,
            };
            FirebaseController.Instance.SaveMatch(m_matchId, data, () =>
            {
                m_view.gameObject.SetActive(false);
                UiMasterController.Instance.ShowResult(score, combo, bonus, best, m_matchId);
            });
        }
        else
        {
            if (score > best)
            {
                best = score;
                GameMasterController.Instance.SetBestScore(best);
                SoundController.Instance.PlaySound("Win");
            }
            else
            {
                SoundController.Instance.PlaySound("Lose");
            }
            m_view.gameObject.SetActive(false);
            UiMasterController.Instance.ShowResult(score, combo, bonus, best);
        }
	}
		
	void generatePlay(){
		m_operaton = Random.Range (0, 2);
		m_view.showOperator (m_operaton);
		while(quizList.Count < maxQuiz){
			Quiz _new = m_factory.generateQuiz(lv);
			quizList.Add(_new);
			m_view.addQuiz(_new);
		}
	}

	void setGameParameter(){
		score = 0;
		combo = 0;
		timeLeft = m_timeLimit + 0.7f;
		life = m_startLife;
		m_view.SetScore (score); 
		m_view.SetCombo (combo);
		m_view.SetTimeFill (timeLeft / m_timeLimit);
	}

	protected override void OnSwipe(){
		if (!m_playing) return;
		bool _result = quizList [0].checkAnswer (2);
		if(_result){
			OnCorrectAnswer (2);
		} else {
			OnIncorrectAnswer (2);
		}
	}
		
	protected override void OnRightTap(){
		if (!m_playing) return;
		bool _result = quizList [0].checkAnswer (m_operaton == 0 ? 1 : 0);
		if(_result){
			OnCorrectAnswer (1);
		} else {
			OnIncorrectAnswer (1);
		}
	}
		
	protected override void OnLeftTap(){
		if (!m_playing) return;
		bool _result = quizList[0].checkAnswer(m_operaton);
		if(_result){
			OnCorrectAnswer (0);
		} else {
			OnIncorrectAnswer (0);
		}
	}

	private void OnIncorrectAnswer(int _side){
		life--;
		miss++;
		m_view.playWrong(_side);
		if (m_countCombo > combo) {
			combo = m_countCombo;
		}
		m_countCombo = 0;
		if (life <= 0) {
			endGame ();
		}
        accuracyTotal += 1;
	}

	private void OnCorrectAnswer(int _side){
		bool done = quizList[0].next();

		hit++;
		score += 1;
		m_countCombo++;

		if (done) {
			quizList.RemoveAt (0);
			timeLeft = m_timeLimit;
			score++;
		}
		m_view.playCorrect(_side);
		m_view.SetScore (score); 
		m_view.SetCombo (m_countCombo);
		if (quizList.Count <= 0) {
			generatePlay ();
			//Morelv
			if (GameInformationMaster.Instance.lvMasterData.ContainsKey ((lv + 1).ToString ())) {
				lv++;
			}
		}
        accuracyHit += 1;
        accuracyTotal += 1;
	}
}
