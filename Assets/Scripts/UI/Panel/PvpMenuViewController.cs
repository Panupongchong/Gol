using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;

public class PvpMenuViewController : BaseViewController
{

    public Button FindMatchButton;
    public Button FriendChallengeButton;
    public Button FriendInviteButton;
    public Button MatchHistoryButton;

    public GameObject MatchSelecContainer;
    public GameObject MatchSelecPrefab;

    private List<MatchSelectController> _matchSelectPool = new List<MatchSelectController>();
    private bool _loading = false;
    private void Start()
    {
        FindMatchButton.onClick.AddListener(findMatch);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _loading = true;
        FirebaseController.Instance.AddUserMatchListener(showCurrentMatch);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        FirebaseController.Instance.RemoveUserMatchListener();
    }

    private void findMatch()
    {
        if (_loading) return;
        FirebaseController.Instance.RequestMatch(startMatch);
    }

    private void startMatch(string matchId)
    {
        Debug.Log("Start a match with " + matchId);
        GameMasterController.Instance.startGame(3, matchId);
        gameObject.SetActive(false);
    }

    private void showCurrentMatch(Dictionary<string, Match> data)
    {
        _loading = false;
        if (data == null || data.Count == 0)
        {
            Debug.Log("No current match");
            return;
        }

        foreach (var obj in _matchSelectPool)
        {
            //set inactive
            obj.gameObject.SetActive(false);
        }

        int count = 0;
        foreach (Match child in data.Values)
        {
            if (_matchSelectPool.Count <= count)
            {
                //create net obj
                var match = Instantiate(MatchSelecPrefab, MatchSelecContainer.transform).GetComponent<MatchSelectController>();
                _matchSelectPool.Add(match);
            }
            //Set active and set data
            _matchSelectPool[count].SetData(child);
            _matchSelectPool[count].gameObject.SetActive(true);
            count++;
            Debug.Log(child.ToString());
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(MatchSelecContainer.GetComponent<RectTransform>());
    }

    protected override void OnBackButton()
    {
        if (_loading) return;
        gameObject.SetActive(false);
        UiMasterController.Instance.ShowMainMenu();
    }

    private void showSetting()
    {
        //uimaster.instnce.showsetting
    }
}
