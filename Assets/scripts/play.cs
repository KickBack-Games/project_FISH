using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using VoxelBusters.NativePlugins;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class play : MonoBehaviour {
	public GameObject fish, hat, titleObj, resetMessage, shelf_GO, shelf_settings, fadeIn, fadeOut, ad, trophyBGobj,audioManager, 
					  obj_txt_hs, obj_txt_score, obj_txt_time, obj_newStar, obj_life_stuff, tutObj, credObj, iapAds, iapAds2, h1, h2, h3, h4, ui_fish2, ui_newOutfit2, ui_hat2;
	public Camera cam;
	public Ads ads;
	public playerMovement pm; 
	public SpriteRenderer messageSpr;
	private SpriteRenderer fishSpr,hatSpr, tutSpr, credSpr;
	private Sprite[] fishSprites,hatSprites, tutSprites, credSprites;
	public Sprite[] messageSprites;

    NativeShare sharefile;

    private int hatChoose, fishChoose, messageChoose, ghostNum, tutPage, credPage;
	//public GameObject bubbles;

	public Button ui_leftSkin, ui_rightSkin, ui_rightHat, ui_leftHat, ui_play, ui_settings,
				  ui_back, ui_resetAll, ui_toMenu, ui_resetNo, ui_resetYes, ui_ghostRepellant, ui_rate, ui_share, ui_mute_music, 
				  ui_mute_SFX, ui_pause, ui_restart, ui_tutorial, ui_credits;

	public Text txtLife,txtHighScrInt,txtHatsRemaining,txtFishRemaining,txtDeath,txtTimePlayed,txtScore,txtGameTimer;

	private float hr, min, sec,ti;
	public float timer, life;

	private bool unlocked;
	public bool onceTrigger, msgDown, isGhost, inMenu, inSettings, trophyBG, paused, inTutorial, inCredits;
	public Sprite sfxOn,sfxOff,musicOn,musicOff;
	public Transform bandaidobj;

	public float difficulty; 
	// Use this for initialization
	void Awake()
    {
    	Time.timeScale = 1;
        Application.targetFrameRate = 60;
        // load all frames in fruitsSprites array
        fishSprites = Resources.LoadAll<Sprite>("fish_sheetv4");
        hatSprites = Resources.LoadAll<Sprite>("hats_wave_2");
        messageSprites = Resources.LoadAll<Sprite>("reset_message");
        tutSprites = Resources.LoadAll<Sprite>("tut_full");
        credSprites = Resources.LoadAll<Sprite>("cred_full");

        if(PlayerPrefs.GetInt("FirstTimer") == 0)
        {
	        inMenu = true;
	        inSettings = false;
	        inTutorial = true;
    	}
    	else
    	{
	        inMenu = true;
	        inSettings = false;
	        inTutorial = false;
    	} 
        if (PlayerPrefs.GetInt("AdFree", 0) == 1)
        {
            iapAds.SetActive(false);
            iapAds2.SetActive(false);
        }
        #if UNITY_IOS
        	Advertisement.Initialize("ca-app-pub-3897066097468868~3353672524");
		#endif
		#if UNITY_ANDROID
		    Advertisement.Initialize("ca-app-pub-3897066097468868~1524503965");
		#endif
    }
	void Start () 
	{
		setSettingsText();
		Screen.SetResolution(1080, 1920, true);

		#if UNITY_IOS
        GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
        #endif
		// Keep these texts from overlapping with buttons in menu
		obj_txt_hs.SetActive(false);
		obj_txt_score.SetActive(false);
		obj_txt_time.SetActive(false);
		tutPage = 0;

		if (PlayerPrefs.GetInt("MuteSFX", 0) == 0)
		{
			ui_mute_SFX.GetComponent<Image>().sprite = sfxOn;
		}
		else
		{
			ui_mute_SFX.GetComponent<Image>().sprite = sfxOff;
		}
		if (PlayerPrefs.GetInt("MuteMusic") == 0)
		{
			ui_mute_music.GetComponent<Image>().sprite = musicOn;
		}
		else
		{
			ui_mute_music.GetComponent<Image>().sprite = musicOff;
		}

		ti = 0;
		// Save player outfit
		
		PlayerPrefs.SetInt("u_skins0", 1);
		PlayerPrefs.SetInt("u_skins1", 1);
		PlayerPrefs.SetInt("u_hats0", 1);
		PlayerPrefs.SetInt("u_hats1", 1);
		PlayerPrefs.SetInt("u_hats25", 1);
		PlayerPrefs.SetInt("u_hats0_new", 2);
		PlayerPrefs.SetInt("u_hats1_new", 2);
		PlayerPrefs.SetInt("u_hats25_new", 2);
		PlayerPrefs.SetInt("u_skins0_new", 2);
		PlayerPrefs.SetInt("u_skins1_new", 2);
		fishChoose = PlayerPrefs.GetInt("Fish", 0);
		hatChoose = PlayerPrefs.GetInt("Hat", 0);
		// End player outfit

		//PRECAUTION
		if (fishChoose == (fishSprites.Length - 1))
			hat.gameObject.SetActive(false);
		else
			hat.gameObject.SetActive(true);
		//END PRECAUTION

		ads = ad.GetComponent<Ads>();
		pm = fish.GetComponent<playerMovement>();
		fishSpr = fish.GetComponent<SpriteRenderer>();
		hatSpr = hat.GetComponent<SpriteRenderer>();
		messageSpr = resetMessage.GetComponent<SpriteRenderer>();
		credSpr = credObj.GetComponent<SpriteRenderer>();
		tutSpr = tutObj.GetComponent<SpriteRenderer>();
		// load the sprites
		fishSpr.sprite = fishSprites[fishChoose];
		hatSpr.sprite = hatSprites[hatChoose];

		// TIME AND LIFE TEXTS
		life = 100f;
		txtLife.text = "";
		txtGameTimer.text = "";
		txtHighScrInt.text = "";
		txtHatsRemaining.text = "";
		txtFishRemaining.text = "";
		txtDeath.text = "";
		txtTimePlayed.text = "";
		timer = 0;		
		disableLost();
		onceTrigger = false;
		trophyBG = false;
		difficulty = 30;
		setSettingsText();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!inMenu)
		{
			if (pm.lost)
			{
				if (!onceTrigger)
				{
					onceTrigger = true;
					life = 0;
					setLifeText();
					StartCoroutine(onLost());
					StopCoroutine(pauseDelay());
					ui_pause.gameObject.SetActive(false);
					obj_life_stuff.SetActive(false);
					obj_txt_time.SetActive(false);

				}
			}
			else
			{
				// begin game stuff
				timer += 1.0f * Time.deltaTime;
				
				if (ti !=  Mathf.Round(timer))
				{
					ti = Mathf.Round(timer);
					if (ti % (difficulty) == 0)
					{
						Instantiate(bandaidobj, new Vector2(Random.Range(-2.5f,2.5f), 5.5f), transform.rotation);
						if (timer > 240)
						{
							difficulty = 100f;
						}
						else if (timer > 180)
						{
							difficulty = 60f;
						}
					}
					tictoc(ti);
					
				}
				
				life = pm.life;
				if (pm.hooked || pm.healed)
				{
					pm.healed = false;
					setLifeText();
				}
			}
		}
	}

	public void playGame()
	{
		inMenu = false;
		pm.SFX = PlayerPrefs.GetInt("MuteSFX", 1);
        pm.MUSIC = PlayerPrefs.GetInt("MuteMusic", 1);

		// Save
		PlayerPrefs.SetInt("Fish", fishChoose);
		PlayerPrefs.SetInt("Hat", hatChoose);
		txtLife.text = 100.ToString();
		txtGameTimer.text = 0.ToString();
		
		// Activate hooks
		h1.SetActive(true);
		h2.SetActive(true);
		h3.SetActive(true);
		h4.SetActive(true);

		// Texts
		obj_txt_hs.SetActive(true);
		obj_txt_time.SetActive(true);
		obj_txt_score.SetActive(true);

		// pause stuff
		ui_pause.gameObject.SetActive(true);
		obj_life_stuff.SetActive(true);

		// in case star is active for new skin/hat
		obj_newStar.SetActive(false);

		disableButtons();
		disableSettings();

		if ((PlayerPrefs.GetInt("Hat") == 5) && (PlayerPrefs.GetInt("Fish") == 7))
		{
            #if UNITY_ANDROID
			PlayGamesPlatform.Instance.ReportProgress(
                GPGSIds.achievement_shark_tale,
                100.0f, (bool success) => {
            });	
#endif
            #if UNITY_IOS
            Social.ReportProgress("shark_tale", 100, (result) => {
                Debug.Log(result ? "Reported achievement" : "Failed to report achievement");
            });
#endif
		}
	}
	public void Paid() 
	{
		PlayerPrefs.SetInt("Paid",1);

	}
	public void restart()
	{

		//fadeOut.SetActive(true);
		Instantiate(fadeOut, new Vector3(.7f, 3f, 0f), Quaternion.identity);
		ui_restart.gameObject.SetActive(false);
	}
	public void toMenu()
	{
		// show every 3rd game
		if (PlayerPrefs.GetInt("AdFree", 0) == 0)
		{
			int num = PlayerPrefs.GetInt("AdCount", 0);
			if (num > 3)
			{
				num = 0;
				ads.ShowRewardedAd();
			}
			else
				num += 1;
			PlayerPrefs.SetInt("AdCount", num);
		}

		Instantiate(fadeOut, new Vector3(.7f, 3f, 0f), Quaternion.identity);

	}
	public void settings()
	{
		setSettingsText();
		PlayerPrefs.SetInt("Fish", fishChoose);
		PlayerPrefs.SetInt("Hat", hatChoose);
		inSettings = true;
	}
	public void toTutorial()
	{
		if (!inCredits) {
			credObj.SetActive(false);
		}
		inTutorial = true;
		inSettings = false;
		tutPage = 0;
		tutSpr.sprite = tutSprites[tutPage];
#if UNITY_ANDROID
        PlayGamesPlatform.Instance.ReportProgress(
            GPGSIds.achievement_back_to_class,
            100.0f, (bool success) => {
        });
#endif
#if UNITY_IOS
        Social.ReportProgress("back_to_class", 100, (result) => {
            Debug.Log(result ? "Reported achievement" : "Failed to report achievement");
        });
#endif

	}

	public void toCredits()
	{
		credObj.SetActive(true);
		inCredits = true;
		inSettings = false;
		credPage = 0;
		credSpr.sprite = credSprites[credPage];
	}

	public void tutorial_right()
	{
		if (inTutorial) 
		{
			if (tutPage < (tutSprites.Length - 1))
				tutPage++;
			tutSpr.sprite = tutSprites[tutPage];
		}
		else if (inCredits) 
		{
			if (credPage < (credSprites.Length - 1))
				credPage++;
			credSpr.sprite = credSprites[credPage];
		}

	}

	public void tutorial_left()
	{
		if (inTutorial) 
		{
			if (tutPage > 0)
				tutPage--;
			tutSpr.sprite = tutSprites[tutPage];
		}
		else if (inCredits) 
		{
			if (credPage > 0)
				credPage--;
			credSpr.sprite = credSprites[credPage];
		}

	}
	public void backToSettings()
	{

		if (PlayerPrefs.GetInt("FirstTimer") == 0)
		{
			inTutorial = false;
			inSettings = false;
			PlayerPrefs.SetInt("FirstTimer", 1);
		}
		else
		{
			if (inTutorial)
			{
				inTutorial = false;
				inSettings = true;
			}
			else if(inCredits) 
			{
				inCredits = false;
				inSettings = true;
			}
		}
	}
	public void back()
	{
		inSettings = false;
	}
	public void noReset()
	{
		ui_resetNo.gameObject.SetActive(false);
		ui_resetYes.gameObject.SetActive(false);
		ui_back.gameObject.SetActive(true);
		ui_rate.gameObject.SetActive(true);
		ui_mute_music.gameObject.SetActive(true);
		ui_mute_SFX.gameObject.SetActive(true);
		ui_tutorial.gameObject.SetActive(true);
		msgDown = false;
	}
	public void yesReset()
	{
		var str_ = "u_skins";
		var int_ = 2;
		// fish
		for (int i = int_; i < fishSprites.Length; i++)
		{
			PlayerPrefs.SetInt(str_ + i.ToString(), 0);
			PlayerPrefs.SetInt(str_+  i.ToString() + "_new", 0);
		}
		// hats
		str_ = "u_hats";
		for (int i = int_; i < hatSprites.Length - 1; i++)
		{
			PlayerPrefs.SetInt(str_ + i.ToString(), 0);
			PlayerPrefs.SetInt(str_ + i.ToString() + "_new", 0);
		}

		// RESET HIGHSCORE
		PlayerPrefs.SetFloat("Highscore", 0);
		// RESET DEATHS
		PlayerPrefs.SetInt("numDeaths", 0);
		PlayerPrefs.SetFloat("totalTime", 0);

		// In any case they were wearing a deleted skin... reset it default skin
		PlayerPrefs.SetInt("u_skins1", 1);

		// Also need to do this for the new star when unlocking stuff
		PlayerPrefs.SetInt("u_hats0_new", 2);
		PlayerPrefs.SetInt("u_hats1_new", 2);
		PlayerPrefs.SetInt("u_hats25_new", 2);
		PlayerPrefs.SetInt("u_skins0_new", 2);
		PlayerPrefs.SetInt("u_skins1_new", 2);
		fishChoose = 0;
		hatChoose = hatSprites.Length-1;
		fishSpr.sprite = fishSprites[fishChoose];
		hatSpr.sprite = hatSprites[hatSprites.Length-1];
		PlayerPrefs.SetInt("Fish", fishChoose);
		PlayerPrefs.SetInt("Hat", hatChoose);

		ui_resetYes.gameObject.SetActive(false);
		ui_resetNo.gameObject.SetActive(false);
		ui_back.gameObject.SetActive(true);
		ui_rate.gameObject.SetActive(true);
		ui_mute_music.gameObject.SetActive(true);
		ui_mute_SFX.gameObject.SetActive(true);
		ui_tutorial.gameObject.SetActive(true);
		msgDown = false;
	}

	public void resetAll()
	{
		resetMessage.gameObject.SetActive(true);
		msgDown = true;
		// Ghost or not?
		ghostNum = Random.Range(0,10);
		if (ghostNum == 5)
		{
			messageSpr.sprite = messageSprites[1];
			ui_ghostRepellant.gameObject.SetActive(true);
			isGhost = true;
            #if UNITY_ANDROID
			PlayGamesPlatform.Instance.ReportProgress(
                GPGSIds.achievement_boo,
                100.0f, (bool success) => {
            });	
#endif
	        #if UNITY_IOS
            Social.ReportProgress("boo", 100, (result) => {
                Debug.Log(result ? "Reported achievement" : "Failed to report achievement");
            });
#endif
		}
		else
		{
			messageSpr.sprite = messageSprites[0];
			ui_resetYes.gameObject.SetActive(true);
			ui_resetNo.gameObject.SetActive(true);
			isGhost = false;
		}

		ui_back.gameObject.SetActive(false);
		ui_rate.gameObject.SetActive(false);
		ui_mute_music.gameObject.SetActive(false);
		ui_mute_SFX.gameObject.SetActive(false);
		ui_tutorial.gameObject.SetActive(false);
	}
	public void byeGhost()
	{
		ui_ghostRepellant.gameObject.SetActive(false);
		ui_resetYes.gameObject.SetActive(true);
		ui_resetNo.gameObject.SetActive(true);
		isGhost = false;
	}
	IEnumerator ghostFlip()
    {
    	yield return new WaitForSeconds(.25f);
    	messageSpr.sprite = messageSprites[0];
    }

	void updateRecords(float timing)
	{
		//THIS IS CALLED WHEN WE LOSE.... THEREFORE WE CAN JUST GO AHEAD AND USE IT AS A LOST FUNCTION
		
		// KEEP TRACK OF DEATHS
		int numDeaths = PlayerPrefs.GetInt("numDeaths", 0);
		numDeaths++;
		PlayerPrefs.SetInt("numDeaths", numDeaths);

		// Logic for total time
		float tTime = PlayerPrefs.GetFloat("totalTime", 0);
		tTime += timing;
		PlayerPrefs.SetFloat("totalTime", tTime);

		// Now that all has been updated above, we can check for unlocks
		checkUnlocked(numDeaths, tTime);
	}

	private void starActivation (int total, int[] unlockIdArr, string type)
	{
		string og = type;
		for (int i = 0; i < total; i++)
		{
			type = "u_" + type + unlockIdArr[i].ToString() + "_new";
			PlayerPrefs.SetInt(type, 1);
			type = og;
		}
	}
	void checkUnlocked(int deaths, float time_)
	{
		int lm;
		bool unlocked = false;
		bool b_hat = false;
		bool b_fish = false;

		// will never get bigger than that. Camn't unlock that many hats or fish at once.
		int[] hatArr = new int[10];
		int[] fishArr = new int[10];

		// Will count the total of unlocked for each and if any then an array will be initialized to activate them as discovered
		int counterHats = 0;
		int counterFish = 0;

		// Timer based unlockables
		if (timer > 10)
		{
			lm = PlayerPrefs.GetInt("u_hats2", 0);
			if (lm == 0)
			{
				b_hat = true;
				unlocked = true;
				PlayerPrefs.SetInt("u_hats2", 1);

				hatArr[counterHats] = 2;
				counterHats += 1;

			}

			if (timer > 20)
			{
				lm = PlayerPrefs.GetInt("u_hats3", 0);
				if (lm == 0)
				{
					unlocked = true;
					b_hat = true;
					PlayerPrefs.SetInt("u_hats3", 1);

					hatArr[counterHats] = 3;
					counterHats += 1;
				}
				if (timer > 30)
				{
					#if UNITY_ANDROID
			            PlayGamesPlatform.Instance.ReportProgress(
			                GPGSIds.achievement_the_more_the_merrier,
			                100.0f, (bool success) => {
			            });
#endif
					#if UNITY_IOS
			            Social.ReportProgress("the_more_the_merrier", 100, (result) => {
			                Debug.Log(result ? "Reported achievement" : "Failed to report achievement");
			            });
#endif
        
					lm = PlayerPrefs.GetInt("u_skins3", 0);
					if (lm == 0)
					{
						unlocked = true;
						b_fish = true;
						PlayerPrefs.SetInt("u_skins3", 1);

						fishArr[counterFish] = 3;
						counterFish += 1;
					}
					lm = PlayerPrefs.GetInt("u_hats4", 0);
					if (lm == 0)
					{
						unlocked = true;
						b_hat = true;
						PlayerPrefs.SetInt("u_hats4", 1);

						hatArr[counterHats] = 4;
						counterHats += 1;
					}
					lm = PlayerPrefs.GetInt("u_hats7", 0);
					if (lm == 0)
					{
						unlocked = true;
						b_hat = true;
						PlayerPrefs.SetInt("u_hats7", 1);

						hatArr[counterHats] = 7;
						counterHats += 1;
					}
					if (timer > 60)
					{
                        #if UNITY_ANDROID
		            	PlayGamesPlatform.Instance.ReportProgress(
			                GPGSIds.achievement_grand_ol_minute,
			                100.0f, (bool success) => {
			            });
#endif
            			#if UNITY_IOS
			            Social.ReportProgress("grand_ol_minute", 100, (result) => {
			                Debug.Log(result ? "Reported achievement" : "Failed to report achievement");
			            });
#endif
						lm = PlayerPrefs.GetInt("u_hats8", 0);
						if (lm == 0)
						{
							unlocked = true;
							b_hat = true;
							PlayerPrefs.SetInt("u_hats8", 1);

							hatArr[counterHats] = 8;
							counterHats += 1;
						}
						lm = PlayerPrefs.GetInt("u_skins4", 0);
						if (lm == 0)
						{
							unlocked = true;
							b_fish = true;
							PlayerPrefs.SetInt("u_skins4", 1);

							fishArr[counterFish] = 4;
							counterFish += 1;
						}
						if (timer > 100)
						{
							lm = PlayerPrefs.GetInt("u_skins5", 0);
							if (lm == 0)
							{
								unlocked = true;
								b_fish = true;
								PlayerPrefs.SetInt("u_skins5", 1);

								fishArr[counterFish] = 5;
								counterFish += 1;
							}
							lm = PlayerPrefs.GetInt("u_hats9", 0);
							if (lm == 0)
							{
								unlocked = true;
								b_hat = true;
								PlayerPrefs.SetInt("u_hats9", 1);

								hatArr[counterHats] = 9;
								counterHats += 1;
							}
						}
						if (timer > 160)
						{
							lm = PlayerPrefs.GetInt("u_skins2", 0);
							if (lm == 0)
							{
								unlocked = true;
								b_fish = true;
								PlayerPrefs.SetInt("u_skins2", 1);

								fishArr[counterFish] = 2;
								counterFish += 1;
							}
							lm = PlayerPrefs.GetInt("u_hats10", 0);
							if (lm == 0)
							{
								unlocked = true;
								b_hat = true;
								PlayerPrefs.SetInt("u_hats10", 1);

								hatArr[counterHats] = 10;
								counterHats += 1;
							}
							if (timer > 240)
							{
								lm = PlayerPrefs.GetInt("u_hats11", 0);
								if (lm == 0)
								{
									unlocked = true;
									b_hat = true;
									PlayerPrefs.SetInt("u_hats11", 1);

									hatArr[counterHats] = 11;
									counterHats += 1;
								}
								lm = PlayerPrefs.GetInt("u_hats12", 0);
								if (lm == 0)
								{
									unlocked = true;
									b_hat = true;
									PlayerPrefs.SetInt("u_hats12", 1);

									hatArr[counterHats] = 12;
									counterHats += 1;
								}
								if (timer >= 360)
								{
									lm = PlayerPrefs.GetInt("u_skins6", 0);
									if (lm == 0)
									{
										unlocked = true;
										b_fish = true;
										PlayerPrefs.SetInt("u_skins6", 1);

										fishArr[counterFish] = 6;
										counterFish += 1;
									}
								}
							}
						}
					}
				}
			}
		}
		else
		{
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.ReportProgress(
                GPGSIds.achievement_participation_trophy,
                100.0f, (bool success) => {
            });
#endif
#if UNITY_IOS
            Social.ReportProgress("participation_trophy", 100, (result) => {
                Debug.Log(result ? "Reported achievement" : "Failed to report achievement");
            });
#endif
        }
		// Achievements
		if (timer >= 90)
		{
            #if UNITY_ANDROID
            PlayGamesPlatform.Instance.ReportProgress(
                GPGSIds.achievement_playing_right,
                100.0f, (bool success) => {
            });
#endif
            #if UNITY_IOS
            Social.ReportProgress("playing_right", 100, (result) => {
                Debug.Log(result ? "Reported achievement" : "Failed to report achievement");
            });
#endif
			if (timer >= 120)
			{
                #if UNITY_ANDROID
	            PlayGamesPlatform.Instance.ReportProgress(
	                GPGSIds.achievement_the_great_hundred,
	                100.0f, (bool success) => {
	            });
#endif
	            #if UNITY_IOS
	            Social.ReportProgress("plus_20", 100, (result) => {
	                Debug.Log(result ? "Reported achievement" : "Failed to report achievement");
	            });
#endif
				if (timer >= 180)
				{
                    #if UNITY_ANDROID
		            PlayGamesPlatform.Instance.ReportProgress(
		                GPGSIds.achievement_180_your_way_out_of_the_hooks_way,
		                100.0f, (bool success) => {
		            });
#endif
		            #if UNITY_IOS
		            Social.ReportProgress("180_your_way_out_of_the_hooks", 100, (result) => {
		                Debug.Log(result ? "Reported achievement" : "Failed to report achievement");
		            });
#endif
					if(timer >= 240)
					{
                        #if UNITY_ANDROID
			            PlayGamesPlatform.Instance.ReportProgress(
			                GPGSIds.achievement_maybe_you_should_take_a_break,
			                100.0f, (bool success) => {
			            });
#endif
				        #if UNITY_IOS
			            Social.ReportProgress("take_a_break", 100, (result) => {
			                Debug.Log(result ? "Reported achievement" : "Failed to report achievement");
			            });
#endif
			            if(timer >= 999)
			            {
				            #if UNITY_ANDROID
				            PlayGamesPlatform.Instance.ReportProgress(
				                GPGSIds.achievement_you_win,
				                100.0f, (bool success) => {
				            });
#endif
        		            #if UNITY_IOS
				            Social.ReportProgress("you_win", 100, (result) => {
				                Debug.Log(result ? "Reported achievement" : "Failed to report achievement");
				            });
#endif
			            }
					}
				}
			}
		}

		// Death based unlockables
		if (deaths >= 100)
		{
            #if UNITY_ANDROID
            PlayGamesPlatform.Instance.ReportProgress(
                GPGSIds.achievement_get_the_boot,
                100.0f, (bool success) => {
            });	
#endif
        	#if UNITY_IOS
            Social.ReportProgress("get_the_boot", 100, (result) => {
                Debug.Log(result ? "Reported achievement" : "Failed to report achievement");
            });
#endif
			lm = PlayerPrefs.GetInt("u_skins9", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_skins9", 1);
				unlocked = true;
				b_fish = true;

				fishArr[counterFish] = 9;
				counterFish += 1;
			}
		}
		else if (deaths >= 50)
		{			
			lm = PlayerPrefs.GetInt("u_skins7", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_skins7", 1);
				unlocked = true;
				b_fish = true;

				fishArr[counterFish] = 7;
				counterFish += 1;
			}
			lm = PlayerPrefs.GetInt("u_hats5", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats5", 1);
				unlocked = true;
				b_hat = true;

				hatArr[counterHats] = 5;
				counterHats += 1;
			}

		}
		else if (deaths >= 25)
		{

			lm = PlayerPrefs.GetInt("u_hats6", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats6", 1);
				unlocked = true;
				b_hat = true;

				hatArr[counterHats] = 6;
				counterHats += 1;
			}
		}
		else if (deaths >= 10)
		{
			lm = PlayerPrefs.GetInt("u_skins8", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_skins8", 1);
				unlocked = true;
				b_fish = true;

				fishArr[counterFish] = 8;
				counterFish += 1;
			}
		}

		// Total time played unlockables
		if (time_  > 10000f)
		{
			lm = PlayerPrefs.GetInt("u_hats24", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats24", 1);
				unlocked = true;
				b_hat = true;
				hatArr[counterHats] = 24;
				counterHats += 1;
			}
		}
		else if (time_  > 5000f)
		{
			lm = PlayerPrefs.GetInt("u_hats23", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats23", 1);
				unlocked = true;
				b_hat = true;
				hatArr[counterHats] = 23;
				counterHats += 1;
			}
		}
		else if (time_  > 2000f)
		{
			lm = PlayerPrefs.GetInt("u_hats22", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats22", 1);
				unlocked = true;
				b_hat = true;
				hatArr[counterHats] = 22;
				counterHats += 1;
			}
		}
		else if (time_  > 1000f)
		{
			lm = PlayerPrefs.GetInt("u_hats21", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats21", 1);
				unlocked = true;
				b_hat = true;
				hatArr[counterHats] = 21;
				counterHats += 1;
			}
		}
		else if (time_  > 600f)
		{
			lm = PlayerPrefs.GetInt("u_hats20", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats20", 1);
				unlocked = true;
				b_hat = true;
				hatArr[counterHats] = 20;
				counterHats += 1;
			}
		}
		else if (time_  > 400f)
		{
			lm = PlayerPrefs.GetInt("u_hats19", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats19", 1);
				unlocked = true;
				b_hat = true;
				hatArr[counterHats] = 19;
				counterHats += 1;
			}
		}
		else if (time_  > 350f)
		{
			lm = PlayerPrefs.GetInt("u_hats18", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats18", 1);
				unlocked = true;
				b_hat = true;
				hatArr[counterHats] = 18;
				counterHats += 1;
			}
		}
		else if (time_  > 300f)
		{
			lm = PlayerPrefs.GetInt("u_hats17", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats17", 1);
				unlocked = true;
				b_hat = true;
				hatArr[counterHats] = 17;
				counterHats += 1;
			}
		}
		else if (time_  > 240f)
		{
			lm = PlayerPrefs.GetInt("u_hats16", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats16", 1);
				unlocked = true;
				b_hat = true;
				hatArr[counterHats] = 16;
				counterHats += 1;
			}
		}
		else if (time_  > 150f)
		{
			lm = PlayerPrefs.GetInt("u_hats15", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats15", 1);
				unlocked = true;
				b_hat = true;
				hatArr[counterHats] = 15;
				counterHats += 1;
			}
		}
		else if (time_  > 80f)
		{
			lm = PlayerPrefs.GetInt("u_hats14", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats14", 1);
				unlocked = true;
				b_hat = true;
				hatArr[counterHats] = 14;
				counterHats += 1;
			}
		}
		else if (time_  > 20f)
		{
			lm = PlayerPrefs.GetInt("u_hats13", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats13", 1);
				unlocked = true;
				b_hat = true;
				hatArr[counterHats] = 13;
				counterHats += 1;
			}
		}


		// Final touch
		if (unlocked)
		{
			//ui_newOutfit.gameObject.SetActive(true);
			ui_newOutfit2.SetActive(true);
			// Initialize arrays with the total amount for hats or fish
			if (b_hat)
			{
				// These wll be initialized at the end when we are done counting how many.
				starActivation(counterHats,hatArr,"hats");
				ui_hat2.SetActive(true);
				//ui_hat.gameObject.SetActive(true);
			}
			if (b_fish)
			{
				starActivation(counterFish,fishArr,"skins");
				ui_fish2.SetActive(true);
				//ui_fish.gameObject.SetActive(true);
			}
		}
	}

	/******************************************************************************
									ENABLERS AND DISABLERS
	********************************************************************************/
	void enableButtons()
	{
		ui_leftHat.gameObject.SetActive(true); 
		ui_rightHat.gameObject.SetActive(true); 
		ui_leftSkin.gameObject.SetActive(true); 
		ui_rightSkin.gameObject.SetActive(true);
		ui_play.gameObject.SetActive(true);
		ui_settings.gameObject.SetActive(true);
		titleObj.SetActive(true);
		//titleObj.transform.position = new Vector2(transform.position.x, 2.5f);
		//shelf_GO.transform.parent = null;
	}

	void disableButtons()
	{
		ui_leftHat.gameObject.SetActive(false); 
		ui_rightHat.gameObject.SetActive(false); 
		ui_leftSkin.gameObject.SetActive(false); 
		ui_rightSkin.gameObject.SetActive(false);
		ui_play.gameObject.SetActive(false);
		ui_settings.gameObject.SetActive(false);
		titleObj.SetActive(false);
	}
	void enableSettings()
	{
		ui_back.gameObject.SetActive(true);
		ui_resetAll.gameObject.SetActive(true);
	}
	void disableSettings()
	{
		ui_back.gameObject.SetActive(false);
		ui_resetAll.gameObject.SetActive(false);
	}
	void enableLost()
	{
		ui_toMenu.gameObject.SetActive(true);
		if (PlayerPrefs.GetInt("AdFree", 0) == 1)
		{
			iapAds2.SetActive(false);
			ui_toMenu.transform.position = new Vector2(0, ui_toMenu.transform.position.y);
		}
		titleObj.SetActive(true);
		titleObj.transform.position = new Vector2(0, 3.59f);
		shelf_GO.SetActive(true);
		shelf_GO.transform.parent = cam.transform;
		shelf_GO.transform.position = new Vector2(0f, .34f);
		ui_fish2.SetActive(false);
		ui_hat2.SetActive(false);
		ui_newOutfit2.SetActive(false);
		ui_share.gameObject.SetActive(true);
	}
	void disableLost()
	{
		cam.transform.position = new Vector2(.75f,transform.position.y);
		titleObj.transform.position = new Vector2(.75f, 3.56f);
		ui_toMenu.gameObject.SetActive(false);
		ui_newOutfit2.SetActive(false);
		ui_fish2.SetActive(false);
		ui_hat2.SetActive(false);
		shelf_GO.SetActive(false);
	}


	/********************************************************************8****
				ARROW FUNCTIONAL STUFF
	**************************************************************************/


	private void newStar(string curr)
	{
		// I THINK INITIALLY THE SKINS AND HATS THAT ARE BY DEFAULT NEED TO BE SET TO 2. SINCE THEY WERE NEVER SET TO 0
		string new_ = "_new";
		new_ = curr+new_;
		if (PlayerPrefs.GetInt(new_,0) == 1)
		{
			// JUST DISCOVERED - ACTIVATE STAR
			obj_newStar.SetActive(true);
			PlayerPrefs.SetInt(new_,2);
		}
		else if (PlayerPrefs.GetInt(new_,0) == 2)
		{
			// NOTHING TO SEE HERRE - DEACTIVATE
			obj_newStar.SetActive(false);
		}

	} 

	public void leftSkin()
	{
		var str_ = "u_skins";
		int int_ = fishChoose;
		bool b = true;
		while(b)
		{
			if(int_ > 0)
			{
				int_ = int_ - 1;
			}
			else
			{
				int_ = fishSprites.Length - 1;
			}
			str_ = "u_skins" + int_.ToString();
			if (PlayerPrefs.GetInt(str_, 0) == 1)
			{
				fishChoose = int_;
				b = false;
				newStar(str_);			
			}
		}

		fishSpr.sprite = fishSprites[fishChoose];
		if (fishChoose == (fishSprites.Length-1))
			hat.gameObject.SetActive(false);
		else
			hat.gameObject.SetActive(true);
	}

	public void rightSkin()
	{
		var str_ = "u_skins";
		int int_ = fishChoose;
		bool b = true;
		while(b)
		{	
			if(int_ > fishSprites.Length - 1)
			{
				int_ = 0;
			}
			else
			{
				int_ = int_ +  1;
			}
			str_ = "u_skins" + int_.ToString();

			if (PlayerPrefs.GetInt(str_, 0) == 1)
			{

				b = false;
				fishChoose = int_;
				newStar(str_);				
			}
		}
		fishSpr.sprite = fishSprites[fishChoose];
		if (fishChoose == (fishSprites.Length - 1))
			hat.gameObject.SetActive(false);
		else
			hat.gameObject.SetActive(true);
	}

	public void leftHat()
	{
		var str_ = "u_hats";
		int int_ = hatChoose;
		bool b = true;
		while(b)
		{
			if(int_ > 0)
			{
				int_ = int_ - 1;
			}
			else
			{
				int_ = hatSprites.Length - 1;
			}
			str_ = "u_hats" + int_.ToString();
			if (PlayerPrefs.GetInt(str_, 0) == 1)
			{
				hatChoose = int_;
				b = false;
				newStar(str_);		
			}
		}

		hatSpr.sprite = hatSprites[hatChoose];
	}
	public void rightHat()
	{
		var str_ = "u_hats";
		int int_ = hatChoose;
		bool b = true;
		while(b)
		{
			if(int_ == hatSprites.Length - 1)
			{
				int_ = 0;
			}
			else
			{
				int_ = int_ +  1;
			}
			str_ = "u_hats" + int_.ToString();

			if (PlayerPrefs.GetInt(str_, 0) == 1)
			{

				b = false;
				hatChoose = int_;
				newStar(str_);					
			}
		}

		hatSpr.sprite = hatSprites[hatChoose];
	}

	void setLifeText()
	{
		txtLife.text = Mathf.Round(life).ToString();
		//txtHighScrInt.text = "";
		// Empty out everything
	}
	void setTimerText()
	{
		txtLife.text = "";
		txtGameTimer.text = "";
		txtHighScrInt.text = Mathf.Round(PlayerPrefs.GetFloat("Highscore", 0)).ToString();
		txtScore.text = Mathf.Round(timer).ToString();
	}
	void setHighScore()
	{
		int timeVar = Mathf.RoundToInt(timer);
		PlayerPrefs.SetFloat("Highscore", timeVar);

	}
	void tictoc(float time)
	{
		txtGameTimer.text = time.ToString();
	}
	public void setSettingsText()
	{
		// Set the remaining hats... count total of unlocked ones
		int unlocked = 0;
		for (int i = 0; i < hatSprites.Length - 1; i++)
		{
			unlocked += PlayerPrefs.GetInt("u_hats" + i.ToString(), 0);
		}
		txtHatsRemaining.text = unlocked + " OUT OF " + (hatSprites.Length - 1).ToString(); // Last hat is blank; does not count

		unlocked = 0;
		for (int i = 0; i < fishSprites.Length; i++)
		{
			unlocked += PlayerPrefs.GetInt("u_skins" + i.ToString(), 0);
		}
		txtFishRemaining.text = unlocked + " OUT OF " + fishSprites.Length;
 
		txtDeath.text = PlayerPrefs.GetInt("numDeaths").ToString();
		sec = Mathf.Round((PlayerPrefs.GetFloat("totalTime")));
		hr = Mathf.Floor((sec / 3600));
		min = Mathf.Floor((sec / 60)) % 60;
		sec = sec % 60;
		
		


		txtTimePlayed.text = "H: " + hr + "  M: " + min + "  S: " + sec;
	}
	public void setSettingsTextInvisible()
	{

		txtHatsRemaining.text = "";
		txtFishRemaining.text = "";
		txtDeath.text = "";
		txtTimePlayed.text = "";
	}

	public void muteMusic()
	{
		int x = PlayerPrefs.GetInt("MuteMusic", 1);
		if(x == 1)
		{
			PlayerPrefs.SetInt("MuteMusic", 0);
			FindObjectOfType<SM>().Stop("Theme");
			ui_mute_music.GetComponent<Image>().sprite = musicOn;
		}
		else 
		{
			PlayerPrefs.SetInt("MuteMusic", 1);
			FindObjectOfType<SM>().Play("Theme");
			ui_mute_music.GetComponent<Image>().sprite = musicOff;
		}
	}

	public void muteSFX() 
	{
		int x = PlayerPrefs.GetInt("MuteSFX", 1);
		if(x == 0)
		{
			// ON!
			PlayerPrefs.SetInt("MuteSFX", 1);
			ui_mute_SFX.GetComponent<Image>().sprite = sfxOff;
		}
		else 
		{
			// OFF!
			PlayerPrefs.SetInt("MuteSFX", 0);
			ui_mute_SFX.GetComponent<Image>().sprite = sfxOn;
		}
	}

	public void Pause()
	{
		if(Time.timeScale == 1) // pause
		{
			if (pm.hooked)
				FindObjectOfType<SM>().Stop("hookAlarm");
			
			paused = true;
			Time.timeScale = 0;
			ui_restart.gameObject.SetActive(true);
		}
		else // unpause
		{
			if (pm.hooked)
				FindObjectOfType<SM>().Play("hookAlarm");
			ui_pause.gameObject.SetActive(false);
			paused = false;
			ui_restart.gameObject.SetActive(false);
			Time.timeScale = 1;
			StartCoroutine(pauseDelay());
		}

	}

	public void rate() 
	{
		#if UNITY_ANDROID
 		Application.OpenURL("market://details?id=com.DarkShotStudios.FishinHats");
		#elif UNITY_IPHONE
        Application.OpenURL("itms-apps://itunes.apple.com/app/1455798300");
		#endif
    }

    public void OnScreen() 
    {
        //string path = System.IO.Path.Combine(Application.persistentDataPath, "screen.png");
        //ScreenCapture.CaptureScreenshot(path);
        // Create share sheet
        ShareSheet _shareSheet = new ShareSheet();
        _shareSheet.Text = "Think you can beat my score?";

        // Attaching screenshot here
        _shareSheet.AttachScreenShot();

        // Show composer
        NPBinding.UI.SetPopoverPointAtLastTouchPosition();
        NPBinding.Sharing.ShowView(_shareSheet, FinishedSharing);
        //StartCoroutine(shareDelay());
    }

    void FinishedSharing(eShareResult _result)
    {
        Debug.Log("Finished sharing");
        Debug.Log("Share Result = " + _result);
    }

    public void IapAds()
    {
        PlayerPrefs.SetInt("AdFree",1);
    }

    IEnumerator onLost()
    {
    	yield return new WaitForSeconds(1.5f);
		enableLost();
		// Record things only once!
		trophyBG = true;
		fish.transform.position = new Vector2(0.0f, -4.0f);
		trophyBGobj.SetActive(true);

		if (Mathf.Round(timer) > Mathf.Round(PlayerPrefs.GetFloat("Highscore", 0)))
		{
			setHighScore();
		}
		// Send to leaderboard
		Mathf.RoundToInt(timer);
#if UNITY_ANDROID

        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Note: make sure to add 'using GooglePlayGames'
            PlayGamesPlatform.Instance.ReportScore(Mathf.RoundToInt(timer),
                GPGSIds.leaderboard_swimmin_time,
                (bool success) =>
                {
                    
                });
        }
