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
}

