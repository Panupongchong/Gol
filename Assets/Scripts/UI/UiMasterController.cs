using System;
using UnityEngine;
using System.Collections;

public class UiMasterController : MonoBehaviour {
	public static UiMasterController _instance;
	public static UiMasterController Instance
	{
		get {
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType<UiMasterController>();
			}

			return _instance;
		}
	}

	public MainMenuViewController m_mainMenuPanel;
	public ResultViewController m_resultPanel;

	public void ShowResult(int score, int combo, int bonus, int best){
		m_resultPanel.SetScore (score);
		m_resultPanel.SetCombo (combo);
		m_resultPanel.SetBonus (bonus);
		m_resultPanel.SetBestScore (best);
		m_resultPanel.gameObject.SetActive (true);
	}
}

