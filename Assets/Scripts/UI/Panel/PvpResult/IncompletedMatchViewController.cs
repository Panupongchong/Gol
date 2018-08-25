using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncompletedMatchViewController : BaseViewController
{
    [SerializeField]
    private Text _player1MatchScore;

    [SerializeField]
    private Text _player2MatchScore;

    [SerializeField]
    private Text[] _roundScore;

    [SerializeField]
    private Button[] _roundDetail;

    [SerializeField]
    private RectTransform _playContainer;

    [SerializeField]
    private RectTransform _waitContainer;

    [SerializeField]
    private Button _playButton;

    [SerializeField]
    private GameObject _waitIcon;

    [SerializeField]
    private GameObject _hiddenIcon;

    private PvpResultViewController _menu;

    public void Initialise(PvpResultViewController menu)
    {
        _menu = menu;
    }

    public void SetData(Match match)
    {
        int playerNo = match.PlayerId1 == FirebaseController.Instance.UserId ? 0 : 1;
        _player1MatchScore.text = playerNo == 0 ? match.Score1.ToString() : match.Score2.ToString();
        _player2MatchScore.text = playerNo == 1 ? match.Score1.ToString() : match.Score2.ToString();

        int roundPlay = match.RoundList.Count;
        for (int i = 0; i < 6; i += 2)
        { // 3 rounds
            int roundTurn = (i / 2 + playerNo) % 2; //Which player goes first
            string score1 = roundPlay < i + 1 ? "" : match.RoundList[(i).ToString()].Score.ToString();
            string score2 = roundPlay < i + 2 ? "" : match.RoundList[(i + 1).ToString()].Score.ToString();
            _roundScore[i].text = roundTurn == 0 ? score1 : score2;
            _roundScore[i + 1].text = roundTurn == 1 ? score1 : score2;

            _roundDetail[i / 2].gameObject.SetActive(score1 != "" && score2 != "");
        }

        int currentRound = roundPlay / 2;
        int currentSide = roundPlay % 2;
        bool playable = (currentRound + playerNo) % 2 == currentSide;
        if (playable)
        {
            _waitContainer.gameObject.SetActive(false);

            _playContainer.position = _roundDetail[currentRound].GetComponent<RectTransform>().position;
            _playContainer.gameObject.SetActive(true);

            _hiddenIcon.SetActive(!((currentRound + playerNo) % 2 == 0));

            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(()=>{
                GameMasterController.Instance.startGame(3, match.Id);
                _menu.gameObject.SetActive(false);
            });
        }
        else
        {
            _playContainer.gameObject.SetActive(false);

            _waitIcon.SetActive(!((currentRound + playerNo) % 2 == 0));
            _waitContainer.position = _roundDetail[currentRound].GetComponent<RectTransform>().position;
            _waitContainer.gameObject.SetActive(true);
        }
    }

    protected override void OnBackButton()
    {
        gameObject.SetActive(false);
    }
}
