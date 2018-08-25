using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;
using System;
using System.Linq;

public class FirebaseController : MonoBehaviour
{

    FirebaseAuth auth;
    FirebaseUser user;
    DatabaseReference db;
    public bool signedIn = false;

    //Data
    List<string> matchList = new List<string>();
    Dictionary<string, Match> matchDetailList = new Dictionary<string, Match>();

    //Singleton
    private static FirebaseController instance = null;
    public static FirebaseController Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);

        initialiseFirebase();
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    public string UserId
    {
        get
        {
            return user == null ? "" : user.UserId;
        }
    }


    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser == null)
        {
            //not signed in yet
            Debug.Log("No user signed in");
        }
        else if (auth.CurrentUser != user)
        {

            signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
#if !UNITY_EDITOR
				//UiMasterController.Instance.showDailySelectionPanel ();
#endif

                Debug.Log("Name : " + user.DisplayName);
                Debug.Log("Email : " + user.Email);
                Debug.Log("Photo : " + user.PhotoUrl);
                Debug.Log("ID : " + user.UserId);
            }
        }
    }

    public void FacebookSignedIn(string accessToken, Action callback = null)
    {
        Credential credential =
        FacebookAuthProvider.GetCredential(accessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            if (callback != null) callback();
        });
    }

    private void initialiseFirebase()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://greater-or-less-e0877.firebaseio.com/");
        FirebaseApp.DefaultInstance.SetEditorP12FileName("Excercise-11ba04d5d068.p12");
        FirebaseApp.DefaultInstance.SetEditorServiceAccountEmail("greater-or-less-e0877@appspot.gserviceaccount.com");
        FirebaseApp.DefaultInstance.SetEditorP12Password("notasecret");
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

    }

    public void ReadDataFromFirebase(string _path, Action<DataSnapshot> _callback)
    {
        Debug.Log("tryin to read data from " + _path);
        FirebaseDatabase.DefaultInstance
            .GetReference(_path)
            .GetValueAsync().ContinueWith(task =>
            {
                Debug.Log("Firebase query finished!");
                if (task.IsFaulted)
                {
                    // Handle the error...
                    Debug.Log("error firebase : " + task.Exception.ToString());
                    Debug.Log("error firebase : " + task.Exception.InnerException.ToString());
                    //show nothing.
                    //show "There is something wrong with the connection."
                }
                if (task.IsCompleted)
                {
                    Debug.Log("Load completed");
                    _callback(task.Result);
                }
            });
    }

    public void GetMatchStatus(Action<Dictionary<string, Match>> _callback)
    {
        if (!signedIn)
        {
            Debug.Log("Sign in required");
            return;
        }

        ReadDataFromFirebase("/user_match/" + user.UserId.ToString(), snapshot =>
        {
            Debug.Log("Callback received, building match status");

            //m_dailyList = new List<Daily> ();

            if (snapshot == null || !snapshot.HasChildren)
            {
                Debug.Log("null!");
            }
            else
            {
                foreach (DataSnapshot child in snapshot.Children)
                {
                    Debug.Log(child.ToString());
                }
            }


            _callback(matchDetailList);
        });
    }

    private Action<Dictionary<string, Match>> _userMatchcallback = null;
    public void AddUserMatchListener(Action<Dictionary<string, Match>> callback)
    {
        _userMatchcallback = callback;
        FirebaseDatabase.DefaultInstance
                        .GetReference("/user_match/" + user.UserId)
                        .ValueChanged += OnUserMatchChanged;
    }

    public void RemoveUserMatchListener()
    {
        FirebaseDatabase.DefaultInstance
                        .GetReference("/user_match/" + user.UserId)
                        .ValueChanged -= OnUserMatchChanged;
    }

    public void AddMatchDetailListener(string id)
    {
        FirebaseDatabase.DefaultInstance
                        .GetReference("/match_detail/" + id)
                        .ValueChanged += OnMatchDetailChanged;
    }

    public void RemoveMatchDetailListener(string id)
    {
        FirebaseDatabase.DefaultInstance
                        .GetReference("/match_detail/" + id)
                        .ValueChanged -= OnMatchDetailChanged;
    }

    private void OnUserMatchChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot snapshot = args.Snapshot;
        if (snapshot == null || !snapshot.HasChildren)
        {
            Debug.Log("no children!");
            if (_userMatchcallback != null) { _userMatchcallback(null); }
        }
        else
        {
            Debug.Log("raw is " + snapshot.GetRawJsonValue());
            List<string> tempMatchList = new List<string>();
            foreach (DataSnapshot s in snapshot.Children)
            {
                tempMatchList.Add(s.Key);
            }
            //Add none existing Match
            foreach (string newId in tempMatchList)
            {
                if (!matchList.Contains(newId))
                {
                    AddMatchDetailListener(newId);
                }
            }

            foreach (string existId in matchList)
            {
                if (!tempMatchList.Contains(existId))
                {
                    RemoveMatchDetailListener(existId);
                }
            }

            //build _userMatchcallback
            matchList = tempMatchList;
        }

        if (_userMatchcallback != null && matchList.Count == matchDetailList.Count)
        {
            _userMatchcallback(matchDetailList);
        }
    }

    private void OnMatchDetailChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        DataSnapshot snapshot = args.Snapshot;
        if (snapshot == null || !snapshot.HasChildren)
        {
            Debug.Log("null");
        }

        if(matchDetailList.ContainsKey(snapshot.Key)){ //Updating existing match
            Debug.Log("Updating match from " + snapshot.GetRawJsonValue());
            matchDetailList[snapshot.Key].UpdateMatch(snapshot);
        }
        else //Creating new Match
        {
            Debug.Log("Creating new match from " + snapshot.GetRawJsonValue());
            Match newMatch = new Match(snapshot, UserId);
            matchDetailList[snapshot.Key] = newMatch;
        }

        if (_userMatchcallback != null && matchList.Count == matchDetailList.Count)
        {
            _userMatchcallback(matchDetailList);
        }
    }

    //Return Match ID
    public void RequestMatch(Action<string> callback)
    {
        if (!signedIn)
        {
            Debug.Log("Sign in required");
            return;
        }

        //Check active match
        ReadDataFromFirebase("/active_match/", (snapshot) =>
        {
            if (snapshot == null || !snapshot.HasChildren)
            {
                //if no active match
                createNewMatch(callback);
            }
            else
            {
                //if there is remove it from list
                Debug.Log(snapshot.GetRawJsonValue());
                string matchId = "";
                List<string> activeList = new List<string>();
                foreach (DataSnapshot child in snapshot.Children)
                {
                    activeList.Add(child.Key);
                }

                foreach (string match in activeList)
                {
                    if (!matchList.Contains(match))
                    {
                        matchId = match;
                        break;
                    }
                }
                if (matchId == "")
                {
                    createNewMatch(callback);
                    return;
                }
                //remove obtain match id
                //FirebaseDatabase.DefaultInstance.GetReference("/active_match/").Child(match).SetValueAsync("");

                //set name picture playerid2
                var matchRef = FirebaseDatabase.DefaultInstance.GetReference("/match_detail/" + matchId);
                var userRef = FirebaseDatabase.DefaultInstance.GetReference("/user_match/" + user.UserId);
                matchRef.Child("player_id2").SetValueAsync(user.UserId);
                matchRef.Child("name").Child(user.UserId).SetValueAsync(user.DisplayName);
                matchRef.Child("picurl").Child(user.UserId).SetValueAsync(user.PhotoUrl.ToString());

                //Set user match
                userRef.Child(matchId).SetValueAsync(true);
                matchList.Add(matchId);
                matchRef.GetValueAsync().ContinueWith((task) => {
                    if (task.IsFaulted)
                    {
                        Debug.LogError("Error getting match : " + matchId);
                    }
                    if (task.IsCompleted)
                    {
                        matchDetailList[matchId] = new Match(task.Result, UserId);
                        //start with found id, save to match list, update match, p2
                        callback(matchId);
                    }
                });
            }
        });
    }

    private void createNewMatch(Action<string> callback)
    {
        //start with push id, save to matchlist, p1
        string newMatchId = FirebaseDatabase.DefaultInstance
                        .GetReference("match_detail").Push().Key;
        matchList.Add(newMatchId);
        Match newMatch = new Match()
        {
            PlayerId1 = user.UserId,
            Names = new Dictionary<string, string> { { user.UserId, user.DisplayName } },
            ProfilePictureUrls = new Dictionary<string, string> { { user.UserId, user.PhotoUrl.ToString() } }
        };
        matchDetailList.Add(newMatchId, newMatch);
        callback(newMatchId);
    }

    public void SaveMatch(string matchId, RoundData roundData, Action callback)
    {
        int round = matchDetailList[matchId].RoundList.Count;
        var roundList = matchDetailList[matchId].RoundList;
        roundList[roundList.Count.ToString()] = roundData;
        matchDetailList[matchId].CalculateScore();
        //Update player current match
        var databaseRef = FirebaseDatabase.DefaultInstance.GetReference("/match_detail/" + matchId);
        if (round == 0)
        {
            databaseRef.SetValueAsync(matchDetailList[matchId].ToDictionary());
            //databaseRef.UpdateChildrenAsync(matchDetailList[matchId].ToDictionary());
            var userRef = FirebaseDatabase.DefaultInstance.GetReference("/user_match/" + user.UserId);
            userRef.Child(matchId).SetValueAsync(true);
            //Set active match
            FirebaseDatabase.DefaultInstance.GetReference("/active_match/" + matchId).SetValueAsync(true);
        }
        else
        {
            databaseRef.Child(round.ToString()).SetValueAsync(roundData.ToDictionary());
            if (round == 5) databaseRef.Child("done/" + user.UserId).SetValueAsync(true);
        }
        callback();
    }
}