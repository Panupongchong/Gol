using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletedMatchViewController : BaseViewController
{
    [SerializeField]
    private Text[] _roundScoreText;

    [SerializeField]
    private Text[] _accuracyText;

    [SerializeField]
    private Text[] _speedText;

    [SerializeField]
    private Text[] _matchScoreText;

    public void SetData(Match match){
        int playerNo = match.PlayerId1 == FirebaseController.Instance.UserId ? 0 : 1;
        _matchScoreText[0].text = playerNo == 0 ? match.Score1.ToString() : match.Score2.ToString();
        _matchScoreText[1].text = playerNo == 1 ? match.Score1.ToString() : match.Score2.ToString();

        int roundPlay = match.RoundList.Count;
        int[] accuracy = new int[] { 0, 0 };
        float[] speed = new float[] { 0f, 0f };
        for (int i = 0; i < 6; i += 2)
        { // 3 rounds
            int roundTurn = (i + playerNo) % 2; //Which player goes first
            string score1 = roundPlay < i + 1 ? "" : match.RoundList[(i).ToString()].Score.ToString();
            string score2 = roundPlay < i + 2 ? "" : match.RoundList[(i + 1).ToString()].Score.ToString();
            _roundScoreText[i].text = roundTurn == 0 ? score1 : score2;
            _roundScoreText[i + 1].text = roundTurn == 1 ? score1 : score2;

            accuracy[i % 2] += match.RoundList[(i).ToString()].AccuracyTotal;
            speed[i % 2] += match.RoundList[(i).ToString()].AverageSpeed;
       }

        //total accuracy
        _accuracyText[0].text = playerNo == 0 ? accuracy[0].ToString() : accuracy[1].ToString();
        _accuracyText[1].text = playerNo == 1 ? accuracy[0].ToString() : accuracy[1].ToString();
        //total time
        speed[0] /= 3f;
        speed[1] /= 3f;
        _speedText[0].text = playerNo == 0 ? speed[0].ToString() : speed[1].ToString();
        _speedText[1].text = playerNo == 1 ? speed[0].ToString() : speed[1].ToString();
    }

    protected override void OnBackButton()
    {
        gameObject.SetActive(false);
    }
}
