using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultViewController : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI ScoreText, ComboText, BonusText, BestScoreText;

    public void UpdateScore()
    {
        //Score.text = GameMasterController.Instance.GameScore.ToString ();
        //HighScore.text = GameMasterController.Instance.getHighScore ().ToString ();
        //Combo
        //Bonus
    }

    public void SetScore(int score)
    {
        ScoreText.SetText(score.ToString());
    }

    public void SetCombo(int combo)
    {
        ComboText.SetText(combo.ToString());
    }

    public void SetBonus(int bonus)
    {
        BonusText.SetText(bonus.ToString());
    }

    public void SetBestScore(int bestScore)
    {
        BestScoreText.SetText(bestScore.ToString());
    }

    public void Repeat()
    {
		SoundController.Instance.PlaySound ("Press");
		//Fade Out
        gameObject.SetActive(false);
        GameMasterController.Instance.startGame();
    }

	public void Home()
	{
		SoundController.Instance.PlaySound ("Press");
		//Fade Out
		gameObject.SetActive(false);
		UiMasterController.Instance.m_mainMenuPanel.gameObject.SetActive (true);
	}
}
