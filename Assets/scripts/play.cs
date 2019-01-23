using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class play : MonoBehaviour {
	public GameObject fish;
	public GameObject hat;
	public playerMovement pm; 
	private SpriteRenderer fishSpr;
	private SpriteRenderer hatSpr;
	private Sprite[] fishSprites;
	private Sprite[] hatSprites;

	private int hatChoose;
	private int fishChoose;

	public bool inMenu;
	public bool inSettings;
	public GameObject bubbles;

	public Button ui_leftSkin, ui_rightSkin, ui_rightHat, ui_leftHat, ui_play, ui_settings,
				  ui_back, ui_resetAll, ui_toMenu, ui_newOutfit, ui_fish, ui_hat, 
				  ui_highscore;

	private bool unlocked;

	public bool onceTrigger;

	public Text txtLife;
	public Text txtHighScrInt;
	public float life;

	public float timer;

	// Use this for initialization
	void Awake()
    {
        // load all frames in fruitsSprites array
        fishSprites = Resources.LoadAll<Sprite>("fish_sheetv2");
        hatSprites = Resources.LoadAll<Sprite>("hats_wave_1");
        inMenu = true;
        inSettings = false;
    }
	void Start () 
	{
		// Save player outfit
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

		// TIME AND LIFE TEXTS
		life = 100f;
		txtLife.text = "";
		txtHighScrInt.text = "";
		//setLifeText();
		timer = 0;

		// load the fish skins
		fishSpr.sprite = fishSprites[fishChoose];
		hatSpr.sprite = hatSprites[hatChoose];
		
		// Add functionality to buttons
		ui_back.onClick.AddListener(back);
		ui_settings.onClick.AddListener(settings);
		ui_play.onClick.AddListener(playGame);
		ui_leftSkin.onClick.AddListener(leftSkin);
		ui_rightSkin.onClick.AddListener(rightSkin);
		ui_rightHat.onClick.AddListener(rightHat);
		ui_leftHat.onClick.AddListener(leftHat);
		ui_resetAll.onClick.AddListener(resetAll);
		ui_toMenu.onClick.AddListener(toMenu);

		disableSettings();
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

	void playGame()
	{
		inMenu = false;

		// Save
		PlayerPrefs.SetInt("Fish", fishChoose);
		PlayerPrefs.SetInt("Hat", hatChoose);
		txtLife.text = 100.ToString();
		disableButtons();
	}
	void toMenu()
	{
		inMenu = true;
		onceTrigger = false;
		SceneManager.LoadScene("main");
		txtLife.text = "";
	}
	void settings()
	{
		bubbles.gameObject.SetActive(false);
		disableButtons();
		enableSettings();
		PlayerPrefs.SetInt("Fish", fishChoose);
		PlayerPrefs.SetInt("Hat", hatChoose);
		inSettings = true;
	}
	void back()
	{
		bubbles.gameObject.SetActive(true);
		enableButtons();
		disableSettings();
		inSettings = false;
	}

	void resetAll()
	{
		print("BOOOOM");
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
		print("records updated.");
		print("Number of deaths: " + PlayerPrefs.GetInt("numDeaths"));
		print("Highscore: " + PlayerPrefs.GetFloat("Highscore"));
		print("Time played: " + PlayerPrefs.GetFloat("totalTime"));
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
								/*lm = PlayerPrefs.GetInt("u_skins6", 0);
								if (lm == 0)
								{
									unlocked = true;
									b_fish = true;
									PlayerPrefs.SetInt("u_skins6", 1);
								}*/
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
		if (deaths == 10)
		{
			lm = PlayerPrefs.GetInt("u_skins8", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_skins8", 1);
				unlocked = true;
				b_fish = true;
			}
		}
		else if (deaths == 20)
		{
			lm = PlayerPrefs.GetInt("u_skins7", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_skins7", 1);
				unlocked = true;
				b_fish = true;
			}
		}
		else if (deaths == 50)
		{
			lm = PlayerPrefs.GetInt("u_hats6", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_hats6", 1);
				unlocked = true;
				b_hat = true;
			}
			lm = PlayerPrefs.GetInt("u_hats5", 0);
			if (lm == 0)
			{
				PlayerPrefs.SetInt("u_skins5", 1);
				unlocked = true;
				b_hat = true;
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
		ui_highscore.gameObject.SetActive(true);
		// replay 
		// highscore
	}
	void disableLost()
	{
		ui_highscore.gameObject.SetActive(false);
		ui_toMenu.gameObject.SetActive(false);
		ui_newOutfit.gameObject.SetActive(false);
		ui_fish.gameObject.SetActive(false);
		ui_hat.gameObject.SetActive(false);
	}


	/********************************************************************8****
				ARROW FUNCTIONAL STUFF
	**************************************************************************/

	void leftSkin()
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

	void rightSkin()
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
	void leftHat()
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
	void rightHat()
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
		txtLife.text = Mathf.Round(timer).ToString() + "  secs";
		txtHighScrInt.text = Mathf.Round(PlayerPrefs.GetFloat("Highscore", 0)).ToString();
	}
	void setHighScore()
	{
		PlayerPrefs.SetFloat("Highscore", Mathf.Round(timer));
	}

	// For testing
	public IEnumerator OnBegin()
    {
        yield return new WaitForSeconds(2);
        fishChoose = Random.Range(0, (fishSprites.Length));
		hatChoose = Random.Range(0, (hatSprites.Length));
        StartCoroutine(OnBegin());
    }
}
