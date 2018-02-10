using UnityEngine;
using System.Collections;

public class MainMenuViewController : BaseViewController {

	public GameObject m_startButton;

	void OnEnable(){
		UIEventListener.Get (m_startButton).onClick += startGame;
	}

	void OnDisable(){
		UIEventListener.Get (m_startButton).onClick -= startGame;
	}

	public void startGame(GameObject _obj){
		SoundController.Instance.PlaySound ("Press");
		GameMasterController.Instance.startGame ();
		//fade out
		gameObject.SetActive(false);
	}

	protected override void OnBackButton(){
		SoundController.Instance.PlaySound ("Back");
		//exit?
	}
}
