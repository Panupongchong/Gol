using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuViewController : BaseViewController {

    public Button StartButton;
    public Button PvpButton;

	void OnEnable(){
        StartButton.onClick.AddListener(startGame);
        PvpButton.onClick.AddListener(ShowPvpMenu);
	}

	void OnDisable(){
        StartButton.onClick.RemoveListener(startGame);
        PvpButton.onClick.RemoveListener(ShowPvpMenu);
	}

	public void startGame(){
		SoundController.Instance.PlaySound ("Press");
		GameMasterController.Instance.startGame ();
		//fade out
		gameObject.SetActive(false);
	}

    public void ShowPvpMenu(){
        var signedIn = FirebaseController.Instance.signedIn;
        if (signedIn)
        {
            //Show pvp menu
            UiMasterController.Instance.PvpMenuPanel.gameObject.SetActive(true);
       }
        else
        {
            //Show pvp login
            UiMasterController.Instance.PvpLoginPanel.gameObject.SetActive(true);
        }
        gameObject.SetActive(false);
   }

	protected override void OnBackButton(){
		SoundController.Instance.PlaySound ("Back");
		//exit?
	}
}
