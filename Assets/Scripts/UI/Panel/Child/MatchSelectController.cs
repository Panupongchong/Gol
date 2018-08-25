using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchSelectController : MonoBehaviour {
    [SerializeField]
    private Button _selectButton;

    [SerializeField]
    private Image[] _playerProfile;

    [SerializeField]
    private Text _matchScore;

    [SerializeField]
    private Text _matchDescription;

    private Match _match;

    private void OnEnable()
    {
        _selectButton.onClick.AddListener(showMatchDetail);
    }

    private void OnDisable()
    {
        _selectButton.onClick.RemoveListener(showMatchDetail);
    }

    public void SetData(Match data)
    {
        _match = data;
        _matchScore.text = data.PlayerNo == 0 ?
            data.Score1.ToString() + " - " + data.Score2.ToString() :
            data.Score2.ToString() + " - " + data.Score1.ToString();
        if (data.ProfilePictureUrls.ContainsKey(data.PlayerId1))
        {
            UiMasterController.Instance.LoadSpriteFromWww(data.ProfilePictureUrls[data.PlayerId1], (sprite) =>
            {
                if(gameObject.activeSelf) _playerProfile[data.PlayerNo].sprite = sprite;
            });
        }
        if (data.ProfilePictureUrls.ContainsKey(data.PlayerId2))
        {
            UiMasterController.Instance.LoadSpriteFromWww(data.ProfilePictureUrls[data.PlayerId2], (sprite) =>
            {
                if (gameObject.activeSelf) _playerProfile[data.OpponentNo].sprite = sprite;
            });
        }
    }

    private void showMatchDetail(){

        UiMasterController.Instance.PvpResultPanel.SetData(_match);
        UiMasterController.Instance.PvpResultPanel.gameObject.SetActive(true);
        //UiMasterController.Instance.PvpMenuPanel.gameObject.SetActive(false);
    }
}
