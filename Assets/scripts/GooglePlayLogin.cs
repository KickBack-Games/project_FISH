using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

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
}