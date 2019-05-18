using System.Collections;
using UnityEngine;

using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GooglePlayLogin : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);	
        // Create client configuration
        PlayGamesClientConfiguration config = new 
            PlayGamesClientConfiguration.Builder()
            .Build();

        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;
        
        // Initialize and activate the platform
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();	
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
            Debug.Log("Signed in!");
          
           
        } else {
            Debug.Log("Sign-in failed...");
        }
    }

    public void ShowAchievements() {
        if (PlayGamesPlatform.Instance.localUser.authenticated) {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else {
          Debug.Log("Cannot show Achievements, not logged in");
        }
    }
}
