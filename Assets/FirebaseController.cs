using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;
using System;

public class FirebaseController : MonoBehaviour {

	FirebaseAuth auth;
	FirebaseUser user;
	DatabaseReference db;
	bool signedIn = false;

	//Data
	Dictionary<string, MatchStatus> matchStatusList = new Dictionary<string, MatchStatus>();
	Dictionary<string, MatchDetail> matchDetailList = new Dictionary<string, MatchDetail>();

	//Singleton
	private static FirebaseController instance = null;
	public static FirebaseController Instance
	{
		get
		{ 
			return instance; 
		}
	}

	[SerializeField]
	public class MatchStatus{
		string status;
	}

	[SerializeField]
	public class MatchDetail{
		string player_id1;
		string player_id2;
		Dictionary<string, Round> roundRecord;
	}

	[SerializeField]
	public class Round{
		Dictionary<string, Record> recordList;
	}

	[SerializeField]
	public class Record{
		int score;
		int hit;
		int miss;
		float time;
	}

	void Awake () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else
			Destroy (this);

		initialiseFirebase ();
	}

	void OnDestroy() {
		auth.StateChanged -= AuthStateChanged;
		auth = null;
	}

	private void AuthStateChanged(object sender, System.EventArgs eventArgs) {
		Debug.Log("Checking auth");
		if (auth.CurrentUser == null) {
			//not signed in yet
		} else if (auth.CurrentUser != user) {
			Debug.Log ("Checking user");

			signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
			if (!signedIn && user != null) {
				Debug.Log ("Last user already signed out, show login panel");
				Debug.Log ("Signed out " + user.UserId);
			}
			user = auth.CurrentUser;
			if (signedIn) {
				Debug.Log ("Signed in " + user.UserId);
				#if !UNITY_EDITOR
				//UiMasterController.Instance.showDailySelectionPanel ();
				#endif
			}
		}
	}

	private void initialiseFirebase(){
		Debug.Log("Setting up Firebase Auth");
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://excercise-8dcc7.firebaseio.com/");
		FirebaseApp.DefaultInstance.SetEditorP12FileName("Excercise-11ba04d5d068.p12");
		FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("excercise-8dcc7@appspot.gserviceaccount.com");
		FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");
		auth = FirebaseAuth.DefaultInstance;
		auth.StateChanged += AuthStateChanged;
		AuthStateChanged (this, null);

	}

	public void readDataFromFirebase(string _path, Action<DataSnapshot> _callback){
		Debug.Log ("tryin to read data from " + _path);
		FirebaseDatabase.DefaultInstance
			.GetReference (_path)
			.GetValueAsync ().ContinueWith (task => {
			Debug.Log ("Firebase query finished!");
			if (task.IsFaulted) {
				// Handle the error...
				Debug.Log ("error firebase : " + task.Exception.ToString ());
				Debug.Log ("error firebase : " + task.Exception.InnerException.ToString ());
				//show nothing.
				//show "There is something wrong with the connection."
			}
			if (task.IsCompleted) {
				Debug.Log ("Load completed");
				_callback (task.Result);
			}
		});
	}

	public void getMatchStatus(Action<Dictionary<string, MatchStatus>> _callback){	
		if (!signedIn) {
			Debug.Log ("Sign in required");
			return;
		}

		readDataFromFirebase ("/user_match/" + user.UserId.ToString (), snapshot => {
			Debug.Log ("Callback received, building match status");

			//m_dailyList = new List<Daily> ();

			if (snapshot == null) {
				Debug.Log ("null!");
			} else if (!snapshot.HasChildren) {
				Debug.Log ("no children!");
			} else {
				foreach (DataSnapshot child in snapshot.Children) {
					Debug.Log (child.ToString ());
				}
			}

			Debug.Log("raw is " + snapshot.GetRawJsonValue());
			matchStatusList = JsonUtility.FromJson<Dictionary<string, MatchStatus>>(snapshot.GetRawJsonValue());

			_callback (matchStatusList);
		});
	}

	public void GetMatchDetail(string matchId, Action<MatchDetail> _callback){
		if (!signedIn) {
			Debug.Log ("Sign in required");
			return;
		}

		if (!matchStatusList.ContainsKey (matchId)) {
			Debug.Assert (false, "The match id does not exist in match status " + matchId);
			return;
		}

		readDataFromFirebase ("/match_detail/" + matchId, snapshot => {

			MatchDetail detail = new MatchDetail ();

			if (snapshot == null) {
				Debug.Log ("null!");
			} else if (!snapshot.HasChildren) {
				Debug.Log ("no children!");
			} else {
				foreach (DataSnapshot child in snapshot.Children) {
					Debug.Log (child.ToString ());
				}
			}

			detail = (MatchDetail)snapshot.Value;
			matchDetailList [matchId] = detail;
			_callback (matchDetailList [matchId]);
		});
	}
}