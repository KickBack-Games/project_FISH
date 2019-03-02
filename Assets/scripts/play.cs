using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class play : MonoBehaviour {
	public GameObject fish, hat, titleObj, resetMessage, shelf_GO, shelf_settings, fadeIn, fadeOut;
	public Camera cam;
	public playerMovement pm; 
	public SpriteRenderer messageSpr;
	private SpriteRenderer fishSpr,hatSpr;
	private Sprite[] fishSprites,hatSprites;
	public Sprite[] messageSprites;

	private int hatChoose, fishChoose, messageChoose, ghostNum;
	//public GameObject bubbles;

	public Button ui_leftSkin, ui_rightSkin, ui_rightHat, ui_leftHat, ui_play, ui_settings,
				  ui_back, ui_resetAll, ui_toMenu, ui_newOutfit, ui_fish, ui_hat, 
				  ui_resetNo, ui_resetYes, ui_ghostRepellant, ui_rate, ui_share, ui_donate;

	public Text txtLife,txtHighScrInt,txtHatsRemaining,txtFishRemaining,txtDeath,txtTimePlayed,txtScore;

	private float hr, min, sec;
	private bool unlocked;
	public float timer,life;
	public bool onceTrigger, msgDown, isGhost, inMenu, inSettings;
	// Use this for initialization
	void Awake()
    {
        // load all frames in fruitsSprites array
        fishSprites = Resources.LoadAll<Sprite>("fish_sheetv4");
        hatSprites = Resources.LoadAll<Sprite>("hats_wave_2");
        messageSprites = Resources.LoadAll<Sprite>("resetMessage");
        inMenu = true;
        inSettings = false;
    }
	void Start () 
	{
		// Save player outfit
		setSettingsText();
		PlayerPrefs.SetInt("u_skins0", 1);
		PlayerPrefs.SetInt("u_skins1", 1);
		PlayerPrefs.SetInt("u_hats0", 1);
		PlayerPrefs.SetInt("u_hats1", 1);
		PlayerPrefs.SetInt("u_hats25", 1);
		fishChoose = PlayerPrefs.GetInt("Fish", 0);
		hatChoose = PlayerPrefs.GetInt("Hat", 0);
		// End player outfit

		//PRECAUTION
		if (fishChoose == (fishSprites.Length - 1))
			hat.gameObject.SetActive(false);
		else
			hat.gameObject.SetActive(true);
		//END PRECAUTION

		pm = fish.GetComponent<playerMovement>();
		fishSpr = fish.GetComponent<SpriteRenderer>();
		hatSpr = hat.GetComponent<SpriteRenderer>();
		messageSpr = resetMessage.GetComponent<SpriteRenderer>();
		// load the sprites
		fishSpr.sprite = fishSprites[fishChoose];
		hatSpr.sprite = hatSprites[hatChoose];

		// TIME AND LIFE TEXTS
		life = 100f;
		txtLife.text = "";
		txtHighScrInt.text = "";
		txtHatsRemaining.text = "";
		txtFishRemaining.text = "";
		txtDeath.text = "";
		txtTimePlayed.text = "";
		timer = 0;		
		disableLost();
		onceTrigger = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!inMenu)
		{
			if (pm.lost)
			{
				enableLost();
				// Record things only once!
				if (!onceTrigger)
				{
					onceTrigger = true;

					if (Mathf.Round(timer) > Mathf.Round(PlayerPrefs.GetFloat("Highscore", 0)))
					{
						setHighScore();
					}
					setTimerText();
					updateRecords(timer);
				}
			}
			else
			{
				// begin game stuff
				timer += 1.0f * Time.deltaTime;
				life = pm.life;
				if (pm.hooked)
				{
					setLifeText();
				}
			}
		}
	}

	public void playGame()
	{
		inMenu = false;

		// Save
		PlayerPrefs.SetInt("Fish", fishChoose);
		PlayerPrefs.SetInt("Hat", hatChoose);
		txtLife.text = 100.ToString();
		disableButtons();
		disableSettings();
	}
	public void toMenu()
	{
		//inMenu = true;
		onceTrigger = false;
		fadeOut.SetActive(true);
		Instantiate(fadeOut, new Vector3(.7f, 3f, 0f), Quaternion.identity);

	//	txtLife.text = "";
	}
	public void settings()
	{
		//bubbles.gameObject.SetActive(false);
		//disableButtons();
		//enableSettings();
		setSettingsText();
		PlayerPrefs.SetInt("Fish", fishChoose);
		PlayerPrefs.SetInt("Hat", hatChoose);
		inSettings = true;
	}
	public void back()
	{
		//bubbles.gameObject.SetActive(true);
		//enableButtons();
		//disableSettings();
		//setSettingsTextInvisible();
		inSettings = false;
	}
	public void noReset()
	{
		ui_resetNo.gameObject.SetActive(false);
		ui_resetYes.gameObject.SetActive(false);
		ui_back.gameObject.SetActive(true);
		ui_rate.gameObject.SetActive(true);
		ui_share.gameObject.SetActive(true);
		ui_donate.gameObject.SetActive(true);
		msgDown = false;
	}
	public void yesReset()
	{
		var str_ = "u_skins";
		var int_ = 2;
		// fish
		for (int i = int_; i < fishSprites.Length; i++)
		{
			str_ += i.ToString();
			PlayerPrefs.SetInt(str_, 0);
			str_ = "u_skins";
		}
		// hats
		for (int i = 1; i < hatSprites.Length - 1; i++)
		{
			str_ += i.ToString();
			PlayerPrefs.SetInt(str_, 0);
			str_ = "u_hats";
		}

		// RESET HIGHSCORE
		PlayerPrefs.SetFloat("Highscore", 0);
		// RESET DEATHS
		PlayerPrefs.SetInt("numDeaths", 0);
		PlayerPrefs.SetFloat("totalTime", 0);

		// In any case they were wearing a deleted skin... reset it default skin

		PlayerPrefs.SetInt("u_skins1", 1);
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
		ui_share.gameObject.SetActive(true);
		ui_donate.gameObject.SetActive(true);
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
		ui_share.gameObject.SetActive(false);
		ui_donate.gameObject.SetActive(false);

		//fsetSettingsTextInvisible();
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
	void checkUnlocked(int deaths, float time_)
	{
		int lm;
		bool unlocked = false;
		bool b_hat = false;
		bool b_fish = false;
		// Timer based unlockables
		if (timer > 10)
		{
			lm = PlayerPrefs.GetInt("u_hats2", 0);
			if (lm == 0)
			{
				b_hat = true;
				unlocked = true;
				PlayerPrefs.SetInt("u_hats2", 1);
			}

			if (timer > 20)
			{
				lm = PlayerPrefs.GetInt("u_hats3", 0);
				if (lm == 0)
				{
					unlocked = true;
					b_hat = true;
					PlayerPrefs.SetInt("u_hats3", 1);
				}
				if (timer > 35)
				{
					lm = PlayerPrefs.GetInt("u_skins3", 0);
					if (lm == 0)
					{
						unlocked = true;
						b_fish = true;
						PlayerPrefs.SetInt("u_skins3", 1);
					}
					lm = PlayerPrefs.GetInt("u_hats4", 0);
					if (lm == 0)
					{
						unlocked = true;
						b_hat = true;
						PlayerPrefs.SetInt("u_hats4", 1);
					}
					lm = PlayerPrefs.GetInt("u_hats7", 0);
					if (lm == 0)
					{
						unlocked = true;
						b_hat = true;
						PlayerPrefs.SetInt("u_hats7", 1);
					}
					if (timer > 60)
					{
						lm = PlayerPrefs.GetInt("u_hats8", 0);
						if (lm == 0)
						{
							unlocked = true;
							b_hat = true;
							PlayerPrefs.SetInt("u_hats8", 1);
						}
						lm = PlayerPrefs.GetInt("u_skins4", 0);
						if (lm == 0)
						{
							unlocked = true;
							b_fish = true;
							PlayerPrefs.SetInt("u_skins4", 1);
						}
						if (timer > 100)
						{
							lm = PlayerPrefs.GetInt("u_skins5", 0);
							if (lm == 0)
							{
								unlocked = true;
								b_fish = true;
								PlayerPrefs.SetInt("u_skins5", 1);
							}
							lm = PlayerPrefs.GetInt("u_hats9", 0);
							if (lm == 0)
							{
								unlocked = true;
								b_hat = true;
								PlayerPrefs.SetInt("u_hats9", 1);
							}
						}
						if (timer > 160)
						{
							lm = PlayerPrefs.GetInt("u_skins6", 0);
							if (lm == 0)
							{
								unlocked = true;
								b_fish = true;
								PlayerPrefs.SetInt("u_skins6", 1);
							}
							lm = PlayerPrefs.GetInt("u_hats10", 0);
							if (lm == 0)
							{
								unlocked = true;
								b_hat = true;
								PlayerPrefs.SetInt("u_hats10", 1);
							}
							if (timer > 240)
							{
								lm = PlayerPrefs.GetInt("u_skins6", 0);
								if (lm == 0)
								{
									unlocked = true;
									b_fish = true;
									PlayerPrefs.SetInt("u_skins6", 1);
								}
								lm = PlayerPrefs.GetInt("u_hats11", 0);
								if (lm == 0)
								{
									unlocked = true;
									b_hat = true;
									PlayerPrefs.SetInt("u_hats11", 1);
								}
								lm = PlayerPrefs.GetInt("u_hats12", 0);
								if (lm == 0)
								{
									unlocked = true;
									b_hat = true;
									PlayerPrefs.SetInt("u_hats12", 1);
								}
							}
						}
					}
				}
			}
		}

		// Death based unlockables
		if (deaths >= 50)
		{
			lm = PlayerPrefs.GetInt("u_skins9", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_skins9", 1);
				unlocked = true;
				b_fish = true;
			}
		}
		else if (deaths >= 40)
		{			
			lm = PlayerPrefs.GetInt("u_skins7", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_skins7", 1);
				unlocked = true;
				b_fish = true;
			}
			lm = PlayerPrefs.GetInt("u_hats5", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_skins5", 1);
				unlocked = true;
				b_hat = true;
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
			}
		}
		else if (deaths >= 10)
		{
			lm = PlayerPrefs.GetInt("u_skins8", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_skins8", 1);
				PlayerPrefs.SetInt("u_skins9", 1);
				unlocked = true;
				b_fish = true;
			}
		}

		// Total time played unlockables
		if (time_  > 2000f)
		{
			lm = PlayerPrefs.GetInt("u_hats24", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats24", 1);
				unlocked = true;
				b_hat = true;
			}
		}
		else if (time_  > 1500f)
		{
			lm = PlayerPrefs.GetInt("u_hats23", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats23", 1);
				unlocked = true;
				b_hat = true;
			}
		}
		else if (time_  > 1000f)
		{
			lm = PlayerPrefs.GetInt("u_hats22", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats22", 1);
				unlocked = true;
				b_hat = true;
			}
		}
		else if (time_  > 750f)
		{
			lm = PlayerPrefs.GetInt("u_hats21", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats21", 1);
				unlocked = true;
				b_hat = true;
			}
		}
		else if (time_  > 500f)
		{
			lm = PlayerPrefs.GetInt("u_hats20", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats20", 1);
				unlocked = true;
				b_hat = true;
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
			}
		}
		else if (time_  > 300f)
		{
			lm = PlayerPrefs.GetInt("u_hats18", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats18", 1);
				unlocked = true;
				b_hat = true;
			}
		}
		else if (time_  > 200f)
		{
			lm = PlayerPrefs.GetInt("u_hats17", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats17", 1);
				unlocked = true;
				b_hat = true;
			}
		}
		else if (time_  > 150f)
		{
			lm = PlayerPrefs.GetInt("u_hats16", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats16", 1);
				unlocked = true;
				b_hat = true;
			}
		}
		else if (time_  > 100f)
		{
			lm = PlayerPrefs.GetInt("u_hats15", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats15", 1);
				unlocked = true;
				b_hat = true;
			}
		}
		else if (time_  > 50f)
		{
			lm = PlayerPrefs.GetInt("u_hats14", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats14", 1);
				unlocked = true;
				b_hat = true;
			}
		}
		else if (time_  > 10f)
		{
			lm = PlayerPrefs.GetInt("u_hats13", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats13", 1);
				unlocked = true;
				b_hat = true;
			}
		}

		// Final touch
		if (unlocked)
			ui_newOutfit.gameObject.SetActive(true);
		if (b_hat)
			ui_hat.gameObject.SetActive(true);
		if (b_fish)
			ui_fish.gameObject.SetActive(true);

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
		ui_newOutfit.gameObject.SetActive(false);
		ui_hat.gameObject.SetActive(false);
		ui_fish.gameObject.SetActive(false);
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
		titleObj.SetActive(true);
		titleObj.transform.position = new Vector2(0, 3.25f);
		shelf_GO.SetActive(true);
		shelf_GO.transform.parent = cam.transform;
		shelf_GO.transform.position = new Vector2(0f, .34f);
	}
	void disableLost()
	{
		cam.transform.position = new Vector2(.75f,transform.position.y);
		titleObj.transform.position = new Vector2(.75f, 2.5f);
		ui_toMenu.gameObject.SetActive(false);
		ui_newOutfit.gameObject.SetActive(false);
		ui_fish.gameObject.SetActive(false);
		ui_hat.gameObject.SetActive(false);
		shelf_GO.SetActive(false);
	}


	/********************************************************************8****
				ARROW FUNCTIONAL STUFF
	**************************************************************************/

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
			}
		}

		hatSpr.sprite = hatSprites[hatChoose];
	}

	void setLifeText()
	{
		txtLife.text = Mathf.Round(life).ToString();
		txtHighScrInt.text = "";
		// Empty out everything
	}
	void setTimerText()
	{
		txtLife.text = "";
		txtHighScrInt.text = Mathf.Round(PlayerPrefs.GetFloat("Highscore", 0)).ToString();
		txtScore.text = Mathf.Round(timer).ToString();
		//txtHighScrInt.transform.SetParent(cam.transform);
		//txtScore.transform.SetParent(cam.transform);
	}
	void setHighScore()
	{
		PlayerPrefs.SetFloat("Highscore", Mathf.Round(timer));
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
}