#endif
#if UNITY_IOS
        Social.ReportScore(Mathf.RoundToInt(timer), "fishin_leaderboard", success => {
            Debug.Log(success ? "Reported score successfully" : "Failed to report score");
        });
#endif
		setTimerText();
		updateRecords(timer);

    }

    IEnumerator pauseDelay()
    {
    	yield return new WaitForSeconds(2);
    	if (!pm.lost)
    		ui_pause.gameObject.SetActive(true);
    }

    IEnumerator shareDelay()
    {
		yield return new WaitForEndOfFrame();
        /*
		Texture2D ss = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
		ss.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0 );
		ss.Apply();

		string filePath = System.IO.Path.Combine( Application.temporaryCachePath, "sharedimg.png" );
		System.IO.File.WriteAllBytes( filePath, ss.EncodeToPNG() );
		
		// To avoid memory leaks
		Destroy( ss );

		new NativeShare().AddFile( filePath ).SetSubject( "Play Fish'n Hats" ).SetText( "Bet you can't beat my highscore!" ).Share();
        */
        sharefile = new NativeShare();
        string path = System.IO.Path.Combine(Application.persistentDataPath, "screen.png");
        sharefile.AddFile(path);
        //sharefile.AddFile(Application.persistentDataPath + @"/screen.png");
        sharefile.Share();
    }
}
