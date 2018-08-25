using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultViewController : BaseViewController {

    [SerializeField]
    private TextMeshProUGUI ScoreText, ComboText, BonusText, BestScoreText;
    private string _matchId;

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

    public void SetMatchId(string matchId){
        _matchId = matchId;
    }

    public void Repeat()
    {
        SoundController.Instance.PlaySound("Press");
        if(_matchId == "")
        {
            GameMasterController.Instance.startGame();
        } else {
            UiMasterController.Instance.PvpMenuPanel.gameObject.SetActive(true);
        }
		//Fade Out
        gameObject.SetActive(false);
    }

    protected override void OnBackButton()
    {
        gameObject.SetActive(false);
        base.OnBackButton();
		SoundController.Instance.PlaySound ("Press");
		//Fade Out
		UiMasterController.Instance.MainMenuPanel.gameObject.SetActive (true);
	}
}
