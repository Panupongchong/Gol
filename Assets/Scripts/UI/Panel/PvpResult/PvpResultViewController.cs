using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvpResultViewController : BaseViewController
{
    [SerializeField]
    private IncompletedMatchViewController _incompleteContainer;

    [SerializeField]
    private GameObject _completeContainer;

    [SerializeField]
    private GameObject _roundInformationContainer;

    [SerializeField]
    private Image[] _playerProfile;

    [SerializeField]
    private Text[] _playerName;

    private Match _match;
    private bool _initialised = false;

    private void Start()
    {
        initialise();
    }

    private void initialise()
    {
        _initialised = true;
        _incompleteContainer.Initialise(this);
    }

    public void SetData(Match data)
    {
        if (!_initialised) initialise();

        _match = data;
        //Check if the match is completed
        bool completed = data.IsCompleted;
        if(completed){
            _completeContainer.SetActive(true);
        } else {
            _incompleteContainer.SetData(data);
            _incompleteContainer.gameObject.SetActive(true);
        }
        _roundInformationContainer.SetActive(false);

        if (data.ProfilePictureUrls.ContainsKey(data.PlayerId1))
        {
            UiMasterController.Instance.LoadSpriteFromWww(data.ProfilePictureUrls[data.PlayerId1], (sprite) =>
            {
                if (gameObject.activeSelf) _playerProfile[data.PlayerNo].sprite = sprite;
            });
            _playerName[data.PlayerNo].text = data.Names[data.PlayerId1];
        } else {
            _playerName[data.PlayerNo].text = "";
            _playerProfile[data.PlayerNo].sprite = null; //Set to default image
        }
        if (data.ProfilePictureUrls.ContainsKey(data.PlayerId2))
        {
            UiMasterController.Instance.LoadSpriteFromWww(data.ProfilePictureUrls[data.PlayerId2], (sprite) =>
            {
                if (gameObject.activeSelf) _playerProfile[data.OpponentNo].sprite = sprite;
            });
            _playerName[data.OpponentNo].text = data.Names[data.PlayerId2];
     }
        else
        {
            _playerName[data.OpponentNo].text = "";
            _playerProfile[data.OpponentNo].sprite = null; //Set to default image
        }
    }

    public void ShowRoundDetail(int roundNo){
        if (_match.RoundList.Count - 1 < roundNo) return;

        _roundInformationContainer.SetActive(true);
    }

    protected override void OnBackButton()
    {
        //UiMasterController.Instance.PvpMenuPanel.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}