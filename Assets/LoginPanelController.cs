using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Facebook.Unity;
using UnityEngine.UI;

public class LoginPanelController : MonoBehaviour {

	public Button m_fbLoginButton;
	private FirebaseAuth auth = null;

	void OnEnable(){
		m_fbLoginButton.onClick.AddListener(facebookLogin);
	}

	void OnDisable(){
	}

	// Use this for initialization
	void Start () {
		m_fbLoginButton.onClick.AddListener(facebookLogin);
		auth = FirebaseAuth.DefaultInstance;
		// Check if user is signed in (non-null) and update UI accordingly.
		FirebaseUser currentUser = auth.CurrentUser;
		if (currentUser == null) {
			//updateUI(currentUser);
			if (!FB.IsInitialized) {
				// Initialize the Facebook SDK
				FB.Init (InitCallback, OnHideUnity);
			} else {
				// Already initialized, signal an app activation App Event
				FB.ActivateApp ();
			}

			if (FB.IsLoggedIn) {
				Debug.Log ("Already login");
				// AccessToken class will have session details
				AccessToken aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
				// Print current access token's User ID
				Debug.Log(aToken.UserId);
				// Print current access token's granted permissions
				foreach (string perm in aToken.Permissions) {
					Debug.Log(perm);
				}
				handleFacebookAccessToken (aToken.TokenString);
				//gameObject.SetActive (false);
				//Debug.Log ("Show login button");
			} else {
				Debug.Log ("Show login button");
			}
		} else {
			Debug.Log ("Already login");
			gameObject.SetActive (false);
		}
	}

	private void AuthCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			AccessToken aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log(aToken.UserId);
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions) {
				Debug.Log(perm);
			}
			handleFacebookAccessToken (aToken.TokenString);
		} else {
			Debug.Log("User cancelled login");
		}
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public void facebookLogin(){
		Debug.Log ("Logging with FB");
		var perms = new List<string>(){"public_profile", "email", "user_friends"};
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}

	private void handleFacebookAccessToken(string accessToken) {

		Credential credential =
			FacebookAuthProvider.GetCredential(accessToken);
		auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
			if (task.IsCanceled) {
				Debug.LogError("SignInWithCredentialAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
				return;
			}

			FirebaseUser newUser = task.Result;
			Debug.LogFormat("User signed in successfully: {0} ({1})",
				newUser.DisplayName, newUser.UserId);
			gameObject.SetActive (false);
		});
	}
}
