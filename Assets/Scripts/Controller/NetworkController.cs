using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviour {


	class Match{
		List<Round> RoundList;
		string Player1;
		string Player2;
	}

	class Round{
		int Score1;
		int Score2;
	}

	public void getPvpMatch(){
		//get match list
		//
	}
}
