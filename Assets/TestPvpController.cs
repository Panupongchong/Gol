using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPvpController : MonoBehaviour {

	public void createRandomMatch(){
		
	}

	// Use this for initialization
	void OnEnable () {
		FirebaseController.Instance.getMatchStatus (showMatches);
	}

	private void showMatches(Dictionary<string, FirebaseController.MatchStatus> data){
		if (data.Count > 0) {
			Debug.Log ("No ongoing match at the moment");
		} else {
			Debug.Log ("There are matches " + data.Count);
			foreach (string matchId in data.Keys) {
				Debug.Log ("Match ID: " + matchId);
			}
		}
	}
}
