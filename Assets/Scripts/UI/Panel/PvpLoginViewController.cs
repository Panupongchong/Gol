using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Facebook.Unity;

public class PvpLoginViewController : BaseViewController {
    public Button FacebookButton;
    private bool freeze = false;
    private void OnEnable()
    {
        base.OnEnable();
        FacebookButton.onClick.AddListener(facebookLogin);
        refresh();
    }

    private void OnDisable()
    {
        base.OnDisable();
        FacebookButton.onClick.RemoveListener(facebookLogin);
    }

    private void refresh(){
        var singedIn = FirebaseController.Instance.signedIn;
        if (!singedIn)
        {
            if (!FB.IsInitialized)
            {
                FB.Init(facebookInitCallback, OnHideUnity);
            }
            else
            {
                FB.ActivateApp();
            }

            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                AccessToken aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
                // Print current access token's User ID
                Debug.Log(aToken.UserId);
                // Print current access token's granted permissions
                foreach (string perm in aToken.Permissions)
                {
                    Debug.Log(perm);
                }
                FirebaseController.Instance.FacebookSignedIn(aToken.TokenString, loginCompleted);
           }
        }
        else
        {
            UiMasterController.Instance.PvpMenuPanel.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    private void facebookLogin()
    {
        if (freeze) return;
        freeze = true;

        Debug.Log("Logging with FB");
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, facebookAuthCallback);
    }

    private void facebookAuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            AccessToken aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
            FirebaseController.Instance.FacebookSignedIn(aToken.TokenString, loginCompleted);
        }
        else
        {
            Debug.Log("User cancelled login");
            freeze = false;
        }
    }

    private void facebookInitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void loginCompleted(){
        freeze = false;
        gameObject.SetActive(false);
        UiMasterController.Instance.PvpMenuPanel.gameObject.SetActive(true);
    }

    protected override void OnBackButton(){
        if (freeze) return;
         gameObject.SetActive(false);
        UiMasterController.Instance.MainMenuPanel.gameObject.SetActive(true);
    }
}
