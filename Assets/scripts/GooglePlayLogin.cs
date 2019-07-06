using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif
public class GooglePlayLogin : MonoBehaviour 
{
	public GameObject leaderBoard;
    #if UNITY_ANDROID
	// Use this for initialization
	void Start () 
	{
		
        // Create client configuration
        PlayGamesClientConfiguration config = new 
            PlayGamesClientConfiguration.Builder()
            .Build();

        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;
        
        // Initialize and activate the platform
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        // Try silent sign-in (second parameter is isSilent)
        // This should always be the last line of Start()
        PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);	
	}
	public void Update() {

        leaderBoard.SetActive(Social.localUser.authenticated);
    }

	public void SignIn() {
        if (!PlayGamesPlatform.Instance.localUser.authenticated) {
            // Sign in with Play Game Services, showing the consent dialog
            // by setting the second parameter to isSilent=false.
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);

        } else {
            // Sign out of play games
            //PlayGamesPlatform.Instance.SignOut();
        	ShowAchievements();
        }
	}

	public void SignInCallback(bool success) {
        if (success) {
           
        } else {
        }
    }

    public void ShowAchievements() {
        if (PlayGamesPlatform.Instance.localUser.authenticated) {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else {
        }
    }

    public void ShowLeaderboards() {
        if (PlayGamesPlatform.Instance.localUser.authenticated) {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        }
        else {        }
    }
#endif
    /****************************************************************IOS*************************************************/
    #if UNITY_IOS
	void Start () 
	{
		
        Social.localUser.Authenticate(success => {
            if (success)
            {
                Debug.Log("Authentication successful");
                string userInfo = "Username: " + Social.localUser.userName +
                    "\nUser ID: " + Social.localUser.id +
                    "\nIsUnderage: " + Social.localUser.underage;
                Debug.Log(userInfo);
            }
            else
                Debug.Log("Authentication failed");
        });
	}
	public void Update() {

        leaderBoard.SetActive(Social.localUser.authenticated);
    }

	public void SignIn() {
		if (!Social.localUser.authenticated)
		{
			Social.localUser.Authenticate();

		}
		else
		{
			ShowAchievements();
		}
		/*
        Social.localUser.Authenticate(success => {
            if (success)
            {
                Debug.Log("Authentication successful");
                string userInfo = "Username: " + Social.localUser.userName +
                    "\nUser ID: " + Social.localUser.id +
                    "\nIsUnderage: " + Social.localUser.underage;
                Debug.Log(userInfo);
            }
            else
            {
            	ShowAchievements();
            }
        });
        */
	}

	public void SignInCallback(bool success) {
        if (success) {
           
        } else {
        }
    }

    public void ShowAchievements() {
        Social.ShowAchievementsUI();
    }

    public void ShowLeaderboards() {
        Social.ShowLeaderboardUI();
    }
    #endif
}