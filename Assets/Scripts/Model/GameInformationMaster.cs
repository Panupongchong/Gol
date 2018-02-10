using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameInformationMaster : MonoBehaviour {
	
	public Dictionary<string,Dictionary<string,object>> lvMasterData;


	private static GameInformationMaster instance = null;

	const string LV_CSV = "csv/lv";

	public static GameInformationMaster Instance
	{
		get
		{ 
			return instance; 
		}
	}

	private void Awake()
	{
		//		if (instance != null && instance != this) 
		//		{
		//			Destroy(this.gameObject);
		//		}
		//
		//		instance = this;
		//		DontDestroyOnLoad( this.gameObject );
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else Destroy(this);
		//Get Update [must download content i.e. critical asset / csv]

		//Load all CSV data into dictionary
		lvMasterData = CSVHelper.Read (LV_CSV);
		//Bluprint
		//read from blueprint.csv
		//then seperate craft_type into different dictionary;
	}
}
